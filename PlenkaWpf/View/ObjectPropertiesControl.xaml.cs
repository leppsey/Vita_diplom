using System.Windows.Controls;

using PlenkaWpf.VM;


namespace PlenkaWpf.View
{
    /// <summary>
    ///     Interaction logic for ObjectPropertiesControl.xaml
    /// </summary>
    public partial class ObjectPropertiesControl : UserControl
    {
        public ObjectPropertiesControl()
        {
            InitializeComponent();
            var vm = new ObjectPropertiesControlVM();
            DataContext = vm;
        }
    }
}
