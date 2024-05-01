using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Isomerization.UI.Features;

public partial class ResearcherPage : INavigableView<ResearcherPageVM>
{
    public ResearcherPage()
    {
        InitializeComponent();
    }

    public ResearcherPageVM ViewModel { get; }
}