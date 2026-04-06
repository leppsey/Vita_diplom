namespace Isomerization.Domain.Models;

public class Pipeline3DTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LineType { get; set; } = string.Empty;
    public string SupportedDN { get; set; } = string.Empty;
    public string RequiredElements { get; set; } = string.Empty;
    public string PreviewPath { get; set; } = string.Empty;
    public string ModelPath { get; set; } = string.Empty;
}
