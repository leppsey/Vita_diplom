using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Isomerization.Domain.Task3;
using Isomerization.Domain.Validation;
using Isomerization.Shared;
using Isomerization.UI.Services;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Win32;
using SkiaSharp;

namespace Isomerization.UI.Features;

/// <summary>
/// ViewModel для ЦИМ-2 (Task3) - расчет трубопроводной системы
/// </summary>
public class Task3ViewModel : ViewModelBase
{
    private readonly IMessageBoxService _messageBoxService;

    public Task3ViewModel(IMessageBoxService messageBoxService)
    {
        _messageBoxService = messageBoxService;
        Variants = new ObservableCollection<PipeVariantRow>();
        
        // Значения по умолчанию
        var defaultParams = PipeCalculationParameters.CreateDefault();
        lambda = defaultParams.lambda;
        eta = defaultParams.eta;
        
        // Инициализация графиков
        InitializeCharts();
    }

    #region Свойства ввода

    private double _d;
    /// <summary>
    /// Диаметр, м
    /// </summary>
    public double D
    {
        get => _d;
        set
        {
            _d = value;
            OnPropertyChanged();
        }
    }

    private double _l;
    /// <summary>
    /// Длина, м
    /// </summary>
    public double L
    {
        get => _l;
        set
        {
            _l = value;
            OnPropertyChanged();
        }
    }

    private double _sumZeta;
    /// <summary>
    /// Сумма местных сопротивлений
    /// </summary>
    public double sumZeta
    {
        get => _sumZeta;
        set
        {
            _sumZeta = value;
            OnPropertyChanged();
        }
    }

    private double _lambda;
    /// <summary>
    /// Коэффициент трения
    /// </summary>
    public double lambda
    {
        get => _lambda;
        set
        {
            _lambda = value;
            OnPropertyChanged();
        }
    }

    private double _eta;
    /// <summary>
    /// КПД насоса
    /// </summary>
    public double eta
    {
        get => _eta;
        set
        {
            _eta = value;
            OnPropertyChanged();
        }
    }

    private double _rho;
    /// <summary>
    /// Плотность среды, кг/м³
    /// </summary>
    public double rho
    {
        get => _rho;
        set
        {
            _rho = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Параметры ТЗ

    private double _efMin;
    /// <summary>
    /// Минимальная производительность
    /// </summary>
    public double EFmin
    {
        get => _efMin;
        set
        {
            _efMin = value;
            OnPropertyChanged();
        }
    }

    private double _esMax;
    /// <summary>
    /// Максимальное энергопотребление трубопровода ESpipe = Q·ΔPΣ, Вт
    /// </summary>
    public double ESmax
    {
        get => _esMax;
        set
        {
            _esMax = value;
            OnPropertyChanged();
        }
    }

    private double _deltaPmax;
    /// <summary>Максимальные суммарные потери ΔPΣ, Па (0 — проверка отключена).</summary>
    public double DeltaPmax
    {
        get => _deltaPmax;
        set { _deltaPmax = value; OnPropertyChanged(); }
    }

    private double _nmax;
    /// <summary>Максимальная мощность насоса N, Вт (0 — отключено).</summary>
    public double Nmax
    {
        get => _nmax;
        set { _nmax = value; OnPropertyChanged(); }
    }

    private double _workingPressure;
    /// <summary>Рабочее давление, Па.</summary>
    public double WorkingPressure
    {
        get => _workingPressure;
        set { _workingPressure = value; OnPropertyChanged(); }
    }

    private double _allowablePressure;
    /// <summary>Допустимое давление, Па (0 — отключено).</summary>
    public double AllowablePressure
    {
        get => _allowablePressure;
        set { _allowablePressure = value; OnPropertyChanged(); }
    }

    private double _allowableStress;
    /// <summary>Допускаемое напряжение [σ], Па (0 — расчёт δ отключён).</summary>
    public double AllowableStress
    {
        get => _allowableStress;
        set { _allowableStress = value; OnPropertyChanged(); }
    }

    private double _actualWallThickness;
    /// <summary>Фактическая толщина стенки δ, м.</summary>
    public double ActualWallThickness
    {
        get => _actualWallThickness;
        set { _actualWallThickness = value; OnPropertyChanged(); }
    }

    private double _fluidTemperature;
    public double FluidTemperature
    {
        get => _fluidTemperature;
        set { _fluidTemperature = value; OnPropertyChanged(); }
    }

    private double _maxFluidTemperature;
    public double MaxFluidTemperature
    {
        get => _maxFluidTemperature;
        set { _maxFluidTemperature = value; OnPropertyChanged(); }
    }

    private double _maxVelocity;
    public double MaxVelocity
    {
        get => _maxVelocity;
        set { _maxVelocity = value; OnPropertyChanged(); }
    }

    private string _recommendedPump = string.Empty;
    /// <summary>Рекомендуемое насосное оборудование (заполняется вручную или из каталога).</summary>
    public string RecommendedPump
    {
        get => _recommendedPump;
        set { _recommendedPump = value; OnPropertyChanged(); }
    }

    #endregion

    #region Производительность из ЦИМ-1

    private double _ef;
    /// <summary>
    /// Производительность (расход) из ЦИМ-1
    /// </summary>
    public double EF
    {
        get => _ef;
        set
        {
            _ef = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(deltaEF));
        }
    }

    private RelayCommand _loadFromCim1Command;
    /// <summary>
    /// Команда для подтягивания EF и rho из ЦИМ-1
    /// </summary>
    public RelayCommand LoadFromCim1Command => _loadFromCim1Command ??= new RelayCommand(_ =>
    {
        try
        {
            var researcherVM = App.GetService<ResearcherPageVM>();
            if (researcherVM == null)
            {
                _messageBoxService.Show("ЦИМ-1 не доступен", "Ошибка", MessageBoxButton.OK);
                return;
            }

            // EF = G (расход сырья из ЦИМ-1)
            EF = researcherVM.G;

            // rho = Density (плотность из выбранного сырья)
            if (researcherVM.SelectedRawMaterial != null)
            {
                rho = researcherVM.SelectedRawMaterial.Density;
                _messageBoxService.Show($"Данные подтянуты из ЦИМ-1:\nEF = {EF:F2} кг/с\nρ = {rho:F2} кг/м³", "Информация", MessageBoxButton.OK);
            }
            else
            {
                _messageBoxService.Show($"Производительность EF = {EF:F2} подтянута из ЦИМ-1\nПлотность не доступна (сырье не выбрано)", "Информация", MessageBoxButton.OK);
            }
        }
        catch (Exception ex)
        {
            _messageBoxService.Show($"Ошибка при подтягивании данных из ЦИМ-1: {ex.Message}", "Ошибка", MessageBoxButton.OK);
        }
    });

    #endregion

    #region Свойства результата

    private PipeCalculationResult? _calculationResult;

    /// <summary>
    /// Общая потеря давления
    /// </summary>
    public double? dP_total => _calculationResult?.dP_total;

    /// <summary>
    /// Энергоемкость
    /// </summary>
    public double? ES => _calculationResult?.ES;

    /// <summary>
    /// Разница EF - EFmin
    /// </summary>
    public double? deltaEF => _calculationResult?.deltaEF;

    /// <summary>
    /// Разница ESmax - ES
    /// </summary>
    public double? deltaES => _calculationResult?.deltaES;

    /// <summary>
    /// Условие по производительности выполнено
    /// </summary>
    public bool? isEFok => _calculationResult?.isEFok;

    /// <summary>
    /// Условие по энергоемкости выполнено
    /// </summary>
    public bool? isESok => _calculationResult?.isESok;

    /// <summary>
    /// Флаг наличия результатов расчета
    /// </summary>
    public bool HasResult => _calculationResult != null;

    public double? dP_fric => _calculationResult?.dP_fric;
    public double? dP_local => _calculationResult?.dP_local;
    public double? Q => _calculationResult?.Q;
    public double? PumpPower => _calculationResult?.PumpPower;
    public double? PipelineEnergyConsumption => _calculationResult?.PipelineEnergyConsumption;
    public double? CalculatedWallThickness => _calculationResult?.CalculatedWallThickness;

    public bool? isDPtotalOk => _calculationResult?.isDPtotalOk;
    public bool? isPumpPowerOk => _calculationResult?.isPumpPowerOk;
    public bool? isAllowablePressureOk => _calculationResult?.isAllowablePressureOk;
    public bool? isWallThicknessOk => _calculationResult?.isWallThicknessOk;
    public bool? isFluidTemperatureOk => _calculationResult?.isFluidTemperatureOk;
    public bool? isNormativeVelocityOk => _calculationResult?.isNormativeVelocityOk;

    private string _cim2FullReport = string.Empty;
    public string Cim2FullReport
    {
        get => _cim2FullReport;
        set { _cim2FullReport = value; OnPropertyChanged(); }
    }

    #endregion

    #region Коллекция вариантов

    /// <summary>
    /// Коллекция вариантов расчета для перебора по диаметрам
    /// </summary>
    public ObservableCollection<PipeVariantRow> Variants { get; }

    #endregion

    #region Параметры для перебора диаметров

    private double _dMin;
    /// <summary>
    /// Минимальный диаметр для перебора, м
    /// </summary>
    public double Dmin
    {
        get => _dMin;
        set
        {
            _dMin = value;
            OnPropertyChanged();
        }
    }

    private double _dMax;
    /// <summary>
    /// Максимальный диаметр для перебора, м
    /// </summary>
    public double Dmax
    {
        get => _dMax;
        set
        {
            _dMax = value;
            OnPropertyChanged();
        }
    }

    private int _steps = 10;
    /// <summary>
    /// Количество шагов для перебора
    /// </summary>
    public int steps
    {
        get => _steps;
        set
        {
            _steps = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Команды

    private RelayCommand _calculateCim2Command;
    /// <summary>
    /// Команда расчета ЦИМ-2
    /// </summary>
    public RelayCommand CalculateCim2Command => _calculateCim2Command ??= new RelayCommand(_ =>
    {
        try
        {
            var parameters = BuildPipeParameters();
            _calculationResult = PipeMathService.Calculate(parameters);
            NotifyPipeResultChanged();

            var r = _calculationResult!.Value;
            var v = r.Validation ?? new CalculationValidationResult();
            var sb = new StringBuilder();
            sb.AppendLine("Пошаговые проверки ЦИМ-2:");
            sb.AppendLine($"1) Потери по длине ΔPₗ: {r.dP_fric:F2} Па");
            sb.AppendLine($"2) Местные потери ΔPₘ: {r.dP_local:F2} Па");
            sb.AppendLine($"3) Суммарные потери ΔPΣ: {r.dP_total:F2} Па — {(r.isDPtotalOk == true ? "выполнено (≤ ΔPmax)" : "не выполнено")}");
            sb.AppendLine($"4) Мощность насоса N: {r.PumpPower:F2} Вт — {(r.isPumpPowerOk == true ? "выполнено (≤ Nmax)" : "не выполнено")}");
            sb.AppendLine($"5) Энергопотребление ESpipe: {r.PipelineEnergyConsumption:F2} Вт — {(r.isESok == true ? "выполнено (≤ ESmax)" : "не выполнено")}");
            sb.AppendLine($"6) Давление: рабочее {WorkingPressure:F0} Па — {(r.isAllowablePressureOk == true ? "выполнено" : "не выполнено")}");
            sb.AppendLine(
                $"7) Толщина стенки: δфакт = {ActualWallThickness * 1000:F2} мм, δрасч = {r.CalculatedWallThickness * 1000:F2} мм — {(r.isWallThicknessOk == true ? "выполнено" : "не выполнено")}");
            sb.AppendLine($"8) Температура среды: {FluidTemperature:F1} °C — {(r.isFluidTemperatureOk == true ? "выполнено" : "не выполнено")}");
            sb.AppendLine($"9) Скорость v: {r.v:F3} м/с — {(r.isNormativeVelocityOk == true ? "выполнено" : "не выполнено")}");
            sb.AppendLine();
            sb.AppendLine("Сводка по критериям (статусы проверок):");
            sb.AppendLine(FormatCriterionStatusLine("Производительность (EF ≥ EFmin)", v.ProductivityStatus));
            sb.AppendLine(FormatCriterionStatusLine("Мощность насоса (N ≤ Nmax)", v.ProcessEnergyStatus));
            sb.AppendLine(FormatCriterionStatusLine("Суммарные потери и допустимое давление", v.PressureStatus));
            sb.AppendLine(FormatCriterionStatusLine("Толщина стенки (δ ≥ δрасч)", v.WallThicknessStatus));
            sb.AppendLine(FormatCriterionStatusLine("Энергопотребление трубопровода (ESpipe)", v.PipelineEnergyStatus));
            sb.AppendLine(FormatCriterionStatusLine("Нормативы (скорость, температура)", v.NormativeStatus));
            sb.AppendLine();
            sb.AppendLine("Рекомендации:");
            foreach (var line in v.Recommendations.Distinct())
                sb.AppendLine("• " + line);
            sb.AppendLine();
            sb.AppendLine(
                $"Итог по ТЗ: рекомендуемый внутренний диаметр D = {D:F4} м; толщина стенки δ ≥ {r.CalculatedWallThickness * 1000:F2} мм; " +
                $"расчётные потери ΔPΣ = {r.dP_total:F2} Па; расчётное ESpipe = {r.PipelineEnergyConsumption:F2} Вт; насос (ориентир) N = {r.PumpPower:F2} Вт.");
            if (!string.IsNullOrWhiteSpace(RecommendedPump))
                sb.AppendLine($"Рекомендуемое насосное оборудование (ввод пользователя): {RecommendedPump}");
            sb.AppendLine("Материал: подобрать по допускаемому напряжению [σ] и давлению согласно нормативной документации.");

            Cim2FullReport = sb.ToString();

            _messageBoxService.Show(sb.ToString(), "Результаты расчёта ЦИМ-2", MessageBoxButton.OK);
        }
        catch (ArgumentException ex)
        {
            _messageBoxService.Show($"Ошибка валидации: {ex.Message}", "Ошибка", MessageBoxButton.OK);
        }
        catch (Exception ex)
        {
            _messageBoxService.Show($"Ошибка при расчете: {ex.Message}", "Ошибка", MessageBoxButton.OK);
        }
    });

    private RelayCommand _sweepDiametersCommand;
    /// <summary>
    /// Команда перебора по диаметрам
    /// </summary>
    public RelayCommand SweepDiametersCommand => _sweepDiametersCommand ??= new RelayCommand(_ =>
    {
        try
        {
            var pBase = BuildPipeParameters();
            pBase.D = 0;

            var variants = PipeMathService.SweepByDiameter(pBase, Dmin, Dmax, steps);
            
            Variants.Clear();
            foreach (var variant in variants)
            {
                Variants.Add(variant);
            }
            
            UpdateGraphics();

            _messageBoxService.Show($"Рассчитано {Variants.Count} вариантов", "Перебор завершен", MessageBoxButton.OK);
        }
        catch (ArgumentException ex)
        {
            _messageBoxService.Show($"Ошибка валидации: {ex.Message}", "Ошибка", MessageBoxButton.OK);
        }
        catch (Exception ex)
        {
            _messageBoxService.Show($"Ошибка при переборе диаметров: {ex.Message}", "Ошибка", MessageBoxButton.OK);
        }
    });

    private RelayCommand _exportVariantsCommand;
    /// <summary>
    /// Команда экспорта таблицы вариантов в CSV
    /// </summary>
    public RelayCommand ExportVariantsCommand => _exportVariantsCommand ??= new RelayCommand(_ =>
    {
        try
        {
            if (!Variants.Any())
            {
                _messageBoxService.Show("Таблица вариантов пуста", "Ошибка", MessageBoxButton.OK);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
                FileName = $"Варианты_перебора_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                DefaultExt = "csv"
            };

            if (dialog.ShowDialog() == true)
            {
                var csv = new StringBuilder();
                
                // Заголовки колонок
                csv.AppendLine("D;v;dP_total;N;ESpipe;isDPok;isNok;isESok;allOk");
                
                foreach (var variant in Variants)
                {
                    csv.AppendLine($"{variant.D.ToString(CultureInfo.InvariantCulture)};{variant.v.ToString(CultureInfo.InvariantCulture)};{variant.dP_total.ToString(CultureInfo.InvariantCulture)};{variant.PumpPower.ToString(CultureInfo.InvariantCulture)};{variant.PipelineEnergyConsumption.ToString(CultureInfo.InvariantCulture)};{variant.isDPtotalOk};{variant.isPumpPowerOk};{variant.isESok};{variant.isAllKeyOk}");
                }
                
                File.WriteAllText(dialog.FileName, csv.ToString(), Encoding.UTF8);
                _messageBoxService.Show($"Таблица экспортирована в файл:\n{dialog.FileName}", "Экспорт завершен", MessageBoxButton.OK);
            }
        }
        catch (Exception ex)
        {
            _messageBoxService.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK);
        }
    });

    #endregion

    /// <summary>Ширина колонки статуса в символах (моноширинный шрифт; «Предупреждение» = 13).</summary>
    private const int CriterionStatusColumnChars = 18;

    /// <summary>Статус слева фиксированной ширины, затем критерий — чтобы в UI статусы не «скакали» из‑за длины названия.</summary>
    private static string FormatCriterionStatusLine(string criterionName, ValidationStatus status) =>
        "  " + status.ToRuLabelCapitalized().PadRight(CriterionStatusColumnChars) + "— " + criterionName;

    private PipeCalculationParameters BuildPipeParameters()
    {
        return new PipeCalculationParameters
        {
            EF = EF,
            EFmin = EFmin,
            ESmax = ESmax,
            rho = rho,
            D = D,
            L = L,
            sumZeta = sumZeta,
            lambda = lambda,
            eta = eta,
            DeltaPmax = DeltaPmax,
            Nmax = Nmax,
            WorkingPressure = WorkingPressure,
            AllowablePressure = AllowablePressure,
            AllowableStress = AllowableStress,
            ActualWallThickness = ActualWallThickness,
            FluidTemperature = FluidTemperature,
            MaxFluidTemperature = MaxFluidTemperature,
            MaxVelocity = MaxVelocity
        };
    }

    private void NotifyPipeResultChanged()
    {
        OnPropertyChanged(nameof(dP_total));
        OnPropertyChanged(nameof(dP_fric));
        OnPropertyChanged(nameof(dP_local));
        OnPropertyChanged(nameof(ES));
        OnPropertyChanged(nameof(Q));
        OnPropertyChanged(nameof(PumpPower));
        OnPropertyChanged(nameof(PipelineEnergyConsumption));
        OnPropertyChanged(nameof(CalculatedWallThickness));
        OnPropertyChanged(nameof(deltaEF));
        OnPropertyChanged(nameof(deltaES));
        OnPropertyChanged(nameof(isEFok));
        OnPropertyChanged(nameof(isESok));
        OnPropertyChanged(nameof(isDPtotalOk));
        OnPropertyChanged(nameof(isPumpPowerOk));
        OnPropertyChanged(nameof(isAllowablePressureOk));
        OnPropertyChanged(nameof(isWallThicknessOk));
        OnPropertyChanged(nameof(isFluidTemperatureOk));
        OnPropertyChanged(nameof(isNormativeVelocityOk));
        OnPropertyChanged(nameof(HasResult));
    }

    #region Графики

    /// <summary>
    /// Оси для графика ΔPΣ(D)
    /// </summary>
    public Axis[] DPTotalXAxes { get; set; }
    
    /// <summary>
    /// Оси Y для графика ΔPΣ(D)
    /// </summary>
    public Axis[] DPTotalYAxes { get; set; }
    
    /// <summary>
    /// Оси для графика ES(D)
    /// </summary>
    public Axis[] ESXAxes { get; set; }
    
    /// <summary>
    /// Оси Y для графика ES(D)
    /// </summary>
    public Axis[] ESYAxes { get; set; }

    /// <summary>
    /// Серия для графика ΔPΣ(D)
    /// </summary>
    private LineSeries<ObservablePoint> DPTotalLineSeries { get; set; }
    
    /// <summary>
    /// Серия для графика ES(D)
    /// </summary>
    private LineSeries<ObservablePoint> ESLineSeries { get; set; }

    /// <summary>
    /// Серии для графика ΔPΣ(D)
    /// </summary>
    public List<ISeries> DPTotalSeries { get; set; }

    /// <summary>
    /// Серии для графика ES(D)
    /// </summary>
    public List<ISeries> ESSeries { get; set; }

    /// <summary>
    /// Инициализация графиков
    /// </summary>
    private void InitializeCharts()
    {
        DPTotalXAxes = new[]
        {
            new Axis
            {
                Name = "D, м",
                TextSize = 14,
                LabelsPaint = new SolidColorPaint(SKColors.Black),
            }
        };

        DPTotalYAxes = new[]
        {
            new Axis
            {
                Name = "ΔPΣ, Па",
                TextSize = 14,
                LabelsPaint = new SolidColorPaint(SKColors.Black),
            }
        };

        ESXAxes = new[]
        {
            new Axis
            {
                Name = "D, м",
                TextSize = 14,
                LabelsPaint = new SolidColorPaint(SKColors.Black),
            }
        };

        ESYAxes = new[]
        {
            new Axis
            {
                Name = "ESpipe, Вт",
                TextSize = 14,
                LabelsPaint = new SolidColorPaint(SKColors.Black),
            }
        };

        DPTotalLineSeries = new LineSeries<ObservablePoint>
        {
            Name = "ΔPΣ",
            Fill = null,
            GeometryStroke = null,
            GeometryFill = null,
        };

        ESLineSeries = new LineSeries<ObservablePoint>
        {
            Name = "ESpipe",
            Fill = null,
            GeometryStroke = null,
            GeometryFill = null,
        };

        DPTotalSeries = new List<ISeries> { DPTotalLineSeries };
        ESSeries = new List<ISeries> { ESLineSeries };
    }

    /// <summary>
    /// Обновление графиков на основе коллекции Variants
    /// </summary>
    private void UpdateGraphics()
    {
        if (!Variants.Any())
        {
            DPTotalLineSeries.Values = Array.Empty<ObservablePoint>();
            ESLineSeries.Values = Array.Empty<ObservablePoint>();
            OnPropertyChanged(nameof(DPTotalSeries));
            OnPropertyChanged(nameof(ESSeries));
            return;
        }

        var dValues = Variants.Select(v => v.D).ToList();
        var dpTotalValues = Variants.Select(v => v.dP_total).ToList();
        var espipeValues = Variants.Select(v => v.PipelineEnergyConsumption).ToList();

        UpdateLineSeries(DPTotalLineSeries, dValues, dpTotalValues);
        UpdateLineSeries(ESLineSeries, dValues, espipeValues);

        OnPropertyChanged(nameof(DPTotalSeries));
        OnPropertyChanged(nameof(ESSeries));
    }

    /// <summary>
    /// Обновление серии графика
    /// </summary>
    private void UpdateLineSeries(LineSeries<ObservablePoint> series, List<double> x, List<double> y)
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

        series.Values = points;
    }

    #endregion
}
