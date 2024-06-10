using System.Collections.ObjectModel;
using Isomerization.Domain.Data;
using Isomerization.Shared;
using Isomerization.UI.Features.Admin.Catalyst;
using Isomerization.UI.Misc;
using Isomerization.UI.Services;
using Microsoft.EntityFrameworkCore;
using Wpf.Ui;
using Wpf.Ui.Extensions;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;


namespace Isomerization.UI.Features.Admin.RawMaterial;

public class RawMaterialPageVM: ViewModelBase
{
    private readonly IsomerizationContext _context;
    private readonly EditDialogService _editDialogService;
    private readonly ISnackbarService _snackbarService;
    private readonly IContentMessageBoxService _messageBoxService;
    public RawMaterialPageVM(IsomerizationContext context, EditDialogService editDialogService, ISnackbarService snackbarService, IContentMessageBoxService messageBoxService)
    {
        _context = context;
        _editDialogService = editDialogService;
        _snackbarService = snackbarService;
        _messageBoxService = messageBoxService;
        RawMaterials = new ObservableCollection<Domain.Models.RawMaterial>(context.RawMaterials.ToList());
    }
    public ObservableCollection<Domain.Models.RawMaterial> RawMaterials { get; set; }


    private RelayCommand _editRawMaterialCommand;
    public RelayCommand EditRawMaterialCommand => _editRawMaterialCommand ??= new RelayCommand(async rawMaterial   =>
    {
        var res = await _editDialogService.ShowDialog<RawMaterialEditControl, Domain.Models.RawMaterial>((Domain.Models.RawMaterial)rawMaterial);
        if (res is null)
        {
            return;
        }

        _context.Entry(res).State = EntityState.Modified;
        _context.SaveChanges();
        _snackbarService.Show("База данных обновлена", "Данные о сырье обновлены", timeout: TimeSpan.FromMilliseconds(2000));
    });
    
    private RelayCommand _addRawMaterialCommand;
    public RelayCommand AddRawMaterialCommand => _addRawMaterialCommand ??= new RelayCommand(async _ =>
    {
        var res = await _editDialogService.ShowDialog<RawMaterialEditControl, Domain.Models.RawMaterial>(new Domain.Models.RawMaterial());

        if (res is null)
        {
            return;
        }

        _context.Entry(res).State = EntityState.Modified;
        _context.SaveChanges();
        
        _snackbarService.Show("База данных обновлена", "Данные о сырье добавлены", timeout: TimeSpan.FromMilliseconds(2000));
    });
    
    private RelayCommand _deleteRawMaterialCommand;
    public RelayCommand DeleteRawMaterialCommand => _deleteRawMaterialCommand ??= new RelayCommand(async rawMaterialObj =>
    {
        var rawMaterial = (Domain.Models.RawMaterial)rawMaterialObj;
        var result = await _messageBoxService.Show(
            $"Вы действительно хотите удалить сырье под названием \"{rawMaterial.Name}\"", "Предупреждение",
            MessageBoxButton.OKCancel);
        if (result != MessageBoxResult.OK)
        {
            return;
        }

        var findedRawMaterial = _context.Catalysts.Find(rawMaterial.RawMaterialId);
        _context.Remove(findedRawMaterial);
        _context.SaveChanges();
        RawMaterials.Remove(rawMaterial);
        _snackbarService.Show("База данных обновлена", "Данные о сырье удалены", timeout: TimeSpan.FromMilliseconds(2000));
    });
}