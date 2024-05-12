using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Isomerization.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : INavigationWindow
{
    public MainWindow()
    {
        ViewModel = App.GetService<MainWindowVM>();
        DataContext = ViewModel;
        InitializeComponent();
        SetPageService(App.GetService<IPageService>());
        App.GetService<IContentDialogService>().SetDialogHost(RootContentDialog);
        App.GetService<INavigationService>().SetNavigationControl(RootNavigation);
        App.GetService<ISnackbarService>().SetSnackbarPresenter(RootSnackBar);
    }
    public MainWindowVM ViewModel { get; set; }

    public INavigationView GetNavigation()
    {
        return RootNavigation;
    }

    public bool Navigate(Type pageType)
    {
        return RootNavigation.Navigate(pageType);
    }

    public void SetServiceProvider(IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }

    public void SetPageService(IPageService pageService)
    {
        RootNavigation.SetPageService(pageService);
    }

    public void ShowWindow()
    {
        Show();
    }

    public void CloseWindow()
    {
        Close();
    }
}