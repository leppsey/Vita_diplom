namespace Isomerization.Domain.Cim2;

public class PipelineElement
{
    public PipelineElementType ElementType { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DN { get; set; }
    public double Length { get; set; }
    public double Zeta { get; set; }
    public string Material { get; set; } = string.Empty;
    public string PressureClass { get; set; } = string.Empty;
    public double TemperatureMin { get; set; }
    public double TemperatureMax { get; set; }
    public string ModelPath { get; set; } = string.Empty;
}
