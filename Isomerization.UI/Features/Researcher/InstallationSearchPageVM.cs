using System.Collections.ObjectModel;
using System.Windows;
using Isomerization.Domain.Data;
using Isomerization.Domain.Models;
using Isomerization.Shared;
using Isomerization.UI.Services;
using Microsoft.EntityFrameworkCore;
using Wpf.Ui;

namespace Isomerization.UI.Features.Researcher;

public class InstallationSearchPageVM: ViewModelBase
{
    private readonly IsomerizationContext _context;
    private readonly INavigationService _navigationService;
    private readonly IMessageBoxService _messageBoxService;
    private readonly IMenuService _menuService;


    public InstallationSearchPageVM(IsomerizationContext context, INavigationService navigationService, IMessageBoxService messageBoxService, IMenuService menuService)
    {
        _context = context;
        _navigationService = navigationService;
        _messageBoxService = messageBoxService;
        _menuService = menuService;
        FindInstallations();
    }

    private RelayCommand _goHomeMenu;
    public RelayCommand GoHomeMenu => _goHomeMenu ??= new RelayCommand(_ =>
    {
        _menuService.GoHome();
    });
    private RelayCommand _goLoginMenu;
    public RelayCommand GoLoginMenu => _goLoginMenu ??= new RelayCommand(_ =>
    {
        _menuService.GoLogin();
    });
    
    public double OctaneNumberMin { get; set; } = 78;
    
    private double? _performanceMin;
    public double? PerformanceMin
    {
        get => _performanceMin;
        set
        {
            _performanceMin = value;
            FindInstallations();
        }
    }
    
    private double? _performanceMax;
    public double? PerformanceMax
    {
        get => _performanceMax;
        set
        {
            _performanceMax = value;
            FindInstallations();
        }
    }
    
    private double? _energyConsumptionMin;
    public double? EnergyConsumptionMin
    {
        get => _energyConsumptionMin;
        set
        {
            _energyConsumptionMin = value;
            FindInstallations();
        }
    }
    
    private double? _energyConsumptionMax;
    public double? EnergyConsumptionMax
    {
        get => _energyConsumptionMax;
        set
        {
            _energyConsumptionMax = value;
            FindInstallations();
        }
    }

    
    public ObservableCollection<Installation> FindedInstallations { get; set; } = new();

    private void FindInstallations()
    {
        try
        {
            var installations = _context.Installations.Include(x=>x.Model).AsQueryable();
            if (EnergyConsumptionMin.HasValue)
            {
                installations = installations.Where(x => x.EnergyConsumption >= EnergyConsumptionMin.Value);
            }
            if (EnergyConsumptionMax.HasValue)
            {
                installations = installations.Where(x => x.EnergyConsumption <= EnergyConsumptionMax.Value);
            }
            if (PerformanceMin.HasValue)
            {
                installations = installations.Where(x => x.Performance >= PerformanceMin.Value);
            }
            if (PerformanceMax.HasValue)
            {
                installations = installations.Where(x => x.Performance <= PerformanceMax.Value);
            }
            FindedInstallations = new ObservableCollection<Installation>(installations);
        }
        catch
        {
            FindedInstallations = new ObservableCollection<Installation>();
        }
        
    }
    
    private RelayCommand _calcCommand;
    public RelayCommand CalcCommand => _calcCommand ??= new RelayCommand(_ =>
    {
        if (!FindedInstallations.Any())
        {
            _messageBoxService.Show("Нельзя перейти к расчету, оборудование не найдено!", "Ошибка!",
                MessageBoxButton.OK);
            return;
        }

        var vm = App.GetService<ResearcherPageVM>();
        vm.AvailableInstallations = FindedInstallations;
        vm.OctaneNumberMin = OctaneNumberMin; 
        // vm.SelectedInstallation = FindedInstallations.First();
        _navigationService.Navigate(typeof(ResearcherPage), vm);
    });

}