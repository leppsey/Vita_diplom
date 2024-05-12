using System.Windows.Controls;
using Isomerization.UI.Misc;

namespace Isomerization.UI.Features.Admin.Catalyst;

public partial class CatalystEditControl : IDialogEditControl<Domain.Models.Catalyst>
{
    public CatalystEditControl()
    {
        ViewModel = App.GetService<CatalystEditControlVM>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public IDialogEditViewModel<Domain.Models.Catalyst> ViewModel { get; set; }
}