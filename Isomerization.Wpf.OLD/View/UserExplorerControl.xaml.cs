using System.Windows.Controls;
using Isomerization.Wpf.OLD.VM;


namespace Isomerization.Wpf.OLD.View
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
