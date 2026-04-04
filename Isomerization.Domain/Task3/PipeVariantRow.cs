namespace Isomerization.Domain.Task3;

/// <summary>
/// Вариант расчета для перебора по диаметрам
/// </summary>
public struct PipeVariantRow
{
    /// <summary>
    /// Диаметр
    /// </summary>
    public double D { get; set; }
    
    /// <summary>
    /// Скорость
    /// </summary>
    public double v { get; set; }
    
    /// <summary>
    /// Общая потеря давления
    /// </summary>
    public double dP_total { get; set; }
    
    /// <summary>
    /// Энергоемкость
    /// </summary>
    public double ES { get; set; }
    
    /// <summary>
    /// Условие по энергоемкости выполнено
    /// </summary>
    public bool isESok { get; set; }
}
