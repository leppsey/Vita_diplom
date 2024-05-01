using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Isomerization.UI.Features.Admin.Kinetic;

public partial class KineticPage : INavigableView<KineticPageVM>
{
    public KineticPage()
    {
        ViewModel = App.GetService<KineticPageVM>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public KineticPageVM ViewModel { get; }
}