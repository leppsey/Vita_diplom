using System.Windows.Controls;
using Isomerization.UI.Misc;

namespace Isomerization.UI.Features.Admin.Installation;

public partial class InstallationEditControl :  IDialogEditControl<Domain.Models.Installation>
{
    public InstallationEditControl()
    {
        ViewModel = App.GetService<InstallationEditControlVM>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public IDialogEditViewModel<Domain.Models.Installation> ViewModel { get; set; }
}