using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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

    [NotMapped]
    public decimal C1
    {
        get => _sortedConcentrations[0].Value;
        set => _sortedConcentrations[0].Value = value;
    }
    [NotMapped]
    public decimal C2
    {
        get => _sortedConcentrations[1].Value;
        set => _sortedConcentrations[1].Value = value;
    }
    [NotMapped]
    public decimal C3
    {
        get => _sortedConcentrations[2].Value;
        set => _sortedConcentrations[2].Value = value;
    }
    [NotMapped]
    public decimal C4
    {
        get => _sortedConcentrations[3].Value;
        set => _sortedConcentrations[3].Value = value;
    }
    [NotMapped]
    public decimal C5
    {
        get => _sortedConcentrations[4].Value;
        set => _sortedConcentrations[4].Value = value;
    }
    [NotMapped]
    public decimal C6
    {
        get => _sortedConcentrations[5].Value;
        set => _sortedConcentrations[5].Value = value;
    }
    [NotMapped]
    public decimal C7
    {
        get => _sortedConcentrations[6].Value;
        set => _sortedConcentrations[6].Value = value;
    }

    private List<Concentration> _sortedConcentrations => Concentrations?.OrderBy(x => x.Order).ToList() ?? new()
    {
        new (),
        new (),
        new (),
        new (),
        new (),
        new (),
        new (),
    };
    public List<Concentration> Concentrations { get; set; }


    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        var conces = new[]
        {
            nameof(C1),
            nameof(C2),
            nameof(C3),
            nameof(C4),
            nameof(C5),
            nameof(C6),
            nameof(C7),
        };
        if (conces.Contains(propertyName))
        {
            foreach (var conce in conces)
            {
                RaiseErrorChanged(conce);
            }
        }
    }
}

public class Concentration
{
    public int ConcentrationId { get; set; }
    public int RawMaterialId { get; set; }
    public decimal Value { get; set; }
    public int Order { get; set; }
}
//
// public class Material
// {
//     public int MaterialId { get; set; }
//     public double OctaneRating { get; set; }
// }
