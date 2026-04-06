namespace Isomerization.Domain.Models;

public class PipelineTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LineType { get; set; } = string.Empty;
    public bool HasPump { get; set; }
    public bool HasReducer { get; set; }
    public bool HasElbow { get; set; }
    public bool HasValve { get; set; }
    public bool HasFilter { get; set; }
    public string SupportedDN { get; set; } = string.Empty;
    public string Template3DPath { get; set; } = string.Empty;
}
