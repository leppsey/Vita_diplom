using FluentValidation;
namespace Isomerization.Domain.Models;

public class KineticValidator: AbstractValidator<Kinetic>
{
    public KineticValidator()
    {
        RuleFor(c=>c.Reaction)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        
        RuleFor(c=>c.PreExponentialFactor)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым")
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.EnergyActivation)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым")
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.Kineticscol)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
    }
}

