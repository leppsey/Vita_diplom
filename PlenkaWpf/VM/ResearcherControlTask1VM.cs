using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using PlenkaAPI;
using PlenkaAPI.Models;
using PlenkaAPI.Task1;
using PlenkaAPI.Task2;
using PlenkaWpf.Utils;

namespace PlenkaWpf.VM;

public class ResearcherControlTask1VM: ViewModelBase
{
    public ResearcherControlTask1VM()
    {
         //Materials = DbContextSingleton.GetInstance().MembraneObjects.Where(o => o.Type.TypeName == "Материал").ToList();
            // Materials = new List<MembraneObject> { new MembraneObject() {ObName="Бензиновая фракция" } };
            // //Material = DbContextSingleton.GetInstance().MembraneObjects.First(v => v.ObName == "Полистирол");
            // Material = Materials.First();
            // //Canals = DbContextSingleton.GetInstance().MembraneObjects.Where(o => o.Type.TypeName == "Канал").ToList();
            // Reactors = new List<MembraneObject> { new MembraneObject() { ObName = "ЛК-2Б" } };
            // //Canal = DbContextSingleton.GetInstance().MembraneObjects.First(v => v.ObName == "Канал");
            // Reactor = Reactors.First();
            // Catalists = new List<MembraneObject> { new MembraneObject() { ObName = "ATIS-2L" } };
            // //Canal = DbContextSingleton.GetInstance().MembraneObjects.First(v => v.ObName == "Канал");
            // Catalist = Catalists.First();
            Length = 80.0;
            Consumption = 9.7;
            Diametr = 3.0;
            Density = 0.651;
            HeatCapacity = 190.931;
            T0 = 120.0;

            С1LineSerie = new LineSeries
                {Title = "С1",};

            С1LineSerie.Fill = Brushes.Transparent;


            С2LineSerie = new LineSeries
                {Title = "С2",};

            С2LineSerie.Fill = Brushes.Transparent;

            
            С3LineSerie = new LineSeries
                {Title = "С3",};

            С3LineSerie.Fill = Brushes.Transparent;

            
            С4LineSerie = new LineSeries
                {Title = "С4",};

            С4LineSerie.Fill = Brushes.Transparent;

            
            С5LineSerie = new LineSeries
                {Title = "С5",};

            С5LineSerie.Fill = Brushes.Transparent;

            
            С6LineSerie = new LineSeries
                {Title = "С6",};

            С6LineSerie.Fill = Brushes.Transparent;

           
            С7LineSerie = new LineSeries
                {Title = "С7",};

            С7LineSerie.Fill = Brushes.Transparent;

            TLineSerie = new LineSeries
                {Title = "T",};

            TLineSerie.Fill = Brushes.Transparent;   
            
            IsCalculated = false;
            
            TSeries = new SeriesCollection
                {TLineSerie};
            
            С1Series = new SeriesCollection
                {С1LineSerie,С2LineSerie,С3LineSerie,С4LineSerie,С5LineSerie,С6LineSerie,С7LineSerie,};
        
    }
    
    
        /// <summary>
        ///     Функция, обновляющая точки графика по словарю со значениями
        /// </summary>
        /// <param name="ls">Серия графика</param>
        private void UpdateLineSeriesByCordAndValue(LineSeries ls, List<double> x, List<double> y)
        {
            if (x.Count != y.Count)
            {
                throw new ArgumentException("Количество значений x не совпадает с количеством значений y");
            }

            var newValues = new ChartValues<ObservablePoint>();

            for (var i = 0; i < x.Count; i++)
            {
                newValues.Add(new ObservablePoint(x[i], y[i]));
            }

            ls.Values = newValues;
        }



    #region Properties

        /// <summary>
        ///     Доступные материалы
        /// </summary>
        // public List<MembraneObject> Materials { get; set; }
        // public List<MembraneObject> Reactors { get; set; }
        // public List<MembraneObject> Catalists { get; set; }


        #region CanalProps

        /// <summary>
        ///     Текущий канал
        /// </summary>
        // private MembraneObject _canal;
        //
        // public MembraneObject Reactor
        // {
        //     get
        //     {
        //         return _canal;
        //     }
        //     set
        //     {
        //         _canal = value;
        //         // OnPropertyChanged(nameof(Length));
        //         // OnPropertyChanged(nameof(Width));
        //         // OnPropertyChanged(nameof(Depth));
        //     }
        // }

        /// <summary>
        ///     Длина канала
        /// </summary>
        // public double? Length
        // {
        //     get
        //     {
        //         return Reactor["Длина"];
        //     }
        //     set
        //     {
        //         Reactor["Длина"] = value;
        //         OnPropertyChanged();
        //     }
        // }

        /// <summary>
        ///     Ширина канала
        /// </summary>
        // public double? Width
        // {
        //     get
        //     {
        //         return Reactor["Ширина"];
        //     }
        //     set
        //     {
        //         Reactor["Ширина"] = value;
        //         OnPropertyChanged();
        //     }
        // }
        //
        // /// <summary>
        // ///     Глубина канала
        // /// </summary>
        // public double? Depth
        // {
        //     get
        //     {
        //         return Reactor["Глубина"];
        //     }
        //     set
        //     {
        //         Reactor["Глубина"] = value;
        //         OnPropertyChanged();
        //     }
        // }

        #endregion
        // private MembraneObject _catalist;
        //
        // /// <summary>
        // ///     Выбранный материал
        // /// </summary>
        // public MembraneObject Catalist
        // {
        //     get
        //     {
        //         return _catalist;
        //     }
        //     set
        //     {
        //         _catalist = value;
        //
        //         OnPropertyChanged();
        //
        //     }
        // }


        #region MaterialProps

        // private MembraneObject _material;
        //
        // /// <summary>
        // ///     Выбранный материал
        // /// </summary>
        // public MembraneObject Material
        // {
        //     get
        //     {
        //         return _material;
        //     }
        //     set
        //     {
        //         _material = value;
        //     }
        // }

        private object _null;

        public object Null // Костыль для того чтобы ErrorStr в TextBox всегда был пустым
        {
            get
            {
                return _null;
            }
            set
            {
                _null = null;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Плоность материала
        /// </summary>
        
        #endregion


            #region VarProps

            /// <summary>
            ///     Скорость крышки
            /// </summary>
        public double? CapSpeed { get; set; } = 1.5;

        /// <summary>
        ///     Температура крашки
        /// </summary>
        public double? CapTemperature { get; set; } = 210;

    #endregion


    #region Graphics

        /// <summary>
        ///     Список со значениями температуры и вязкости на протяжении канала
        /// </summary>
        public List<CordC> CordСs
        {
            get
            {
                if (Task1MathClass != null)
                {
                    return Task1MathClass.Results.CordCs;
                }

                return null;
            }
        }

        /// <summary>
        ///     Серия точек 1 вещества
        /// </summary>
        private LineSeries С1LineSerie { get; }

        public SeriesCollection С1Series { get; set; }

        /// <summary>
        ///     Серия точек 2 вещества
        /// </summary>
        private LineSeries С2LineSerie { get; }

        public SeriesCollection С2Series { get; set; }
        
        /// <summary>
        ///     Серия точек 3 вещества
        /// </summary>
        private LineSeries С3LineSerie { get; }

        public SeriesCollection С3Series { get; set; }
        
        /// <summary>
        ///     Серия точек 4 вещества
        /// </summary>
        private LineSeries С4LineSerie { get; }

        public SeriesCollection С4Series { get; set; }
        
        /// <summary>
        ///     Серия точек 5 вещества
        /// </summary>
        private LineSeries С5LineSerie { get; }

        public SeriesCollection С5Series { get; set; }
        
        /// <summary>
        ///     Серия точек 6 вещества
        /// </summary>
        private LineSeries С6LineSerie { get; }

        public SeriesCollection С6Series { get; set; }
        
        /// <summary>
        ///     Серия точек 7 вещества
        /// </summary>
        private LineSeries С7LineSerie { get; }

        public SeriesCollection С7Series { get; set; }
        
        /// <summary>
        ///     Серия точек 6 вещества
        /// </summary>
        private LineSeries TLineSerie { get; }

        public SeriesCollection TSeries { get; set; }

    #endregion


        /// <summary>
        ///     Текущая занаятая память
        /// </summary>
        public long TotalMemory
        {
            get
            {
                var currentProcess = Process.GetCurrentProcess();

                return currentProcess.WorkingSet64 / (1024 * 1024);
            }
        }

        private Task1MathClass _task1MathClass;

        public Task1MathClass Task1MathClass
        {
            get
            {
                return _task1MathClass;
            }
            set
            {
                _task1MathClass = value;
                OnPropertyChanged();
            }
        }    
        
        private Task2MathClass _task2MathClass;

        public Task2MathClass Task2MathClass
        {
            get
            {
                return _task2MathClass;
            }
            set
            {
                _task2MathClass = value;
                OnPropertyChanged();
            }
        }

        private void UpdateInterfaceElelemts()
        {
            var x = Task1MathClass.Results.CordCs.Select(x => x.Cord).ToList();
            var c1 = Task1MathClass.Results.CordCs.Select(x => x.C1).ToList();
            var c2 = Task1MathClass.Results.CordCs.Select(x => x.C2).ToList();
            var c3 = Task1MathClass.Results.CordCs.Select(x => x.C3).ToList();
            var c4 = Task1MathClass.Results.CordCs.Select(x => x.C4).ToList();
            var c5 = Task1MathClass.Results.CordCs.Select(x => x.C5).ToList();
            var c6 = Task1MathClass.Results.CordCs.Select(x => x.C6).ToList();
            var c7 = Task1MathClass.Results.CordCs.Select(x => x.C7).ToList();
            var t = Task1MathClass.Results.CordCs.Select(x => x.T).ToList();

            UpdateLineSeriesByCordAndValue(С1LineSerie, x, c1);
            UpdateLineSeriesByCordAndValue(С2LineSerie, x, c2);
            UpdateLineSeriesByCordAndValue(С3LineSerie, x, c3);
            UpdateLineSeriesByCordAndValue(С4LineSerie, x, c4);
            UpdateLineSeriesByCordAndValue(С5LineSerie, x, c5);
            UpdateLineSeriesByCordAndValue(С6LineSerie, x, c6);
            UpdateLineSeriesByCordAndValue(С7LineSerie, x, c7);
            UpdateLineSeriesByCordAndValue(TLineSerie, x, t);

            OnPropertyChanged(nameof(С1Series));
            OnPropertyChanged(nameof(С2Series));
            OnPropertyChanged(nameof(С3Series));
            OnPropertyChanged(nameof(С4Series));
            OnPropertyChanged(nameof(С5Series));
            OnPropertyChanged(nameof(С6Series));
            OnPropertyChanged(nameof(С7Series));
            OnPropertyChanged(nameof(TSeries));
            OnPropertyChanged(nameof(CordСs));
            OnPropertyChanged(nameof(TotalMemory));
        }

        /// <summary>
        ///     Шаг расчета
        /// </summary>
        public double? Step { get; set; } = 1;

        public double? Length { get; set; }
        public double? Diametr { get; set; }
        public double? Consumption { get; set; }
        public double? HeatCapacity { get; set; }
        public double? Density { get; set; }
        
        public double? T0 { get; set; }


        private bool _isCalculated;

        public bool IsCalculated
        {
            get
            {
                return _isCalculated;
            }
            set
            {
                _isCalculated = value;

                if (IsCalculated)
                {
                    TabControlVisibility = Visibility.Visible;
                }
                else
                {
                    TabControlVisibility = Visibility.Hidden;
                }
            }
        }

        private Visibility _tabControlVisibility;

        public Visibility TabControlVisibility
        {
            get
            {
                return _tabControlVisibility;
            }
            set
            {
                _tabControlVisibility = value;
                OnPropertyChanged();
            }
        }

    #endregion


    #region Commands

        private RelayCommand _calcCommand;

        /// <summary>
        ///     Команда, выполняющая расчет
        /// </summary>
        public RelayCommand CalcCommand
        {
            get
            {
                return _calcCommand ??= new RelayCommand(o =>
                {
                    IsCalculated = true;

                    var cp = new Task1CalculationParameters
                    {
                        // MaterialName = Material.ObName,
                        // W = (double) Width,
                        D = (double) Diametr,
                        L = (double) Length,
                        G = (double) Consumption,
                        P = (double) Density,
                        HeatCap = (double) HeatCapacity,
                        
                        T0 = (double) T0,
                        Step = (double) Step,
                    };

                    Task1MathClass = new Task1MathClass(cp);
                    Task1MathClass.Calculate();
                    OnPropertyChanged(nameof(Task1MathClass));
                    UpdateInterfaceElelemts();
                });
            }
        }

    #endregion
}