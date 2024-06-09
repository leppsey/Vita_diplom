using System;
using Isomerization.Shared;

namespace Isomerization.Domain.Models;

public class Installation: ViewModelBase
{
    public int InstallationId { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public double Pressure { get; set; }
    public double Performance { get; set; }
    public double EnergyConsumption { get; set; }
    public double Temperature { get; set; }
    public double Height { get; set; }
    public double Width { get; set; }
    public double Length { get; set; }
    public double Diameter { get; set; }
    public double Volume { get; set; }
    public double RawMaterialConsumption { get; set; }
    public DateTime DateOfCommissioning { get; set; }
    public DateTime DateOfPlannedWorks { get; set; }
    public string Status { get; set; }

    public Model Model { get; set; }
}