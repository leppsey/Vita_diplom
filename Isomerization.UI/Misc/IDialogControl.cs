using UI.Services.MyDialogService;

namespace Isomerization.UI.Misc;

public interface IDialogEditControl<TResult>: IViewWithVM<IDialogEditViewModel<TResult>>
{
    IDialogEditViewModel<TResult> ViewModel { get; set; }
}
public interface IDialogEditViewModel<TResult>: IDataHolder<TResult>, IResultHolder<TResult>, IInteractionAware{}