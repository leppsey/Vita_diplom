using System.Collections.Generic;
using DAL;

namespace Isomerization.Domain.Models;

public class RawMaterial: ValidatableViewModel<RawMaterialValidator>
{
    public int RawMaterialId { get; set; }
    public string Name { get; set; }
    public double Compound { get; set; }
    public double Density { get; set; }
    public double HeatCapacity { get; set; }
    public double OctaneRating { get; set; } 
    public List<Concetration> Concetrations { get; set; }
}

public class Concetration
{
    public int ConcetrationId { get; set; }
    public int RawMaterialId { get; set; }
    public double Value { get; set; }
    public int Order { get; set; }
}