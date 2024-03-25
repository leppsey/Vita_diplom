namespace Isomerization.Domain.Models;

public class RawMaterial
{
    public int RawMaterialId { get; set; }
    public string Name { get; set; }
    public double Consumption { get; set; }
    public double Compound { get; set; }
    public double Density { get; set; }
    public double HeatCapacity { get; set; }
    public double Viscosity { get; set; }
    public double SulfurContent { get; set; }
    public double OctaneRating { get; set; }
}