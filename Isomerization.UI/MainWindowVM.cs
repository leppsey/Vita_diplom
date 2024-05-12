using Isomerization.Shared;
using Isomerization.UI.Services;

namespace Isomerization.UI;

public class MainWindowVM: ViewModelBase
{
    private readonly IMenuService _menuService;

    public MainWindowVM(IMenuService menuService)
    {
        _menuService = menuService;
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

    public bool IsMenuEnabled { get; set; }
}