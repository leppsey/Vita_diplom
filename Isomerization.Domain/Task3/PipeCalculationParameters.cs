namespace Isomerization.Domain.Task3;

public struct PipeCalculationParameters
{
    /// <summary>Производительность (расход) из ЦИМ-1, кг/с.</summary>
    public double EF { get; set; }

    /// <summary>Минимальная производительность, кг/с.</summary>
    public double EFmin { get; set; }

    /// <summary>Максимальное энергопотребление трубопровода ESpipe = Q·ΔPΣ, Вт.</summary>
    public double ESmax { get; set; }

    public double rho { get; set; }
    public double D { get; set; }
    public double L { get; set; }
    public double sumZeta { get; set; }
    public double lambda { get; set; }
    public double eta { get; set; }

    /// <summary>Максимальные суммарные потери давления ΔPΣ, Па (≤ 0 — проверка отключена).</summary>
    public double DeltaPmax { get; set; }

    /// <summary>Максимальная мощность насоса N, Вт (≤ 0 — проверка отключена).</summary>
    public double Nmax { get; set; }

    /// <summary>Рабочее давление, Па.</summary>
    public double WorkingPressure { get; set; }

    /// <summary>Допустимое давление для материала/ТЗ, Па (≤ 0 — проверка отключена).</summary>
    public double AllowablePressure { get; set; }

    /// <summary>Допускаемое напряжение материала [σ], Па (≤ 0 — расчёт δрасч отключён).</summary>
    public double AllowableStress { get; set; }

    /// <summary>Фактическая толщина стенки, м (≤ 0 — проверка отключена).</summary>
    public double ActualWallThickness { get; set; }

    /// <summary>Температура среды, °C.</summary>
    public double FluidTemperature { get; set; }

    /// <summary>Максимально допустимая температура, °C (≤ 0 — проверка отключена).</summary>
    public double MaxFluidTemperature { get; set; }

    /// <summary>Максимально допустимая скорость потока, м/с (≤ 0 — проверка отключена).</summary>
    public double MaxVelocity { get; set; }

    public static PipeCalculationParameters CreateDefault()
    {
        return new PipeCalculationParameters
        {
            lambda = 0.03,
            eta = 0.7
        };
    }
}
