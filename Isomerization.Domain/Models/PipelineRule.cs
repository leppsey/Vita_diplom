namespace Isomerization.Domain.Models;

public class PipelineRule
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ConditionType { get; set; } = string.Empty;
    public string ConditionOperator { get; set; } = string.Empty;
    public string ConditionValue { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string ActionValue { get; set; } = string.Empty;
    public int Priority { get; set; }
    public bool IsEnabled { get; set; } = true;
}
