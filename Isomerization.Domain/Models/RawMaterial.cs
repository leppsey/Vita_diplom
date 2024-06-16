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
    public List<Concentration> Concentrations { get; set; }
}

public class Concentration
{
    public int ConcentrationId { get; set; }
    public int RawMaterialId { get; set; }
    public double Value { get; set; }
    public int Order { get; set; }
}
//
// public class Material
// {
//     public int MaterialId { get; set; }
//     public double OctaneRating { get; set; }
// }
