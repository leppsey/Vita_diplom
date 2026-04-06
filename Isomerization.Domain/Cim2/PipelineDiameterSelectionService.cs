using System;
using System.Collections.Generic;
using System.Linq;

namespace Isomerization.Domain.Cim2;

public class PipelineDiameterSelectionService
{
    private static readonly int[] StandardDn = { 25, 32, 40, 50, 65, 80, 100, 125, 150, 200, 250, 300 };

    public (int CalculatedDn, int StandardDn) Select(double massFlowRate, double density, double allowedVelocity)
    {
        density *= 1000; // перевод плотности из г/см³ в кг/м³
        if (massFlowRate <= 0) throw new ArgumentException("Расход должен быть больше 0", nameof(massFlowRate));
        if (density <= 0) throw new ArgumentException("Плотность должна быть больше 0", nameof(density));
        if (allowedVelocity <= 0) throw new ArgumentException("Допустимая скорость должна быть больше 0", nameof(allowedVelocity));

        var volumetricFlow = massFlowRate / density;
        var diameterM = Math.Sqrt((4.0 * volumetricFlow) / (Math.PI * allowedVelocity));
        var diameterMm = (int)Math.Ceiling(diameterM * 1000.0);

        var nearestStandardDn = StandardDn.FirstOrDefault(x => x >= diameterMm);
        if (nearestStandardDn == 0)
        {
            nearestStandardDn = StandardDn.Last();
        }

        return (diameterMm, nearestStandardDn);
    }

    public int IncreaseToNextStandardDn(int currentDn)
    {
        return StandardDn.FirstOrDefault(x => x > currentDn) is var next && next > 0 ? next : currentDn;
    }

    public IReadOnlyList<int> GetStandardDn() => StandardDn;
}
