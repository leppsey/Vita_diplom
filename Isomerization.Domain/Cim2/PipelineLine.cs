using System.Collections.Generic;

namespace Isomerization.Domain.Cim2;

public class PipelineLine
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PipelineLineType LineType { get; set; }
    public List<PipelineElement> Elements { get; set; } = new();
    public PipelineCalculationResult CalculationResult { get; set; } = new();
    public PipelineValidationResult ValidationResult { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public string Selected3DTemplate { get; set; } = string.Empty;
}
