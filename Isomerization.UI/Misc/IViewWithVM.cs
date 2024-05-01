namespace Isomerization.UI.Misc;

public interface IViewWithVM<ViewModelType> : IViewWithVM
{
    public ViewModelType ViewModel { get; set; }

    object IViewWithVM.ViewModelObject => ViewModel;
}


public interface IViewWithVM
{
    public object ViewModelObject { get; }
}
