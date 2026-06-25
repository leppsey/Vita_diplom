using System.Windows;
using Wpf.Ui.Controls;

namespace Isomerization.UI.Features.Researcher;

public partial class Cim2Page : INavigableView<Cim2PageViewModel>
{
    public Cim2Page()
    {
        ViewModel = App.GetService<Cim2PageViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
        ViewModel.RenderModelChanged += (_, _) => ApplyRenderModel();
        ViewModel.RequestReset3DView += (_, _) => RenderControl.ResetView();
        Loaded += OnLoaded;
    }

    public Cim2PageViewModel ViewModel { get; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel.LoadFromSession();
        ApplyRenderModel();
    }

    private void ApplyRenderModel()
    {
        RenderControl.Model = ViewModel.RenderModel;
    }
}
