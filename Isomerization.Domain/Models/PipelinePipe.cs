namespace Isomerization.Domain.Models;

public class PipelinePipe
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DN { get; set; }
    public string Material { get; set; } = string.Empty;
    public double Roughness { get; set; }
    public string PressureClass { get; set; } = string.Empty;
    public double TemperatureMin { get; set; }
    public double TemperatureMax { get; set; }
    public double WallThickness { get; set; }
    public string Standard { get; set; } = string.Empty;
    public string ModelPath { get; set; } = string.Empty;
}
