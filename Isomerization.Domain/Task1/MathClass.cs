using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Isomerization.Domain.Task1;

/// <summary>
/// Математический класс
/// </summary>
public class MathClass // todo как-то красиво переписать все это
{
    /// <summary>
    /// Конструктор класса
    /// </summary>
    /// <param name="cp">Параметры для вычислений</param>
    public MathClass(CalculationParameters cp)
    {
        Cp = cp;
    }

    /// <summary>
    ///     Результаты вычислений
    /// </summary>
    public CalculationResults Results { get; private set; }

    private int GetDecimalDigitsCount(double number)
    {
        var str = number.ToString(new NumberFormatInfo
                { NumberDecimalSeparator = ".", })
            .Split('.');

        return str.Length == 2 ? str[1].Length : 0;
    }

    /// <summary>
    ///     Функция производит вычисления с заданными параметрами
    /// </summary>
    public void Calculate1()
    {
        var sw = new Stopwatch();
        sw.Start();
        var h = Step;
        var hN = (int)Math.Round(L / h, 0);
        
        double[] K =
        {
            0.039  * Activity,
            0.01   * Activity,
            0.009  * Activity,
            0.005  * Activity,
            0.031  * Activity,
            0.001  * Activity,
            0.012  * Activity,
            0.015  * Activity,
            0.017  * Activity,
            0.035  * Activity,
            0.001  * Activity,
            0.06   * Activity,
            0.06   * Activity,
            0.031  * Activity,
            0.035  * Activity,
            0.0001 * Activity,
        };

        double[] H =
        {
            -8.22, 8.22, -6.98, 6.98, -4.44, 4.44, -5.00, 5.00, -7.00, 7.00, -6.50, 6.50, -7.50, 7.50, -7.90,
            7.9
        };
        // var C_temp = new double[7,tN,hN];
        var cTemp = new double[7, hN + 1];
        var r = new double[7];
        var v = 4 * G / (P * Math.PI * D * D);
        var x = h;
        var cordCs = new List<CordC>();
        var tTemp = T0;
        cordCs.Add(new CordC
        {
            Cord = 0, C1 = C0[0], C2 = C0[1], C3 = C0[2], C4 = C0[3], C5 = C0[4], C6 = C0[5], C7 = C0[6], T = T0
        });
        for (var reactor = 0; reactor < 1; reactor++)
        {
            for (var i = 0; i < 7; i++)
            {
                cTemp[i, 0] = C0[i];
            }

            for (var k = 1; k <= hN; k++)
            {
                // расчет r[i]
                r[0] = -K[0] * C0[0] + K[1] * C0[1];
                r[1] = K[0] * C0[0] - K[1] * C0[1];
                r[2] = -K[2] * C0[2] + K[3] * C0[3] - K[4] * C0[2] + K[5] * C0[4];
                r[3] = K[2] * C0[2] - K[3] * C0[3] - K[6] * C0[3] + K[7] * C0[5] - K[8] * C0[3] + K[9] * C0[6];
                r[4] = K[4] * C0[2] - K[5] * C0[4] - K[10] * C0[4] + K[11] * C0[5] - K[12] * C0[4] + K[13] * C0[6];
                r[5] = K[6] * C0[3] - K[7] * C0[5] + K[10] * C0[4] - K[11] * C0[5] + K[15] * C0[6] - K[14] * C0[5];
                r[6] = K[8] * C0[3] - K[9] * C0[6] + K[12] * C0[4] - K[13] * C0[6] + K[14] * C0[5] - K[15] * C0[6];
                for (var i = 0; i < 7; i++)
                {
                    // C_temp[i, k] = Round(h*r[i]/v + C_temp[i, k - 1],2);
                    cTemp[i, k] = h * r[i] / v + cTemp[i, k - 1];
                    C0[i] = cTemp[i, k];
                }

                var sum = K[0] * C0[0] * H[0] +
                          K[1] * C0[1] * H[1] +
                          K[2] * H[2] * C0[2] +
                          K[3] * H[3] * C0[3] +
                          K[4] * H[4] * C0[2] +
                          K[5] * H[5] * C0[4] +
                          K[6] * H[6] * C0[3] +
                          K[7] * H[7] * C0[5] +
                          K[8] * H[8] * C0[3] +
                          K[9] * H[9] * C0[6] +
                          K[10] * H[10] * C0[4] +
                          K[11] * H[11] * C0[5] +
                          K[12] * H[12] * C0[4] +
                          K[13] * H[13] * C0[6] +
                          K[14] * H[14] * C0[5] +
                          K[15] * H[15] * C0[6];
                tTemp = h * sum / (v * P * HeatCap) + tTemp;

                cordCs.Add(new CordC
                {
                    Cord = x, C1 = C0[0], C2 = C0[1], C3 = C0[2], C4 = C0[3], C5 = C0[4], C6 = C0[5], C7 = C0[6],
                    T = tTemp
                });
                x += h;
            }
        }

        double[] oktNumbers = { 61.7, 92.3, 24.8, 66.0, 75.0, 91.1, 91.8 };
        var oktNumber = 0d;
        for (var i = 0; i < 7; i++)
        {
            oktNumber += oktNumbers[i] * C0[i] / 100;
        }

        sw.Stop();

        Results = new CalculationResults
            { CordCs = cordCs, MathTimer = sw, OKT = Math.Round(oktNumber, 2) };
    }

    public void Calculate()
    {
        var sw = new Stopwatch();
        sw.Start();
        // var tay = Step;
        var h = Step;
        // var tN = (int)Round(t / tay,0);
        var hN = (int)Math.Round(L / h, 0);
        // k это 
        var R = 8.31;
        double[] K =
        {
            0.027, 0.03, 0.05, 0.04, 0.05
        };
        double[] C0 =
        {
            1, 0, 0, 0, 0, 0, 0
        };

        double[] H =
        {
            -8.22, 8.22, -6.98, 6.98, -4.44
        };
        // var C_temp = new double[7,tN,hN];
        var cTemp = new double[7, hN + 1];
        var r = new double[7];
        var v = 4 * G / (P * Math.PI * D * D);
        var x = h;
        var cordCs = new List<CordC>();
        var tTemp = T0;
        cordCs.Add(new CordC
        {
            Cord = 0, C1 = C0[0], C2 = C0[1], C3 = C0[2], C4 = C0[3], C5 = C0[4], C6 = C0[5], C7 = C0[6], T = T0
        });
        for (var reactor = 0; reactor < 1; reactor++)
        {
            for (var i = 0; i < 7; i++)
            {
                cTemp[i, 0] = C0[i];
            }

            for (var k = 1; k <= hN; k++)
            {
                
                // расчет r[i]
                r[0] = -K[0] *  C0[0]-K[1] *  C0[0]-K[2] *  C0[0];
                r[1] = K[0] *  C0[0]-K[3] *  C0[1]-K[4] *  C0[1];
                r[2] = K[2] *  C0[0]+K[4] *  C0[1];
                r[3] = K[1] *  C0[0]+K[3] *  C0[1];
                r[4] = 0;
                r[5] = 0;
                r[6] = 0;
                for (var i = 0; i < 7; i++)
                {
                    // C_temp[i, k] = Round(h*r[i]/v + C_temp[i, k - 1],2);
                    cTemp[i, k] = h * r[i]/v  + cTemp[i, k - 1];
                    C0[i] = cTemp[i, k];
                }

                var sum = K[0] *  C0[0]  * H[0]  +
                          K[1] *  C0[0]  * H[1]  +
                          K[2] *  C0[0] * H[2]  +
                          K[3] *  C0[1] * H[3]  +
                          K[4] *  C0[1] * H[4]  ;
                tTemp = h * sum / (v * P * HeatCap) + tTemp;

                cordCs.Add(new CordC
                {
                    Cord = x, C1 = C0[0], C2 = C0[1], C3 = C0[2], C4 = C0[3], C5 = C0[4], C6 = C0[5], C7 = C0[6],
                    T = tTemp
                });
                x += h;
            }
        }

        double[] oktNumbers = { 10.8, 15.1, 92.0, 79.0, 0, 0, 0 };
        var oktNumber = 0d;
        for (var i = 0; i < 7; i++)
        {
            oktNumber += oktNumbers[i] * C0[i] ;
        }

        sw.Stop();

        Results = new CalculationResults
            { CordCs = cordCs, MathTimer = sw, OKT = Math.Round(oktNumber, 2) };
    }


    //todo rename


    #region Parameters

    public CalculationParameters Cp { get; init; }

    private double L => Cp.L;

    private double G => Cp.G;

    private double T0 => Cp.T0;

    private double P => Cp.P;

    private double D => Cp.D;

    private double HeatCap => Cp.HeatCap;

    private double Step => Cp.Step;
    private double Activity => Cp.Activity;
    private double[] C0 => Cp.C0;

    #endregion
}