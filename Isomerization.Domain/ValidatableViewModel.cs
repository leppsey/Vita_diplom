using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

using FluentValidation;


namespace DAL;

public abstract class ValidatableViewModel<ValidatorType>: INotifyDataErrorInfo where ValidatorType: IValidator, new()
{
    [NotMapped]
    public ValidatorType Validator { get; set; } = new ValidatorType();
    public IEnumerable GetErrors(string propertyName)
    {
        //честно говоря, я немного удивлен что это так работает, кстати очередной пример слабости дженериков в .net,
        //если бы можно было писать T<> в секции where, то здесь бы я написал ValidatorType: AbstractValidator<> вместо ValidatorType: IValidator
        var errors = Validator
                     .Validate(new ValidationContext<ValidatableViewModel<ValidatorType>>(this))
                     .GetErrors(propertyName);

        return errors;
    }

    public bool HasErrors
    {
        get
        {
            var res = Validator.Validate(new ValidationContext<ValidatableViewModel<ValidatorType>>(this));

            return !res.IsValid;
        }
    }

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public string AllErrors
    {
        get
        {
            var sb = new StringBuilder();

            foreach (var error in Validator.Validate(new ValidationContext<ValidatableViewModel<ValidatorType>>(this)).Errors)
            {
                sb.AppendLine(error.ErrorMessage);
            }

            return sb.ToString();
        }
    }
}
