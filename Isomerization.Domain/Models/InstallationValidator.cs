using FluentValidation;
namespace Isomerization.Domain.Models;

public class InstallationValidator: AbstractValidator<Installation>
{
    public InstallationValidator()
    {
        RuleFor(c=>c.Name)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Type)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Pressure)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Pressure)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.Performance)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Performance)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");

        RuleFor(c=>c.EnergyConsumption)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.EnergyConsumption)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");

        RuleFor(c=>c.Temperature)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Temperature)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");

        RuleFor(c=>c.Height)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Height)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");

        RuleFor(c=>c.Width)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Width)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");

        RuleFor(c=>c.Length)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Length)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");

        RuleFor(c=>c.Diameter)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Diameter)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");

        RuleFor(c=>c.Volume)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.Volume)
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");

        RuleFor(c=>c.DateOfCommissioning)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        RuleFor(c=>c.DateOfPlannedWorks)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        // todo DateOfPlannedWorks > DateOfCommissioning проверять обязательно
        RuleFor(c=>c.Status)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        

        
    }
    
}