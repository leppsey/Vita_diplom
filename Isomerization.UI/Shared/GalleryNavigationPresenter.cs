using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui;
using Wpf.Ui.Input;

namespace Isomerization.UI.Shared;

public class GalleryNavigationPresenter : Control
{
    /// <summary>
    ///     Property for <see cref="ItemsSource" />.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource),
                                    typeof(object), typeof(GalleryNavigationPresenter), new PropertyMetadata(null));

    /// <summary>
    ///     Property for <see cref="TemplateButtonCommand" />.
    /// </summary>
    public static readonly DependencyProperty TemplateButtonCommandProperty =
        DependencyProperty.Register(nameof(TemplateButtonCommand),
                                    typeof(IRelayCommand), typeof(GalleryNavigationPresenter), new PropertyMetadata(null));

    /// <summary>
    ///     Creates a new instance of the class and sets the default <see cref="FrameworkElement.Loaded" /> event.
    /// </summary>
    public GalleryNavigationPresenter()
    {
        SetValue(TemplateButtonCommandProperty, new RelayCommand<string>(o => OnTemplateButtonClick(o ?? string.Empty)));
    }

    public object ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    ///     Command triggered after clicking the titlebar button.
    /// </summary>
    public IRelayCommand TemplateButtonCommand => (IRelayCommand) GetValue(TemplateButtonCommandProperty);

    private void OnTemplateButtonClick(string parameter)
    {
        var navigationService = App.GetService<INavigationService>();

        if (navigationService == null)
        {
            return;
        }

        var pageType = NameToPageTypeConverter.Convert(parameter);

        if (pageType != null)
        {
            navigationService.Navigate(pageType);
        }
    }
}


internal class NameToPageTypeConverter
{
    private static readonly Type[] PageTypes = Assembly
                                               .GetExecutingAssembly()
                                               .GetTypes()
                                               .Where(t => t.Namespace?.ToLower().StartsWith("isomerization.ui") ?? false)
                                               .ToArray();

    public static Type? Convert(string pageName)
    {
        pageName = pageName.Trim().ToLower();

        return PageTypes.FirstOrDefault(singlePageType => singlePageType.Name.ToLower() == pageName);
    }
}
