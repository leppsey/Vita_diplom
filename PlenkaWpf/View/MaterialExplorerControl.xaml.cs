using System.Windows;
using System.Windows.Controls;

using PlenkaAPI.Models;

using PlenkaWpf.VM;


namespace PlenkaWpf.View
{
    /// <summary>
    ///     Логика взаимодействия для MaterialExplorerControl.xaml
    /// </summary>
    public partial class MaterialExplorerControl : UserControl
    {
        public MaterialExplorerControl()
        {
            InitializeComponent();
            DataContext = new MaterialExplorerControlVm();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var win = new MaterialEditWindow((DataContext as MaterialExplorerControlVm).SelectedMemObject);
            win.ShowDialog();
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new MaterialEditWindow(new MembraneObject());
            win.ShowDialog();
        }
    }
}
