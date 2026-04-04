namespace Isomerization.Domain.Task3;

public record struct PipeCalculationResult
{
    /// <summary>
    /// Скорость
    /// </summary>
    public double v { get; set; }
    
    /// <summary>
    /// Потеря давления на трение
    /// </summary>
    public double dP_fric { get; set; }
    
    /// <summary>
    /// Потеря давления на местные сопротивления
    /// </summary>
    public double dP_local { get; set; }
    
    /// <summary>
    /// Общая потеря давления (ΔPΣ)
    /// </summary>
    public double dP_total { get; set; }
    
    /// <summary>
    /// Энергоемкость
    /// </summary>
    public double ES { get; set; }
    
    /// <summary>
    /// Разница EF - EFmin
    /// </summary>
    public double deltaEF { get; set; }
    
    /// <summary>
    /// Разница ESmax - ES
    /// </summary>
    public double deltaES { get; set; }
    
    /// <summary>
    /// Условие по производительности выполнено
    /// </summary>
    public bool isEFok { get; set; }
    
    /// <summary>
    /// Условие по энергоемкости выполнено
    /// </summary>
    public bool isESok { get; set; }
}
