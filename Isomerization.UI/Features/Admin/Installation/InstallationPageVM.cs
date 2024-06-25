using System.Collections.ObjectModel;
using System.Windows;
using Isomerization.Domain.Data;
using Isomerization.Domain.Models;
using Isomerization.Shared;
using Isomerization.UI.Misc;
using Isomerization.UI.Services;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Wpf.Ui;
using Wpf.Ui.Extensions;
namespace Isomerization.UI.Features.Admin.Installation;

public class InstallationPageVM: ViewModelBase
{
    private readonly IsomerizationContext _context;
    private readonly EditDialogService _editDialogService;
    private readonly ISnackbarService _snackbarService;
    private readonly IContentMessageBoxService _messageBoxService;
    public ObservableCollection<Domain.Models.Installation> Installations { get; set; }


    public InstallationPageVM(IsomerizationContext context, EditDialogService editDialogService, ISnackbarService snackbarService, IContentMessageBoxService messageBoxService)
    {
        _context = context;
        _editDialogService = editDialogService;
        _snackbarService = snackbarService;
        _messageBoxService = messageBoxService;
        Installations = new ObservableCollection<Domain.Models.Installation>(context.Installations.ToList());
    }
    private RelayCommand _editInstallationCommand;
    public RelayCommand EditInstallationCommand => _editInstallationCommand ??= new RelayCommand(async installation   =>
    {
        var initialObject = (Domain.Models.Installation)installation;

        var res = await _editDialogService.ShowDialog<InstallationEditControl, Domain.Models.Installation>(initialObject.Adapt<Domain.Models.Installation>());
        if (res is null)
        {
            return;
        }

        res.Adapt(initialObject);
        _context.Entry(initialObject).State = EntityState.Modified;
        _context.SaveChanges();
        _snackbarService.Show("База данных обновлена", "Данные об установке обновлены", timeout: TimeSpan.FromMilliseconds(2000));
    });
    
    private RelayCommand _addInstallationCommand;
    public RelayCommand AddInstallationCommand => _addInstallationCommand ??= new RelayCommand(async _ =>
    {
        var res = await _editDialogService.ShowDialog<InstallationEditControl, Domain.Models.Installation>(new Domain.Models.Installation(){Model = new Model()});

        if (res is null)
        {
            return;
        }

        _context.Entry(res).State = EntityState.Added;
        _context.SaveChanges();
        Installations.Add(res);
        _snackbarService.Show("База данных обновлена", "Данные об установке добавлены", timeout: TimeSpan.FromMilliseconds(2000));
    });

    
    private RelayCommand _deleteInstallationCommand;
    public RelayCommand DeleteInstallationCommand => _deleteInstallationCommand ??= new RelayCommand(async installationObj =>
    {
        var installation = (Domain.Models.Installation)installationObj;
        var result = await _messageBoxService.Show(
            $"Вы действительно хотите удалить установку под названием \"{installation.Name}\"", "Предупреждение",
            MessageBoxButton.OKCancel);
        if (result != MessageBoxResult.OK)
        {
            return;
        }

        var findedInstallation = _context.Catalysts.Find(installation.InstallationId);
        _context.Remove(findedInstallation);
        _context.SaveChanges();
        Installations.Remove(installation);
        _snackbarService.Show("База данных обновлена", "Данные об установке удалены", timeout: TimeSpan.FromMilliseconds(2000));
    });
}