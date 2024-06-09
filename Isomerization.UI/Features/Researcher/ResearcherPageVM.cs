using System.Collections.ObjectModel;
using System.Diagnostics;
using Isomerization.Domain.Data;
using Isomerization.Domain.Models;
using Isomerization.Domain.Task1;
using Isomerization.Shared;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;

namespace Isomerization.UI.Features;

public class ResearcherPageVM: ViewModelBase
{
    private readonly IsomerizationContext _context;

    public ResearcherPageVM(IsomerizationContext context)
    {
        _context = context;
        
        RawMaterials = new ObservableCollection<RawMaterial>(_context.RawMaterials.Include(x=>x.Concetrations));
        SelectedRawMaterial = RawMaterials.FirstOrDefault();
        Catalysts = new ObservableCollection<Catalyst>(_context.Catalysts);
        SelectedCatalyst = Catalysts.FirstOrDefault();
    }
    public ObservableCollection<Installation> Installations { get; set; }
    public Installation SelectedInstallation { get; set; }

    public ObservableCollection<RawMaterial> RawMaterials { get; set; }
    public RawMaterial SelectedRawMaterial { get; set; }

    public ObservableCollection<Catalyst> Catalysts { get; set; }
    public Catalyst SelectedCatalyst { get; set; }
    
    //todo delete
    public string SOMESTRING { get; set; } = "12345";

    public Axis[] XAxes { get; set; } =
    {
        new Axis
        {
            Name = "Время пребывания, с",
            TextSize = 14,
            LabelsPaint = new SolidColorPaint(SKColors.Black),
        }
    };
    public Axis[] ConcentrationYAxes { get; set; } =
    {
        new Axis
        {
            Name = "Концентрация, %",
            TextSize = 14,
            LabelsPaint = new SolidColorPaint(SKColors.Black)
        }
    };
    public Axis[] TemperatureYAxes { get; set; } =
    {
        new Axis
        {
            Name = "Температура, С",
            TextSize = 14,
            LabelsPaint = new SolidColorPaint(SKColors.Black)
        }
    };
    /// <summary>
    /// Начальная температура
    /// </summary>
    public double T0 { get; set; } = 120;
    /// <summary>
    /// Расход сырья
    /// </summary>
    public double G { get; set; } = 0.01;
    /// <summary>
    /// Погрешность в процентах
    /// </summary>
    public double EMax { get; set; } = 5;
    /// <summary>
    /// Максимальное число делений сетки пополам 
    /// </summary>
    public double QMax { get; set; } = 4;

    /// <summary>
    /// Время проведения реакции
    /// </summary>
    public double Tau { get; set; } = 600;

    /// <summary>
    /// Шаг
    /// </summary>
    public double H { get; set; } = 1;

    /// <summary>
    /// Время эксплуатации катализатора
    /// </summary>
    public double CatalystT { get; set; } = 3600;
    public MathClass MathClass { get; set; }

    private RelayCommand _calcCommand;

    public RelayCommand CalcCommand => _calcCommand ??= new RelayCommand(_ =>
    {
        var calcParams = new CalculationParameters()
        {
            D = SelectedInstallation.Diameter,
            P = SelectedRawMaterial.Density,
            T0 = T0,
            G = SelectedInstallation.RawMaterialConsumption,
            Step = H,
            HeatCap = SelectedRawMaterial.HeatCapacity,
            L = Tau,
            Activity = SelectedCatalyst.Activity,
            C0 = SelectedRawMaterial.Concetrations.OrderBy(x=>x.Order).Select(x=>x.Value).ToArray(),
            
        };
        MathClass= new MathClass(calcParams);
        MathClass.Calculate();
        UpdateGraphics();
        IsCalculated = true;
    });
    
    public List<CordC> CordСs
    {
        get
        {
            if (MathClass != null)
            {
                return MathClass.Results.CordCs;
            }

            return null;
        }
    }

    public bool IsCalculated { get; set; } = false;
    public List<ISeries> CSeries { get; set; }


    private static LineSeries<ObservablePoint> CreateSerie(string name)
    {
        return new LineSeries<ObservablePoint>()
        {
            Name = name,
            Fill = null,
            
            GeometryStroke = null,
            GeometryFill = null,
            // GeometrySize = 8,
        };
    }

    /// <summary>
    ///     Серия точек 1 вещества
    /// </summary>
    private LineSeries<ObservablePoint> C1LineSerie { get; set; } = CreateSerie("C1");

    /// <summary>
    ///     Серия точек 2 вещества
    /// </summary>
    private LineSeries<ObservablePoint> C2LineSerie { get; set; } = CreateSerie( "C2");

        
    /// <summary>
    ///     Серия точек 3 вещества
    /// </summary>
    private LineSeries<ObservablePoint> C3LineSerie { get; set; } = CreateSerie( "C3");

    /// <summary>
    ///     Серия точек 4 вещества
    /// </summary>
    private LineSeries<ObservablePoint> C4LineSerie { get; set; } = CreateSerie( "C4");

    /// <summary>
    ///     Серия точек 5 вещества
    /// </summary>
    private LineSeries<ObservablePoint> C5LineSerie { get; set; } = CreateSerie( "C5");

    /// <summary>
    ///     Серия точек 6 вещества
    /// </summary>
    private LineSeries<ObservablePoint> C6LineSerie { get; set; } = CreateSerie( "C6");
        
    /// <summary>
    ///     Серия точек 7 вещества
    /// </summary>
    private LineSeries<ObservablePoint> C7LineSerie { get; set; } = CreateSerie( "C7");

    
    private LineSeries<ObservablePoint> TLineSerie { get; set; } = CreateSerie("T");
    public List<ISeries> TSeries { get; set; }
    public long TotalMemory
    {
        get
        {
            var currentProcess = Process.GetCurrentProcess();

            return currentProcess.WorkingSet64 / (1024 * 1024);
        }
    }
    public void UpdateGraphics()
    {
        var x = MathClass.Results.CordCs.Select(c => c.Cord).ToList();
        var c1 = MathClass.Results.CordCs.Select(c => c.C1).ToList();
        var c2 = MathClass.Results.CordCs.Select(c => c.C2).ToList();
        var c3 = MathClass.Results.CordCs.Select(c => c.C3).ToList();
        var c4 = MathClass.Results.CordCs.Select(c => c.C4).ToList();
        var c5 = MathClass.Results.CordCs.Select(c => c.C5).ToList();
        var c6 = MathClass.Results.CordCs.Select(c => c.C6).ToList();
        var c7 = MathClass.Results.CordCs.Select(c => c.C7).ToList();
        var t = MathClass.Results.CordCs.Select(c => c.T).ToList();
        
        UpdateLineSeriesByCordAndValue(C1LineSerie, x, c1);
        UpdateLineSeriesByCordAndValue(C2LineSerie, x, c2);
        UpdateLineSeriesByCordAndValue(C3LineSerie, x, c3);
        UpdateLineSeriesByCordAndValue(C4LineSerie, x, c4);
        UpdateLineSeriesByCordAndValue(C5LineSerie, x, c5);
        UpdateLineSeriesByCordAndValue(C6LineSerie, x, c6);
        UpdateLineSeriesByCordAndValue(C7LineSerie, x, c7);
        UpdateLineSeriesByCordAndValue(TLineSerie, x, t);

        if (MathClass.Cp.MaterialCount == 4)
        {
            CSeries = new List<ISeries>()
            {
                C1LineSerie,
                C2LineSerie,
                C3LineSerie,
                C4LineSerie,
            };
        }
        else if (MathClass.Cp.MaterialCount == 7)
        {
            CSeries = new List<ISeries>()
            {
                C1LineSerie,
                C2LineSerie,
                C3LineSerie,
                C4LineSerie,
                C5LineSerie,
                C6LineSerie,
                C7LineSerie,
            };
        }
        
        TSeries = new List<ISeries>() { TLineSerie };
        
        OnPropertyChanged(nameof(MathClass));
        OnPropertyChanged(nameof(TotalMemory));

    }
    
    private void UpdateLineSeriesByCordAndValue(LineSeries<ObservablePoint> serie, List<double> x, List<double> y)
    {
        if (x.Count != y.Count)
        {
            throw new ArgumentException("Количество значений x не совпадает с количеством значений y");
        }

        var points = new ObservablePoint[x.Count];
        for (var i = 0; i < x.Count; i++)
        {
            points[i] = new ObservablePoint(x[i], y[i]);
        }

        serie.Values = points;
    }
}