using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Isomerization.Domain.Task3;
using Isomerization.Shared;
using Isomerization.UI.Services;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Win32;
using SkiaSharp;

namespace Isomerization.UI.Features;

/// <summary>
/// ViewModel для ЦИМ-2 (Task3) - расчет трубопроводной системы
/// </summary>
public class Task3ViewModel : ViewModelBase
{
    private readonly IMessageBoxService _messageBoxService;

    public Task3ViewModel(IMessageBoxService messageBoxService)
    {
        _messageBoxService = messageBoxService;
        Variants = new ObservableCollection<PipeVariantRow>();
        
        // Значения по умолчанию
        var defaultParams = PipeCalculationParameters.CreateDefault();
        lambda = defaultParams.lambda;
        eta = defaultParams.eta;
        
        // Инициализация графиков
        InitializeCharts();
    }

    #region Свойства ввода

    private double _d;
    /// <summary>
    /// Диаметр, м
    /// </summary>
    public double D
    {
        get => _d;
        set
        {
            _d = value;
            OnPropertyChanged();
        }
    }

    private double _l;
    /// <summary>
    /// Длина, м
    /// </summary>
    public double L
    {
        get => _l;
        set
        {
            _l = value;
            OnPropertyChanged();
        }
    }

    private double _sumZeta;
    /// <summary>
    /// Сумма местных сопротивлений
    /// </summary>
    public double sumZeta
    {
        get => _sumZeta;
        set
        {
            _sumZeta = value;
            OnPropertyChanged();
        }
    }

    private double _lambda;
    /// <summary>
    /// Коэффициент трения
    /// </summary>
    public double lambda
    {
        get => _lambda;
        set
        {
            _lambda = value;
            OnPropertyChanged();
        }
    }

    private double _eta;
    /// <summary>
    /// КПД насоса
    /// </summary>
    public double eta
    {
        get => _eta;
        set
        {
            _eta = value;
            OnPropertyChanged();
        }
    }

    private double _rho;
    /// <summary>
    /// Плотность среды, кг/м³
    /// </summary>
    public double rho
    {
        get => _rho;
        set
        {
            _rho = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Параметры ТЗ

    private double _efMin;
    /// <summary>
    /// Минимальная производительность
    /// </summary>
    public double EFmin
    {
        get => _efMin;
        set
        {
            _efMin = value;
            OnPropertyChanged();
        }
    }

    private double _esMax;
    /// <summary>
    /// Максимальная энергоемкость
    /// </summary>
    public double ESmax
    {
        get => _esMax;
        set
        {
            _esMax = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Производительность из ЦИМ-1

    private double _ef;
    /// <summary>
    /// Производительность (расход) из ЦИМ-1
    /// </summary>
    public double EF
    {
        get => _ef;
        set
        {
            _ef = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(deltaEF));
        }
    }

    private RelayCommand _loadFromCim1Command;
    /// <summary>
    /// Команда для подтягивания EF и rho из ЦИМ-1
    /// </summary>
    public RelayCommand LoadFromCim1Command => _loadFromCim1Command ??= new RelayCommand(_ =>
    {
        try
        {
            var researcherVM = App.GetService<ResearcherPageVM>();
            if (researcherVM == null)
            {
                _messageBoxService.Show("ЦИМ-1 не доступен", "Ошибка", MessageBoxButton.OK);
                return;
            }

            // EF = G (расход сырья из ЦИМ-1)
            EF = researcherVM.G;

            // rho = Density (плотность из выбранного сырья)
            if (researcherVM.SelectedRawMaterial != null)
            {
                rho = researcherVM.SelectedRawMaterial.Density;
                _messageBoxService.Show($"Данные подтянуты из ЦИМ-1:\nEF = {EF:F2} кг/с\nρ = {rho:F2} кг/м³", "Информация", MessageBoxButton.OK);
            }
            else
            {
                _messageBoxService.Show($"Производительность EF = {EF:F2} подтянута из ЦИМ-1\nПлотность не доступна (сырье не выбрано)", "Информация", MessageBoxButton.OK);
            }
        }
        catch (Exception ex)
        {
            _messageBoxService.Show($"Ошибка при подтягивании данных из ЦИМ-1: {ex.Message}", "Ошибка", MessageBoxButton.OK);
        }
    });

    #endregion

    #region Свойства результата

    private PipeCalculationResult? _calculationResult;

    /// <summary>
    /// Общая потеря давления
    /// </summary>
    public double? dP_total => _calculationResult?.dP_total;

    /// <summary>
    /// Энергоемкость
    /// </summary>
    public double? ES => _calculationResult?.ES;

    /// <summary>
    /// Разница EF - EFmin
    /// </summary>
    public double? deltaEF => _calculationResult?.deltaEF;

    /// <summary>
    /// Разница ESmax - ES
    /// </summary>
    public double? deltaES => _calculationResult?.deltaES;

    /// <summary>
    /// Условие по производительности выполнено
    /// </summary>
    public bool? isEFok => _calculationResult?.isEFok;

    /// <summary>
    /// Условие по энергоемкости выполнено
    /// </summary>
    public bool? isESok => _calculationResult?.isESok;

    /// <summary>
    /// Флаг наличия результатов расчета
    /// </summary>
    public bool HasResult => _calculationResult != null;

    #endregion

    #region Коллекция вариантов

    /// <summary>
    /// Коллекция вариантов расчета для перебора по диаметрам
    /// </summary>
    public ObservableCollection<PipeVariantRow> Variants { get; }

    #endregion

    #region Параметры для перебора диаметров

    private double _dMin;
    /// <summary>
    /// Минимальный диаметр для перебора, м
    /// </summary>
    public double Dmin
    {
        get => _dMin;
        set
        {
            _dMin = value;
            OnPropertyChanged();
        }
    }

    private double _dMax;
    /// <summary>
    /// Максимальный диаметр для перебора, м
    /// </summary>
    public double Dmax
    {
        get => _dMax;
        set
        {
            _dMax = value;
            OnPropertyChanged();
        }
    }

    private int _steps = 10;
    /// <summary>
    /// Количество шагов для перебора
    /// </summary>
    public int steps
    {
        get => _steps;
        set
        {
            _steps = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Команды

    private RelayCommand _calculateCim2Command;
    /// <summary>
    /// Команда расчета ЦИМ-2
    /// </summary>
    public RelayCommand CalculateCim2Command => _calculateCim2Command ??= new RelayCommand(_ =>
    {
        try
        {
            var parameters = new PipeCalculationParameters
            {
                EF = EF,
                EFmin = EFmin,
                ESmax = ESmax,
                rho = rho,
                D = D,
                L = L,
                sumZeta = sumZeta,
                lambda = lambda,
                eta = eta
            };

            _calculationResult = PipeMathService.Calculate(parameters);
            
            OnPropertyChanged(nameof(dP_total));
            OnPropertyChanged(nameof(ES));
            OnPropertyChanged(nameof(deltaEF));
            OnPropertyChanged(nameof(deltaES));
            OnPropertyChanged(nameof(isEFok));
            OnPropertyChanged(nameof(isESok));
            OnPropertyChanged(nameof(HasResult));

            var resultText = $"Результаты расчета:\n" +
                           $"Общая потеря давления (ΔPΣ): {dP_total:F2} Па\n" +
                           $"Энергоемкость (ES): {ES:F2} Вт\n" +
                           $"Разница по производительности (ΔEF): {deltaEF:F2}\n" +
                           $"Разница по энергоемкости (ΔES): {deltaES:F2}\n" +
                           $"Условие по производительности: {(isEFok == true ? "Выполнено" : "Не выполнено")}\n" +
                           $"Условие по энергоемкости: {(isESok == true ? "Выполнено" : "Не выполнено")}";

            // Добавляем подсказки, если условия не выполнены
            var hints = new List<string>();
            
            if (isEFok == false)
            {
                hints.Add($"⚠ Производительность (EF = {EF:F2}) ниже минимума (EFmin = {EFmin:F2}).\n   Рекомендация: увеличьте EF минимум на {Math.Abs(deltaEF.Value):F2} кг/с");
            }
            
            if (isESok == false)
            {
                hints.Add($"⚠ Энергоемкость (ES = {ES:F2} Вт) превышает максимум (ESmax = {ESmax:F2} Вт).\n   Рекомендации для уменьшения ES:\n" +
                         $"   • Увеличьте диаметр D (наиболее эффективно)\n" +
                         $"   • Уменьшите длину L\n" +
                         $"   • Уменьшите сумму местных сопротивлений Σζ\n" +
                         $"   • Увеличьте КПД насоса η");
            }
            
            if (hints.Any())
            {
                resultText += "\n\n" + string.Join("\n\n", hints);
            }

            _messageBoxService.Show(resultText, "Результаты расчета", MessageBoxButton.OK);
        }
        catch (ArgumentException ex)
        {
            _messageBoxService.Show($"Ошибка валидации: {ex.Message}", "Ошибка", MessageBoxButton.OK);
        }
        catch (Exception ex)
        {
            _messageBoxService.Show($"Ошибка при расчете: {ex.Message}", "Ошибка", MessageBoxButton.OK);
        }
    });

    private RelayCommand _sweepDiametersCommand;
    /// <summary>
    /// Команда перебора по диаметрам
    /// </summary>
    public RelayCommand SweepDiametersCommand => _sweepDiametersCommand ??= new RelayCommand(_ =>
    {
        try
        {
            var pBase = new PipeCalculationParameters
            {
                EF = EF,
                EFmin = EFmin,
                ESmax = ESmax,
                rho = rho,
                L = L,
                sumZeta = sumZeta,
                lambda = lambda,
                eta = eta,
                D = 0 // Будет установлен в цикле
            };

            var variants = PipeMathService.SweepByDiameter(pBase, Dmin, Dmax, steps);
            
            Variants.Clear();
            foreach (var variant in variants)
            {
                Variants.Add(variant);
            }
            
            UpdateGraphics();

            _messageBoxService.Show($"Рассчитано {Variants.Count} вариантов", "Перебор завершен", MessageBoxButton.OK);
        }
        catch (ArgumentException ex)
        {
            _messageBoxService.Show($"Ошибка валидации: {ex.Message}", "Ошибка", MessageBoxButton.OK);
        }
        catch (Exception ex)
        {
            _messageBoxService.Show($"Ошибка при переборе диаметров: {ex.Message}", "Ошибка", MessageBoxButton.OK);
        }
    });

    private RelayCommand _exportVariantsCommand;
    /// <summary>
    /// Команда экспорта таблицы вариантов в CSV
    /// </summary>
    public RelayCommand ExportVariantsCommand => _exportVariantsCommand ??= new RelayCommand(_ =>
    {
        try
        {
            if (!Variants.Any())
            {
                _messageBoxService.Show("Таблица вариантов пуста", "Ошибка", MessageBoxButton.OK);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
                FileName = $"Варианты_перебора_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                DefaultExt = "csv"
            };

            if (dialog.ShowDialog() == true)
            {
                var csv = new StringBuilder();
                
                // Заголовки колонок
                csv.AppendLine("D;v;dP_total;ES;isESok");
                
                // Данные
                foreach (var variant in Variants)
                {
                    csv.AppendLine($"{variant.D.ToString(CultureInfo.InvariantCulture)};{variant.v.ToString(CultureInfo.InvariantCulture)};{variant.dP_total.ToString(CultureInfo.InvariantCulture)};{variant.ES.ToString(CultureInfo.InvariantCulture)};{variant.isESok}");
                }
                
                File.WriteAllText(dialog.FileName, csv.ToString(), Encoding.UTF8);
                _messageBoxService.Show($"Таблица экспортирована в файл:\n{dialog.FileName}", "Экспорт завершен", MessageBoxButton.OK);
            }
        }
        catch (Exception ex)
        {
            _messageBoxService.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK);
        }
    });

    #endregion

    #region Графики

    /// <summary>
    /// Оси для графика ΔPΣ(D)
    /// </summary>
    public Axis[] DPTotalXAxes { get; set; }
    
    /// <summary>
    /// Оси Y для графика ΔPΣ(D)
    /// </summary>
    public Axis[] DPTotalYAxes { get; set; }
    
    /// <summary>
    /// Оси для графика ES(D)
    /// </summary>
    public Axis[] ESXAxes { get; set; }
    
    /// <summary>
    /// Оси Y для графика ES(D)
    /// </summary>
    public Axis[] ESYAxes { get; set; }

    /// <summary>
    /// Серия для графика ΔPΣ(D)
    /// </summary>
    private LineSeries<ObservablePoint> DPTotalLineSeries { get; set; }
    
    /// <summary>
    /// Серия для графика ES(D)
    /// </summary>
    private LineSeries<ObservablePoint> ESLineSeries { get; set; }

    /// <summary>
    /// Серии для графика ΔPΣ(D)
    /// </summary>
    public List<ISeries> DPTotalSeries { get; set; }

    /// <summary>
    /// Серии для графика ES(D)
    /// </summary>
    public List<ISeries> ESSeries { get; set; }

    /// <summary>
    /// Инициализация графиков
    /// </summary>
    private void InitializeCharts()
    {
        DPTotalXAxes = new[]
        {
            new Axis
            {
                Name = "D, м",
                TextSize = 14,
                LabelsPaint = new SolidColorPaint(SKColors.Black),
            }
        };

        DPTotalYAxes = new[]
        {
            new Axis
            {
                Name = "ΔPΣ, Па",
                TextSize = 14,
                LabelsPaint = new SolidColorPaint(SKColors.Black),
            }
        };

        ESXAxes = new[]
        {
            new Axis
            {
                Name = "D, м",
                TextSize = 14,
                LabelsPaint = new SolidColorPaint(SKColors.Black),
            }
        };

        ESYAxes = new[]
        {
            new Axis
            {
                Name = "ES, Вт",
                TextSize = 14,
                LabelsPaint = new SolidColorPaint(SKColors.Black),
            }
        };

        DPTotalLineSeries = new LineSeries<ObservablePoint>
        {
            Name = "ΔPΣ",
            Fill = null,
            GeometryStroke = null,
            GeometryFill = null,
        };

        ESLineSeries = new LineSeries<ObservablePoint>
        {
            Name = "ES",
            Fill = null,
            GeometryStroke = null,
            GeometryFill = null,
        };

        DPTotalSeries = new List<ISeries> { DPTotalLineSeries };
        ESSeries = new List<ISeries> { ESLineSeries };
    }

    /// <summary>
    /// Обновление графиков на основе коллекции Variants
    /// </summary>
    private void UpdateGraphics()
    {
        if (!Variants.Any())
        {
            DPTotalLineSeries.Values = Array.Empty<ObservablePoint>();
            ESLineSeries.Values = Array.Empty<ObservablePoint>();
            OnPropertyChanged(nameof(DPTotalSeries));
            OnPropertyChanged(nameof(ESSeries));
            return;
        }

        var dValues = Variants.Select(v => v.D).ToList();
        var dpTotalValues = Variants.Select(v => v.dP_total).ToList();
        var esValues = Variants.Select(v => v.ES).ToList();

        UpdateLineSeries(DPTotalLineSeries, dValues, dpTotalValues);
        UpdateLineSeries(ESLineSeries, dValues, esValues);

        OnPropertyChanged(nameof(DPTotalSeries));
        OnPropertyChanged(nameof(ESSeries));
    }

    /// <summary>
    /// Обновление серии графика
    /// </summary>
    private void UpdateLineSeries(LineSeries<ObservablePoint> series, List<double> x, List<double> y)
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

        series.Values = points;
    }

    #endregion
}
