using Isomerization.Domain.Models;

namespace Isomerization.UI.Services;

public interface IUserService
{
    public User CurrentUser { get; set; }
}

public static class UserExtensions
{
    public static bool IsAdmin(this User user)
    {
        return user.UserRole.Name.Equals(UserRoles.Admin, StringComparison.InvariantCultureIgnoreCase);
    }
    
    public static bool IsResearcher(this User user)
    {
        return user.UserRole.Name.Equals(UserRoles.Researcher, StringComparison.InvariantCultureIgnoreCase);
    }
}