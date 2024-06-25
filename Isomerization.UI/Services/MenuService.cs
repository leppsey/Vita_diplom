using Isomerization.UI.Features;
using Isomerization.UI.Features.Admin;
using Isomerization.UI.Features.Researcher;
using Wpf.Ui;

namespace Isomerization.UI.Services;

public class MenuService : IMenuService
{
    private readonly INavigationService _navigationService;
    private readonly IUserService _userService;

    public MenuService(INavigationService navigationService, IUserService userService)
    {
        _navigationService = navigationService;
        _userService = userService;
    }

    public void GoHome()
    {
        if (_userService.CurrentUser is null)
        {
            return;
        }
        App.GetService<MainWindowVM>().IsMenuEnabled = true;
        if (_userService.CurrentUser.IsAdmin())
        {
            _navigationService.Navigate(typeof(AllItemsEditPage));
        }
        else if(_userService.CurrentUser.IsResearcher())
        {
            _navigationService.Navigate(typeof(ResearcherPage));
            App.GetService<MainWindowVM>().IsMenuEnabled = false;
        }
    }

    public void GoLogin()
    {
        _userService.CurrentUser = null;
        _navigationService.Navigate(typeof(LoginPage));
    }
}