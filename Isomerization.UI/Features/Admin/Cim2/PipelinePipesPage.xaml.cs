using Wpf.Ui.Controls;

namespace Isomerization.UI.Features.Admin.Cim2;

public partial class PipelinePipesPage : INavigableView<PipelinePipesPageVM>
{
    public PipelinePipesPage()
    {
        ViewModel = App.GetService<PipelinePipesPageVM>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public PipelinePipesPageVM ViewModel { get; }
}
