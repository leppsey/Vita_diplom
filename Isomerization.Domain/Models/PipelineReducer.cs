namespace Isomerization.Domain.Models;

public class PipelineReducer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DNIn { get; set; }
    public int DNOut { get; set; }
    public double Zeta { get; set; }
    public string PressureClass { get; set; } = string.Empty;
    public double TemperatureMin { get; set; }
    public double TemperatureMax { get; set; }
    public string Standard { get; set; } = string.Empty;
    public string ModelPath { get; set; } = string.Empty;
}
