using Wpf.Ui.Controls;

namespace Isomerization.UI.Shared;

public record NavigationCard
{
    public string Name { get; init; }

    public SymbolRegular Icon { get; init; }

    public string Description { get; init; }

    public string Link { get; init; }
}
