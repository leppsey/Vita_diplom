using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using PlenkaWpf.VM;


namespace PlenkaWpf.View
{
    /// <summary>
    /// Interaction logic for UnitCreateWindow.xaml
    /// </summary>
    public partial class UnitCreateWindow 
    {
        public UnitCreateWindow()
        {
            InitializeComponent();
            var vm = new UnitCreateWindowVM();
            DataContext = vm;
            vm.ClosingRequest += (sender, e) => Close();
        }
    }
}
