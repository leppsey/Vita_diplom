using FluentValidation;
namespace Isomerization.Domain.Models;

public class RawMaterialValidator: AbstractValidator<RawMaterial>
{
    public RawMaterialValidator()
    {
        RuleFor(c=>c.Name)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым");
        
        // RuleFor(c=>c.Compound)
        //     .NotEmpty()
        //     .WithMessage("Поле не должно быть пустым")
        //     .GreaterThan(0)
        //     .WithMessage("Значение не может быть меньше нуля");

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
        
        RuleFor(c=>c.OctaneRating)
            .NotEmpty()
            .WithMessage("Поле не должно быть пустым")
            .GreaterThan(0)
            .WithMessage("Значение не может быть меньше нуля");

        RuleFor(x => x.C1)
            .Must((material, arg2, arg3) => material.C1 + material.C2 + material.C3 + material.C4 + material.C5 + material.C6 + material.C7 == 100)
            .WithMessage(material => $"Сумма концетраций должна быть равна 100, текущая сумма {material.C1 + material.C2 + material.C3 + material.C4 + material.C5 + material.C6 + material.C7}");
        RuleFor(x => x.C2)
            .Must((material, arg2, arg3) => material.C1 + material.C2 + material.C3 + material.C4 + material.C5 + material.C6 + material.C7 == 100)
            .WithMessage(material => $"Сумма концетраций должна быть равна 100, текущая сумма {material.C1 + material.C2 + material.C3 + material.C4 + material.C5 + material.C6 + material.C7}");
        RuleFor(x => x.C3)
            .Must((material, arg2, arg3) => material.C1 + material.C2 + material.C3 + material.C4 + material.C5 + material.C6 + material.C7 == 100)
            .WithMessage(material => $"Сумма концетраций должна быть равна 100, текущая сумма {material.C1 + material.C2 + material.C3 + material.C4 + material.C5 + material.C6 + material.C7}");
        RuleFor(x => x.C4)
            .Must((material, arg2, arg3) => material.C1 + material.C2 + material.C3 + material.C4 + material.C5 + material.C6 + material.C7 == 100)
            .WithMessage(material => $"Сумма концетраций должна быть равна 100, текущая сумма {material.C1 + material.C2 + material.C3 + material.C4 + material.C5 + material.C6 + material.C7}");
        RuleFor(x => x.C5)
            .Must((material, arg2, arg3) => material.C1 + material.C2 + material.C3 + material.C4 + material.C5 + material.C6 + material.C7 == 100)
            .WithMessage(material => $"Сумма концетраций должна быть равна 100, текущая сумма {material.C1 + material.C2 + material.C3 + material.C4 + material.C5 + material.C6 + material.C7}");
        RuleFor(x => x.C6)
            .Must((material, arg2, arg3) => material.C1 + material.C2 + material.C3 + material.C4 + material.C5 + material.C6 + material.C7 == 100)
            .WithMessage(material => $"Сумма концетраций должна быть равна 100, текущая сумма {material.C1 + material.C2 + material.C3 + material.C4 + material.C5 + material.C6 + material.C7}");
        RuleFor(x => x.C7)
            .Must((material, arg2, arg3) => material.C1 + material.C2 + material.C3 + material.C4 + material.C5 + material.C6 + material.C7 == 100)
            .WithMessage(material => $"Сумма концетраций должна быть равна 100, текущая сумма {material.C1 + material.C2 + material.C3 + material.C4 + material.C5 + material.C6 + material.C7}");

    }
    
}