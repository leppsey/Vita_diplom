using Isomerization.Shared;
using Isomerization.UI.Misc;
using UI.Services.MyDialogService;

namespace Isomerization.UI.Features.Admin.Catalyst;

public class CatalystEditControlVM: ViewModelBase,  IDialogEditViewModel<Domain.Models.Catalyst>
{
    public Domain.Models.Catalyst Result { get; set; }

    public Domain.Models.Catalyst Data { get; set; }

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

