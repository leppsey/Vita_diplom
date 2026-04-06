using System.Collections.ObjectModel;
using System.Windows;
using Isomerization.Domain.Data;
using Isomerization.Domain.Models;
using Isomerization.Shared;
using Isomerization.UI.Misc;
using Isomerization.UI.Services;
using Microsoft.EntityFrameworkCore;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace Isomerization.UI.Features.Admin.Cim2;

public class Cim2DictionariesPageVM : ViewModelBase
{
    private readonly IsomerizationContext _context;
    private readonly EditDialogService _editDialogService;
    private readonly ISnackbarService _snackbarService;
    private readonly IContentMessageBoxService _messageBoxService;

    public Cim2DictionariesPageVM(IsomerizationContext context, EditDialogService editDialogService, ISnackbarService snackbarService, IContentMessageBoxService messageBoxService)
    {
        _context = context;
        _editDialogService = editDialogService;
        _snackbarService = snackbarService;
        _messageBoxService = messageBoxService;

        Pipes = new ObservableCollection<PipelinePipe>(_context.PipelinePipes.ToList());
        Elbows = new ObservableCollection<PipelineElbow>(_context.PipelineElbows.ToList());
        Reducers = new ObservableCollection<PipelineReducer>(_context.PipelineReducers.ToList());
        Valves = new ObservableCollection<PipelineValve>(_context.PipelineValves.ToList());
        Pumps = new ObservableCollection<PipelinePump>(_context.PipelinePumps.ToList());
        Filters = new ObservableCollection<PipelineFilter>(_context.PipelineFilters.ToList());
        Templates = new ObservableCollection<PipelineTemplate>(_context.PipelineTemplates.ToList());
        Templates3D = new ObservableCollection<Pipeline3DTemplate>(_context.Pipeline3DTemplates.ToList());
        Rules = new ObservableCollection<PipelineRule>(_context.PipelineRules.ToList());
    }

    public ObservableCollection<PipelinePipe> Pipes { get; }
    public ObservableCollection<PipelineElbow> Elbows { get; }
    public ObservableCollection<PipelineReducer> Reducers { get; }
    public ObservableCollection<PipelineValve> Valves { get; }
    public ObservableCollection<PipelinePump> Pumps { get; }
    public ObservableCollection<PipelineFilter> Filters { get; }
    public ObservableCollection<PipelineTemplate> Templates { get; }
    public ObservableCollection<Pipeline3DTemplate> Templates3D { get; }
    public ObservableCollection<PipelineRule> Rules { get; }

    private RelayCommand _saveAllCommand;
    public RelayCommand SaveAllCommand => _saveAllCommand ??= new RelayCommand(_ =>
    {
        _context.SaveChanges();
        _snackbarService.Show("База данных обновлена", "Изменения справочников ЦИМ-2 сохранены", timeout: TimeSpan.FromMilliseconds(2000));
    });

    private RelayCommand _addPipeCommand;
    public RelayCommand AddPipeCommand => _addPipeCommand ??= new RelayCommand(_ => AddEntity(Pipes, _context.PipelinePipes, new PipelinePipe
    {
        Name = "Новая труба", DN = 80, Material = "Сталь 20", PressureClass = "PN16", Standard = "ГОСТ"
    }));

    private RelayCommand _addElbowCommand;
    public RelayCommand AddElbowCommand => _addElbowCommand ??= new RelayCommand(_ => AddEntity(Elbows, _context.PipelineElbows, new PipelineElbow
    {
        Name = "Новый отвод", DN = 80, Angle = 90, Zeta = 0.8, PressureClass = "PN16", Standard = "ГОСТ"
    }));

    private RelayCommand _addReducerCommand;
    public RelayCommand AddReducerCommand => _addReducerCommand ??= new RelayCommand(_ => AddEntity(Reducers, _context.PipelineReducers, new PipelineReducer
    {
        Name = "Новый переход", DNIn = 100, DNOut = 80, Zeta = 0.15, PressureClass = "PN16", Standard = "ГОСТ"
    }));

    private RelayCommand _addValveCommand;
    public RelayCommand AddValveCommand => _addValveCommand ??= new RelayCommand(_ => AddEntity(Valves, _context.PipelineValves, new PipelineValve
    {
        Name = "Новая арматура", Type = "Gate", DN = 80, Zeta = 0.2, PressureClass = "PN16", Standard = "ГОСТ"
    }));

    private RelayCommand _addPumpCommand;
    public RelayCommand AddPumpCommand => _addPumpCommand ??= new RelayCommand(_ => AddEntity(Pumps, _context.PipelinePumps, new PipelinePump
    {
        Name = "Новый насос", DN = 80, Efficiency = 0.7
    }));

    private RelayCommand _addFilterCommand;
    public RelayCommand AddFilterCommand => _addFilterCommand ??= new RelayCommand(_ => AddEntity(Filters, _context.PipelineFilters, new PipelineFilter
    {
        Name = "Новый фильтр", DN = 80, Zeta = 1.2, PressureClass = "PN16"
    }));

    private RelayCommand _addTemplateCommand;
    public RelayCommand AddTemplateCommand => _addTemplateCommand ??= new RelayCommand(_ => AddEntity(Templates, _context.PipelineTemplates, new PipelineTemplate
    {
        Name = "Новый шаблон", LineType = "ReactorInletLine", SupportedDN = "80"
    }));

    private RelayCommand _addTemplate3DCommand;
    public RelayCommand AddTemplate3DCommand => _addTemplate3DCommand ??= new RelayCommand(_ => AddEntity(Templates3D, _context.Pipeline3DTemplates, new Pipeline3DTemplate
    {
        Name = "Новый 3D шаблон", LineType = "ReactorInletLine", SupportedDN = "80"
    }));

    private RelayCommand _addRuleCommand;
    public RelayCommand AddRuleCommand => _addRuleCommand ??= new RelayCommand(_ => AddEntity(Rules, _context.PipelineRules, new PipelineRule
    {
        Name = "Новое правило", ConditionType = "ReactorSelected", ConditionOperator = "=", ConditionValue = "true", ActionType = "SetLineType", ActionValue = "ReactorInletLine", IsEnabled = true
    }));

    private RelayCommand _editPipeCommand;
    public RelayCommand EditPipeCommand => _editPipeCommand ??= new RelayCommand(async o => await EditEntity(o as PipelinePipe));
    private RelayCommand _editElbowCommand;
    public RelayCommand EditElbowCommand => _editElbowCommand ??= new RelayCommand(async o => await EditEntity(o as PipelineElbow));
    private RelayCommand _editReducerCommand;
    public RelayCommand EditReducerCommand => _editReducerCommand ??= new RelayCommand(async o => await EditEntity(o as PipelineReducer));
    private RelayCommand _editValveCommand;
    public RelayCommand EditValveCommand => _editValveCommand ??= new RelayCommand(async o => await EditEntity(o as PipelineValve));
    private RelayCommand _editPumpCommand;
    public RelayCommand EditPumpCommand => _editPumpCommand ??= new RelayCommand(async o => await EditEntity(o as PipelinePump));
    private RelayCommand _editFilterCommand;
    public RelayCommand EditFilterCommand => _editFilterCommand ??= new RelayCommand(async o => await EditEntity(o as PipelineFilter));
    private RelayCommand _editTemplateCommand;
    public RelayCommand EditTemplateCommand => _editTemplateCommand ??= new RelayCommand(async o => await EditEntity(o as PipelineTemplate));
    private RelayCommand _editTemplate3DCommand;
    public RelayCommand EditTemplate3DCommand => _editTemplate3DCommand ??= new RelayCommand(async o => await EditEntity(o as Pipeline3DTemplate));
    private RelayCommand _editRuleCommand;
    public RelayCommand EditRuleCommand => _editRuleCommand ??= new RelayCommand(async o => await EditEntity(o as PipelineRule));

    private RelayCommand _deletePipeCommand;
    public RelayCommand DeletePipeCommand => _deletePipeCommand ??= new RelayCommand(async o =>
    {
        if (o is PipelinePipe entity)
            await DeleteEntity(Pipes, _context.PipelinePipes, entity);
    });

    private RelayCommand _deleteElbowCommand;
    public RelayCommand DeleteElbowCommand => _deleteElbowCommand ??= new RelayCommand(async o =>
    {
        if (o is PipelineElbow entity)
            await DeleteEntity(Elbows, _context.PipelineElbows, entity);
    });

    private RelayCommand _deleteReducerCommand;
    public RelayCommand DeleteReducerCommand => _deleteReducerCommand ??= new RelayCommand(async o =>
    {
        if (o is PipelineReducer entity)
            await DeleteEntity(Reducers, _context.PipelineReducers, entity);
    });

    private RelayCommand _deleteValveCommand;
    public RelayCommand DeleteValveCommand => _deleteValveCommand ??= new RelayCommand(async o =>
    {
        if (o is PipelineValve entity)
            await DeleteEntity(Valves, _context.PipelineValves, entity);
    });

    private RelayCommand _deletePumpCommand;
    public RelayCommand DeletePumpCommand => _deletePumpCommand ??= new RelayCommand(async o =>
    {
        if (o is PipelinePump entity)
            await DeleteEntity(Pumps, _context.PipelinePumps, entity);
    });

    private RelayCommand _deleteFilterCommand;
    public RelayCommand DeleteFilterCommand => _deleteFilterCommand ??= new RelayCommand(async o =>
    {
        if (o is PipelineFilter entity)
            await DeleteEntity(Filters, _context.PipelineFilters, entity);
    });

    private RelayCommand _deleteTemplateCommand;
    public RelayCommand DeleteTemplateCommand => _deleteTemplateCommand ??= new RelayCommand(async o =>
    {
        if (o is PipelineTemplate entity)
            await DeleteEntity(Templates, _context.PipelineTemplates, entity);
    });

    private RelayCommand _deleteTemplate3DCommand;
    public RelayCommand DeleteTemplate3DCommand => _deleteTemplate3DCommand ??= new RelayCommand(async o =>
    {
        if (o is Pipeline3DTemplate entity)
            await DeleteEntity(Templates3D, _context.Pipeline3DTemplates, entity);
    });

    private RelayCommand _deleteRuleCommand;
    public RelayCommand DeleteRuleCommand => _deleteRuleCommand ??= new RelayCommand(async o =>
    {
        if (o is PipelineRule entity)
            await DeleteEntity(Rules, _context.PipelineRules, entity);
    });

    private void AddEntity<TEntity>(ObservableCollection<TEntity> list, Microsoft.EntityFrameworkCore.DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class
    {
        dbSet.Add(entity);
        _context.SaveChanges();
        list.Add(entity);
        _snackbarService.Show("База данных обновлена", "Запись добавлена", timeout: TimeSpan.FromMilliseconds(1500));
    }

    private async Task DeleteEntity<TEntity>(ObservableCollection<TEntity> list, Microsoft.EntityFrameworkCore.DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class
    {
        var result = await _messageBoxService.Show("Удалить выбранную запись?", "Предупреждение", MessageBoxButton.OKCancel);
        if (result != MessageBoxResult.OK) return;
        dbSet.Remove(entity);
        _context.SaveChanges();
        list.Remove(entity);
        _snackbarService.Show("База данных обновлена", "Запись удалена", timeout: TimeSpan.FromMilliseconds(1500));
    }

    private async Task EditEntity<TEntity>(TEntity? entity) where TEntity : class
    {
        if (entity is null)
        {
            return;
        }

        var res = await _editDialogService.ShowDialog<Cim2GenericEditControl, object>(entity);
        if (res is null)
        {
            return;
        }

        _context.Entry(entity).State = EntityState.Modified;
        _context.SaveChanges();
        _snackbarService.Show("База данных обновлена", "Запись обновлена", timeout: TimeSpan.FromMilliseconds(1500));
    }
}
