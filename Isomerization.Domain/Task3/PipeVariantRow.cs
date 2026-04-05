namespace Isomerization.Domain.Task3;

/// <summary>Вариант расчёта для перебора по диаметрам.</summary>
public struct PipeVariantRow
{
    public double D { get; set; }
    public double v { get; set; }
    public double dP_total { get; set; }
    public double PumpPower { get; set; }
    public double PipelineEnergyConsumption { get; set; }
    public bool isDPtotalOk { get; set; }
    public bool isPumpPowerOk { get; set; }
    public bool isESok { get; set; }
    public bool isAllKeyOk { get; set; }

    /// <summary>Устаревшее: для совместимости с экспортом/графиками.</summary>
    public double ES => PumpPower;
}
