using System.Windows.Controls;
using Isomerization.UI.Misc;

namespace Isomerization.UI.Features.Researcher;

public partial class InstallationRenderControl : UserControl, IViewWithVM<InstallationRenderControlVM>
{
    public InstallationRenderControl()
    {
        InitializeComponent();
        ViewModel = App.GetService<InstallationRenderControlVM>();
        DataContext = ViewModel;
    }

    public InstallationRenderControlVM ViewModel { get; set; }
}