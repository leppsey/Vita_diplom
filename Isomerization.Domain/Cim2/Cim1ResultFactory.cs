using Isomerization.Domain.Task1;

namespace Isomerization.Domain.Cim2;

public static class Cim1ResultFactory
{
    public static Cim1Result Create(IsomerizationProcessResult processResult, double flowRate, double density, double temperature)
    {
        return new Cim1Result
        {
            ReactorId = processResult.Installation.InstallationId,
            ReactorName = processResult.Installation.Name,
            FlowRate = flowRate,
            Temperature = temperature,
            PressureKPa = processResult.Installation.Pressure,
            DensityGsm3 = density,
            Productivity = processResult.Productivity,
            ProductQuality = processResult.OctaneNumber,
            EnergyConsumption = processResult.ProcessEnergyConsumption
        };
    }
}
