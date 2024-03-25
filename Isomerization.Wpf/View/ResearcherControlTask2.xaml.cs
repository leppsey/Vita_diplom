using System.Windows.Controls;
using Isomerization.Wpf.VM;

namespace Isomerization.Wpf.View;

public partial class ResearcherControlTask2 : UserControl
{
    public ResearcherControlTask2()
    {
        InitializeComponent();
        var vm = new ResearcherControlTask2VM();
        DataContext = vm;
    }
}