using System.Windows.Controls;
using System.Windows.Media;

namespace Isomerization.UI.Services;

/// <summary>
/// Для многострочного текста с выравниванием через пробелы нужен моноширинный шрифт,
/// иначе в пропорциональном шрифте пробел уже букв и колонки «плывут».
/// </summary>
internal static class MultilineDialogTypography
{
    /// <summary>Consolas: кириллица и латиница одной ширины на Windows.</summary>
    internal static readonly FontFamily MonospaceBody = new("Consolas");

    internal static void ApplyMonospaceIfMultiline(TextBlock block, string? text)
    {
        if (text != null && text.IndexOf('\n') >= 0)
            block.FontFamily = MonospaceBody;
    }
}
