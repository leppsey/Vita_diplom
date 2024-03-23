using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PlenkaWpf.VM;

namespace PlenkaWpf.View;

public partial class ResearcherControlTask1 : UserControl
{
    public ResearcherControlTask1()
    {
        InitializeComponent();
        var vm = new ResearcherControlTask1VM();
        DataContext = vm;
    }
    private void Button_Click(object sender, RoutedEventArgs e) // нарушение mvvm
    {
        if (!IsValid(MainGrid))
            MessageBox.Show("Невозможно произвести расчёт, есть ошибки ввода данных", "Ошибка!", MessageBoxButton.OK,
                MessageBoxImage.Error);
        else
            (DataContext as ResearcherControlTask1VM).CalcCommand.Execute(null);
    }
    private bool IsValid(DependencyObject obj)
    {
        // The dependency object is valid if it has no errors and all
        // of its children (that are dependency objects) are error-free.
        return !Validation.GetHasError(obj) &&
               LogicalTreeHelper.GetChildren(obj)
                   .OfType<DependencyObject>()
                   .All(IsValid);
    }
}