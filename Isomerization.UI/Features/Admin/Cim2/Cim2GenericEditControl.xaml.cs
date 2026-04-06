using Isomerization.UI.Misc;

namespace Isomerization.UI.Features.Admin.Cim2;

public partial class Cim2GenericEditControl : IDialogEditControl<object>
{
    public Cim2GenericEditControl()
    {
        ViewModel = App.GetService<Cim2GenericEditControlVM>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public IDialogEditViewModel<object> ViewModel { get; set; }
}
