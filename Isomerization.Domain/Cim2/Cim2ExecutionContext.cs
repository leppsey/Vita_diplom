using System.Collections.Generic;

namespace Isomerization.Domain.Cim2;

public class Cim2ExecutionContext
{
    public Cim2Request Request { get; set; } = new();
    public PipelineLineType LineType { get; set; } = PipelineLineType.ReactorInletLine;
    public int CalculatedDn { get; set; }
    public int SelectedDn { get; set; }
    public bool RequiresReducer { get; set; }
    public bool RequiresElbow { get; set; }
    public bool RequiresPumpAssembly { get; set; }
    public bool SimplifyTemplate { get; set; }
    public List<string> RuleRecommendations { get; } = new();
}
