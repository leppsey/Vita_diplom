using System.Windows.Controls;
using Isomerization.UI.Misc;

namespace Isomerization.UI.Features.Admin.RawMaterial;

public partial class RawMaterialEditControl : IDialogEditControl<Domain.Models.RawMaterial>
{
    public RawMaterialEditControl()
    {
        ViewModel = App.GetService<RawMaterialEditControlVM>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public IDialogEditViewModel<Domain.Models.RawMaterial> ViewModel { get; set; }
}