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
                    d = d ?? 0,
                    l = l ?? 0,
                    Q = Q ?? 0,
                    cAin = cAin ?? 0,
                    T = T ?? 0,
                    k01 = k01 ?? 0,
                    ea1 = ea1 ?? 0,
                    k02 = k02 ?? 0,
                    ea2 = ea2 ?? 0,
                    deltaX0 = deltaX0 ?? 0,
                    ku = ku ?? 0,
                    eMax = eMax ?? 0,
                    qMax = qMax ?? 0
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