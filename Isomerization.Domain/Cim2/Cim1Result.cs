namespace Isomerization.Domain.Cim2;

/// <summary>
/// Результат ЦИМ-1, который используется как вход для ЦИМ-2.
/// </summary>
public class Cim1Result
{
    public int ReactorId { get; set; }
    public string ReactorName { get; set; } = string.Empty;
    public double FlowRate { get; set; }
    public double Temperature { get; set; }
    public double PressureKPa { get; set; }
    public double DensityGsm3 { get; set; }
    public double Productivity { get; set; }
    public double ProductQuality { get; set; }
    public double EnergyConsumption { get; set; }
}
