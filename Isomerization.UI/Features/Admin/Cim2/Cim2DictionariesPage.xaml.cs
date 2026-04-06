using Wpf.Ui.Controls;

namespace Isomerization.UI.Features.Admin.Cim2;

public partial class Cim2DictionariesPage : INavigableView<Cim2DictionariesPageVM>
{
    public Cim2DictionariesPage()
    {
        ViewModel = App.GetService<Cim2DictionariesPageVM>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public Cim2DictionariesPageVM ViewModel { get; }
}
