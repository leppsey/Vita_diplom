using FluentValidation;
namespace Isomerization.Domain.Models;

public class PipelineValidator: AbstractValidator<Pipeline>
{
    public PipelineValidator()
    {
        RuleFor(c=>c.DateOfCommissioning)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        
        RuleFor(c=>c.Length)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Length)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.Width)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Width)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.Diameter)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Diameter)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.Material)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        
    }
    
}