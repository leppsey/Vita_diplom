using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Isomerization.Domain;

public class ResultWrapper<T>
{
    private ResultWrapper() { }
    public T? Result { get; set; }
    public List<ValidationResult> ValidationResults { get; set; }

    public static ResultWrapper<T> WithResult(T result)
    {
        return new ResultWrapper<T>()
        {
            Result = result,
        };
    }
    public static ResultWrapper<T> WithError(params string[] messages)
    {
        var validationErrors = messages.Select(message => new ValidationResult(message)).ToList();
        return new ResultWrapper<T>()
        {
            ValidationResults = validationErrors
        };
    }
}
