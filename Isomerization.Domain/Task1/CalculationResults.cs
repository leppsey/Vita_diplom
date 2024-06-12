using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Isomerization.Domain.Task1;

public struct CalculationResults
{
    public int MaterialCount { get; set; }
    public List<CordC> CordCs { get; init; }

    /// <summary>
    ///     Таймер для времени расчета
    /// </summary>
    public Stopwatch MathTimer { get; init; }

    public double OKT { get; init; }

    public double C1 => CordCs?.Last().C1 ?? 0;
    public double C2 => CordCs?.Last().C2 ?? 0;
    public double C3 => CordCs?.Last().C3 ?? 0;
    public double C4 => CordCs?.Last().C4 ?? 0;
    public double C5 => CordCs?.Last().C5 ?? 0;
    public double C6 => CordCs?.Last().C6 ?? 0;
    public double C7 => CordCs?.Last().C7 ?? 0;
}