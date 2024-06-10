using Isomerization.Shared;
using Isomerization.UI.Misc;

namespace Isomerization.UI.Features.Admin.RawMaterial;

public class RawMaterialEditControlVM: ViewModelBase, IDialogEditViewModel<Domain.Models.RawMaterial>
{
    public Domain.Models.RawMaterial Data { get; set; }
    public Domain.Models.RawMaterial Result { get; private set; }
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