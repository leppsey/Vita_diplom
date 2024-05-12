using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using PasswordBox = Wpf.Ui.Controls.PasswordBox;

namespace Isomerization.UI;

public partial class LoginPage : INavigableView<LoginPageVM>
{
    public LoginPage()
    {
        ViewModel = App.GetService<LoginPageVM>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Password = ((PasswordBox) sender).Password;
    }

    public LoginPageVM ViewModel { get; }
}