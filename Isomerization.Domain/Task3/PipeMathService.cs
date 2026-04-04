using System;
using System.Collections.Generic;

namespace Isomerization.Domain.Task3;

/// <summary>
/// Сервис для расчета трубопроводной системы
/// </summary>
public class PipeMathService
{
    /// <summary>
    /// Выполняет расчет трубопроводной системы
    /// </summary>
    /// <param name="p">Параметры расчета</param>
    /// <returns>Результаты расчета</returns>
    /// <exception cref="ArgumentException">При невалидных входных данных</exception>
    public static PipeCalculationResult Calculate(PipeCalculationParameters p)
    {
        ValidateParameters(p);
        
        // Q = EF / rho (объемный расход, м³/с)
        var Q = p.EF / p.rho;
        
        // v = 4*Q / (π*D^2) (скорость, м/с)
        var v = 4 * Q / (Math.PI * p.D * p.D);
        
        // dP_fric = lambda*(L/D)*(rho*v^2/2) (потеря давления на трение, Па)
        var dP_fric = p.lambda * (p.L / p.D) * (p.rho * v * v / 2);
        
        // dP_local = sumZeta*(rho*v^2/2) (потеря давления на местные сопротивления, Па)
        var dP_local = p.sumZeta * (p.rho * v * v / 2);
        
        // dP_total = dP_fric + dP_local (общая потеря давления, Па)
        var dP_total = dP_fric + dP_local;
        
        // ES = (Q * dP_total) / eta (энергоемкость, Вт)
        var ES = (Q * dP_total) / p.eta;
        
        // deltaEF = EF - EFmin
        var deltaEF = p.EF - p.EFmin;
        
        // deltaES = ESmax - ES
        var deltaES = p.ESmax - ES;
        
        // isEFok = EF >= EFmin
        var isEFok = p.EF >= p.EFmin;
        
        // isESok = ES <= ESmax
        var isESok = ES <= p.ESmax;
        
        return new PipeCalculationResult
        {
            v = v,
            dP_fric = dP_fric,
            dP_local = dP_local,
            dP_total = dP_total,
            ES = ES,
            deltaEF = deltaEF,
            deltaES = deltaES,
            isEFok = isEFok,
            isESok = isESok
        };
    }
    
    /// <summary>
    /// Выполняет перебор диаметров и расчет вариантов
    /// </summary>
    /// <param name="pBase">Базовые параметры (EF, rho, L, sumZeta, lambda, eta, EFmin, ESmax)</param>
    /// <param name="Dmin">Минимальный диаметр, м</param>
    /// <param name="Dmax">Максимальный диаметр, м</param>
    /// <param name="steps">Количество шагов (должно быть >= 2)</param>
    /// <returns>Список вариантов расчетов</returns>
    /// <exception cref="ArgumentException">При невалидных входных данных</exception>
    public static List<PipeVariantRow> SweepByDiameter(
        PipeCalculationParameters pBase, 
        double Dmin, 
        double Dmax, 
        int steps)
    {
        if (Dmin <= 0)
        {
            throw new ArgumentException("Dmin должен быть больше 0", nameof(Dmin));
        }
        
        if (Dmax <= 0)
        {
            throw new ArgumentException("Dmax должен быть больше 0", nameof(Dmax));
        }
        
        if (Dmin >= Dmax)
        {
            throw new ArgumentException("Dmin должен быть меньше Dmax", nameof(Dmin));
        }
        
        if (steps < 2)
        {
            throw new ArgumentException("steps должен быть >= 2", nameof(steps));
        }
        
        // Проверяем базовые параметры (без D, т.к. он будет меняться)
        if (pBase.rho <= 0)
        {
            throw new ArgumentException("rho должен быть больше 0", nameof(pBase));
        }
        
        if (pBase.L < 0)
        {
            throw new ArgumentException("L должен быть >= 0", nameof(pBase));
        }
        
        if (pBase.eta <= 0 || pBase.eta > 1)
        {
            throw new ArgumentException("eta должен быть в интервале (0, 1]", nameof(pBase));
        }
        
        var results = new List<PipeVariantRow>();
        
        // Равномерное распределение диаметров от Dmin до Dmax
        var stepSize = (Dmax - Dmin) / (steps - 1);
        
        for (int i = 0; i < steps; i++)
        {
            var D = Dmin + i * stepSize;
            
            // Создаем параметры с текущим диаметром
            var p = pBase;
            p.D = D;
            
            // Выполняем расчет
            var result = Calculate(p);
            
            // Добавляем вариант в список
            results.Add(new PipeVariantRow
            {
                D = D,
                v = result.v,
                dP_total = result.dP_total,
                ES = result.ES,
                isESok = result.isESok
            });
        }
        
        return results;
    }
    
    /// <summary>
    /// Валидация параметров расчета
    /// </summary>
    private static void ValidateParameters(PipeCalculationParameters p)
    {
        if (p.D <= 0)
        {
            throw new ArgumentException("D должен быть больше 0", nameof(p));
        }
        
        if (p.L < 0)
        {
            throw new ArgumentException("L должен быть >= 0", nameof(p));
        }
        
        if (p.rho <= 0)
        {
            throw new ArgumentException("rho должен быть больше 0", nameof(p));
        }
        
        if (p.eta <= 0 || p.eta > 1)
        {
            throw new ArgumentException("eta должен быть в интервале (0, 1]", nameof(p));
        }
    }
}
