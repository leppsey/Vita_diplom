using Isomerization.Domain.Cim2;
using Isomerization.Domain.Models;

namespace Isomerization.UI.Services;

public interface ICim2SessionService
{
    Cim1Result? LastCim1Result { get; set; }
    Cim2Result? LastCim2Result { get; set; }
    Model? ReactorModel { get; set; }
}

public class Cim2SessionService : ICim2SessionService
{
    public Cim1Result? LastCim1Result { get; set; }
    public Cim2Result? LastCim2Result { get; set; }
    public Model? ReactorModel { get; set; }
}
