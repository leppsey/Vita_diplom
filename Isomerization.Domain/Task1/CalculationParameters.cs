namespace Isomerization.Domain.Task1;

public struct CalculationParameters
{
    public string MaterialName { get; init; }
    public double Step { get; init; }
    public double Volume { get; init; }

    public double G { get; init; }
    public double D { get; init; }
    public double P { get; init; }

    public double HeatCap { get; init; }
    public double T0 { get; init; }
    public double[] C0 { get; init; }
    public double Activity { get; init; }

    public int MaterialCount => C0.Length;
}