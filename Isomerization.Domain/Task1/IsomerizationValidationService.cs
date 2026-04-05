using System;
using Isomerization.Domain.Models;
using Isomerization.Domain.Validation;

namespace Isomerization.Domain.Task1;

public static class IsomerizationValidationService
{
    /// <summary>
    /// Строит показатели и выполняет цепочку: производительность (G vs EFmin) → ESproc → октан → изопентан.
    /// </summary>
    public static IsomerizationProcessResult BuildAndValidate(
        Installation installation,
        MathClass math,
        double g,
        double? performanceMin,
        double? energyConsumptionMin,
        double? energyConsumptionMax,
        double octaneNumberMin)
    {
        var cp = math.Cp;
        var res = math.Results;
        var validation = new CalculationValidationResult
        {
            ProductivityStatus = ValidationStatus.Valid,
            ProcessEnergyStatus = ValidationStatus.Valid,
            OctaneStatus = ValidationStatus.Valid,
            IsopentaneStatus = ValidationStatus.Valid,
            PressureStatus = ValidationStatus.Valid,
            WallThicknessStatus = ValidationStatus.Valid,
            PipelineEnergyStatus = ValidationStatus.Valid,
            NormativeStatus = ValidationStatus.Valid
        };

        var esProc = installation.EnergyConsumption;
        var tau = cp.G > 0 ? cp.Volume / cp.G : 0;

        var (isoIn, isoOut) = GetIsopentaneFractions(res, cp);
        var degree = ComputeIsomerizationDegree(isoIn, isoOut);

        // 1. Производительность
        if (performanceMin.HasValue && g < performanceMin.Value)
        {
            validation.ProductivityStatus = ValidationStatus.Invalid;
            validation.Recommendations.Add(
                $"Производительность EF (G = {g:F3} кг/с) ниже EFmin ({performanceMin.Value:F3} кг/с). " +
                "Рекомендации: увеличить расход сырья G; при необходимости скорректировать температуру T, время пребывания τ (через объём установки и G), выбрать другой реакторный блок.");
        }

        // 2. Энергопотребление процесса (по установке)
        if (energyConsumptionMin.HasValue && esProc < energyConsumptionMin.Value)
        {
            validation.ProcessEnergyStatus = ValidationStatus.Invalid;
            validation.Recommendations.Add(
                $"Энергопотребление процесса ESproc ({esProc:F2}) ниже заданного минимума ({energyConsumptionMin.Value:F2}). " +
                "Рекомендации: скорректировать параметры процесса или выбрать другой реакторный блок.");
        }

        if (energyConsumptionMax.HasValue && esProc > energyConsumptionMax.Value)
        {
            validation.ProcessEnergyStatus = ValidationStatus.Invalid;
            validation.Recommendations.Add(
                $"Энергопотребление процесса ESproc ({esProc:F2}) превышает допустимое ({energyConsumptionMax.Value:F2}). " +
                "Рекомендации: уменьшить температуру процесса T; изменить расход сырья G; изменить время пребывания τ; выбрать другой реакторный блок; повторить расчёт.");
        }

        // 3. Октановое число (только если пройдены предыдущие обязательные проверки)
        if (validation.ProductivityStatus != ValidationStatus.Invalid
            && validation.ProcessEnergyStatus != ValidationStatus.Invalid)
        {
            if (res.OKT < octaneNumberMin)
            {
                validation.OctaneStatus = ValidationStatus.Invalid;
                validation.Recommendations.Add(
                    $"Октановое число ({res.OKT:F2}) ниже требуемого ({octaneNumberMin:F2}). " +
                    "Рекомендации: скорректировать T, G, τ или катализатор/реакторный блок; повторить расчёт.");
            }
        }
        else
        {
            validation.OctaneStatus = ValidationStatus.Warning;
            validation.Recommendations.Add("Проверка октанового числа не выполнялась: сначала устраните нарушения по производительности или энергопотреблению процесса.");
        }

        // 4. Изопентан / степень (информативно)
        if (validation.OctaneStatus == ValidationStatus.Invalid)
        {
            validation.IsopentaneStatus = ValidationStatus.Warning;
        }

        return new IsomerizationProcessResult
        {
            Installation = installation,
            Math = math,
            Productivity = g,
            ProcessEnergyConsumption = esProc,
            OctaneNumber = res.OKT,
            IsopentaneConcentration = isoOut * 100,
            IsomerizationDegree = degree,
            ResidenceTimeSeconds = tau,
            Validation = validation
        };
    }

    /// <summary>
    /// Условие «годится для отбора»: производительность и энергия процесса в норме, октан в норме.
    /// </summary>
    public static bool PassesSelection(IsomerizationProcessResult r) =>
        r.Validation.ProductivityStatus != ValidationStatus.Invalid
        && r.Validation.ProcessEnergyStatus != ValidationStatus.Invalid
        && r.Validation.OctaneStatus == ValidationStatus.Valid;

    private static (double inletFrac, double outletFrac) GetIsopentaneFractions(CalculationResults res, CalculationParameters cp)
    {
        if (cp.MaterialCount == 7)
        {
            var i = IsomerizationComponentIndices.IsopentaneIndex7;
            var inlet = i < cp.C0.Length ? (double)cp.C0[i] : 0;
            var outlet = res.CordCs is { Count: > 0 }
                ? GetOutletFraction7(res, i)
                : inlet;
            return (inlet, outlet);
        }

        if (cp.MaterialCount == 4)
        {
            var i = IsomerizationComponentIndices.TargetIsomerIndex4;
            var inlet = i < cp.C0.Length ? (double)cp.C0[i] : 0;
            var outlet = res.CordCs is { Count: > 0 }
                ? GetOutletFraction4(res, i)
                : inlet;
            return (inlet, outlet);
        }

        return (0, 0);
    }

    private static double GetOutletFraction7(CalculationResults res, int index)
    {
        var last = res.CordCs[^1];
        return index switch
        {
            0 => last.C1 / 100,
            1 => last.C2 / 100,
            2 => last.C3 / 100,
            3 => last.C4 / 100,
            4 => last.C5 / 100,
            5 => last.C6 / 100,
            6 => last.C7 / 100,
            _ => 0
        };
    }

    private static double GetOutletFraction4(CalculationResults res, int index)
    {
        var last = res.CordCs[^1];
        return index switch
        {
            0 => last.C1,
            1 => last.C2,
            2 => last.C3,
            3 => last.C4,
            _ => 0
        };
    }

    /// <summary>Степень изомеризации по ключевому компоненту: (x_out - x_in) / max(ε, 1 - x_in).</summary>
    private static double ComputeIsomerizationDegree(double xIn, double xOut)
    {
        xIn = Math.Clamp(xIn, 0, 1);
        xOut = Math.Clamp(xOut, 0, 1);
        var denom = Math.Max(1e-9, 1 - xIn);
        return Math.Clamp((xOut - xIn) / denom, 0, 1);
    }
}
