namespace Isomerization.Domain.Models;

public class PipelineElbow
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DN { get; set; }
    public double Angle { get; set; }
    public double Zeta { get; set; }
    public string PressureClass { get; set; } = string.Empty;
    public double TemperatureMin { get; set; }
    public double TemperatureMax { get; set; }
    public string Standard { get; set; } = string.Empty;
    public string ModelPath { get; set; } = string.Empty;
}
