using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using Isomerization.Domain.Data;
using Isomerization.Domain.Models;
using Isomerization.Domain.Task1;
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

    public ResearcherPageVM(IsomerizationContext context,
        IMessageBoxService messageBoxService,
        IMenuService menuService,
        IUserService userService,
        ISnackbarService snackbarService)
    {
        _context = context;
        _messageBoxService = messageBoxService;
        _menuService = menuService;
        _userService = userService;
        _snackbarService = snackbarService;

        RawMaterials = new ObservableCollection<RawMaterial>(_context.RawMaterials.Include(x=>x.Concentrations));
        SelectedRawMaterial = RawMaterials.FirstOrDefault();
        Catalysts = new ObservableCollection<Catalyst>(_context.Catalysts);
        SelectedCatalyst = Catalysts.FirstOrDefault();
        App.GetService<MainWindowVM>().IsMenuEnabled = false;
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

        AvailableInstallations = new ObservableCollection<Installation>(_context.Installations);
        // SelectedInstallation = AvailableInstallations.First(x => x.InstallationId == res.InstallationId);
        SelectedCatalyst = Catalysts.First(x => x.CatalystId == res.CatalystId);
        SelectedRawMaterial = RawMaterials.First(x => x.RawMaterialId == res.RawMaterialId);

        T0 = res.Temp;
        G = res.Consumption;
        H = res.Step;
        OctaneNumberMin = res.OctaneNumberMin;
    });
    private RelayCommand _saveModelCommand;
    public RelayCommand SaveModelCommand => _saveModelCommand ??= new RelayCommand(_ =>
    {
        var isomerization = new DIMIsomerization()
        {
            Catalyst = SelectedCatalyst,
            // Installation = SelectedInstallation,
            RawMaterial = SelectedRawMaterial,
            Name = $"Модель изомеризации - {DateTime.Now:G}",
            // Consumption = SelectedInstallation.RawMaterialConsumption,
            Temp = T0,
            User = _userService.CurrentUser,
            Step = H,
            Consumption = G,
            OctaneNumberMin = OctaneNumberMin,
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
    /// Погрешность в процентах
    /// </summary>
    public double EMax { get; set; } = 5;
    /// <summary>
    /// Максимальное число делений сетки пополам 
    /// </summary>
    public double QMax { get; set; } = 4;

    /// <summary>
    /// Время проведения реакции
    /// </summary>
    public double Tau { get; set; } = 100;

    /// <summary>
    /// Шаг
    /// </summary>
    public int H { get; set; } = 1;

    /// <summary>
    /// Время эксплуатации катализатора
    /// </summary>
    public double CatalystT { get; set; } = 3600;
    public double OctaneNumberMin { get; set; } = 78;
    public MathClass MathClass { get; set; }

    private RelayCommand _calcCommand;

    public RelayCommand CalcCommand => _calcCommand ??= new RelayCommand(async _ =>
    {
        var mathResults = new ConcurrentBag<(Installation Installation, MathClass Math)>();
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
            mathResults.Add((installation, math));
        });

        bool IsResultOk(CalculationResults results)
        {
            return results.OKT >= OctaneNumberMin;
        }

        var satisfyingCalcs = mathResults.Where(x => IsResultOk(x.Math.Results)).ToList();

        if (!satisfyingCalcs.Any())
        {
            IsCalculated = false;
            _messageBoxService.Show("Установок, удовлетворяющих условиям, не найдено", "Установки не найдены", MessageBoxButton.OK);
            return;
        }

        var bestCalc = satisfyingCalcs.MaxBy(x => x.Math.Results.OKT);
        var otherCalcs = satisfyingCalcs.Except(new[] { bestCalc }).ToList();
        MathClass = bestCalc.Math;
        
        UpdateGraphics();
        SelectedInstallation = bestCalc.Installation;
        IsCalculated = true;
        var res = MathClass.Results;


        var bestInstallationResultText =
            $"Самое высокое октановое число достигается с установкой {bestCalc.Installation.Name}, октановое число: {bestCalc.Math.Results.OKT:F2}\n";
        var otherInstallationResultText = otherCalcs.Any() ? "Результаты для остальных установок:\n" : string.Empty;
        foreach (var otherCalc in otherCalcs)
        {
            otherInstallationResultText +=
                $"Установка {otherCalc.Installation.Name}, октановое число: {otherCalc.Math.Results.OKT:F2}\n";
        }
        
        var answerText =
            bestInstallationResultText +
            otherInstallationResultText +
            $"Выходная концентрация вещества 1: {res.CordCs.Last().C1:F2}\n" +
            $"Выходная концентрация вещества 2: {res.CordCs.Last().C2:F2}\n" +
            $"Выходная концентрация вещества 3: {res.CordCs.Last().C3:F2}\n" +
            $"Выходная концентрация вещества 4: {res.CordCs.Last().C4:F2}\n";
        if (res.MaterialCount == 7)
        {
            answerText +=
                $"Выходная концентрация вещества 5: {res.CordCs.Last().C5:F2}\n" +
                $"Выходная концентрация вещества 6: {res.CordCs.Last().C6:F2}\n" +
                $"Выходная концентрация вещества 7: {res.CordCs.Last().C7:F2}\n";
        }

        _messageBoxService.Show(answerText, "Результаты расчета", MessageBoxButton.OK);
    });
    
    
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