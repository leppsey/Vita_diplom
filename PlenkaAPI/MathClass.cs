using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;


namespace PlenkaAPI
{
    /// <summary>
    ///     Структура для отображения данных в таблице с результатами
    /// </summary>
    public struct CordC
    {
        /// <summary>
        ///     Координата канала по X
        /// </summary>
        public double Cord { get; set; }

        /// <summary>
        ///     концентрация 1 вещества
        /// </summary>
        public double C1 { get; set; }

        /// <summary>
        ///     концентрация 2 вещества
        /// </summary>
        public double C2 { get; set; }

        /// <summary>
        ///     концентрация 3 вещества
        /// </summary>
        public double C3 { get; set; }

        /// <summary>
        ///     концентрация 4 вещества
        /// </summary>
        public double C4 { get; set; }

        /// <summary>
        ///     концентрация 5 вещества
        /// </summary>
        public double C5 { get; set; }

        /// <summary>
        ///     концентрация 6 вещества
        /// </summary>
        public double C6 { get; set; }

        /// <summary>
        ///     концентрация 7 вещества
        /// </summary>
        public double C7 { get; set; }

        public double T { get; set; }
    }


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


    public struct CalculationResults
    {
        public List<CordC> CordCs { get; init; }

        /// <summary>
        ///     Таймер для времени расчета
        /// </summary>
        public Stopwatch MathTimer { get; init; }

        public double OKT { get; init; }
    }


    public class MathClass // todo как-то красиво переписать все это
    {
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
        public void Calculate()
        {
            var sw = new Stopwatch();
            sw.Start();
            // var tay = Step;
            var h = Step;
            // var tN = (int)Round(t / tay,0);
            var hN = (int)Math.Round(L / h, 0);
            double[] K =
            {
                0.039, 0.01, 0.009, 0.005, 0.031, 0.001, 0.012, 0.015, 0.017, 0.035, 0.001, 0.06, 0.06, 0.031, 0.035,
                0.0001
            };

            double[] H =
            {
                82.2, 8.22, 6.98, 6.98, 4.44, 4.44, 5.00, 5.00, 7.00, 7.00, 6.50, 6.50, 7.50, 7.50, 7.90,
                7.9
            };

            var C0 = new double[7];
            C0[0] = 59.37;
            C0[1] = 17.39;
            C0[2] = 5.41;
            C0[3] = 10.8;
            C0[4] = 4.52;
            C0[5] = 1.06;
            C0[6] = 0.92;
            double[] oktNumbers = { 61.7, 92.3, 24.8, 66.0, 75.0, 91.1, 91.8 };
            // var C_temp = new double[7,tN,hN];
            var cTemp = new double[7, hN + 1];
            var r = new double[7];
            var v = 4 * G / (P * 3.14 * D * D);
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
                    tTemp = h * sum / (v * P * Heat_cap) + tTemp;

                    cordCs.Add(new CordC
                    {
                        Cord = x, C1 = C0[0], C2 = C0[1], C3 = C0[2], C4 = C0[3], C5 = C0[4], C6 = C0[5], C7 = C0[6],
                        T = tTemp
                    });
                    x += h;
                }
            }

            var oktNumber = 0d;
            for (var i = 0; i < 7; i++)
            {
                oktNumber += oktNumbers[i] * C0[i] / 100;
            }

            sw.Stop();

            Results = new CalculationResults
                { CordCs = cordCs, MathTimer = sw, OKT = Math.Round(oktNumber, 2) };
        }


        //todo rename
        public void Modeling(double d, double l, double Q, double cAin, double T, double k01, double ea1, double k02,
            double ea2, double deltaX0, double ku, double eMax, double qMax)
        {
            if (ku < 0)
            {
                //todo change todo to messagebox
                Console.WriteLine("Сеточное число Куранта должно быть больше нуля");
            }

            if (ku >= 1)
            {
                Console.WriteLine("Сеточное число Куранта должно быть меньше 1");
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
                Console.WriteLine(
                    "Решение с погрешностью, не превосходящей предельно допустимую погрешность, не получено");
            }
            //тута то что должен выводить
            Console.Write("S=" + s.ToString("G3") + " u=" + u.ToString("G3") + " tR=" + tR.ToString("G3") + " teta=" +
                          teta.ToString("G3") + " k1=" + k1.ToString("G3") + " k2=" + k2.ToString("G3") +
                          " deltaX=" +
                          deltaX.ToString("G3") + " deltaT=" +
                          deltaT.ToString("G3") + " M=" + M + " N=" + N +
                          " CBmax=" +
                          CBmax.ToString("G3") + " eA=" + eA.ToString("G3") + " eps=" + eps.ToString("G3") +
                          " q=" + q);
        }


        #region Parameters

        public CalculationParameters Cp { get; init; }

        // private double W
        // {
        //     get
        //     {
        //         return Cp.W;
        //     }
        // }
        //
        // private double H
        // {
        //     get
        //     {
        //         return Cp.H;
        //     }
        // }

        private double L
        {
            get { return Cp.L; }
        }

        private double G
        {
            get { return Cp.G; }
        }

        private double T0
        {
            get { return Cp.T0; }
        }

        private double P
        {
            get { return Cp.P; }
        }

        private double D
        {
            get { return Cp.D; }
        }

        private double Heat_cap
        {
            get { return Cp.HeatCap; }
        }

        // private double T0
        // {
        //     get
        //     {
        //         return Cp.T0;
        //     }
        // }
        //
        // private double Vu
        // {
        //     get
        //     {
        //         return Cp.Vu;
        //     }
        // }
        //
        // private double Tu
        // {
        //     get
        //     {
        //         return Cp.Tu;
        //     }
        // }
        //
        // private double U0
        // {
        //     get
        //     {
        //         return Cp.U0;
        //     }
        // }
        //
        // private double C1
        // {
        //     get
        //     {
        //         return Cp.Const1;
        //     }
        // }
        //
        // private double C2
        // {
        //     get
        //     {
        //         return Cp.Const2;
        //     }
        // }
        //
        // private double Tr
        // {
        //     get
        //     {
        //         return Cp.Tr;
        //     }
        // }
        // private double Tg
        // {
        //     get
        //     {
        //         return Cp.Tg;
        //     }
        // }
        //
        // private double N
        // {
        //     get
        //     {
        //         return Cp.N;
        //     }
        // }
        //
        // private double Au
        // {
        //     get
        //     {
        //         return Cp.Au;
        //     }
        // }

        private double Step
        {
            get { return Cp.Step; }
        }

        #endregion
    }
}