using System.Collections.Generic;

namespace Isomerization.Domain.Validation;

/// <summary>Агрегированный результат проверок (ЦИМ-1 и/или ЦИМ-2).</summary>
public class CalculationValidationResult
{
    public ValidationStatus ProductivityStatus { get; set; }
    public ValidationStatus ProcessEnergyStatus { get; set; }
    public ValidationStatus PressureStatus { get; set; }
    public ValidationStatus WallThicknessStatus { get; set; }
    public ValidationStatus PipelineEnergyStatus { get; set; }
    public ValidationStatus NormativeStatus { get; set; }

    /// <summary>ЦИМ-1: октановое число.</summary>
    public ValidationStatus OctaneStatus { get; set; }

    /// <summary>ЦИМ-1: концентрация изопентана / степень изомеризации.</summary>
    public ValidationStatus IsopentaneStatus { get; set; }

    public List<string> Recommendations { get; set; } = new();

    public bool AllCoreValid =>
        ProductivityStatus != ValidationStatus.Invalid
        && ProcessEnergyStatus != ValidationStatus.Invalid
        && PressureStatus != ValidationStatus.Invalid
        && WallThicknessStatus != ValidationStatus.Invalid
        && PipelineEnergyStatus != ValidationStatus.Invalid
        && NormativeStatus != ValidationStatus.Invalid
        && OctaneStatus != ValidationStatus.Invalid
        && IsopentaneStatus != ValidationStatus.Invalid;
}
