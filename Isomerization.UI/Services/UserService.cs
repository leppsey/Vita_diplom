using Isomerization.Domain.Models;

namespace Isomerization.UI.Services;

public class UserService : IUserService
{
    private User _currentUser;


    public User CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            // костыль, мне лень придумывать что-то лучше
            App.GetService<MainWindowVM>().IsMenuEnabled = _currentUser is not null;
        }
    }
}