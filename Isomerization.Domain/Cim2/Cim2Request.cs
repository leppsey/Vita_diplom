namespace Isomerization.Domain.Cim2;

public class Cim2Request
{
    public Cim1Result Cim1Result { get; set; } = new();
    public bool RequiresDirectionChange { get; set; }
    public bool RequiresPump { get; set; }
    public double AllowedVelocity { get; set; } = 2.5;
    public double PipeLength { get; set; } = 20;
    public double SumZetaOverride { get; set; }
    public double Efficiency { get; set; } = 0.7;
    public double AllowableStress { get; set; } = 120_000_000;
    public double MaxPressureLoss { get; set; }
    public double MaxEnergyConsumption { get; set; }
    public string PreferredPressureClass { get; set; } = "PN16";
}
