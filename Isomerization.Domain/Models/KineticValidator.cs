using FluentValidation;
namespace Isomerization.Domain.Models;

public class KineticValidator: AbstractValidator<Kinetic>
{
    public KineticValidator()
    {
        RuleFor(c=>c.PreExponentialFactor)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым")
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.StehiometricFactor)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым")
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.ReactionRateConstant)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым")
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c => c.RawMaterial)
            .NotNull()
            .WithMessage("Поле не должно быть пустым");
    }
}

