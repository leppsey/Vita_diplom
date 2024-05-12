namespace Isomerization.Domain.Models;

/// <summary>
/// Пользователь
/// </summary>
public class User
{
    public int UserId { get; set; }
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Пароль
    /// </summary>
    // todo to hashed
    public string Password { get; set; }
    /// <summary>
    /// Логин
    /// </summary>
    public string Login { get; set; }
    /// <summary>
    /// Роль
    /// </summary>
    public virtual UserRole UserRole{ get; set; }
    public int UserRoleId{ get; set; }
}