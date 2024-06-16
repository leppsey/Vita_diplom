using System.Collections.ObjectModel;
using System.Windows;
using Isomerization.Domain.Data;
using Isomerization.Domain.Models;
using Isomerization.Shared;

namespace Isomerization.UI.Features.Researcher;

public class SelectDIMIsomerizationWindowVM: ViewModelBase
{
    private RelayCommand _applyCommand;
    public Window Window { get; set; }
    public ObservableCollection<DIMIsomerization> DimIsomerizations { get; set; }
    public DIMIsomerization SelectedDimIsomerization { get; set; }
    public DIMIsomerization SelectedDimIsomerizationFinal { get; set; }

    public SelectDIMIsomerizationWindowVM(IsomerizationContext context)
    {
        DimIsomerizations = new ObservableCollection<DIMIsomerization>(context.DimIsomerizations.ToList());
        SelectedDimIsomerization = DimIsomerizations.FirstOrDefault();
    }
    public RelayCommand ApplyCommand => _applyCommand ??= new RelayCommand(_ =>
    {
        SelectedDimIsomerizationFinal = SelectedDimIsomerization;
        Window.Close();
    });
    
    private RelayCommand _cancelCommand;
    public RelayCommand CancelCommand => _cancelCommand ??= new RelayCommand(_ =>
    {
        Window.Close();
    });
    
}