using System.Collections.ObjectModel;
using System.Reflection;
using Isomerization.Domain.Data;
using Isomerization.Shared;
using Isomerization.UI.Features.Admin.Cim2;
using Isomerization.UI.Features.Admin.Installation;
using Isomerization.UI.Features.Admin.Kinetic;
using Isomerization.UI.Features.Admin.RawMaterial;
using Microsoft.EntityFrameworkCore;
using Wpf.Ui;

namespace Isomerization.UI.Features.Admin;

public class AllItemsEditPageVM : ViewModelBase
{
    private readonly IsomerizationContext _context;
    private readonly INavigationService _navigationService;

    public AllItemsEditPageVM(
        IsomerizationContext context,
        INavigationService navigationService)
    {
        _context = context;
        _navigationService = navigationService;

        BuildDashboard();
    }

    public string SystemTitle { get; } =
        "Интеллектуальная система для технологического проектирования установок изомеризации";

    public string PanelTitle { get; } = "Панель инженера по знаниям";

    public string PanelSubtitle { get; } =
        "Управление информационным обеспечением, продукционными правилами и параметрами ЦИМ-2";

    public ObservableCollection<DashboardStatusCard> StatusCards { get; } = new();
    public ObservableCollection<DashboardSectionGroup> SectionGroups { get; } = new();

    public IReadOnlyList<string> WorkflowSteps { get; } = new[]
    {
        "Проверить и актуализировать справочники процесса и ЦИМ-2.",
        "Проверить продукционные правила.",
        "Запустить симулятор правил.",
        "Передать данные проектировщику для формирования ЦИМ-2."
    };

    private RelayCommand? _navigateSectionCommand;
    public RelayCommand NavigateSectionCommand => _navigateSectionCommand ??= new RelayCommand(param =>
    {
        if (param is not DashboardSectionCard card)
        {
            return;
        }

        if (!card.IsNavigable || string.IsNullOrWhiteSpace(card.Link))
        {
            return;
        }

        var pageType = ResolvePageType(card.Link);
        if (pageType != null)
        {
            _navigationService.Navigate(pageType);
        }
    });

    private RelayCommand? _addRuleCommand;
    public RelayCommand AddRuleCommand => _addRuleCommand ??= new RelayCommand(_ =>
        _navigationService.Navigate(typeof(PipelineRulesPage)));

    private RelayCommand? _openSimulatorCommand;
    public RelayCommand OpenSimulatorCommand => _openSimulatorCommand ??= new RelayCommand(_ =>
        _navigationService.Navigate(typeof(PipelineRulesPage)));

    private void BuildDashboard()
    {
        var catalystCount = _context.Catalysts.Count();
        var installationCount = _context.Installations.Count();
        var rawMaterialCount = _context.RawMaterials.Count();
        var kineticCount = _context.Kinetics.Count();
        var pipeCount = _context.PipelinePipes.Count();
        var fittingCount = _context.PipelineElbows.Count()
                           + _context.PipelineReducers.Count()
                           + _context.PipelineValves.Count()
                           + _context.PipelinePumps.Count()
                           + _context.PipelineFilters.Count();
        var templateCount = _context.PipelineTemplates.Count() + _context.Pipeline3DTemplates.Count();
        var rulesCount = _context.PipelineRules.Count();
        var activeRulesCount = _context.PipelineRules.Count(x => x.IsEnabled);
        var inactiveRulesCount = rulesCount - activeRulesCount;

        var dictionarySections = 7;
        var warningCount = inactiveRulesCount;

        StatusCards.Clear();
        StatusCards.Add(new DashboardStatusCard
        {
            Title = "Справочники",
            Value = $"{dictionarySections} разделов",
            Caption = "Информационное обеспечение системы"
        });
        StatusCards.Add(new DashboardStatusCard
        {
            Title = "Продукционные правила",
            Value = rulesCount > 0 ? $"{rulesCount} правил" : "0 правил",
            Caption = "Правила проектирования ЦИМ-2"
        });
        StatusCards.Add(new DashboardStatusCard
        {
            Title = "Предупреждения",
            Value = warningCount.ToString(),
            Caption = "Требуют проверки"
        });
        StatusCards.Add(new DashboardStatusCard
        {
            Title = "Последнее обновление",
            Value = DateTime.Now.ToString("dd.MM.yyyy"),
            Caption = "База знаний актуализирована"
        });

        SectionGroups.Clear();

        SectionGroups.Add(new DashboardSectionGroup
        {
            Title = "База данных процесса",
            Cards = new List<DashboardSectionCard>
            {
                CreateCard("Сырье",
                    "Номенклатура и свойства сырья для расчёта ЦИМ-1",
                    $"{rawMaterialCount} записей · актуально",
                    "Актуально",
                    DashboardStatusKind.Actual,
                    nameof(RawMaterialPage),
                    Wpf.Ui.Controls.SymbolRegular.Beaker24),
                CreateCard("Катализаторы",
                    "Характеристики катализаторов процесса изомеризации",
                    $"{catalystCount} записей · актуально",
                    "Актуально",
                    DashboardStatusKind.Actual,
                    nameof(CatalystPage),
                    Wpf.Ui.Controls.SymbolRegular.Cube24),
                CreateCard("Технологические установки",
                    "Конфигурации установок и 3D-модели оборудования",
                    $"{installationCount} записей · актуально",
                    "Актуально",
                    DashboardStatusKind.Actual,
                    nameof(InstallationPage),
                    Wpf.Ui.Controls.SymbolRegular.Building24),
                CreateCard("Кинетические параметры",
                    "Константы скорости и энергетические параметры реакций",
                    $"{kineticCount} записей · актуально",
                    "Актуально",
                    DashboardStatusKind.Actual,
                    nameof(KineticPage),
                    Wpf.Ui.Controls.SymbolRegular.DataHistogram24)
            }
        });

        SectionGroups.Add(new DashboardSectionGroup
        {
            Title = "База данных ЦИМ-2",
            Cards = new List<DashboardSectionCard>
            {
                CreateCard("ЦИМ-2: Трубы и типоразмеры",
                    "Номинальные диаметры, толщина стенки и гидравлические параметры",
                    $"{pipeCount} записей · актуально",
                    "Актуально",
                    DashboardStatusKind.Actual,
                    nameof(PipelinePipesPage),
                    Wpf.Ui.Controls.SymbolRegular.Database24),
                CreateCard("ЦИМ-2: Арматура и детали",
                    "Отводы, переходы, арматура, насосы и фильтры линии",
                    $"{fittingCount} записей · актуально",
                    "Актуально",
                    DashboardStatusKind.Actual,
                    nameof(Cim2DictionariesPage),
                    Wpf.Ui.Controls.SymbolRegular.Settings24),
                CreateCard("ЦИМ-2: Шаблоны трубопроводных линий",
                    "Типовые конфигурации и 3D-шаблоны трубопроводных линий",
                    $"{templateCount} шаблонов · актуально",
                    "Актуально",
                    DashboardStatusKind.Actual,
                    nameof(Cim2DictionariesPage),
                    Wpf.Ui.Controls.SymbolRegular.Document24)
            }
        });

        var rulesMetadata = rulesCount > 0
            ? $"{rulesCount} правил · {activeRulesCount} активно"
            : "нет записей";
        var rulesStatus = inactiveRulesCount > 0 ? "Требует проверки" : "Актуально";
        var rulesStatusKind = inactiveRulesCount > 0
            ? DashboardStatusKind.NeedsReview
            : DashboardStatusKind.Actual;

        SectionGroups.Add(new DashboardSectionGroup
        {
            Title = "База знаний",
            Cards = new List<DashboardSectionCard>
            {
                CreateCard("База продукционных правил ЦИМ-2",
                    "Правила выбора и проверки параметров трубопроводной системы",
                    rulesMetadata,
                    rulesStatus,
                    rulesStatusKind,
                    nameof(PipelineRulesPage),
                    Wpf.Ui.Controls.SymbolRegular.DocumentBulletList24),
                CreateCard("Симулятор правил",
                    "Проверка срабатывания правил на тестовых сценариях",
                    "доступен в разделе правил",
                    "Актуально",
                    DashboardStatusKind.Actual,
                    nameof(PipelineRulesPage),
                    Wpf.Ui.Controls.SymbolRegular.Play24)
            }
        });
    }

    private static DashboardSectionCard CreateCard(
        string name,
        string description,
        string metadata,
        string statusText,
        DashboardStatusKind statusKind,
        string? link,
        Wpf.Ui.Controls.SymbolRegular icon,
        bool isNavigable = true)
    {
        return new DashboardSectionCard
        {
            Name = name,
            Description = description,
            MetadataText = metadata,
            StatusText = statusText,
            StatusKind = statusKind,
            Link = link,
            Icon = icon,
            IsNavigable = isNavigable && !string.IsNullOrWhiteSpace(link)
        };
    }

    private static Type? ResolvePageType(string pageName)
    {
        pageName = pageName.Trim().ToLowerInvariant();
        return Assembly.GetExecutingAssembly()
            .GetTypes()
            .FirstOrDefault(t =>
                t.Namespace?.StartsWith("Isomerization.UI", StringComparison.OrdinalIgnoreCase) == true
                && t.Name.Equals(pageName, StringComparison.OrdinalIgnoreCase));
    }
}
