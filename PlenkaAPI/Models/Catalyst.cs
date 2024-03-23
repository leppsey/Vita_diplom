using System;

namespace PlenkaAPI.Models;

public class Catalyst
{
    public int CatalystId { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public double Activity { get; set; }
    public double LoadingRate { get; set; }
    public double TemperatureReaction { get; set; }
    public int OperatingTime { get; set; }
    public int ServiceLife { get; set; }
    public DateTime DateOfCommissioning { get; set; }
    public DateTime DateOfDecommissioning { get; set; }
}