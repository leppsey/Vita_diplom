using System.Windows.Controls;
using Isomerization.UI.Misc;

namespace Isomerization.UI.Features.Admin.Kinetic;

public partial class KineticEditControl : IDialogEditControl<Domain.Models.Kinetic>
{
    public KineticEditControl()
    {
        ViewModel = App.GetService<KineticEditControlVM>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public IDialogEditViewModel<Domain.Models.Kinetic> ViewModel { get; set; }
}