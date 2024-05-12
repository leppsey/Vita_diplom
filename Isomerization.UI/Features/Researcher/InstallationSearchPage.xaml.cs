using System.Windows.Controls;
using Isomerization.UI.Misc;

namespace Isomerization.UI.Features.Researcher;

public partial class InstallationSearchPage : IViewWithVM<InstallationSearchPageVM>
{
    public InstallationSearchPage()
    {
        ViewModel = App.GetService<InstallationSearchPageVM>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public InstallationSearchPageVM ViewModel { get; set; }
}