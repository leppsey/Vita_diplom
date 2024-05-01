using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Isomerization.Wpf.OLD.VM;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using MessageBox = HandyControl.Controls.MessageBox;
using Separator = LiveCharts.Wpf.Separator;


namespace Isomerization.Wpf.OLD.View;

/// <summary>
///     Логика взаимодействия для ResearcherControl.xaml
/// </summary>
public partial class ResearcherControl : UserControl, IСhangeableControl
{
    public ResearcherControl()
    {
        InitializeComponent();
        var vm = new ResearcherControlVM();
        DataContext = vm;

        //vm.ClosingRequest += (sender, e) => Close();
    }

    public WindowState PreferedWindowState { get; set; } = WindowState.Maximized;
    public string WindowTitle { get; set; } = "Программный комплекс для технологического проектирования установок изомеризации";
    public double? PreferedHeight { get; set; }
    public double? PreferedWidth { get; set; }
    public event IСhangeableControl.ChangingRequestHandler ChangingRequest;

    public void OnChangingRequest(UserControl newControl)
    {
        ChangingRequest?.Invoke(this, newControl);
    }

    public static byte[] EncodeVisual(FrameworkElement element, int dpi)
    {
        var bitmap = new RenderTargetBitmap((int)element.ActualWidth * dpi / 96,
            ((int)element.ActualHeight + 50) * dpi / 96, dpi, dpi
            , PixelFormats.Pbgra32);

        bitmap.Render(element);
        var frame = BitmapFrame.Create(bitmap);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(frame);

        using var stream = new MemoryStream();

        encoder.Save(stream);
        var bit = stream.ToArray();
        stream.Close();

        return bit;
    }

    private static CartesianChart CopyChart(CartesianChart chartToCopy, double width, double height)
    {
        var copiedChart = new CartesianChart
        {
            DisableAnimations = true,
            Width = width,
            Height = height,
            Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = chartToCopy.Series[0].Values
                }
            },
            AxisX = new AxesCollection
            {
                new()
                {
                    Separator = new Separator
                    {
                        Step = chartToCopy.AxisX[0].Separator.Step,
                        Stroke = chartToCopy.AxisX[0].Separator.Stroke
                    },
                    Title = chartToCopy.AxisX[0].Title,
                    MinValue = chartToCopy.AxisX[0].MinValue
                }
            },
            AxisY = new AxesCollection
            {
                new()
                {
                    Separator = new Separator
                    {
                        Step = chartToCopy.AxisY[0].Separator.Step,
                        Stroke = chartToCopy.AxisY[0].Separator.Stroke
                    },
                    Title = chartToCopy.AxisY[0].Title,
                    MinValue = chartToCopy.AxisY[0].MinValue
                }
            }
        };

        return copiedChart;
    }

    private static byte[] ChartToBitmap(CartesianChart chart, double width = 1000, double height = 1000, int dpi = 150)
    {
        var nonVisibleChart = CopyChart(chart, width, height);

        var viewbox = new Viewbox
        {
            Child = nonVisibleChart
        };

        viewbox.Measure(nonVisibleChart.RenderSize);
        viewbox.Arrange(new Rect(new Point(0, 0), nonVisibleChart.RenderSize));
        nonVisibleChart.Update(true, true); //force chart redraw
        viewbox.UpdateLayout();

        return EncodeVisual(nonVisibleChart, dpi);
    }

    private void MenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        var dlg = new SaveFileDialog
        {
            DefaultExt = ".pdf",
            FileName = "АНАЛИЗ_" + DateTime.Now.ToString().Replace(':', '_')
        };

        var res = dlg.ShowDialog();

        if (res == true)
        {
            if ((Task1Control.DataContext as ResearcherControlTask1VM).IsCalculated)
            {
                var tempChartBitMap = ChartToBitmap(Task1Control.tempChart);

                //var nChartBitMap = ChartToBitmap(nChart);


                // FileSystem.ExportPdf(dlg.FileName, tempChartBitMap, nChartBitMap, (DataContext as ResearcherControlVM).MathClass);
            }
            else
            {
                MessageBox.Show("Нет данных для сохранения", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void Validation_OnError(object? sender, ValidationErrorEventArgs e)
    {
        //throw new NotImplementedException();
    }

    private void ChangeUser(object sender, RoutedEventArgs e)
    {
        OnChangingRequest(new LoginControl());
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}