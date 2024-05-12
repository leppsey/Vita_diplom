using System.Windows;
using System.Windows.Controls;

namespace Isomerization.UI.Shared;

public enum HeaderTextPlacement
{
    Top,
    Left
}
public class LabeledControlPresenter : ContentControl
{
    /// <summary>
    ///     Property for <see cref="HeaderText" />.
    /// </summary>
    public static readonly DependencyProperty HeaderTextProperty =
        DependencyProperty.Register(nameof(HeaderText),
                                    typeof(string), typeof(LabeledControlPresenter), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty HeaderFontSizeProperty =
        DependencyProperty.Register(nameof(HeaderFontSize), typeof(double), typeof(LabeledControlPresenter), new PropertyMetadata(14d));

    public static readonly DependencyProperty HeaderTextPlacementProperty = DependencyProperty.Register(
        nameof(HeaderTextPlacement), typeof(HeaderTextPlacement), typeof(LabeledControlPresenter), new PropertyMetadata(HeaderTextPlacement.Top));

    public static readonly DependencyProperty RightTextProperty = DependencyProperty.Register(
        nameof(RightText), typeof(string), typeof(LabeledControlPresenter), new PropertyMetadata(string.Empty));
    
    public static readonly DependencyProperty RightTextFontSizeProperty = DependencyProperty.Register(
        nameof(RightTextFontSize), typeof(double), typeof(LabeledControlPresenter), new PropertyMetadata(14d));

    public string RightText
    {
        get { return (string)GetValue(RightTextProperty); }
        set { SetValue(RightTextProperty, value); }
    }
    public HeaderTextPlacement HeaderTextPlacement
    {
        get { return (HeaderTextPlacement)GetValue(HeaderTextPlacementProperty); }
        set { SetValue(HeaderTextPlacementProperty, value); }
    }
    public string HeaderText
    {
        get => (string) GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    public double HeaderFontSize
    {
        get => (double) GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }


    public double RightTextFontSize
    {
        get { return (double)GetValue(RightTextFontSizeProperty); }
        set { SetValue(RightTextFontSizeProperty, value); }
    }

}
