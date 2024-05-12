
using System.Collections.ObjectModel;
using Isomerization.Domain.Data;
using Isomerization.Shared;
using Isomerization.UI.Features.Admin.Catalyst;
using Isomerization.UI.Misc;
using Isomerization.UI.Services;
using Microsoft.EntityFrameworkCore;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace Isomerization.UI.Features.Admin;

public class CatalystPageVM: ViewModelBase
{
    private readonly IsomerizationContext _context;
    private readonly EditDialogService _editDialogService;
    private readonly ISnackbarService _snackbarService;
    private readonly IContentMessageBoxService _messageBoxService;

    public CatalystPageVM(IsomerizationContext context, EditDialogService editDialogService, ISnackbarService snackbarService, IContentMessageBoxService messageBoxService)
    {
        _context = context;
        _editDialogService = editDialogService;
        _snackbarService = snackbarService;
        _messageBoxService = messageBoxService;
        Catalysts = new ObservableCollection<Domain.Models.Catalyst>(context.Catalysts.ToList());
    }
    public ObservableCollection<Domain.Models.Catalyst> Catalysts { get; set; }


    private RelayCommand _editCatalystCommand;
    public RelayCommand EditCatalystCommand => _editCatalystCommand ??= new RelayCommand(async catalyst   =>
    {
        var res = await _editDialogService.ShowDialog<CatalystEditControl, Domain.Models.Catalyst>((Domain.Models.Catalyst)catalyst);
        if (res is null)
        {
            return;
        }

        _context.Entry(res).State = EntityState.Modified;
        _context.SaveChanges();
        _snackbarService.Show("База данных обновлена", "Данные о катализаторе обновлены", timeout: TimeSpan.FromMilliseconds(2000));
    });
    
    private RelayCommand _addCatalystCommand;
    public RelayCommand AddCatalystCommand => _addCatalystCommand ??= new RelayCommand(async _ =>
    {
        var res = await _editDialogService.ShowDialog<CatalystEditControl, Domain.Models.Catalyst>(new Domain.Models.Catalyst());

        if (res is null)
        {
            return;
        }

        _context.Entry(res).State = EntityState.Modified;
        _context.SaveChanges();
        
        _snackbarService.Show("База данных обновлена", "Данные о катализаторе добавлены", timeout: TimeSpan.FromMilliseconds(2000));
    });
    
    private RelayCommand _deleteCatalystCommand;
    public RelayCommand DeleteCatalystCommand => _deleteCatalystCommand ??= new RelayCommand(async catalystObj =>
    {
        var catalyst = (Domain.Models.Catalyst)catalystObj;
        var result = await _messageBoxService.Show(
            $"Вы действительно хотите удалить катализатор под названием \"{catalyst.Name}\"", "Предупреждение",
            MessageBoxButton.OKCancel);
        if (result != MessageBoxResult.OK)
        {
            return;
        }

        var findedCatalyst = _context.Catalysts.Find(catalyst.CatalystId);
        _context.Remove(findedCatalyst);
        _context.SaveChanges();
        Catalysts.Remove(catalyst);
        _snackbarService.Show("База данных обновлена", "Данные о катализаторе удалены", timeout: TimeSpan.FromMilliseconds(2000));
    });
}