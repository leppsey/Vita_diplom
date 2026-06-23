using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Isomerization.Domain.Cim2;
using Isomerization.Domain.Data;
using Isomerization.Domain.Models;
using Isomerization.Shared;
using Isomerization.UI.Services;
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

    public event EventHandler? RenderModelChanged;

    public bool RequiresPump { get; set; }
    public bool RequiresDirectionChange { get; set; }
    public double AllowedVelocity { get; set; } = 2.5;
    public double PipeLength { get; set; } = 20;
    public double MaxPressureLoss { get; set; } = 250000;
    public double MaxEnergyConsumption { get; set; } = 10000;
    public double Efficiency { get; set; } = 0.7;
    public double AllowableStress { get; set; } = 120_000_000;
    public string PreferredPressureClass { get; set; } = "PN16";

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
        RenderModelChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ClearRenderModel()
    {
        RenderModel = null;
        Is3DBlockVisible = false;
        OnPropertyChanged(nameof(RenderModel));
        OnPropertyChanged(nameof(Is3DBlockVisible));
        RenderModelChanged?.Invoke(this, EventArgs.Empty);
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

        OnPropertyChanged(nameof(Elements));
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

        var sb = new StringBuilder();
        sb.AppendLine($"Шаблон: {TemplateName}");
        sb.AppendLine($"DN: {SelectedDn}");
        sb.AppendLine($"v: {Velocity:F3} м/с");
        sb.AppendLine($"ΔPΣ: {PressureLossTotal:F2} Па");
        sb.AppendLine($"Энергопотребление: {EnergyConsumption:F2} кВт");
        sb.AppendLine($"Проверка: {(IsValid ? "пройдена" : "не пройдена")}");
        _messageBoxService.Show(sb.ToString(), "Результат ЦИМ-2", MessageBoxButton.OK);
    });
}
