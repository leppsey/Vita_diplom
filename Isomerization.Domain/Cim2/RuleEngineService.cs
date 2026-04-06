using System;
using System.Globalization;
using System.Linq;
using Isomerization.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Isomerization.Domain.Cim2;

public class RuleEngineService
{
    private readonly IsomerizationContext _context;
    private readonly PipelineDiameterSelectionService _diameterSelectionService;

    public RuleEngineService(IsomerizationContext context, PipelineDiameterSelectionService diameterSelectionService)
    {
        _context = context;
        _diameterSelectionService = diameterSelectionService;
    }

    public void ApplyRules(Cim2ExecutionContext executionContext)
    {
        var rules = _context.PipelineRules
            .AsNoTracking()
            .Where(x => x.IsEnabled)
            .OrderBy(x => x.Priority)
            .ToList();

        foreach (var rule in rules)
        {
            if (!Matches(rule, executionContext))
            {
                continue;
            }

            Execute(rule, executionContext);
        }
    }

    private static bool Matches(Models.PipelineRule rule, Cim2ExecutionContext context)
    {
        return rule.ConditionType switch
        {
            "ReactorSelected" => CompareBool(context.Request.Cim1Result.ReactorId > 0, rule.ConditionOperator, rule.ConditionValue),
            "DNDiffers" => CompareBool(context.CalculatedDn != context.SelectedDn, rule.ConditionOperator, rule.ConditionValue),
            "DirectionChange" => CompareBool(context.Request.RequiresDirectionChange, rule.ConditionOperator, rule.ConditionValue),
            "RequiresPump" => CompareBool(context.Request.RequiresPump, rule.ConditionOperator, rule.ConditionValue),
            "Velocity" => CompareDouble(context.Request.AllowedVelocity, rule.ConditionOperator, rule.ConditionValue),
            _ => false
        };
    }

    private void Execute(Models.PipelineRule rule, Cim2ExecutionContext context)
    {
        switch (rule.ActionType)
        {
            case "SetLineType":
                if (Enum.TryParse<PipelineLineType>(rule.ActionValue, true, out var lineType))
                {
                    context.LineType = lineType;
                }
                break;
            case "AddElement":
                if (string.Equals(rule.ActionValue, "Reducer", StringComparison.OrdinalIgnoreCase))
                {
                    context.RequiresReducer = true;
                }
                if (string.Equals(rule.ActionValue, "Elbow", StringComparison.OrdinalIgnoreCase))
                {
                    context.RequiresElbow = true;
                }
                break;
            case "AddPumpAssembly":
                context.RequiresPumpAssembly = true;
                break;
            case "IncreaseDN":
                context.SelectedDn = _diameterSelectionService.IncreaseToNextStandardDn(context.SelectedDn);
                context.RuleRecommendations.Add("Правило: увеличен DN из-за ограничения по скорости.");
                break;
            case "SimplifyTemplate":
                context.SimplifyTemplate = true;
                context.RuleRecommendations.Add("Правило: выбран более простой шаблон для снижения потерь.");
                break;
            case "RecommendAlternative":
                context.RuleRecommendations.Add("Правило: требуется альтернативный DN или шаблон линии по энергопотреблению.");
                break;
        }
    }

    private static bool CompareBool(bool actual, string op, string expectedRaw)
    {
        var expected = expectedRaw.Equals("true", StringComparison.OrdinalIgnoreCase);
        return op switch
        {
            "=" => actual == expected,
            "!=" => actual != expected,
            _ => false
        };
    }

    private static bool CompareDouble(double actual, string op, string expectedRaw)
    {
        if (!double.TryParse(expectedRaw, NumberStyles.Any, CultureInfo.InvariantCulture, out var expected))
        {
            return false;
        }

        return op switch
        {
            ">" => actual > expected,
            ">=" => actual >= expected,
            "<" => actual < expected,
            "<=" => actual <= expected,
            "=" => Math.Abs(actual - expected) < 1e-9,
            _ => false
        };
    }
}
