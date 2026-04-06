using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using Isomerization.Domain.Data;
using Isomerization.Domain.Cim2;
using Isomerization.Domain.Models;
using Isomerization.Domain.Task1;
using Isomerization.Domain.Validation;
using Isomerization.Shared;
using Isomerization.UI.Features.Researcher;
using Isomerization.UI.Services;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkiaSharp;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace Isomerization.UI.Features;

public class ResearcherPageVM: ViewModelBase
{
    private readonly IsomerizationContext _context;
    private readonly IMessageBoxService _messageBoxService;
    private readonly IMenuService _menuService;
    private readonly IUserService _userService;
    private readonly ISnackbarService _snackbarService;
    private readonly INavigationService _navigationService;
    private readonly ICim2SessionService _cim2SessionService;

    public ResearcherPageVM(IsomerizationContext context,
        IMessageBoxService messageBoxService,
        IMenuService menuService,
        IUserService userService,
        ISnackbarService snackbarService,
        INavigationService navigationService,
        ICim2SessionService cim2SessionService)
    {
        _context = context;
        _messageBoxService = messageBoxService;
        _menuService = menuService;
        _userService = userService;
        _snackbarService = snackbarService;
        _navigationService = navigationService;
        _cim2SessionService = cim2SessionService;

        RawMaterials = new ObservableCollection<RawMaterial>(_context.RawMaterials.Include(x=>x.Concentrations));
        SelectedRawMaterial = RawMaterials.FirstOrDefault();
        Catalysts = new ObservableCollection<Catalyst>(_context.Catalysts);
        SelectedCatalyst = Catalysts.FirstOrDefault();
        App.GetService<MainWindowVM>().IsMenuEnabled = false;
    }
    
    private Task3ViewModel _task3VM;
    /// <summary>
    /// ViewModel для ЦИМ-2 (Task3)
    /// </summary>
    public Task3ViewModel Task3VM
    {
        get
        {
            if (_task3VM == null)
            {
                _task3VM = App.GetService<Task3ViewModel>();
            }
            return _task3VM;
        }
    }
    public ObservableCollection<Installation> AvailableInstallations { get; set; }
    public Installation SelectedInstallation { get; set; }
    public ObservableCollection<RawMaterial> RawMaterials { get; set; }
    public RawMaterial SelectedRawMaterial { get; set; }

    public ObservableCollection<Catalyst> Catalysts { get; set; }
    public Catalyst SelectedCatalyst { get; set; }

    public bool Draw7Concentrations => MathClass?.Results.MaterialCount == 7;

    public Axis[] XAxes { get; set; } =
    {
        new Axis
        {
            Name = "Время пребывания, с",
            TextSize = 14,
            LabelsPaint = new SolidColorPaint(SKColors.Black),
        }
    };
    public Axis[] ConcentrationYAxes { get; set; } =
    {
        new Axis
        {
            Name = "Концентрация, %",
            TextSize = 14,
            LabelsPaint = new SolidColorPaint(SKColors.Black)
        }
    };
    public Axis[] TemperatureYAxes { get; set; } =
    {
        new Axis
        {
            Name = "Температура, С",
            TextSize = 14,
            LabelsPaint = new SolidColorPaint(SKColors.Black)
        }
    };
    
    private RelayCommand _loadModelCommand;
    public RelayCommand LoadModelCommand => _loadModelCommand ??= new RelayCommand(_ =>
    {
        var window = App.GetService<SelectDIMIsomerizationWindow>();
        window.ShowDialog();
        var res = window.ViewModel.SelectedDimIsomerizationFinal;
        if (res is null)
        {
            return;
        }

        // AvailableInstallations = new ObservableCollection<Installation>(_context.Installations);
        // SelectedInstallation = AvailableInstallations.First(x => x.InstallationId == res.InstallationId);
        SelectedCatalyst = Catalysts.First(x => x.CatalystId == res.CatalystId);
        SelectedRawMaterial = RawMaterials.First(x => x.RawMaterialId == res.RawMaterialId);
        
        OctaneNumberMin = res.OctaneNumberMin;
        PerformanceMin =  res.PerformanceMin;
        PerformanceMax =  res.PerformanceMax;
        EnergyConsumptionMax =  res.EnergyConsumptionMax;
        EnergyConsumptionMin =  res.EnergyConsumptionMin;
        T0 = res.Temp;
        G = res.Consumption;
        H = res.Step;
    });
    private RelayCommand _openHelpCommand;
    public RelayCommand OpenHelpCommand => _openHelpCommand ??= new RelayCommand(_ =>
    {
        const string url = "https://vitalina-opal.vercel.app/";
        try
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch
        {
            MessageBox.Show("Ошибка при открытии браузера", "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    });
    private RelayCommand _saveModelCommand;
    public RelayCommand SaveModelCommand => _saveModelCommand ??= new RelayCommand(_ =>
    {
        var isomerization = new DIMIsomerization()
        {
            Catalyst = SelectedCatalyst,
            RawMaterial = SelectedRawMaterial,
            Name = $"Модель изомеризации - {DateTime.Now:G}",
            Temp = T0,
            User = _userService.CurrentUser,
            Step = H,
            Consumption = G,
            OctaneNumberMin = OctaneNumberMin,
            PerformanceMin = PerformanceMin,
            PerformanceMax = PerformanceMax,
            EnergyConsumptionMax = EnergyConsumptionMax,
            EnergyConsumptionMin = EnergyConsumptionMin,
            Productivity = LastProcessResult?.Productivity ?? 0,
            ProcessEnergyConsumption = LastProcessResult?.ProcessEnergyConsumption ?? 0,
            OctaneNumber = LastProcessResult?.OctaneNumber ?? 0,
            IsopentaneConcentration = LastProcessResult?.IsopentaneConcentration ?? 0,
            IsomerizationDegree = LastProcessResult?.IsomerizationDegree ?? 0,
            IsProductivityValid = LastProcessResult?.Validation.ProductivityStatus == ValidationStatus.Valid,
            IsEnergyValid = LastProcessResult?.Validation.ProcessEnergyStatus == ValidationStatus.Valid,
            IsOctaneValid = LastProcessResult?.Validation.OctaneStatus == ValidationStatus.Valid,
            IsIsopentaneValid = LastProcessResult?.Validation.IsopentaneStatus == ValidationStatus.Valid,
        };
        _context.DimIsomerizations.Add(isomerization);
        _context.SaveChanges();
        _messageBoxService.Show("Данные о цифровой модели изомеризации добавлены", "База данных обновлена", MessageBoxButton.OK);
    });
    
    private RelayCommand _goHomeMenu;
    public RelayCommand GoHomeMenu => _goHomeMenu ??= new RelayCommand(_ =>
    {
        _menuService.GoHome();
    });
    private RelayCommand _goLoginMenu;
    public RelayCommand GoLoginMenu => _goLoginMenu ??= new RelayCommand(_ =>
    {
        _menuService.GoLogin();
    });
    /// <summary>
    /// Начальная температура
    /// </summary>
    public double T0 { get; set; } = 120;
    /// <summary>
    /// Расход сырья
    /// </summary>
    public double G { get; set; } = 9;

    /// <summary>
    /// Шаг
    /// </summary>
    public int H { get; set; } = 1;

    
    public double? PerformanceMin { get; set; }
    public double? PerformanceMax { get; set; }
    public double? EnergyConsumptionMin { get; set; }
    public double? EnergyConsumptionMax { get; set; }
    public double OctaneNumberMin { get; set; } = 78;
    public MathClass MathClass { get; set; }

    /// <summary>Последний полный результат проверки ЦИМ-1 (после расчёта).</summary>
    public IsomerizationProcessResult? LastProcessResult { get; set; }

    /// <summary>Текст рекомендаций и статусов для отображения на странице.</summary>
    public string LastCim1ValidationText { get; set; } = string.Empty;

    private RelayCommand _calcCommand;
    private RelayCommand _generateCim2Command;

    public RelayCommand GenerateCim2Command => _generateCim2Command ??= new RelayCommand(_ =>
    {
        if (!IsCalculated || LastProcessResult == null || SelectedRawMaterial == null)
        {
            _messageBoxService.Show("Сначала выполните корректный расчет ЦИМ-1.", "ЦИМ-2", MessageBoxButton.OK);
            return;
        }

        var cim1Result = Cim1ResultFactory.Create(LastProcessResult, G, SelectedRawMaterial.Density, T0);
        _cim2SessionService.LastCim1Result = cim1Result;
        _navigationService.Navigate(typeof(Cim2Page));
    });

    public RelayCommand CalcCommand => _calcCommand ??= new RelayCommand(async _ =>
    {
        var processBag = new ConcurrentBag<IsomerizationProcessResult>();
        AvailableInstallations = FindInstallations();
        if (!AvailableInstallations.Any())
        {
            _messageBoxService.Show("Нельзя перейти к расчету, оборудование не найдено!", "Ошибка!",
                MessageBoxButton.OK);
            return;
        }
        Parallel.ForEach(AvailableInstallations, installation =>
        {
            var calcParams = new CalculationParameters()
            {
                Volume = installation.Volume,
                D = installation.Diameter,
                P = SelectedRawMaterial.Density,
                T0 = T0,
                G = G,
                Step = H,
                HeatCap = SelectedRawMaterial.HeatCapacity,
                Activity = SelectedCatalyst.Activity/100,
                C0 = SelectedRawMaterial.Concentrations.OrderBy(x=>x.Order).Select(x=>x.Value/100).ToArray(),
            };
            var math = new MathClass(calcParams);
            math.Calculate();
            var evaluated = IsomerizationValidationService.BuildAndValidate(
                installation,
                math,
                G,
                PerformanceMin,
                EnergyConsumptionMin,
                EnergyConsumptionMax,
                OctaneNumberMin);
            processBag.Add(evaluated);
        });

        var evaluatedList = processBag.ToList();
        var passing = evaluatedList.Where(IsomerizationValidationService.PassesSelection).ToList();

        if (!passing.Any())
        {
            IsCalculated = false;
            LastProcessResult = null;
            var failText = new StringBuilder();
            failText.AppendLine("Нет установок, прошедших цепочку проверок: производительность EF → энергопотребление процесса ESproc → октановое число.");
            failText.AppendLine();
            foreach (var grp in evaluatedList.GroupBy(x => x.Installation.Name))
            {
                var r = grp.First();
                failText.AppendLine($"— {r.Installation.Name}: EF={r.Productivity:F3} кг/с, ESproc={r.ProcessEnergyConsumption:F2}, ОКТ={r.OctaneNumber:F2}");
                foreach (var rec in r.Validation.Recommendations.Distinct())
                    failText.AppendLine("  • " + rec);
                failText.AppendLine();
            }
            LastCim1ValidationText = failText.ToString();
            OnPropertyChanged(nameof(LastCim1ValidationText));
            OnPropertyChanged(nameof(LastProcessResult));
            _messageBoxService.Show(failText.ToString(), "Проверки не пройдены", MessageBoxButton.OK);
            return;
        }

        var best = passing.MaxBy(x => x.OctaneNumber);
        var otherCalcs = passing.Where(x => x.Installation.InstallationId != best.Installation.InstallationId).ToList();
        MathClass = best.Math;
        LastProcessResult = best;

        best.Validation.Recommendations.Add(
            $"Рекомендуемый расход G: {G:F3} кг/с; рекомендуемая температура T: {T0:F1} °C; " +
            $"рекомендуемое время пребывания τ: {best.ResidenceTimeSeconds:F2} с; рекомендуемый реакторный блок: {best.Installation.Name}.");

        UpdateGraphics();
        SelectedInstallation = best.Installation;
        IsCalculated = true;
        var res = MathClass.Results;

        var summary = new StringBuilder();
        summary.AppendLine($"Выбрана установка: {best.Installation.Name}");
        summary.AppendLine($"Производительность EF: {best.Productivity:F3} кг/с");
        summary.AppendLine($"Энергопотребление процесса ESproc: {best.ProcessEnergyConsumption:F2}");
        summary.AppendLine($"Октановое число: {best.OctaneNumber:F2}");
        summary.AppendLine($"Концентрация изопентана (выход), %: {best.IsopentaneConcentration:F2}");
        summary.AppendLine($"Степень изомеризации (по ключевому компоненту): {best.IsomerizationDegree:F4}");
        summary.AppendLine();
        summary.AppendLine("Другие подходящие установки:");
        foreach (var o in otherCalcs)
            summary.AppendLine($"— {o.Installation.Name}, ОКТ={o.OctaneNumber:F2}");
        summary.AppendLine();
        summary.AppendLine("Концентрации на выходе:");
        summary.AppendLine($"C1…C4: {res.CordCs.Last().C1:F2}; {res.CordCs.Last().C2:F2}; {res.CordCs.Last().C3:F2}; {res.CordCs.Last().C4:F2}");
        if (res.MaterialCount == 7)
        {
            summary.AppendLine($"C5…C7: {res.CordCs.Last().C5:F2}; {res.CordCs.Last().C6:F2}; {res.CordCs.Last().C7:F2}");
        }
        summary.AppendLine();
        summary.AppendLine("Рекомендации:");
        foreach (var rec in best.Validation.Recommendations.Distinct())
            summary.AppendLine("• " + rec);

        LastCim1ValidationText = summary.ToString();
        OnPropertyChanged(nameof(LastCim1ValidationText));
        OnPropertyChanged(nameof(LastProcessResult));

        _messageBoxService.Show(summary.ToString(), "Результаты расчёта ЦИМ-1", MessageBoxButton.OK);
    });
    private ObservableCollection<Installation> FindInstallations()
    {
        try
        {
            var installations = _context.Installations.Include(x=>x.Model).AsQueryable();
            if (EnergyConsumptionMin.HasValue)
            {
                installations = installations.Where(x => x.EnergyConsumption >= EnergyConsumptionMin.Value);
            }
            if (EnergyConsumptionMax.HasValue)
            {
                installations = installations.Where(x => x.EnergyConsumption <= EnergyConsumptionMax.Value);
            }
            if (PerformanceMin.HasValue)
            {
                installations = installations.Where(x => x.Performance >= PerformanceMin.Value);
            }
            if (PerformanceMax.HasValue)
            {
                installations = installations.Where(x => x.Performance <= PerformanceMax.Value);
            }
            return new ObservableCollection<Installation>(installations);
        }
        catch
        {
            return new ObservableCollection<Installation>();
        }
        
    }

    public List<CordC> CordСs
    {
        get
        {
            if (MathClass != null)
            {
                return MathClass.Results.CordCs;
            }

            return null;
        }
    }

    public bool IsCalculated { get; set; } = false;
    public List<ISeries> CSeries { get; set; }


    private static LineSeries<ObservablePoint> CreateSerie(string name)
    {
        return new LineSeries<ObservablePoint>()
        {
            Name = name,
            Fill = null,
            
            GeometryStroke = null,
            GeometryFill = null,
            // GeometrySize = 8,
        };
    }

    /// <summary>
    ///     Серия точек 1 вещества
    /// </summary>
    private LineSeries<ObservablePoint> C1LineSerie { get; set; } = CreateSerie("C1");

    /// <summary>
    ///     Серия точек 2 вещества
    /// </summary>
    private LineSeries<ObservablePoint> C2LineSerie { get; set; } = CreateSerie( "C2");

        
    /// <summary>
    ///     Серия точек 3 вещества
    /// </summary>
    private LineSeries<ObservablePoint> C3LineSerie { get; set; } = CreateSerie( "C3");

    /// <summary>
    ///     Серия точек 4 вещества
    /// </summary>
    private LineSeries<ObservablePoint> C4LineSerie { get; set; } = CreateSerie( "C4");

    /// <summary>
    ///     Серия точек 5 вещества
    /// </summary>
    private LineSeries<ObservablePoint> C5LineSerie { get; set; } = CreateSerie( "C5");

    /// <summary>
    ///     Серия точек 6 вещества
    /// </summary>
    private LineSeries<ObservablePoint> C6LineSerie { get; set; } = CreateSerie( "C6");
        
    /// <summary>
    ///     Серия точек 7 вещества
    /// </summary>
    private LineSeries<ObservablePoint> C7LineSerie { get; set; } = CreateSerie( "C7");

    
    private LineSeries<ObservablePoint> TLineSerie { get; set; } = CreateSerie("T");
    public List<ISeries> TSeries { get; set; }
    public long TotalMemory
    {
        get
        {
            var currentProcess = Process.GetCurrentProcess();

            return currentProcess.WorkingSet64 / (1024 * 1024);
        }
    }
    public void UpdateGraphics()
    {
        var x = MathClass.Results.CordCs.Select(c => c.Cord).ToList();
        var c1 = MathClass.Results.CordCs.Select(c => c.C1).ToList();
        var c2 = MathClass.Results.CordCs.Select(c => c.C2).ToList();
        var c3 = MathClass.Results.CordCs.Select(c => c.C3).ToList();
        var c4 = MathClass.Results.CordCs.Select(c => c.C4).ToList();
        var c5 = MathClass.Results.CordCs.Select(c => c.C5).ToList();
        var c6 = MathClass.Results.CordCs.Select(c => c.C6).ToList();
        var c7 = MathClass.Results.CordCs.Select(c => c.C7).ToList();
        var t = MathClass.Results.CordCs.Select(c => c.T).ToList();
        
        UpdateLineSeriesByCordAndValue(C1LineSerie, x, c1);
        UpdateLineSeriesByCordAndValue(C2LineSerie, x, c2);
        UpdateLineSeriesByCordAndValue(C3LineSerie, x, c3);
        UpdateLineSeriesByCordAndValue(C4LineSerie, x, c4);
        UpdateLineSeriesByCordAndValue(C5LineSerie, x, c5);
        UpdateLineSeriesByCordAndValue(C6LineSerie, x, c6);
        UpdateLineSeriesByCordAndValue(C7LineSerie, x, c7);
        UpdateLineSeriesByCordAndValue(TLineSerie, x, t);

        if (MathClass.Cp.MaterialCount == 4)
        {
            CSeries = new List<ISeries>()
            {
                C1LineSerie,
                C2LineSerie,
                C3LineSerie,
                C4LineSerie,
            };
        }
        else if (MathClass.Cp.MaterialCount == 7)
        {
            CSeries = new List<ISeries>()
            {
                C1LineSerie,
                C2LineSerie,
                C3LineSerie,
                C4LineSerie,
                C5LineSerie,
                C6LineSerie,
                C7LineSerie,
            };
        }
        
        TSeries = new List<ISeries>() { TLineSerie };
        
        OnPropertyChanged(nameof(MathClass));
        OnPropertyChanged(nameof(TotalMemory));
        OnPropertyChanged(nameof(Draw7Concentrations));
    }
    
    private void UpdateLineSeriesByCordAndValue(LineSeries<ObservablePoint> serie, List<double> x, List<double> y)
    {
        if (x.Count != y.Count)
        {
            throw new ArgumentException("Количество значений x не совпадает с количеством значений y");
        }

        var points = new ObservablePoint[x.Count];
        for (var i = 0; i < x.Count; i++)
        {
            points[i] = new ObservablePoint(x[i], y[i]);
        }

        serie.Values = points;
    }
}