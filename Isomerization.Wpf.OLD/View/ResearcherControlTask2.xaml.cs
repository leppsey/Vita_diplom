using System.Windows.Controls;
using Isomerization.Wpf.OLD.VM;

namespace Isomerization.Wpf.OLD.View;

public partial class ResearcherControlTask2 : UserControl
{
    public ResearcherControlTask2()
    {
        InitializeComponent();
        var vm = new ResearcherControlTask2VM();
        DataContext = vm;
    }
}