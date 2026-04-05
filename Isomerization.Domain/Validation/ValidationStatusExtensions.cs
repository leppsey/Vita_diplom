namespace Isomerization.Domain.Validation;

public static class ValidationStatusExtensions
{
    /// <summary>Краткая подпись статуса для отчётов и UI.</summary>
    public static string ToRuLabel(this ValidationStatus status) =>
        status switch
        {
            ValidationStatus.Valid => "выполнено",
            ValidationStatus.Warning => "предупреждение",
            ValidationStatus.Invalid => "не выполнено",
            _ => status.ToString()
        };

    /// <summary>С заглавной буквы (для начала строки).</summary>
    public static string ToRuLabelCapitalized(this ValidationStatus status)
    {
        var s = status.ToRuLabel();
        return s.Length == 0 ? s : char.ToUpperInvariant(s[0]) + s[1..];
    }
}
