using System.Windows;
using System.Windows.Controls;
using Isomerization.Domain.Models;
using Isomerization.UI.Misc;

namespace Isomerization.UI.Features.Researcher;

public partial class InstallationRenderControl : UserControl, IViewWithVM<InstallationRenderControlVM>
{
    public InstallationRenderControl()
    {
        ViewModel = App.GetService<InstallationRenderControlVM>();
        InitializeComponent();
        RootGrid.DataContext = ViewModel;
    }

    public InstallationRenderControlVM ViewModel { get; set; }
    
    public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
        nameof(Model), typeof(Model), typeof(InstallationRenderControl), new FrameworkPropertyMetadata(
            null,
            new PropertyChangedCallback(PropertyChangedCallback)));

    private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is InstallationRenderControl control)
        {
            control.ViewModel.Model = e.NewValue as Model;
        }
    }

    public Model Model
    {
        get { return (Model)GetValue(ModelProperty); }
        set
        {
            SetValue(ModelProperty, value);
            ViewModel.Model = value;
        }
    }
}