using System.Linq;
using Isomerization.Domain.Task3;

namespace Isomerization.Domain.Cim2;

public class PipelineCalculationService
{
    public PipelineCalculationResult Calculate(PipelineLine line, Cim2Request request)
    {
        var firstPipe = line.Elements.First(x => x.ElementType == PipelineElementType.Pipe);
        var sumLocalZeta = line.Elements.Where(x => x.ElementType != PipelineElementType.Pipe).Sum(x => x.Zeta);
        var totalLength = line.Elements.Where(x => x.ElementType == PipelineElementType.Pipe).Sum(x => x.Length);

        var p = PipeCalculationParameters.CreateDefault();
        p.EF = request.Cim1Result.FlowRate;
        p.EFmin = request.Cim1Result.Productivity;
        p.ESmax = request.MaxEnergyConsumption;
        p.rho = request.Cim1Result.DensityGsm3*1000; // перевод плотности из г/см³ в кг/м³
        p.D = firstPipe.DN / 1000.0;
        p.L = totalLength;
        p.sumZeta = request.SumZetaOverride > 0 ? request.SumZetaOverride : sumLocalZeta;
        p.eta = request.Efficiency;
        p.WorkingPressure = request.Cim1Result.PressureKPa*100000;
        p.AllowablePressure = request.Cim1Result.PressureKPa * 1.15*100000;
        p.AllowableStress = request.AllowableStress;
        p.ActualWallThickness = 0.008;
        p.FluidTemperature = request.Cim1Result.Temperature;
        p.MaxFluidTemperature = 300;
        p.MaxVelocity = request.AllowedVelocity;
        p.DeltaPmax = request.MaxPressureLoss;

        var result = PipeMathService.Calculate(p);
        return new PipelineCalculationResult
        {
            Velocity = result.v,
            PressureLossLinear = result.dP_fric,
            PressureLossLocal = result.dP_local,
            PressureLossTotal = result.dP_total,
            PumpPower = result.PumpPower,
            EnergyConsumption = result.PipelineEnergyConsumption,
            CalculatedWallThickness = result.CalculatedWallThickness
        };
    }
}
