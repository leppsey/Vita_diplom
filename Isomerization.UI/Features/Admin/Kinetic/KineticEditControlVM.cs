using System.Collections.ObjectModel;
using Isomerization.Domain.Data;
using Isomerization.Shared;
using Isomerization.UI.Misc;

namespace Isomerization.UI.Features.Admin.Kinetic;

public class KineticEditControlVM: ViewModelBase, IDialogEditViewModel<Domain.Models.Kinetic>
{
    private readonly IsomerizationContext _context;

    public KineticEditControlVM(IsomerizationContext context)
    {
        _context = context;
        RawMaterials = new ObservableCollection<Domain.Models.RawMaterial>(_context.RawMaterials);
    }
    
    public ObservableCollection<Domain.Models.RawMaterial> RawMaterials { get; set; }
    public Domain.Models.Kinetic Data { get; set; }
    public Domain.Models.Kinetic Result { get; private set; }
    public Action FinishInteraction { get; set; }
    
    private RelayCommand _applyCommand;

    public RelayCommand ApplyCommand
    {
        get
        {
            return _applyCommand ??= new RelayCommand(o =>
            {
                Result = Data;
                FinishInteraction();
            }, _=> !Data?.HasErrors ?? false);
        }
    }
    
    private RelayCommand _cancelCommand;

    public RelayCommand CancelCommand
    {
        get
        {
            return _cancelCommand ??= new RelayCommand(o =>
            {
                Result = null;
                FinishInteraction();
            });
        }
    }
}