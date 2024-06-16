using DAL;

namespace Isomerization.Domain.Models;


public class Kinetic: ValidatableViewModel<KineticValidator>
{
    public int KineticId { get; set; }
    public RawMaterial RawMaterial { get; set; }
    public int RawMaterialId { get; set; }
    public double PreExponentialFactor { get; set; }
    public double EnergyActivation { get; set; }
    public double StehiometricFactor { get; set; }
    public double ReactionRateConstant { get; set; }
}