using System.Collections.Generic;

namespace Isomerization.Domain.Cim2;

public class Cim2Result
{
    public PipelineLine Line { get; set; } = new();
    public string TemplateName { get; set; } = string.Empty;
    public string Template3DPath { get; set; } = string.Empty;
    public List<string> Recommendations { get; set; } = new();
}
