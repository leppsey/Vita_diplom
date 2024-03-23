using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlenkaAPI;

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
    public static ResultWrapper<T> WithError(string message)
    {
        return new ResultWrapper<T>()
        {
            ValidationResults = new List<ValidationResult>()
            {
                new(message)
            }
        };
    }
}
