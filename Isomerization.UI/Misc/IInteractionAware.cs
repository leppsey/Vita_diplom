using System;


namespace UI.Services.MyDialogService;

public interface IInteractionAware
{
    Action FinishInteraction { get; set; }
}
