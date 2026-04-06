using System.Linq;

namespace Isomerization.Domain.Cim2;

public class PipelineValidationService
{
    public PipelineValidationResult Validate(PipelineLine line, Cim2Request request)
    {
        var result = new PipelineValidationResult { IsValid = true };
        var c = line.CalculationResult;

        if (request.AllowedVelocity > 0 && c.Velocity > request.AllowedVelocity)
        {
            result.IsValid = false;
            result.Violations.Add($"Скорость потока {c.Velocity:F2} м/с превышает допустимую {request.AllowedVelocity:F2} м/с.");
        }

        if (request.MaxPressureLoss > 0 && c.PressureLossTotal > request.MaxPressureLoss)
        {
            result.IsValid = false;
            result.Violations.Add($"Суммарные потери {c.PressureLossTotal:F0} Па превышают лимит {request.MaxPressureLoss:F0} Па.");
        }

        if (request.MaxEnergyConsumption > 0 && c.EnergyConsumption > request.MaxEnergyConsumption)
        {
            result.IsValid = false;
            result.Violations.Add($"Энергопотребление {c.EnergyConsumption:F2} Вт превышает лимит {request.MaxEnergyConsumption:F2} Вт.");
        }

        var outOfTemp = line.Elements
            .Where(x => request.Cim1Result.Temperature < x.TemperatureMin || request.Cim1Result.Temperature > x.TemperatureMax)
            .ToList();
        if (outOfTemp.Any())
        {
            result.IsValid = false;
            foreach (var element in outOfTemp)
            {
                result.Violations.Add($"Элемент {element.Name} не подходит по температуре {request.Cim1Result.Temperature:F1} °C.");
            }
        }

        if (!line.Elements.Any(x => x.ElementType == PipelineElementType.Pipe))
        {
            result.IsValid = false;
            result.Violations.Add("Линия не содержит прямого трубного участка.");
        }

        if (result.IsValid)
        {
            result.Warnings.Add("Линия соответствует базовым ограничениям ЦИМ-2.");
        }

        return result;
    }
}
