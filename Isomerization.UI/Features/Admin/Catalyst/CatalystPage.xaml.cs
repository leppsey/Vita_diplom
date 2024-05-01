using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Isomerization.UI.Features.Admin;

public partial class CatalystPage : INavigableView<CatalystPageVM>
{
    public CatalystPage()
    {
        ViewModel = App.GetService<CatalystPageVM>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public CatalystPageVM ViewModel { get; }
}