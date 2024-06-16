using System.Windows;
using Isomerization.UI.Misc;

namespace Isomerization.UI.Features.Researcher;

public partial class SelectDIMIsomerizationWindow : IViewWithVM<SelectDIMIsomerizationWindowVM>
{
    public SelectDIMIsomerizationWindow()
    {
        ViewModel = App.GetService<SelectDIMIsomerizationWindowVM>();
        DataContext = ViewModel;
        InitializeComponent();
        ViewModel.Window = this;
    }

    public SelectDIMIsomerizationWindowVM ViewModel { get; set; }
}