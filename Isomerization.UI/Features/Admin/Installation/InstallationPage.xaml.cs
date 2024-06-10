using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Isomerization.UI.Features.Admin.Installation;

public partial class InstallationPage : INavigableView<InstallationPageVM>
{
    public InstallationPage()
    {
        ViewModel = App.GetService<InstallationPageVM>();
        DataContext = ViewModel;
        InitializeComponent();    }

    public InstallationPageVM ViewModel { get; }
}