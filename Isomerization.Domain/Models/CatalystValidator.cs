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
        
        RuleFor(c=>c.Activity)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым")
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.LoadingRate)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым")
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        RuleFor(c=>c.TemperatureReaction)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым")
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");
        
        // RuleFor(c=>c.OperatingTime)
        //     .NotEmpty()
        //     .WithMessage("Поле не должно быть пустым")
        //     .GreaterThan(0)
        //     .WithMessage("Значение не может быть меньше нуля");
        //
        RuleFor(c=>c.ServiceLife)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым")
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");

        RuleFor(c => c.DateOfDecommissioning)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");

        RuleFor(c => c.DateOfCommissioning)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
    }
}