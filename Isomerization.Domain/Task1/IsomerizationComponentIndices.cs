namespace Isomerization.Domain.Task1;

/// <summary>Индексы ключевых компонентов в массивах концентраций (0-based, как в <see cref="CalculationParameters.C0"/>).</summary>
public static class IsomerizationComponentIndices
{
    /// <summary>Изопентан при 7 компонентах: «вещество 5» в UI (C5 в результатах).</summary>
    public const int IsopentaneIndex7 = 4;

    /// <summary>При 4 компонентах условно берём компонент с индексом 2 как целевой для оценки степени изомеризации.</summary>
    public const int TargetIsomerIndex4 = 2;
}
