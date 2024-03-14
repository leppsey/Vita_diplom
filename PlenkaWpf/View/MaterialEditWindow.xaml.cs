using PlenkaAPI.Models;

using PlenkaWpf.VM;


namespace PlenkaWpf.View
{
    /// <summary>
    ///     Логика взаимодействия для MaterialEditWindow.xaml
    /// </summary>
    public partial class MaterialEditWindow
    {
        public MaterialEditWindow(MembraneObject material)
        {
            InitializeComponent();
            var vm = new MaterialEditWindowVm(material);
            DataContext = vm;
            vm.ClosingRequest += (sender, e) => Close();
        }
    }
}
