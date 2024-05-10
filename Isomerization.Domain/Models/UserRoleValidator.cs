using FluentValidation;
namespace Isomerization.Domain.Models;

public class UserRoleValidator: AbstractValidator<UserRole>
{
    public UserRoleValidator()
    {
        RuleFor(c=>c.Name)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
    }
    
}