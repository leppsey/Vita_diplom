using System.Windows;

namespace Isomerization.UI.Misc;

public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
{
    public BooleanToVisibilityConverter() : 
        base(Visibility.Visible, Visibility.Collapsed) {}
}
public sealed class InvertedBooleanToVisibilityConverter : BooleanConverter<Visibility>
{
    public InvertedBooleanToVisibilityConverter() : 
        base(Visibility.Collapsed, Visibility.Visible) {}
}