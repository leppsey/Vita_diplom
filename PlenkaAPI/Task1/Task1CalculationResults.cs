using System.Collections.Generic;
using System.Diagnostics;

namespace PlenkaAPI;

public struct Task1CalculationResults
{
    public List<CordC> CordCs { get; init; }

    /// <summary>
    ///     Таймер для времени расчета
    /// </summary>
    public Stopwatch MathTimer { get; init; }

    public double OKT { get; init; }
}