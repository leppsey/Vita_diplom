namespace Isomerization.UI.Misc;

public interface IInteractionAware
{
    Action FinishInteraction { get; set; }
}