using System.Windows;
using System.Windows.Controls;
using Isomerization.Domain.Models;
using Wpf.Ui.Controls;

namespace Isomerization.UI.Features;

public partial class ResearcherPage : INavigableView<ResearcherPageVM>
{
    public ResearcherPage()
    {
        InitializeComponent();
    }

    public ResearcherPageVM ViewModel { get; }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var box = (ComboBox)sender;
        var installation = (Installation)box.SelectedItem;
        RenderControl.ModelPath = installation.ModelPath;
    }
}