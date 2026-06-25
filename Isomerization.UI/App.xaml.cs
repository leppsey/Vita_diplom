using System;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Windows;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Isomerization.Domain;
using Isomerization.Domain.Cim2;
using Isomerization.Domain.Data;
using Isomerization.UI.Features.Researcher;
using Isomerization.UI.Misc;
using Isomerization.UI.Services;
using Microsoft.EntityFrameworkCore;
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
        
        // Явная регистрация Task3ViewModel для избежания проблем с DI
        builder.RegisterType<Features.Task3ViewModel>().AsSelf();
        // Явная регистрация ViewModel ЦИМ-2 (не оканчивается на "VM")
        builder.RegisterType<Cim2PageViewModel>().AsSelf();
        builder.RegisterType<Features.Admin.Cim2.PipelineRulesPageVM>().AsSelf();
        builder.RegisterType<Features.Admin.Cim2.PipelineRuleEditControlVM>().AsSelf();

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
        builder.RegisterType<Cim2SessionService>().As<ICim2SessionService>().SingleInstance();
        builder.RegisterType<EditDialogService>().AsSelf().SingleInstance();
        builder.RegisterType<SelectDIMIsomerizationWindow>().AsSelf();

        builder.RegisterType<PipelineLineTypeResolver>().AsSelf().SingleInstance();
        builder.RegisterType<PipelineDiameterSelectionService>().AsSelf().SingleInstance();
        builder.RegisterType<RuleEngineService>().AsSelf().SingleInstance();
        builder.RegisterType<PipelineRuleSimulationService>().AsSelf().SingleInstance();
        builder.RegisterType<PipelineTemplateSelector>().AsSelf().SingleInstance();
        builder.RegisterType<PipelineElementSelector>().AsSelf().SingleInstance();
        builder.RegisterType<PipelineCalculationService>().AsSelf().SingleInstance();
        builder.RegisterType<PipelineValidationService>().AsSelf().SingleInstance();
        builder.RegisterType<PipelineRecommendationService>().AsSelf().SingleInstance();
        builder.RegisterType<Pipeline3DTemplateSelector>().AsSelf().SingleInstance();
        builder.RegisterType<Cim2OrchestratorService>().AsSelf().SingleInstance();
        #endregion
        
       
        
        builder.RegisterType<MainWindow>().As<INavigationWindow>().AsSelf().SingleInstance();
        builder.RegisterType<MainWindowVM>().AsSelf().SingleInstance();
        
        Container = builder.Build();
        var dbContext = Container.Resolve<IsomerizationContext>();
        var wasCreated = dbContext.Database.EnsureCreated();
        SqliteSchemaUpgrade.Apply(dbContext);
        // dbContext.Database.EnsureDeleted();
        if (wasCreated)
        {
            dbContext.Database.ExecuteSqlRaw("PRAGMA journal_mode = 'delete';");
            DatabaseInitializer.Init(dbContext);

        }
        // Для уже существующей БД: гарантируем, что каталоги и шаблоны ЦИМ-2 заполнены.
        DatabaseInitializer.EnsureCim2Seed(dbContext);
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