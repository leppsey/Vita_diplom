namespace Isomerization.Domain.Models;

/// <summary>
/// Цифровая информационная модель изомеризации
/// </summary>
public class DIMIsomerization
{
    public int DIMIsomerizationId { get; set; }
    public RawMaterial RawMaterial { get; set; }
    public int RawMaterialId { get; set; }
    public User User { get; set; }
    public int UserId { get; set; }
    public Catalyst Catalyst { get; set; }
    public int CatalystId { get; set; }
    public Pipeline Pipeline { get; set; }
    public int PipelineId { get; set; }
    public Installation Installation { get; set; }
    public int InstallationId { get; set; }
}