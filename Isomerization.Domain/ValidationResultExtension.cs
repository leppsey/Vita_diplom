using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;


namespace DAL;

public static class ValidationResultExtension
{
    public static bool HasError(this ValidationResult result, string propertyName)
    {
        return result.Errors.Any(x => x.PropertyName == propertyName);
    }

    public static IEnumerable<ValidationFailure> GetErrors(this ValidationResult result, string propertyName)
    {
        return result.Errors.Where(e => e.PropertyName == propertyName);
    }
}