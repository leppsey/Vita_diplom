using UI.Services.MyDialogService;

namespace Isomerization.UI.Features.Admin.Catalyst;

public class CatalystEditControlVM: ViewModelBase, IInteractionAware, IResultHolder, IDataHolder
{
    private Domain.Models.Catalyst EditingCatalyst { get; set; }
    public Action FinishInteraction { get; set; }
    public object Result { get; set; }

    public object Data
    {
        get => EditingCatalyst;
        set => EditingCatalyst = (Domain.Models.Catalyst) value;
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

    private RelayCommand _applyCommand;

    public RelayCommand ApplyCommand
    {
        get
        {
            return _applyCommand ??= new RelayCommand(o =>
            {
                Result = EditingCatalyst;
                FinishInteraction();
            }, _=> !EditingCatalyst.HasErrors);
        }
    }
}