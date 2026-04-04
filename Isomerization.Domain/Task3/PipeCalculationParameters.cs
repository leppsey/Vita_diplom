namespace Isomerization.Domain.Task3;

public struct PipeCalculationParameters
{
    /// <summary>
    /// Производительность (расход) из ЦИМ-1
    /// </summary>
    public double EF { get; set; }
    
    /// <summary>
    /// Минимальная производительность
    /// </summary>
    public double EFmin { get; set; }
    
    /// <summary>
    /// Максимальная энергоемкость
    /// </summary>
    public double ESmax { get; set; }
    
    /// <summary>
    /// Плотность среды
    /// </summary>
    public double rho { get; set; }
    
    /// <summary>
    /// Диаметр
    /// </summary>
    public double D { get; set; }
    
    /// <summary>
    /// Длина
    /// </summary>
    public double L { get; set; }
    
    /// <summary>
    /// Сумма местных сопротивлений
    /// </summary>
    public double sumZeta { get; set; }
    
    /// <summary>
    /// Коэффициент трения (по умолчанию 0.03)
    /// </summary>
    public double lambda { get; set; }
    
    /// <summary>
    /// КПД насоса (по умолчанию 0.7)
    /// </summary>
    public double eta { get; set; }
    
    /// <summary>
    /// Создает экземпляр с параметрами по умолчанию
    /// </summary>
    public static PipeCalculationParameters CreateDefault()
    {
        return new PipeCalculationParameters
        {
            lambda = 0.03,
            eta = 0.7
        };
    }
}
