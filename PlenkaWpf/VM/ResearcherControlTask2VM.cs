using System.Linq;
using System.Text;
using HandlebarsDotNet.Extensions;
using HandyControl.Controls;
using PlenkaAPI.Task2;
using PlenkaWpf.Utils;

namespace PlenkaWpf.VM;

public class ResearcherControlTask2VM: ViewModelBase
{
    public double? d { get; set; }
    public double? l { get; set; }
    public double? Q { get; set; }
    public double? cAin { get; set; }
    public double? T { get; set; }
    public double? k01 { get; set; }
    public double? ea1 { get; set; }
    public double? k02 { get; set; }
    public double? ea2 { get; set; }
    public double? deltaX0 { get; set; }
    public double? ku { get; set; }
    public double? eMax { get; set; }
    public double? qMax { get; set; }

    public Task2CalculationResults Result { get; set; }
    
    private RelayCommand _calcCommand;
    public RelayCommand CalcCommand
    {
        get
        {
            return _calcCommand ??= new RelayCommand(o =>
            {
                // IsCalculated = true;

                var cp = new Task2CalculationParameters
                {
                    d = (double)d,
                    l = (double)l,
                    Q = (double)Q,
                    cAin = (double)cAin,
                    T = (double)T,
                    k01 = (double)k01,
                    ea1 = (double)ea1,
                    k02 = (double)k02,
                    ea2 = (double)ea2,
                    deltaX0 = (double)deltaX0,
                    ku = (double)ku,
                    eMax = (double)eMax,
                    qMax = (double)qMax
                };
                var math = new Task2MathClass(cp);
                var res = math.Calculate();
                if (res.ValidationResults != null && res.ValidationResults.Any())
                {
                    var sb = new StringBuilder();
                    foreach (var validationResult in res.ValidationResults)
                    {
                        sb.Append($"{validationResult.ErrorMessage}\n");
                    }

                    MessageBox.Error(sb.ToString(), "Ошибка расчета!");
                }
                else
                {
                    Result = res.Result;
                    MessageBox.Info(Result.ToString());
                }
            });
        }
    }
}