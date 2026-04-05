using System;

namespace Isomerization.Domain.Models;

/// <summary>Трубопровод.</summary>
public class Pipeline
{
    public int PipelineId { get; set; }
    public DateTime DateOfCommissioning { get; set; }
    public double Length { get; set; }
    public double Width { get; set; }
    public double Diameter { get; set; }
    public string Material { get; set; } = string.Empty;

    public double PressureLossLinear { get; set; }
    public double PressureLossLocal { get; set; }
    public double PressureLossTotal { get; set; }
    public double PumpPower { get; set; }
    public double PipelineEnergyConsumption { get; set; }
    public double AllowablePressure { get; set; }
    public double CalculatedWallThickness { get; set; }
    public double ActualWallThickness { get; set; }
    public bool IsPressureValid { get; set; }
    public bool IsWallThicknessValid { get; set; }
    public bool IsEnergyValid { get; set; }
    public bool IsNormativeValid { get; set; }
}
