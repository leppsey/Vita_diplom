namespace Isomerization.Domain.Cim2;

public class PipelineCalculationResult
{
    public double Velocity { get; set; }
    public double PressureLossLinear { get; set; }
    public double PressureLossLocal { get; set; }
    public double PressureLossTotal { get; set; }
    public double PumpPower { get; set; }
    public double EnergyConsumption { get; set; }
    public double CalculatedWallThickness { get; set; }
}
