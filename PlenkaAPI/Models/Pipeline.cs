using System;
using System.Reflection.Metadata.Ecma335;

namespace PlenkaAPI.Models;

/// <summary>
/// Трубопровод
/// </summary>
public class Pipeline
{
    public int PipelineId { get; set; }
    /// <summary>
    /// дата ввода в эксплуатацию
    /// </summary>
    public DateTime DateOfCommissioning { get; set; }
    /// <summary>
    /// Длина
    /// </summary>
    public double Length { get; set; }
    /// <summary>
    /// Высота
    /// </summary>
    public double Width { get; set; }
    /// <summary>
    /// Диаметр
    /// </summary>  
    public double Diameter { get; set; }
    /// <summary>
    /// Материал
    /// </summary>
    public string Material { get; set; }
}