using Wpf.Ui.Controls;

namespace Isomerization.UI.Features.Researcher;

public partial class Cim2Page : INavigableView<Cim2PageViewModel>
{
    public Cim2Page()
    {
        ViewModel = App.GetService<Cim2PageViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
        ViewModel.LoadFromSession();
    }

    public Cim2PageViewModel ViewModel { get; }
}
