using Isomerization.Domain.Validation;

namespace Isomerization.Domain.Task3;

public record struct PipeCalculationResult
{
    public double v { get; set; }
    public double dP_fric { get; set; }
    public double dP_local { get; set; }
    public double dP_total { get; set; }

    /// <summary>Объёмный расход, м³/с.</summary>
    public double Q { get; set; }

    /// <summary>Мощность насоса N = Q·ΔPΣ/η, Вт.</summary>
    public double PumpPower { get; set; }

    /// <summary>Энергопотребление трубопровода ESpipe = Q·ΔPΣ, Вт (без учёта КПД насоса).</summary>
    public double PipelineEnergyConsumption { get; set; }

    /// <summary>Расчётная минимальная толщина стенки (тонкостенная оболочка): p·D/(2[σ]).</summary>
    public double CalculatedWallThickness { get; set; }

    /// <summary>Устаревшее имя: совпадает с <see cref="PumpPower"/> для обратной совместимости графиков.</summary>
    public double ES { get; set; }

    public double deltaEF { get; set; }
    /// <summary>ESmax − ESpipe.</summary>
    public double deltaES { get; set; }

    public bool isEFok { get; set; }
    /// <summary>ESpipe ≤ ESmax.</summary>
    public bool isESok { get; set; }

    public bool isDPtotalOk { get; set; }
    public bool isPumpPowerOk { get; set; }
    public bool isAllowablePressureOk { get; set; }
    public bool isWallThicknessOk { get; set; }
    public bool isFluidTemperatureOk { get; set; }
    public bool isNormativeVelocityOk { get; set; }

    public CalculationValidationResult? Validation { get; set; }
}
