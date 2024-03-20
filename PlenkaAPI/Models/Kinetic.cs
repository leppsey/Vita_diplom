namespace PlenkaAPI.Models;


public class Kinetic
{
    public int KineticId { get; set; }
    public RawMaterial RawMaterial { get; set; }
    public int RawMaterialId { get; set; }
    public string Reaction { get; set; }
    public double PreExponentialFactor { get; set; }
    public double EnergyActivation { get; set; }
    public string Kineticscol { get; set; }
}