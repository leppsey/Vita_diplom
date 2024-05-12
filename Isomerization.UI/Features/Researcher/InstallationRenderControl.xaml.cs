using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Isomerization.UI.Misc;

namespace Isomerization.UI.Features.Researcher;

public partial class InstallationRenderControl : UserControl, IViewWithVM<InstallationRenderControlVM>
{
    public InstallationRenderControl()
    {
        ViewModel = App.GetService<InstallationRenderControlVM>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public InstallationRenderControlVM ViewModel { get; set; }
    
    public static readonly DependencyProperty ModelPathProperty = DependencyProperty.RegisterAttached(
        nameof(ModelPath), typeof(string), typeof(InstallationRenderControl), new FrameworkPropertyMetadata(
            "null",
            new PropertyChangedCallback(PropertyChangedCallback)));

    private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Debug.WriteLine(e.NewValue);
    }

    public string ModelPath
    {
        get { return (string)GetValue(ModelPathProperty); }
        set
        {
            SetValue(ModelPathProperty, value);
            ViewModel.FilePath = value;
        }
    }
}