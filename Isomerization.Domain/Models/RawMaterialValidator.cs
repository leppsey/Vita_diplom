using FluentValidation;
namespace Isomerization.Domain.Models;

public class RawMaterialValidator: AbstractValidator<RawMaterial>
{
    public RawMaterialValidator()
    {
        RuleFor(c=>c.Name)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        
        RuleFor(c=>c.Consumption)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым")
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.Compound)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым")
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");

        RuleFor(c=>c.Density)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым")
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");

        RuleFor(c=>c.HeatCapacity)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым")
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.Viscosity)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым")
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.SulfurContent)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым")
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.OctaneRating)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым")
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");


    }
    
}