using System.Collections.ObjectModel;
using System.Windows;
using Isomerization.Domain.Data;
using Isomerization.Shared;
using Isomerization.UI.Features.Admin.RawMaterial;
using Isomerization.UI.Misc;
using Isomerization.UI.Services;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Wpf.Ui;
using Wpf.Ui.Extensions;
namespace Isomerization.UI.Features.Admin.Kinetic;

public class KineticPageVM: ViewModelBase
{
    private readonly IsomerizationContext _context;
    private readonly EditDialogService _editDialogService;
    private readonly ISnackbarService _snackbarService;
    private readonly IContentMessageBoxService _messageBoxService;
    
    public ObservableCollection<Domain.Models.Kinetic> Kinetics { get; set; }
    public KineticPageVM(IsomerizationContext context, EditDialogService editDialogService, ISnackbarService snackbarService, IContentMessageBoxService messageBoxService)
    {
        _context = context;
        _editDialogService = editDialogService;
        _snackbarService = snackbarService;
        _messageBoxService = messageBoxService;
        Kinetics = new ObservableCollection<Domain.Models.Kinetic>(_context.Kinetics.Include(x => x.RawMaterial));
    }
    
    
    private RelayCommand _editKineticCommand;
    public RelayCommand EditKineticCommand => _editKineticCommand ??= new RelayCommand(async kinetic   =>
    {
        var initialObject = (Domain.Models.Kinetic)kinetic;

        var res = await _editDialogService.ShowDialog<KineticEditControl, Domain.Models.Kinetic>(initialObject.Adapt<Domain.Models.Kinetic>());
        if (res is null)
        {
            return;
        }

        var existingEntity = _context.Kinetics
            .Include(k => k.RawMaterial)
            .SingleOrDefault(k => k.KineticId == initialObject.KineticId);

        if (existingEntity != null)
        {
            // Удалить зависимую сущность
            _context.Kinetics.Remove(existingEntity);
            _context.SaveChanges();

            // Создать новую зависимую сущность
            initialObject.RawMaterial = res.RawMaterial; // newRawMaterial - это новый объект RawMaterial
            _context.Kinetics.Add(initialObject);
            _context.SaveChanges();
        }
        _snackbarService.Show("База данных обновлена", "Данные о кинетике обновлены", timeout: TimeSpan.FromMilliseconds(2000));
    });
    
    private RelayCommand _addKineticCommand;
    public RelayCommand AddKineticCommand => _addKineticCommand ??= new RelayCommand(async _ =>
    {
        var res = await _editDialogService.ShowDialog<KineticEditControl, Domain.Models.Kinetic>(new Domain.Models.Kinetic());

        if (res is null)
        {
            return;
        }

        _context.Entry(res).State = EntityState.Added;
        _context.SaveChanges();
        Kinetics.Add(res);
        _snackbarService.Show("База данных обновлена", "Данные о кинетике добавлены", timeout: TimeSpan.FromMilliseconds(2000));
    });
    
    private RelayCommand _deleteKineticCommand;
    public RelayCommand DeleteKineticCommand => _deleteKineticCommand ??= new RelayCommand(async kineticObj =>
    {
        var kinetic = (Domain.Models.Kinetic)kineticObj;
        var result = await _messageBoxService.Show(
            $"Вы действительно хотите удалить кинетику", "Предупреждение",
            MessageBoxButton.OKCancel);
        if (result != MessageBoxResult.OK)
        {
            return;
        }

        var findedKinetic = _context.Kinetics.Find(kinetic.KineticId);
        _context.Remove(findedKinetic);
        _context.SaveChanges();
        Kinetics.Remove(findedKinetic);
        _snackbarService.Show("База данных обновлена", "Данные о кинетике удалены", timeout: TimeSpan.FromMilliseconds(2000));
    });
}