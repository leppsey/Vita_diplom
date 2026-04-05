using System;
using System.Collections.Generic;
using Isomerization.Domain.Validation;

namespace Isomerization.Domain.Task3;

/// <summary>Сервис расчёта и пошаговой проверки трубопроводной системы (ЦИМ-2).</summary>
public static class PipeMathService
{
    public static PipeCalculationResult Calculate(PipeCalculationParameters p)
    {
        ValidateParameters(p);

        var Q = p.EF / p.rho;
        var v = 4 * Q / (Math.PI * p.D * p.D);
        var dP_fric = p.lambda * (p.L / p.D) * (p.rho * v * v / 2);
        var dP_local = p.sumZeta * (p.rho * v * v / 2);
        var dP_total = dP_fric + dP_local;
        var eSpipe = Q * dP_total;
        var N = p.eta > 1e-12 ? eSpipe / p.eta : double.PositiveInfinity;

        var deltaCalc = p.AllowableStress > 1e-12 && p.WorkingPressure > 0
            ? p.WorkingPressure * p.D / (2.0 * p.AllowableStress)
            : 0.0;

        var isEFok = p.EF >= p.EFmin;
        var isESok = p.ESmax <= 0 || eSpipe <= p.ESmax;
        var isDPtotalOk = p.DeltaPmax <= 0 || dP_total <= p.DeltaPmax;
        var isPumpPowerOk = p.Nmax <= 0 || N <= p.Nmax;
        var isAllowablePressureOk = p.AllowablePressure <= 0 || p.WorkingPressure <= p.AllowablePressure;
        var isWallThicknessOk = p.AllowableStress <= 0 || p.ActualWallThickness <= 0 || deltaCalc <= 0 ||
                                p.ActualWallThickness >= deltaCalc;
        var isFluidTemperatureOk = p.MaxFluidTemperature <= 0 || p.FluidTemperature <= p.MaxFluidTemperature;
        var isNormativeVelocityOk = p.MaxVelocity <= 0 || v <= p.MaxVelocity;

        var validation = BuildPipelineValidation(
            p, v, dP_fric, dP_local, dP_total, eSpipe, N, deltaCalc,
            isEFok, isDPtotalOk, isPumpPowerOk, isESok, isAllowablePressureOk, isWallThicknessOk,
            isFluidTemperatureOk, isNormativeVelocityOk);

        return new PipeCalculationResult
        {
            v = v,
            dP_fric = dP_fric,
            dP_local = dP_local,
            dP_total = dP_total,
            Q = Q,
            PumpPower = N,
            PipelineEnergyConsumption = eSpipe,
            CalculatedWallThickness = deltaCalc,
            ES = N,
            deltaEF = p.EF - p.EFmin,
            deltaES = p.ESmax > 0 ? p.ESmax - eSpipe : 0,
            isEFok = isEFok,
            isESok = isESok,
            isDPtotalOk = isDPtotalOk,
            isPumpPowerOk = isPumpPowerOk,
            isAllowablePressureOk = isAllowablePressureOk,
            isWallThicknessOk = isWallThicknessOk,
            isFluidTemperatureOk = isFluidTemperatureOk,
            isNormativeVelocityOk = isNormativeVelocityOk,
            Validation = validation
        };
    }

    public static List<PipeVariantRow> SweepByDiameter(
        PipeCalculationParameters pBase,
        double Dmin,
        double Dmax,
        int steps)
    {
        if (Dmin <= 0)
            throw new ArgumentException("Dmin должен быть больше 0", nameof(Dmin));
        if (Dmax <= 0)
            throw new ArgumentException("Dmax должен быть больше 0", nameof(Dmax));
        if (Dmin >= Dmax)
            throw new ArgumentException("Dmin должен быть меньше Dmax", nameof(Dmin));
        if (steps < 2)
            throw new ArgumentException("steps должен быть >= 2", nameof(steps));
        if (pBase.rho <= 0)
            throw new ArgumentException("rho должен быть больше 0", nameof(pBase));
        if (pBase.L < 0)
            throw new ArgumentException("L должен быть >= 0", nameof(pBase));
        if (pBase.eta <= 0 || pBase.eta > 1)
            throw new ArgumentException("eta должен быть в интервале (0, 1]", nameof(pBase));

        var results = new List<PipeVariantRow>();
        var stepSize = (Dmax - Dmin) / (steps - 1);

        for (var i = 0; i < steps; i++)
        {
            var D = Dmin + i * stepSize;
            var p = pBase;
            p.D = D;
            var result = Calculate(p);
            results.Add(new PipeVariantRow
            {
                D = D,
                v = result.v,
                dP_total = result.dP_total,
                PumpPower = result.PumpPower,
                PipelineEnergyConsumption = result.PipelineEnergyConsumption,
                isDPtotalOk = result.isDPtotalOk,
                isPumpPowerOk = result.isPumpPowerOk,
                isESok = result.isESok,
                isAllKeyOk = result.isEFok && result.isDPtotalOk && result.isPumpPowerOk && result.isESok &&
                             result.isAllowablePressureOk && result.isWallThicknessOk &&
                             result.isFluidTemperatureOk && result.isNormativeVelocityOk
            });
        }

        return results;
    }

    private static CalculationValidationResult BuildPipelineValidation(
        PipeCalculationParameters p,
        double v,
        double dP_fric,
        double dP_local,
        double dP_total,
        double eSpipe,
        double N,
        double deltaCalc,
        bool isEFok,
        bool isDPtotalOk,
        bool isPumpPowerOk,
        bool isESok,
        bool isAllowablePressureOk,
        bool isWallThicknessOk,
        bool isFluidTemperatureOk,
        bool isNormativeVelocityOk)
    {
        var r = new CalculationValidationResult
        {
            ProductivityStatus = isEFok ? ValidationStatus.Valid : ValidationStatus.Invalid,
            ProcessEnergyStatus = isPumpPowerOk ? ValidationStatus.Valid : ValidationStatus.Invalid,
            PipelineEnergyStatus = isESok ? ValidationStatus.Valid : ValidationStatus.Invalid,
            PressureStatus = isDPtotalOk && isAllowablePressureOk ? ValidationStatus.Valid : ValidationStatus.Invalid,
            WallThicknessStatus = isWallThicknessOk ? ValidationStatus.Valid : ValidationStatus.Invalid,
            NormativeStatus = isFluidTemperatureOk && isNormativeVelocityOk ? ValidationStatus.Valid : ValidationStatus.Invalid,
            OctaneStatus = ValidationStatus.Valid,
            IsopentaneStatus = ValidationStatus.Valid
        };

        // Потери по длине / местные — предупреждение при доле местных > 50% от суммарных
        if (dP_total > 1e-9 && dP_local > dP_fric)
        {
            r.Recommendations.Add(
                "Доля местных потерь давления сопоставима или больше потерь по длине. Рекомендации: уменьшить количество местных сопротивлений или коэффициенты ζᵢ; при необходимости увеличить внутренний диаметр.");
        }

        if (!isEFok)
        {
            r.Recommendations.Add(
                $"Производительность EF ({p.EF:F3} кг/с) ниже EFmin ({p.EFmin:F3} кг/с). Увеличьте расход или скорректируйте исходные данные ЦИМ-1.");
        }

        if (!isDPtotalOk)
        {
            r.Recommendations.Add(
                $"Суммарные потери давления ΔPΣ ({dP_total:F0} Па) превышают ΔPmax ({p.DeltaPmax:F0} Па). Рекомендации: увеличить внутренний диаметр трубопровода; сократить длину участка; уменьшить количество местных сопротивлений; уменьшить коэффициенты ζᵢ; повторить расчёт.");
        }

        if (!isPumpPowerOk)
        {
            r.Recommendations.Add(
                $"Мощность насосного оборудования N ({N:F0} Вт) превышает Nmax ({p.Nmax:F0} Вт). Рекомендации: выбрать насос большей мощности; увеличить КПД насоса η; увеличить внутренний диаметр трубопровода; повторить расчёт.");
        }

        if (!isESok)
        {
            r.Recommendations.Add(
                $"Энергопотребление трубопровода ESpipe ({eSpipe:F0} Вт) превышает ESmax ({p.ESmax:F0} Вт). Рекомендации: снизить потери давления; повысить КПД насоса; оптимизировать параметры трубопровода; повторить расчёт.");
        }

        if (!isAllowablePressureOk)
        {
            r.Recommendations.Add(
                $"Рабочее давление ({p.WorkingPressure:F0} Па) превышает допустимое ({p.AllowablePressure:F0} Па). Снизьте давление или подберите материал/класс прочности.");
        }

        if (!isWallThicknessOk)
        {
            r.Recommendations.Add(
                $"Толщина стенки δ ({p.ActualWallThickness * 1000:F2} мм) меньше расчётной δрасч ({deltaCalc * 1000:F2} мм). Рекомендации: увеличить толщину стенки; выбрать другой материал; выбрать трубу другого класса прочности.");
        }

        if (!isFluidTemperatureOk)
        {
            r.Recommendations.Add(
                $"Температура среды ({p.FluidTemperature:F1} °C) выше допустимой ({p.MaxFluidTemperature:F1} °C). Снизьте температуру или скорректируйте ТЗ.");
        }

        if (!isNormativeVelocityOk)
        {
            r.Recommendations.Add(
                $"Скорость потока v ({v:F2} м/с) превышает нормативный предел ({p.MaxVelocity:F2} м/с). Увеличьте диаметр или уменьшите расход.");
        }

        if (r.AllCoreValid)
        {
            r.Recommendations.Add(
                "Заключение: расчётные потери давления и энергопотребление трубопроводной системы укладываются в заданные ограничения ТЗ при текущих параметрах.");
        }

        return r;
    }

    private static void ValidateParameters(PipeCalculationParameters p)
    {
        if (p.D <= 0)
            throw new ArgumentException("D должен быть больше 0", nameof(p));
        if (p.L < 0)
            throw new ArgumentException("L должен быть >= 0", nameof(p));
        if (p.rho <= 0)
            throw new ArgumentException("rho должен быть больше 0", nameof(p));
        if (p.eta <= 0 || p.eta > 1)
            throw new ArgumentException("eta должен быть в интервале (0, 1]", nameof(p));
    }
}
