using Isomerization.Domain.Models;

using Isomerization.Wpf.OLD.VM;


namespace Isomerization.Wpf.OLD.View
{
    /// <summary>
    ///     Логика взаимодействия для UserEditWindow.xaml
    /// </summary>
    public partial class UserEditWindow
    {
        public UserEditWindow(User user)
        {
            InitializeComponent();
            // var vm = new UserEditVM(user);
            // DataContext = vm;
            // vm.ClosingRequest += (sender, e) => Close();
        }
    }
}
