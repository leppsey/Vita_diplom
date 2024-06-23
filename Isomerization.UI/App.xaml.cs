using System;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Windows;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Isomerization.Domain;
using Isomerization.Domain.Data;
using Isomerization.UI.Features.Researcher;
using Isomerization.UI.Misc;
using Isomerization.UI.Services;
using Wpf.Ui;

namespace Isomerization.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IContainer Container { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var builder = new ContainerBuilder();

        
        #region VM And Views

        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Name.EndsWith("Page"))
            .AsSelf();
        
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Name.EndsWith("Control"))
            .AsSelf();

        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Name.EndsWith("VM"))
            .AsSelf();

        #endregion
        
        #region Service Registration

        builder.RegisterType<UserService>().As<IUserService>().SingleInstance();
        builder.RegisterType<PageService>().As<IPageService>().SingleInstance();
        builder.RegisterType<MenuService>().As<IMenuService>().SingleInstance();
        builder.RegisterType<ContentDialogService>().As<IContentDialogService>().SingleInstance();
        builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();
        builder.RegisterType<SnackbarService>().As<ISnackbarService>().SingleInstance();
        
        builder.RegisterType<AutofacServiceProvider>().As<IServiceProvider>().SingleInstance();

        builder.RegisterInstance(new IsomerizationContext()).SingleInstance();
        builder.RegisterType<MessageBoxService>().As<IMessageBoxService>().SingleInstance();
        builder.RegisterType<ContentMessageBoxService>().As<IContentMessageBoxService>().SingleInstance();
        builder.RegisterType<EditDialogService>().AsSelf().SingleInstance();
        builder.RegisterType<SelectDIMIsomerizationWindow>().AsSelf();
        #endregion
        
       
        
        builder.RegisterType<MainWindow>().As<INavigationWindow>().AsSelf().SingleInstance();
        builder.RegisterType<MainWindowVM>().AsSelf().SingleInstance();
        
        Container = builder.Build();
        var dbContext = Container.Resolve<IsomerizationContext>();
        // dbContext.Database.EnsureDeleted();
        if (dbContext.Database.EnsureCreated())
        {
            DatabaseInitializer.Init(dbContext);

        }
        var navWindow = Container.Resolve<INavigationWindow>();
        navWindow.ShowWindow();

        var nav = Container.Resolve<INavigationService>();
        nav.Navigate(typeof(LoginPage));
    }
    
    public static T GetService<T>()
    {
        return Container.Resolve<T>();
    }    
}