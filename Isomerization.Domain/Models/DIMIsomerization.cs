namespace Isomerization.Domain.Models;

/// <summary>
/// Цифровая информационная модель изомеризации
/// </summary>
public class DIMIsomerization
{
    public string Name { get; set; }
    public int DIMIsomerizationId { get; set; }
    public RawMaterial RawMaterial { get; set; }
    public int RawMaterialId { get; set; }
    public User User { get; set; }
    public int UserId { get; set; }
    public Catalyst Catalyst { get; set; }
    public int CatalystId { get; set; }
    
    public double Temp { get; set; }
    public double Consumption { get; set; }
    public int Step { get; set; }
    public double OctaneNumberMin { get; set; }
    public double? PerformanceMin { get; set; }
    public double? PerformanceMax { get; set; }
    public double? EnergyConsumptionMin { get; set; }
    public double? EnergyConsumptionMax { get; set; }
}