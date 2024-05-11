using FluentValidation;
namespace Isomerization.Domain.Models;

public class UserValidator: AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(c=>c.Name)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        
        RuleFor(c=>c.Password)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");

        RuleFor(c=>c.Login)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
    }
}