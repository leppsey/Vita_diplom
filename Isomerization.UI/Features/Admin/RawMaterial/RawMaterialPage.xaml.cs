using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Isomerization.UI.Features.Admin.RawMaterial;

public partial class RawMaterialPage : INavigableView<RawMaterialPageVM>
{
    public RawMaterialPage()
    {
        ViewModel = App.GetService<RawMaterialPageVM>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public RawMaterialPageVM ViewModel { get; }
}