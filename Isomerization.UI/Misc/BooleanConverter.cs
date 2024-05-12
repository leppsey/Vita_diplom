using System.Globalization;
using System.Windows.Data;

namespace Isomerization.UI.Misc;

public class BooleanConverter<T> : IValueConverter
{
    protected BooleanConverter(T trueValue, T falseValue)
    {
        _true = trueValue;
        _false = falseValue;
    }

    private readonly T _true;
    private readonly T _false;

    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true ? _true : _false;
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is T && EqualityComparer<T>.Default.Equals((T) value, _true);
    }
}