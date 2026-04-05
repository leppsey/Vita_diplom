namespace Isomerization.Domain.Models;

/// <summary>Цифровая информационная модель изомеризации.</summary>
public class DIMIsomerization
{
    public string Name { get; set; } = string.Empty;
    public int DIMIsomerizationId { get; set; }
    public RawMaterial RawMaterial { get; set; } = null!;
    public int RawMaterialId { get; set; }
    public User User { get; set; } = null!;
    public int UserId { get; set; }
    public Catalyst Catalyst { get; set; } = null!;
    public int CatalystId { get; set; }

    public double Temp { get; set; }
    public double Consumption { get; set; }
    public int Step { get; set; }
    public double OctaneNumberMin { get; set; }
    public double? PerformanceMin { get; set; }
    public double? PerformanceMax { get; set; }
    public double? EnergyConsumptionMin { get; set; }
    public double? EnergyConsumptionMax { get; set; }

    public double Productivity { get; set; }
    public double ProcessEnergyConsumption { get; set; }
    public double OctaneNumber { get; set; }
    public double IsopentaneConcentration { get; set; }
    public double IsomerizationDegree { get; set; }
    public bool IsProductivityValid { get; set; }
    public bool IsEnergyValid { get; set; }
    public bool IsOctaneValid { get; set; }
    public bool IsIsopentaneValid { get; set; }
}
