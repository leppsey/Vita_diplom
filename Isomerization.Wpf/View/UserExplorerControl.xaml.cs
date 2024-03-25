using System.Windows.Controls;
using Isomerization.Wpf.VM;


namespace Isomerization.Wpf.View
{
    /// <summary>
    ///     Логика взаимодействия для UserExplorerControl.xaml
    /// </summary>
    public partial class UserExplorerControl : UserControl
    {
        public UserExplorerControl()
        {
            InitializeComponent();
            DataContext = new UserExplorerControlVM();
        }
    }
}
