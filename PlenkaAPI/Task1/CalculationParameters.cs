namespace PlenkaAPI;

public struct CalculationParameters
{
    public string MaterialName { get; init; }
    public double Step { get; init; } // Нада
    public double L { get; init; } // Нада

    public double G { get; init; } // Нада
    public double D { get; init; } // Нада
    public double P { get; init; } // Нада

    public double HeatCap { get; init; } // Нада
    public double T0 { get; init; } // Нада
}