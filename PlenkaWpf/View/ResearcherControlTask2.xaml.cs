using System.Windows.Controls;
using PlenkaWpf.VM;

namespace PlenkaWpf.View;

public partial class ResearcherControlTask2 : UserControl
{
    public ResearcherControlTask2()
    {
        InitializeComponent();
        var vm = new ResearcherControlTask2VM();
        DataContext = vm;
    }
}