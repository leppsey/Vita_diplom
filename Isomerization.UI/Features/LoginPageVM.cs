using Isomerization.Domain.Data;
using Isomerization.Domain.Models;
using Isomerization.UI.Features;
using Isomerization.UI.Features.Admin;
using Isomerization.UI.Features.Researcher;
using Isomerization.UI.Services;
using Microsoft.EntityFrameworkCore;
using Wpf.Ui;
using Wpf.Ui.Controls;
using MessageBoxButton = System.Windows.MessageBoxButton;

namespace Isomerization.UI;

public class LoginPageVM : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IUserService _userService;
    private readonly IsomerizationContext _context;
    private readonly IMessageBoxService _messageBoxService;

    public LoginPageVM(INavigationService navigationService, IUserService userService, IsomerizationContext context, IMessageBoxService messageBoxService)
    {
        _navigationService = navigationService;
        _userService = userService;
        _context = context;
        _messageBoxService = messageBoxService;
    }

    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    private RelayCommand _enterAdminCommand;
    public RelayCommand EnterAdminCommand => _enterAdminCommand ??= new RelayCommand(_ =>
    {
        _userService.CurrentUser = new User()
        {
            UserRole = new UserRole() { Name = UserRoles.Admin },
        };
        _navigationService.Navigate(typeof(AllItemsEditPage));
    });

    private RelayCommand _enterResearcherCommand;
    public RelayCommand EnterResearcherCommand => _enterResearcherCommand ??= new RelayCommand(_ =>
    {
        _userService.CurrentUser = new User()
        {
            UserRole = new UserRole() { Name = UserRoles.Researcher },
        };
        _navigationService.Navigate(typeof(InstallationSearchPage));
    });
    
    private RelayCommand _enterCommand;
    public RelayCommand EnterCommand => _enterCommand ??= new RelayCommand(async _ =>
    {
        var user = _context.Users.Include(x => x.UserRole).FirstOrDefault(u =>
            u.Login.ToLower() == Login.ToLower()
            && u.Password == Password);
        if (user is null)
        {
            await _messageBoxService.Show("Пользователь с таким именем и паролем не найден, попробуйте снова", "Ошибка",
                MessageBoxButton.OK);
            return;
        }

        switch (user.UserRole.Name)
        {
            case UserRoles.Researcher:
                _userService.CurrentUser = user;
                _navigationService.Navigate(typeof(InstallationSearchPage));
                break;
            case UserRoles.Admin:
                _userService.CurrentUser = user;
                _navigationService.Navigate(typeof(AllItemsEditPage));
                break;
            default:
                await _messageBoxService.Show("У пользователя нет прав на использование этой программы",
                    "Ошибка",
                    MessageBoxButton.OK);
                return;
        }
    });
    
}