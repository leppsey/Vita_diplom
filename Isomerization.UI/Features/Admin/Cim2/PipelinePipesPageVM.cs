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

public class PipelinePipesPageVM : ViewModelBase
{
    private readonly IsomerizationContext _context;
    private readonly EditDialogService _editDialogService;
    private readonly ISnackbarService _snackbarService;
    private readonly IContentMessageBoxService _messageBoxService;

    public PipelinePipesPageVM(IsomerizationContext context, EditDialogService editDialogService, ISnackbarService snackbarService, IContentMessageBoxService messageBoxService)
    {
        _context = context;
        _editDialogService = editDialogService;
        _snackbarService = snackbarService;
        _messageBoxService = messageBoxService;
        Items = new ObservableCollection<PipelinePipe>(_context.PipelinePipes.ToList());
    }

    public ObservableCollection<PipelinePipe> Items { get; }

    private RelayCommand _addCommand;
    public RelayCommand AddCommand => _addCommand ??= new RelayCommand(_ =>
    {
        var entity = new PipelinePipe
        {
            Name = "Новая труба",
            DN = 80,
            Material = "Сталь 20",
            PressureClass = "PN16",
            Standard = "ГОСТ",
            ModelPath = string.Empty
        };
        _context.PipelinePipes.Add(entity);
        _context.SaveChanges();
        Items.Add(entity);
        _snackbarService.Show("База данных обновлена", "Труба добавлена", timeout: TimeSpan.FromMilliseconds(2000));
    });

    private RelayCommand _deleteCommand;
    public RelayCommand DeleteCommand => _deleteCommand ??= new RelayCommand(async obj =>
    {
        if (obj is not PipelinePipe entity)
        {
            return;
        }

        var result = await _messageBoxService.Show($"Удалить элемент \"{entity.Name}\"?", "Предупреждение", MessageBoxButton.OKCancel);
        if (result != MessageBoxResult.OK) return;
        _context.PipelinePipes.Remove(entity);
        _context.SaveChanges();
        Items.Remove(entity);
        _snackbarService.Show("База данных обновлена", "Труба удалена", timeout: TimeSpan.FromMilliseconds(2000));
    });

    private RelayCommand _editCommand;
    public RelayCommand EditCommand => _editCommand ??= new RelayCommand(async obj =>
    {
        if (obj is not PipelinePipe entity)
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
        _snackbarService.Show("База данных обновлена", "Труба обновлена", timeout: TimeSpan.FromMilliseconds(2000));
    });

    private RelayCommand _saveCommand;
    public RelayCommand SaveCommand => _saveCommand ??= new RelayCommand(_ =>
    {
        _context.SaveChanges();
        _snackbarService.Show("База данных обновлена", "Изменения сохранены", timeout: TimeSpan.FromMilliseconds(2000));
    });
}
