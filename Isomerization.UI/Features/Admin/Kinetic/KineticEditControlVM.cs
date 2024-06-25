using System.Collections.ObjectModel;
using Isomerization.Domain.Data;
using Isomerization.Shared;
using Isomerization.UI.Misc;
using Microsoft.EntityFrameworkCore;

namespace Isomerization.UI.Features.Admin.Kinetic;

public class KineticEditControlVM: ViewModelBase, IDialogEditViewModel<Domain.Models.Kinetic>
{
    private readonly IsomerizationContext _context;

    public KineticEditControlVM(IsomerizationContext context)
    {
        _context = context;
        RawMaterials = new ObservableCollection<Domain.Models.RawMaterial>(_context.RawMaterials.Include(x=>x.Concentrations));
    }
    
    public ObservableCollection<Domain.Models.RawMaterial> RawMaterials { get; set; }

    public Domain.Models.Kinetic Data
    {
        get => _data;
        set
        {
            _data = value;
            _data.RawMaterial = RawMaterials.FirstOrDefault(x => x.RawMaterialId == _data.RawMaterial?.RawMaterialId);
        }
    }

    public Domain.Models.Kinetic Result { get; private set; }
    public Action FinishInteraction { get; set; }
    
    private RelayCommand _applyCommand;

    public RelayCommand ApplyCommand
    {
        get
        {
            return _applyCommand ??= new RelayCommand(o =>
            {
                // Data.RawMaterialId = Data.RawMaterial.RawMaterialId;
                Result = Data;
                FinishInteraction();
            }, _=> !Data?.HasErrors ?? false);
        }
    }
    
    private RelayCommand _cancelCommand;
    private Domain.Models.Kinetic _data;

    public RelayCommand CancelCommand
    {
        get
        {
            return _cancelCommand ??= new RelayCommand(o =>
            {
                Result = null;
                FinishInteraction();
            });
        }
    }
}