using System.Windows;
using Autofac;
using Wpf.Ui;

namespace Isomerization.UI;

public class PageService : IPageService
{
    private readonly IComponentContext _container;

    public PageService(IComponentContext container)
    {
        _container = container;
    }

    public T GetPage<T>() where T : class
    {
        return _container.Resolve<T>();
    }

    public FrameworkElement GetPage(Type pageType)
    {
        return _container.Resolve(pageType) as FrameworkElement;
    }
}