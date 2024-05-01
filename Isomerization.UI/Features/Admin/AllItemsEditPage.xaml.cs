using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Isomerization.UI.Features.Admin;

public partial class AllItemsEditPage : INavigableView<AllItemsEditPageVM>
{
    public AllItemsEditPage()
    {
        ViewModel = App.GetService<AllItemsEditPageVM>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public AllItemsEditPageVM ViewModel { get; }
}