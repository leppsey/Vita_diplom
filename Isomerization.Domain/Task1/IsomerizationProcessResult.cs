using Isomerization.Domain.Models;
using Isomerization.Domain.Validation;

namespace Isomerization.Domain.Task1;

/// <summary>Итоговые показатели и проверки процесса изомеризации для одной установки.</summary>
public class IsomerizationProcessResult
{
    public Installation Installation { get; init; } = null!;
    public MathClass Math { get; init; } = null!;

    /// <summary>Производительность (расход сырья), кг/с — соответствует EF в ЦИМ-2.</summary>
    public double Productivity { get; set; }

    /// <summary>Энергопотребление процесса (по данным реакторного блока), условные единицы как в БД установки.</summary>
    public double ProcessEnergyConsumption { get; set; }

    public double OctaneNumber { get; set; }
    public double IsopentaneConcentration { get; set; }
    public double IsomerizationDegree { get; set; }

    public double ResidenceTimeSeconds { get; set; }

    public CalculationValidationResult Validation { get; set; } = new();
}
