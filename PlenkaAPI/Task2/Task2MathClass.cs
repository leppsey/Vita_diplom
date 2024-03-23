using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PlenkaAPI.Task2;

public class Task2MathClass
{
    private readonly Task2CalculationParameters _parameters;

    public Task2MathClass(Task2CalculationParameters parameters)
    {
        _parameters = parameters;
    }
    public ResultWrapper<Task2CalculationResults> Calculate()
    {
        if (ku < 0)
        {
            return ResultWrapper<Task2CalculationResults>.WithError("Сеточное число Куранта должно быть больше нуля");
        }

        if (ku >= 1)
        {
            return ResultWrapper<Task2CalculationResults>.WithError("Сеточное число Куранта должно быть меньше 1");
        }

        var s = (Math.PI * Math.Pow(d, 2)) / 4;
        var u = (Q * Math.Pow(10, -3)) / s;
        var tR = l / u;
        var teta = 2 * tR;
        var k1 = k01 * Math.Exp(-ea1 / (8.31 * (T + 273)));
        var k2 = k02 * Math.Exp(-ea2 / (8.31 * (T + 273)));
        var q = 0d;
        var eps = 2 * eMax;
        double deltaT = 0d, deltaX = 0d, CBmax = 0d;
        var CB1 = new double[2, 2];
        var x = new double[2];
        var t = new double[2];
        var CA = new double[2, 2];
        var CB = new double[2, 2];
        var eA = 0.0;
        int M = 0, N = 0, M1 = 0, N1 = 0;
        while (eps >= eMax && q <= qMax)
        {
            if (q == 0)
            {
                deltaX = deltaX0;
                deltaT = ku * deltaX / u;
                M = (int)Math.Round(l / deltaX, 0);
                N = (int)Math.Round(teta / deltaT, 0);
            }
            else
            {
                deltaX = deltaX / 2;
                deltaT = deltaT / 2;
                M = 2 * (M + 1);
                N = 2 * (N + 1);
            }

            CA = new double[N, M];
            CB = new double[N, M];
            x = new double[M];
            t = new double[N];
            for (var i = 0; i < M; i++)
            {
                x[i] = (i - 1) * deltaX;
                CA[0, i] = 0;
                CB[0, i] = 0;
            }

            for (var j = 1; j < N; j++)
            {
                t[j] = (j - 1) * deltaT;
                CA[j, 0] = cAin;
                CB[j, 0] = 0;
            }

            for (var j = 0; j < N - 1; j++)
            {
                for (var i = 1; i < M; i++)
                {
                    CA[j + 1, i] = -ku * (CA[j, i] - CA[j, i - 1]) + (1 - k1 * deltaT) * CA[j, i] +
                                   k2 * deltaT * CB[j, i];
                    CB[j + 1, i] = -ku * (CB[j, i] - CB[j, i - 1]) + (1 - k2 * deltaT) * CB[j, i] +
                                   k1 * deltaT * CA[j, i];
                }
            }


            if (q != 0)
            {
                var sum = 0.0;
                for (var j = 0; j < N1; j++)
                {
                    for (var i = 0; i < M1; i++)
                    {
                        sum += Math.Pow(CB[2 * j, 2 * i] - CB1[j, i], 2);
                    }
                }

                eA = Math.Sqrt((sum / (M1 * N1)));
                CBmax = CB.Cast<double>().Max();
                eps = (eA / CBmax) * 100;
            }

            CB1 = new double[N, M];
            for (var j = 0; j < N; j++)
            {
                for (var i = 0; i < M; i++)
                {
                    CB1[j, i] = CB[j, i];
                }
            }

            M1 = M;
            N1 = N;
            q += 1;
        }

        q--;
        if (q > qMax)
        {
            return ResultWrapper<Task2CalculationResults>.WithError(
                "Решение с погрешностью, не превосходящей предельно допустимую погрешность, не получено");
        }

        var result = new Task2CalculationResults()
        {
            S = s,
            U = u,
            tR = tR,
            teta = teta,
            k1 = k1,
            k2 = k2,
            deltaX = deltaX,
            deltaT = deltaT,
            M = M,
            N = N,
            CBmax = CBmax,
            eA = eA,
            eps = eps,
            q = q,
        };
        return ResultWrapper<Task2CalculationResults>.WithResult(result);
    }


    private double d => _parameters.d;
    private double l => _parameters.l;
    private double Q => _parameters.Q;
    private double cAin => _parameters.cAin;
    private double T => _parameters.T;
    private double k01 => _parameters.k01;
    private double ea1 => _parameters.ea1;
    private double k02 => _parameters.k02;
    private double ea2 => _parameters.ea2;
    private double deltaX0 => _parameters.deltaX0;
    private double ku => _parameters.ku;
    private double eMax => _parameters.eMax;
    private double qMax => _parameters.qMax;
}