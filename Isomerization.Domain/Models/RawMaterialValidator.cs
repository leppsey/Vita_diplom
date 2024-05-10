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
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Consumption)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.Compound)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Compound)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");

        RuleFor(c=>c.Density)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Density)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");

        RuleFor(c=>c.HeatCapacity)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.HeatCapacity)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.Viscosity)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Viscosity)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.SulfurContent)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.SulfurContent)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.OctaneRating)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.OctaneRating)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");


    }
    
}