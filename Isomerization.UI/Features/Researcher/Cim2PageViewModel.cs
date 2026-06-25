using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Isomerization.Domain.Cim2;
using Isomerization.Domain.Data;
using Isomerization.Domain.Models;
using Isomerization.Shared;
using Isomerization.UI.Services;
using Microsoft.Win32;
using Microsoft.EntityFrameworkCore;

namespace Isomerization.UI.Features.Researcher;

public class Cim2PageViewModel : ViewModelBase
{
    private readonly Cim2OrchestratorService _orchestrator;
    private readonly ICim2SessionService _session;
    private readonly IMessageBoxService _messageBoxService;
    private readonly IMenuService _menuService;
    private readonly IsomerizationContext _context;

    public Cim2PageViewModel(
        Cim2OrchestratorService orchestrator,
        ICim2SessionService session,
        IMessageBoxService messageBoxService,
        IMenuService menuService,
        IsomerizationContext context)
    {
        _orchestrator = orchestrator;
        _session = session;
        _messageBoxService = messageBoxService;
        _menuService = menuService;
        _context = context;
        Elements = new ObservableCollection<PipelineElement>();
    }

    public ObservableCollection<PipelineElement> Elements { get; }
    public ObservableCollection<Cim2LineElementRow> ElementRows { get; } = new();
    public ObservableCollection<Cim2MetricCard> MetricCards { get; } = new();
    public ObservableCollection<Cim2ConstraintCheckRow> ConstraintChecks { get; } = new();
    public ObservableCollection<Cim2RuleTraceRow> RuleTraceRows { get; } = new();
    public ObservableCollection<Cim2AlternativeOptionRow> AlternativeOptions { get; } = new();
    public ObservableCollection<Cim2JournalEntry> JournalEntries { get; } = new();

    public string ReactorName { get; set; } = string.Empty;
    public double FlowRate { get; set; }
    public double Temperature { get; set; }
    public double Pressure { get; set; }
    public double Density { get; set; }

    public int SelectedDn { get; set; }
    public string TemplateName { get; set; } = string.Empty;

    public double Velocity { get; set; }
    public double PressureLossTotal { get; set; }
    public double EnergyConsumption { get; set; }
    public double PumpPower { get; set; }
    public double WallThickness { get; set; }

    public bool IsValid { get; set; }
    public string ViolationsText { get; set; } = string.Empty;
    public string RecommendationsText { get; set; } = string.Empty;
    public string Template3DName { get; set; } = string.Empty;
    public string Template3DPath { get; set; } = string.Empty;
    public Model? RenderModel { get; private set; }
    public bool Is3DBlockVisible { get; private set; }
    public bool Is3DExpanded { get; private set; }
    public double RenderViewportHeight => Is3DExpanded ? 700 : 380;
    public double RenderBlockMinHeight => Is3DExpanded ? 760 : 420;

    public event EventHandler? RenderModelChanged;
    public event EventHandler? RequestReset3DView;
    private Cim2ModelPreviewWindow? _previewWindow;

    public bool RequiresPump { get; set; }
    public bool RequiresDirectionChange { get; set; }
    public double AllowedVelocity { get; set; } = 2.5;
    public double PipeLength { get; set; } = 20;
    public double MaxPressureLoss { get; set; } = 250000;
    public double MaxEnergyConsumption { get; set; } = 10000;
    public double Efficiency { get; set; } = 0.7;
    public double AllowableStress { get; set; } = 120_000_000;
    public string PreferredPressureClass { get; set; } = "PN16";
    public string OverallStatusText { get; private set; } = "Расчет не выполнен.";
    public string FormationStatusText => IsValid ? "Конфигурация допустима" : "Требуется корректировка";
    public int ElementsCount => ElementRows.Count;
    public string ActiveRulesText => RuleTraceRows.Count == 0
        ? "Правила еще не применялись"
        : $"{RuleTraceRows.Count(x => x.Result == "Сработало")} из {RuleTraceRows.Count} правил сработали";
    public string SelectedElementDetails { get; private set; } = "Выберите элемент в таблице состава линии.";
    public bool ShowElementParameters { get; set; }

    private Cim2LineElementRow _selectedElementRow;
    public Cim2LineElementRow SelectedElementRow
    {
        get => _selectedElementRow;
        set
        {
            _selectedElementRow = value;
            BuildSelectedElementDetails();
            OnPropertyChanged();
        }
    }

    public bool HasCim1Input => _session.LastCim1Result != null;

    public void LoadFromSession()
    {
        var cim1 = _session.LastCim1Result;
        if (cim1 == null)
        {
            ClearRenderModel();
            return;
        }

        ReactorName = cim1.ReactorName;
        FlowRate = cim1.FlowRate;
        Temperature = cim1.Temperature;
        Pressure = cim1.PressureKPa;
        Density = cim1.DensityGsm3;
        Apply3DFromCim2Result(_session.LastCim2Result);
        OnPropertyChanged(nameof(ReactorName));
        OnPropertyChanged(nameof(FlowRate));
        OnPropertyChanged(nameof(Temperature));
        OnPropertyChanged(nameof(Pressure));
        OnPropertyChanged(nameof(Density));
        OnPropertyChanged(nameof(HasCim1Input));
    }

    private void Apply3DFromCim2Result(Cim2Result? cim2Result)
    {
        if (cim2Result == null || string.IsNullOrWhiteSpace(cim2Result.Template3DName))
        {
            ClearRenderModel();
            return;
        }

        var template = _context.Pipeline3DTemplates
            .AsNoTracking()
            .FirstOrDefault(t => t.Name == cim2Result.Template3DName);

        if (template == null && !string.IsNullOrWhiteSpace(cim2Result.Template3DPath))
        {
            template = _context.Pipeline3DTemplates
                .AsNoTracking()
                .FirstOrDefault(t => t.ModelPath == cim2Result.Template3DPath);
        }

        if (template == null || string.IsNullOrWhiteSpace(template.ModelPath))
        {
            ClearRenderModel();
            return;
        }

        Template3DName = template.Name;
        Template3DPath = template.ModelPath;
        RenderModel = new Model
        {
            ObjPath = template.ModelPath.Replace('\\', '/'),
        };
        Is3DBlockVisible = true;

        OnPropertyChanged(nameof(Template3DName));
        OnPropertyChanged(nameof(Template3DPath));
        OnPropertyChanged(nameof(RenderModel));
        OnPropertyChanged(nameof(Is3DBlockVisible));
        OnPropertyChanged(nameof(RenderViewportHeight));
        OnPropertyChanged(nameof(RenderBlockMinHeight));
        OnPropertyChanged(nameof(ElementsCount));
        RenderModelChanged?.Invoke(this, EventArgs.Empty);
        UpdatePreviewWindow();
    }

    private void ClearRenderModel()
    {
        RenderModel = null;
        Is3DBlockVisible = false;
        OnPropertyChanged(nameof(RenderModel));
        OnPropertyChanged(nameof(Is3DBlockVisible));
        OnPropertyChanged(nameof(RenderViewportHeight));
        OnPropertyChanged(nameof(RenderBlockMinHeight));
        OnPropertyChanged(nameof(ElementsCount));
        RenderModelChanged?.Invoke(this, EventArgs.Empty);
        UpdatePreviewWindow();
    }

    private void UpdatePreviewWindow()
    {
        if (_previewWindow == null)
        {
            return;
        }

        _previewWindow.SetModel(RenderModel, Template3DName, Template3DPath);
    }

    private RelayCommand _goHomeMenu;
    public RelayCommand GoHomeMenu => _goHomeMenu ??= new RelayCommand(_ => _menuService.GoHome());

    private RelayCommand _goBackCommand;
    public RelayCommand GoBackCommand => _goBackCommand ??= new RelayCommand(_ => _menuService.GoHome());

    private RelayCommand _runCim2Command;
    public RelayCommand RunCim2Command => _runCim2Command ??= new RelayCommand(_ =>
    {
        if (_session.LastCim1Result == null)
        {
            _messageBoxService.Show("Нет данных ЦИМ-1. Сначала выполните расчет ЦИМ-1.", "ЦИМ-2", MessageBoxButton.OK);
            return;
        }

        var request = new Cim2Request
        {
            Cim1Result = _session.LastCim1Result,
            RequiresDirectionChange = RequiresDirectionChange,
            RequiresPump = RequiresPump,
            AllowedVelocity = AllowedVelocity,
            PipeLength = PipeLength,
            MaxPressureLoss = MaxPressureLoss,
            MaxEnergyConsumption = MaxEnergyConsumption,
            Efficiency = Efficiency,
            AllowableStress = AllowableStress,
            PreferredPressureClass = PreferredPressureClass
        };

        var result = _orchestrator.Execute(request);
        _session.LastCim2Result = result;

        Elements.Clear();
        ElementRows.Clear();
        foreach (var element in result.Line.Elements)
        {
            Elements.Add(element);
        }

        SelectedDn = result.Line.Elements.FirstOrDefault(x => x.ElementType == PipelineElementType.Pipe)?.DN ?? 0;
        TemplateName = result.TemplateName;
        Velocity = result.Line.CalculationResult.Velocity;
        PressureLossTotal = result.Line.CalculationResult.PressureLossTotal;
        EnergyConsumption = result.Line.CalculationResult.EnergyConsumption;
        PumpPower = result.Line.CalculationResult.PumpPower;
        WallThickness = result.Line.CalculationResult.CalculatedWallThickness;
        IsValid = result.Line.ValidationResult.IsValid;
        ViolationsText = result.Line.ValidationResult.Violations.Count == 0
            ? "Нарушений не обнаружено."
            : string.Join("\n", result.Line.ValidationResult.Violations);
        RecommendationsText = string.Join("\n", result.Recommendations);
        Template3DName = result.Template3DName;
        Template3DPath = result.Template3DPath;
        Apply3DFromCim2Result(result);
        BuildElementRows(result.Line);
        BuildMetricCards(result, request);
        BuildConstraintChecks(result, request);
        BuildRuleTraceRows(result, request);
        BuildAlternatives(result, request);
        BuildRecommendations(result, request);
        AddToJournal(result, request);

        OnPropertyChanged(nameof(Elements));
        OnPropertyChanged(nameof(ElementRows));
        OnPropertyChanged(nameof(SelectedDn));
        OnPropertyChanged(nameof(TemplateName));
        OnPropertyChanged(nameof(Velocity));
        OnPropertyChanged(nameof(PressureLossTotal));
        OnPropertyChanged(nameof(EnergyConsumption));
        OnPropertyChanged(nameof(PumpPower));
        OnPropertyChanged(nameof(WallThickness));
        OnPropertyChanged(nameof(IsValid));
        OnPropertyChanged(nameof(ViolationsText));
        OnPropertyChanged(nameof(RecommendationsText));
        OnPropertyChanged(nameof(Template3DName));
        OnPropertyChanged(nameof(Template3DPath));
        OnPropertyChanged(nameof(OverallStatusText));
        OnPropertyChanged(nameof(FormationStatusText));
        OnPropertyChanged(nameof(ElementsCount));
        OnPropertyChanged(nameof(ActiveRulesText));
        OnPropertyChanged(nameof(SelectedElementDetails));
        OnPropertyChanged(nameof(ShowElementParameters));

        var sb = new StringBuilder();
        sb.AppendLine($"Шаблон: {TemplateName}");
        sb.AppendLine($"DN: {SelectedDn}");
        sb.AppendLine($"v: {Velocity:F3} м/с");
        sb.AppendLine($"ΔPΣ: {PressureLossTotal:F2} Па");
        sb.AppendLine($"Энергопотребление: {EnergyConsumption:F2} Вт");
        sb.AppendLine($"Проверка: {(IsValid ? "пройдена" : "не пройдена")}");
        _messageBoxService.Show(sb.ToString(), "Результат ЦИМ-2", MessageBoxButton.OK);
    });

    private RelayCommand _toggle3DExpandCommand;
    public RelayCommand Toggle3DExpandCommand => _toggle3DExpandCommand ??= new RelayCommand(_ =>
    {
        Is3DExpanded = !Is3DExpanded;
        OnPropertyChanged(nameof(Is3DExpanded));
        OnPropertyChanged(nameof(RenderViewportHeight));
        OnPropertyChanged(nameof(RenderBlockMinHeight));
    }, _ => Is3DBlockVisible);

    private RelayCommand _open3DInWindowCommand;
    public RelayCommand Open3DInWindowCommand => _open3DInWindowCommand ??= new RelayCommand(_ =>
    {
        if (RenderModel == null)
        {
            return;
        }

        if (_previewWindow == null || !_previewWindow.IsLoaded)
        {
            _previewWindow = new Cim2ModelPreviewWindow
            {
                Owner = Application.Current?.MainWindow
            };
            _previewWindow.Closed += (_, _) => _previewWindow = null;
        }

        _previewWindow.SetModel(RenderModel, Template3DName, Template3DPath);
        _previewWindow.Show();
        _previewWindow.Activate();
    }, _ => Is3DBlockVisible && RenderModel != null);

    private RelayCommand _reset3DViewCommand;
    public RelayCommand Reset3DViewCommand => _reset3DViewCommand ??= new RelayCommand(_ =>
    {
        RequestReset3DView?.Invoke(this, EventArgs.Empty);
    }, _ => Is3DBlockVisible);

    private RelayCommand _toggleElementParamsCommand;
    public RelayCommand ToggleElementParamsCommand => _toggleElementParamsCommand ??= new RelayCommand(_ =>
    {
        ShowElementParameters = !ShowElementParameters;
        OnPropertyChanged(nameof(ShowElementParameters));
    }, _ => ElementRows.Count > 0);

    private RelayCommand _saveCalculationCommand;
    public RelayCommand SaveCalculationCommand => _saveCalculationCommand ??= new RelayCommand(_ =>
    {
        if (_session.LastCim2Result == null)
        {
            _messageBoxService.Show("Сначала выполните расчет ЦИМ-2.", "Сохранение", MessageBoxButton.OK);
            return;
        }

        AddToJournal(_session.LastCim2Result, BuildRequestSnapshot());
        _messageBoxService.Show("Расчет сохранен в журнале.", "Сохранение", MessageBoxButton.OK);
    });

    private RelayCommand _saveVariantCommand;
    public RelayCommand SaveVariantCommand => _saveVariantCommand ??= new RelayCommand(_ =>
    {
        if (_session.LastCim2Result == null)
        {
            _messageBoxService.Show("Нет рассчитанного варианта линии.", "Вариант", MessageBoxButton.OK);
            return;
        }

        AddToJournal(_session.LastCim2Result, BuildRequestSnapshot());
        _messageBoxService.Show("Вариант линии сохранен в журнале.", "Вариант", MessageBoxButton.OK);
    });

    private RelayCommand _exportExcelCommand;
    public RelayCommand ExportExcelCommand => _exportExcelCommand ??= new RelayCommand(_ =>
    {
        if (_session.LastCim2Result == null)
        {
            _messageBoxService.Show("Сначала выполните расчет ЦИМ-2.", "Экспорт", MessageBoxButton.OK);
            return;
        }

        var dialog = new SaveFileDialog
        {
            Title = "Экспорт результатов ЦИМ-2",
            Filter = "CSV файл (*.csv)|*.csv",
            FileName = $"cim2_export_{DateTime.Now:yyyyMMdd_HHmm}.csv"
        };
        if (dialog.ShowDialog() != true)
        {
            return;
        }

        var lines = new List<string>
        {
            "Показатель;Значение;Единица;Ограничение;Статус"
        };
        lines.AddRange(MetricCards.Select(x =>
            $"{x.Title};{x.ValueText};{x.Unit};{x.LimitText};{x.StatusText}"));
        File.WriteAllLines(dialog.FileName, lines, Encoding.UTF8);
        _messageBoxService.Show("Экспорт CSV выполнен.", "Экспорт", MessageBoxButton.OK);
    });

    private RelayCommand _buildReportCommand;
    public RelayCommand BuildReportCommand => _buildReportCommand ??= new RelayCommand(_ =>
    {
        if (_session.LastCim2Result == null)
        {
            _messageBoxService.Show("Сначала выполните расчет ЦИМ-2.", "Отчет", MessageBoxButton.OK);
            return;
        }

        var dialog = new SaveFileDialog
        {
            Title = "Сформировать отчет ЦИМ-2",
            Filter = "Текстовый файл (*.txt)|*.txt",
            FileName = $"cim2_report_{DateTime.Now:yyyyMMdd_HHmm}.txt"
        };
        if (dialog.ShowDialog() != true)
        {
            return;
        }

        var report = BuildReportText();
        File.WriteAllText(dialog.FileName, report, Encoding.UTF8);
        _messageBoxService.Show("Отчет ЦИМ-2 сформирован.", "Отчет", MessageBoxButton.OK);
    });

    private Cim2Request BuildRequestSnapshot() => new()
    {
        Cim1Result = _session.LastCim1Result ?? new Cim1Result(),
        RequiresDirectionChange = RequiresDirectionChange,
        RequiresPump = RequiresPump,
        AllowedVelocity = AllowedVelocity,
        PipeLength = PipeLength,
        MaxPressureLoss = MaxPressureLoss,
        MaxEnergyConsumption = MaxEnergyConsumption,
        Efficiency = Efficiency,
        AllowableStress = AllowableStress,
        PreferredPressureClass = PreferredPressureClass
    };

    private void BuildElementRows(PipelineLine line)
    {
        ElementRows.Clear();
        var totalLength = line.Elements.Where(x => x.ElementType == PipelineElementType.Pipe).Sum(x => x.Length);
        var totalZeta = line.Elements.Where(x => x.ElementType != PipelineElementType.Pipe).Sum(x => x.Zeta);
        var linearTotal = line.CalculationResult.PressureLossLinear;
        var localTotal = line.CalculationResult.PressureLossLocal;

        for (var i = 0; i < line.Elements.Count; i++)
        {
            var element = line.Elements[i];
            var elementLoss = 0.0;
            if (element.ElementType == PipelineElementType.Pipe)
            {
                elementLoss = totalLength > 0 ? linearTotal * (element.Length / totalLength) : 0;
            }
            else
            {
                elementLoss = totalZeta > 0 ? localTotal * (element.Zeta / totalZeta) : 0;
            }

            var share = line.CalculationResult.PressureLossTotal > 0
                ? elementLoss / line.CalculationResult.PressureLossTotal * 100.0
                : 0;

            ElementRows.Add(new Cim2LineElementRow
            {
                Index = i + 1,
                Element = element,
                LossPressure = elementLoss,
                LossSharePercent = share,
                StatusText = elementLoss > 0 ? "Учитывается" : "Справочно"
            });
        }

        if (ElementRows.Count > 0)
        {
            SelectedElementRow = ElementRows[0];
        }
    }

    private void BuildMetricCards(Cim2Result result, Cim2Request request)
    {
        MetricCards.Clear();
        MetricCards.Add(new Cim2MetricCard
        {
            Title = "Скорость потока",
            Value = result.Line.CalculationResult.Velocity,
            ValueText = result.Line.CalculationResult.Velocity.ToString("F3", CultureInfo.InvariantCulture),
            Unit = "м/с",
            LimitText = request.AllowedVelocity > 0 ? $"<= {request.AllowedVelocity:F3}" : "—",
            StatusText = GetLimitStatus(result.Line.CalculationResult.Velocity, request.AllowedVelocity),
        });
        MetricCards.Add(new Cim2MetricCard
        {
            Title = "Суммарные потери давления",
            Value = result.Line.CalculationResult.PressureLossTotal,
            ValueText = result.Line.CalculationResult.PressureLossTotal.ToString("F0", CultureInfo.InvariantCulture),
            Unit = "Па",
            LimitText = request.MaxPressureLoss > 0 ? $"<= {request.MaxPressureLoss:F0}" : "—",
            StatusText = GetLimitStatus(result.Line.CalculationResult.PressureLossTotal, request.MaxPressureLoss),
        });
        MetricCards.Add(new Cim2MetricCard
        {
            Title = "Энергопотребление",
            Value = result.Line.CalculationResult.EnergyConsumption,
            ValueText = result.Line.CalculationResult.EnergyConsumption.ToString("F2", CultureInfo.InvariantCulture),
            Unit = "Вт",
            LimitText = request.MaxEnergyConsumption > 0 ? $"<= {request.MaxEnergyConsumption:F2}" : "—",
            StatusText = GetLimitStatus(result.Line.CalculationResult.EnergyConsumption, request.MaxEnergyConsumption),
        });
        MetricCards.Add(new Cim2MetricCard
        {
            Title = "Мощность насоса",
            Value = result.Line.CalculationResult.PumpPower,
            ValueText = result.Line.CalculationResult.PumpPower.ToString("F2", CultureInfo.InvariantCulture),
            Unit = "Вт",
            LimitText = "—",
            StatusText = "Справочно"
        });
        MetricCards.Add(new Cim2MetricCard
        {
            Title = "Статус конфигурации",
            Value = result.Line.ValidationResult.IsValid ? 1 : 0,
            ValueText = result.Line.ValidationResult.IsValid ? "Допустима" : "Требует корректировки",
            Unit = "",
            LimitText = "ТЗ",
            StatusText = result.Line.ValidationResult.IsValid ? "Допустимо" : "Нарушение"
        });
    }

    private void BuildConstraintChecks(Cim2Result result, Cim2Request request)
    {
        ConstraintChecks.Clear();
        AddCheck("Скорость потока", result.Line.CalculationResult.Velocity, request.AllowedVelocity, "v ≤ vmax");
        AddCheck("Потери давления", result.Line.CalculationResult.PressureLossTotal, request.MaxPressureLoss, "ΔPΣ ≤ ΔPmax");
        AddCheck("Энергопотребление", result.Line.CalculationResult.EnergyConsumption, request.MaxEnergyConsumption, "ES ≤ ESmax");

        var linePn = ParsePressureClass(PreferredPressureClass);
        var workingBar = Pressure / 100.0;
        var classStatus = linePn >= workingBar;
        ConstraintChecks.Add(new Cim2ConstraintCheckRow
        {
            Parameter = "Класс давления",
            Actual = $"PN{linePn:F0}",
            Limit = $"PN >= {workingBar:F1} бар",
            Condition = "PN линии ≥ Pраб",
            Result = classStatus ? "Выполнено" : "Нарушение",
            StatusText = classStatus ? "Допустимо" : "Нарушение"
        });

        var hasPump = result.Line.Elements.Any(x => x.ElementType == PipelineElementType.Pump);
        ConstraintChecks.Add(new Cim2ConstraintCheckRow
        {
            Parameter = "Наличие насоса",
            Actual = hasPump ? "Да" : "Нет",
            Limit = RequiresPump ? "Да" : "Не обязательно",
            Condition = "Требуется насос",
            Result = !RequiresPump || hasPump ? "Выполнено" : "Нарушение",
            StatusText = !RequiresPump || hasPump ? "Допустимо" : "Нарушение"
        });

        var hasElbow = result.Line.Elements.Any(x => x.ElementType == PipelineElementType.Elbow);
        ConstraintChecks.Add(new Cim2ConstraintCheckRow
        {
            Parameter = "Поворот потока",
            Actual = hasElbow ? "Да" : "Нет",
            Limit = RequiresDirectionChange ? "Да" : "Не обязательно",
            Condition = "Требуется поворот",
            Result = !RequiresDirectionChange || hasElbow ? "Выполнено" : "Нарушение",
            StatusText = !RequiresDirectionChange || hasElbow ? "Допустимо" : "Нарушение"
        });

        OverallStatusText = ConstraintChecks.All(x => x.Result == "Выполнено")
            ? "Конфигурация трубопроводной линии допустима."
            : "Конфигурация требует корректировки.";
    }

    private void BuildRuleTraceRows(Cim2Result result, Cim2Request request)
    {
        RuleTraceRows.Clear();
        var speedOk = request.AllowedVelocity <= 0 || result.Line.CalculationResult.Velocity <= request.AllowedVelocity;
        var lossOk = request.MaxPressureLoss <= 0 || result.Line.CalculationResult.PressureLossTotal <= request.MaxPressureLoss;
        var energyOk = request.MaxEnergyConsumption <= 0 || result.Line.CalculationResult.EnergyConsumption <= request.MaxEnergyConsumption;
        var pnOk = ParsePressureClass(PreferredPressureClass) >= Pressure / 100.0;

        RuleTraceRows.Add(new Cim2RuleTraceRow
        {
            RuleCode = "R1",
            Condition = "Если v ≤ vmax и ΔPΣ ≤ ΔPmax и ES ≤ ESmax",
            Result = speedOk && lossOk && energyOk ? "Сработало" : "Не сработало",
            Recommendation = speedOk && lossOk && energyOk
                ? "Конфигурация допустима по основным ограничениям."
                : "Проверить несоответствующие ограничения."
        });
        RuleTraceRows.Add(new Cim2RuleTraceRow
        {
            RuleCode = "R2",
            Condition = "Если v > vmax",
            Result = speedOk ? "Не сработало" : "Сработало",
            Recommendation = speedOk ? "Изменение DN не требуется." : "Рекомендуется увеличить DN трубопровода."
        });
        RuleTraceRows.Add(new Cim2RuleTraceRow
        {
            RuleCode = "R3",
            Condition = "Если ΔPΣ > ΔPmax",
            Result = lossOk ? "Не сработало" : "Сработало",
            Recommendation = lossOk
                ? "Потери давления в допустимых пределах."
                : "Рекомендуется уменьшить длину линии или количество местных сопротивлений."
        });
        RuleTraceRows.Add(new Cim2RuleTraceRow
        {
            RuleCode = "R4",
            Condition = "Если ES > ESmax",
            Result = energyOk ? "Не сработало" : "Сработало",
            Recommendation = energyOk
                ? "Энергопотребление соответствует ТЗ."
                : "Рекомендуется подобрать насос с более высоким КПД или скорректировать конфигурацию."
        });
        RuleTraceRows.Add(new Cim2RuleTraceRow
        {
            RuleCode = "R5",
            Condition = "Если PN линии < Pраб",
            Result = pnOk ? "Не сработало" : "Сработало",
            Recommendation = pnOk ? "Класс давления достаточен." : "Рекомендуется выбрать более высокий класс давления."
        });
        RuleTraceRows.Add(new Cim2RuleTraceRow
        {
            RuleCode = "R6",
            Condition = "Если требуется насос/поворот, но элемент отсутствует",
            Result = ConstraintChecks.Any(x => x.Parameter is "Наличие насоса" or "Поворот потока" && x.Result == "Нарушение")
                ? "Сработало"
                : "Не сработало",
            Recommendation = "Добавить недостающие элементы согласно технологическим требованиям."
        });
    }

    private void BuildAlternatives(Cim2Result result, Cim2Request request)
    {
        AlternativeOptions.Clear();
        foreach (var dn in new[] { 80, 100, 150 })
        {
            var option = EstimateAlternative(dn, result, request);
            AlternativeOptions.Add(option);
        }

        var recommended = AlternativeOptions
            .Where(x => x.Status == "Допустимо")
            .OrderBy(x => x.Dn)
            .FirstOrDefault();
        if (recommended != null)
        {
            recommended.Comment = "Рекомендуемый вариант (минимальный допустимый DN).";
        }
        else if (AlternativeOptions.Count > 0)
        {
            AlternativeOptions[0].Comment = "Требуется корректировка исходных параметров или шаблона линии.";
        }
    }

    private Cim2AlternativeOptionRow EstimateAlternative(int dn, Cim2Result result, Cim2Request request)
    {
        var density = Density * 1000.0;
        var q = density > 0 ? FlowRate / density : 0;
        var d = dn / 1000.0;
        var area = Math.PI * d * d / 4.0;
        var velocity = area > 0 ? q / area : 0;
        var baseVelocity = Math.Max(result.Line.CalculationResult.Velocity, 1e-6);
        var ratio = velocity / baseVelocity;
        var pressureLoss = result.Line.CalculationResult.PressureLossTotal * ratio * ratio;
        var energy = result.Line.CalculationResult.EnergyConsumption * ratio * ratio;
        var pumpPower = result.Line.CalculationResult.PumpPower * ratio * ratio;

        var speedOk = request.AllowedVelocity <= 0 || velocity <= request.AllowedVelocity;
        var lossOk = request.MaxPressureLoss <= 0 || pressureLoss <= request.MaxPressureLoss;
        var energyOk = request.MaxEnergyConsumption <= 0 || energy <= request.MaxEnergyConsumption;
        var status = speedOk && lossOk && energyOk ? "Допустимо" : "Нарушение";

        return new Cim2AlternativeOptionRow
        {
            Dn = dn,
            Velocity = velocity,
            PressureLoss = pressureLoss,
            EnergyConsumption = energy,
            PumpPower = pumpPower,
            Status = status,
            Comment = status == "Допустимо" ? "Соответствует ограничениям ТЗ." : "Превышены ограничения по скорости/потерям/энергии."
        };
    }

    private void BuildRecommendations(Cim2Result result, Cim2Request request)
    {
        var rec = new List<string>();
        if (ConstraintChecks.All(x => x.Result == "Выполнено"))
        {
            rec.Add("Текущая конфигурация трубопроводной линии допустима.");
            rec.Add("Скорость потока, суммарные потери давления и энергопотребление не превышают ограничения технического задания.");
            rec.Add("Изменение диаметра трубопровода не требуется.");
        }
        else
        {
            if (ConstraintChecks.Any(x => x.Parameter == "Скорость потока" && x.Result == "Нарушение"))
            {
                rec.Add("Рекомендуется увеличить DN трубопровода.");
            }

            if (ConstraintChecks.Any(x => x.Parameter == "Потери давления" && x.Result == "Нарушение"))
            {
                rec.Add("Рекомендуется уменьшить длину линии или количество местных сопротивлений.");
                rec.Add("Рассмотрите замену арматуры на элементы с меньшим коэффициентом ζ.");
            }

            if (ConstraintChecks.Any(x => x.Parameter == "Энергопотребление" && x.Result == "Нарушение"))
            {
                rec.Add("Рекомендуется выбрать насос с более высоким КПД или скорректировать параметры линии.");
            }

            if (ConstraintChecks.Any(x => x.Parameter == "Класс давления" && x.Result == "Нарушение"))
            {
                rec.Add("Рекомендуется выбрать более высокий класс давления (PN).");
            }

            if (ConstraintChecks.Any(x => x.Parameter == "Наличие насоса" && x.Result == "Нарушение"))
            {
                rec.Add("Добавьте насосный элемент в конфигурацию линии.");
            }

            if (ConstraintChecks.Any(x => x.Parameter == "Поворот потока" && x.Result == "Нарушение"))
            {
                rec.Add("Добавьте отводы для обеспечения требуемой трассировки.");
            }
        }

        RecommendationsText = string.Join("\n", rec.Distinct());
    }

    private void AddToJournal(Cim2Result result, Cim2Request request)
    {
        JournalEntries.Insert(0, new Cim2JournalEntry
        {
            Timestamp = DateTime.Now,
            Reactor = ReactorName,
            FlowRate = FlowRate,
            Dn = SelectedDn,
            Velocity = result.Line.CalculationResult.Velocity,
            PressureLoss = result.Line.CalculationResult.PressureLossTotal,
            Energy = result.Line.CalculationResult.EnergyConsumption,
            Status = result.Line.ValidationResult.IsValid ? "Допустимо" : "Нарушение"
        });
        while (JournalEntries.Count > 20)
        {
            JournalEntries.RemoveAt(JournalEntries.Count - 1);
        }

        OnPropertyChanged(nameof(JournalEntries));
    }

    private void AddCheck(string parameter, double actual, double limit, string condition)
    {
        var hasLimit = limit > 0;
        var ok = !hasLimit || actual <= limit;
        ConstraintChecks.Add(new Cim2ConstraintCheckRow
        {
            Parameter = parameter,
            Actual = actual.ToString("F2", CultureInfo.InvariantCulture),
            Limit = hasLimit ? limit.ToString("F2", CultureInfo.InvariantCulture) : "—",
            Condition = condition,
            Result = ok ? "Выполнено" : "Нарушение",
            StatusText = ok ? "Допустимо" : "Нарушение"
        });
    }

    private string BuildReportText()
    {
        var sb = new StringBuilder();
        sb.AppendLine("ОТЧЕТ ЦИМ-2");
        sb.AppendLine($"Дата: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
        sb.AppendLine();
        sb.AppendLine("Исходные данные:");
        sb.AppendLine($"Реактор: {ReactorName}");
        sb.AppendLine($"Расход: {FlowRate:F3} кг/с");
        sb.AppendLine($"Температура: {Temperature:F2} °C");
        sb.AppendLine($"Давление: {Pressure:F2} кПа");
        sb.AppendLine($"Плотность: {Density:F3} г/см3");
        sb.AppendLine();
        sb.AppendLine("Параметры ЦИМ-2:");
        sb.AppendLine($"vmax: {AllowedVelocity:F2} м/с");
        sb.AppendLine($"ΔPmax: {MaxPressureLoss:F0} Па");
        sb.AppendLine($"ESmax: {MaxEnergyConsumption:F2} Вт");
        sb.AppendLine($"КПД: {Efficiency:F2}");
        sb.AppendLine($"Класс давления: {PreferredPressureClass}");
        sb.AppendLine();
        sb.AppendLine("Состав линии:");
        foreach (var row in ElementRows)
        {
            sb.AppendLine($"{row.Index}. {row.Element.ElementType} {row.Element.Name}, DN{row.Element.DN}, L={row.Element.Length:F2}, ζ={row.Element.Zeta:F3}, ΔP={row.LossPressure:F1} Па");
        }

        sb.AppendLine();
        sb.AppendLine("Проверка требований ТЗ:");
        foreach (var row in ConstraintChecks)
        {
            sb.AppendLine($"{row.Parameter}: {row.Actual}; {row.Condition}; {row.Result}");
        }

        sb.AppendLine();
        sb.AppendLine("Сработавшие правила:");
        foreach (var row in RuleTraceRows)
        {
            sb.AppendLine($"{row.RuleCode}: {row.Result} | {row.Recommendation}");
        }

        sb.AppendLine();
        sb.AppendLine("Рекомендации:");
        sb.AppendLine(RecommendationsText);
        sb.AppendLine();
        sb.AppendLine($"Шаблон 3D: {Template3DName}");
        sb.AppendLine($"Путь 3D: {Template3DPath}");
        return sb.ToString();
    }

    private void BuildSelectedElementDetails()
    {
        if (SelectedElementRow == null)
        {
            SelectedElementDetails = "Выберите элемент в таблице состава линии.";
            OnPropertyChanged(nameof(SelectedElementDetails));
            return;
        }

        var e = SelectedElementRow.Element;
        SelectedElementDetails =
            $"Тип: {e.ElementType}\n" +
            $"Наименование: {e.Name}\n" +
            $"DN: {e.DN}\n" +
            $"Длина: {e.Length:F2} м\n" +
            $"ζ: {e.Zeta:F3}\n" +
            $"Потери: {SelectedElementRow.LossPressure:F2} Па\n" +
            $"Материал: {e.Material}\n" +
            $"Класс давления: {e.PressureClass}\n" +
            $"Температурный диапазон: {e.TemperatureMin:F1} .. {e.TemperatureMax:F1} °C";
        OnPropertyChanged(nameof(SelectedElementDetails));
    }

    private static string GetLimitStatus(double actual, double limit)
    {
        if (limit <= 0)
        {
            return "Справочно";
        }

        if (actual > limit)
        {
            return "Нарушение";
        }

        if (actual > limit * 0.9)
        {
            return "Близко к пределу";
        }

        return "Допустимо";
    }

    private static double ParsePressureClass(string pressureClass)
    {
        if (string.IsNullOrWhiteSpace(pressureClass))
        {
            return 0;
        }

        var digits = new string(pressureClass.Where(char.IsDigit).ToArray());
        if (double.TryParse(digits, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        return 0;
    }
}

public class Cim2MetricCard
{
    public string Title { get; set; } = string.Empty;
    public double Value { get; set; }
    public string ValueText { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string LimitText { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
}

public class Cim2ConstraintCheckRow
{
    public string Parameter { get; set; } = string.Empty;
    public string Actual { get; set; } = string.Empty;
    public string Limit { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
}

public class Cim2LineElementRow
{
    public int Index { get; set; }
    public PipelineElement Element { get; set; } = new();
    public double LossPressure { get; set; }
    public double LossSharePercent { get; set; }
    public string StatusText { get; set; } = string.Empty;
}

public class Cim2RuleTraceRow
{
    public string RuleCode { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
}

public class Cim2AlternativeOptionRow
{
    public int Dn { get; set; }
    public double Velocity { get; set; }
    public double PressureLoss { get; set; }
    public double EnergyConsumption { get; set; }
    public double PumpPower { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}

public class Cim2JournalEntry
{
    public DateTime Timestamp { get; set; }
    public string Reactor { get; set; } = string.Empty;
    public double FlowRate { get; set; }
    public int Dn { get; set; }
    public double Velocity { get; set; }
    public double PressureLoss { get; set; }
    public double Energy { get; set; }
    public string Status { get; set; } = string.Empty;
}
