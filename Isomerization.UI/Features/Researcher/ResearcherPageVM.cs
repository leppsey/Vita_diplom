using System.Collections.ObjectModel;
using Isomerization.Domain.Data;
using Isomerization.Domain.Models;

namespace Isomerization.UI.Features;

public class ResearcherPageVM: ViewModelBase
{
    private readonly IsomerizationContext _context;

    public ResearcherPageVM(IsomerizationContext context)
    {
        _context = context;
        
        RawMaterials = new ObservableCollection<RawMaterial>(_context.RawMaterials);
        SelectedRawMaterial = RawMaterials.FirstOrDefault();
        Catalysts = new ObservableCollection<Catalyst>(_context.Catalysts);
        SelectedCatalyst = Catalysts.FirstOrDefault();
    }
    public ObservableCollection<Installation> Installations { get; set; }
    public Installation SelectedInstallation { get; set; }

    public ObservableCollection<RawMaterial> RawMaterials { get; set; }
    public RawMaterial SelectedRawMaterial { get; set; }

    public ObservableCollection<Catalyst> Catalysts { get; set; }
    public Catalyst SelectedCatalyst { get; set; }

    /// <summary>
    /// Концетрация С0
    /// </summary>
    public double C0 { get; set; } = 850;
    /// <summary>
    /// Начальная температура
    /// </summary>
    public double T0 { get; set; } = 600;
    /// <summary>
    /// Расход сырья
    /// </summary>
    public double G { get; set; } = 0.01;
    /// <summary>
    /// Погрешность в процентах
    /// </summary>
    public double EMax { get; set; } = 5;
    /// <summary>
    /// Максимальное число делений сетки пополам 
    /// </summary>
    public double QMax { get; set; } = 4;
    /// <summary>
    /// Время преывания
    /// </summary>
    public double Tau { get; set; }

    /// <summary>
    /// Шаг
    /// </summary>
    public double H { get; set; } = 0.1;

    /// <summary>
    /// Время эксплуатации катализатора
    /// </summary>
    public double CatalystT { get; set; } = 3600;
    
    // стехиометрические коэффициенты
    public double C1 { get; set; } = 1;
    public double C2 { get; set; } = 1;
    public double C3 { get; set; } = 1;
    public double C4 { get; set; } = 1;
}