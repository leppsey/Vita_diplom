using System.IO;
using System.Windows;
using System.Windows.Controls;
using Isomerization.UI.Misc;
using Microsoft.Win32;

namespace Isomerization.UI.Features.Admin.Installation;

public partial class InstallationEditControl :  IDialogEditControl<Domain.Models.Installation>
{
    public InstallationEditControl()
    {
        ViewModel = App.GetService<InstallationEditControlVM>();
        DataContext = ViewModel;
        InitializeComponent();
    }

    public IDialogEditViewModel<Domain.Models.Installation> ViewModel { get; set; }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog()
        {
            Filter = "IFC2x3 files (*.ifc)|*.ifc|obj files (.obj)|*.obj",
            Multiselect = false,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        };
        var res = dialog.ShowDialog();
        if (res is not null && res.Value)
        {
            ViewModel.Data.Model.ObjPath = dialog.FileName;
        }
    }
}