using System.Collections.Generic;

namespace Isomerization.Domain.Cim2;

public class PipelineValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Violations { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
