using System.Windows.Controls;

using PlenkaWpf.VM;


namespace PlenkaWpf.View
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
