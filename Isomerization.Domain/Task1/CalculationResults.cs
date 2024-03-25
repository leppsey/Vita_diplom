using System.Collections.Generic;
using System.Diagnostics;

namespace Isomerization.Domain.Task1;

public struct CalculationResults
{
    public List<CordC> CordCs { get; init; }

    /// <summary>
    ///     Таймер для времени расчета
    /// </summary>
    public Stopwatch MathTimer { get; init; }

    public double OKT { get; init; }
}