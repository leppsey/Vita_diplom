using FluentValidation;

namespace Isomerization.Domain.Models;

public class CatalystValidator: AbstractValidator<Catalyst>
{
    public CatalystValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Type)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
    }
}