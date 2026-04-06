using System.Collections.Generic;
using System.Linq;

namespace Isomerization.Domain.Cim2;

public class PipelineRecommendationService
{
    public List<string> BuildRecommendations(PipelineLine line, Cim2Request request)
    {
        var recommendations = new List<string>();
        var c = line.CalculationResult;

        if (request.AllowedVelocity > 0 && c.Velocity > request.AllowedVelocity)
        {
            recommendations.Add("Увеличить DN трубопровода для снижения скорости потока.");
        }

        if (c.PressureLossLocal > c.PressureLossLinear)
        {
            recommendations.Add("Уменьшить количество местных сопротивлений.");
        }

        var hasNinetyDegreeElbow = line.Elements.Any(x => x.ElementType == PipelineElementType.Elbow && x.Zeta >= 0.8);
        if (hasNinetyDegreeElbow)
        {
            recommendations.Add("Заменить отвод 90° на два отвода 45° для уменьшения потерь.");
        }

        if (request.MaxEnergyConsumption > 0 && c.EnergyConsumption > request.MaxEnergyConsumption)
        {
            recommendations.Add("Выбрать другой шаблон линии или другой DN для снижения энергопотребления.");
        }

        if (line.Elements.Any(x => x.ElementType == PipelineElementType.Pump) && c.PumpPower > 0)
        {
            recommendations.Add("Проверить класс давления и КПД насоса для оптимизации энергозатрат.");
        }

        if (!recommendations.Any())
        {
            recommendations.Add("Текущая конфигурация линии является допустимой.");
        }

        return recommendations;
    }
}
