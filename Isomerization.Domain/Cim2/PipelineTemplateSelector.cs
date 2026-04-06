using System;
using System.Linq;
using Isomerization.Domain.Data;
using Isomerization.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Isomerization.Domain.Cim2;

public class PipelineTemplateSelector
{
    private readonly IsomerizationContext _context;

    public PipelineTemplateSelector(IsomerizationContext context)
    {
        _context = context;
    }

    public PipelineTemplate Select(PipelineLineType lineType, int dn, bool simplifyTemplate)
    {
        var lineTypeText = lineType.ToString();
        var allTemplates = _context.PipelineTemplates
            .AsNoTracking()
            .ToList();

        if (!allTemplates.Any())
        {
            throw new InvalidOperationException("Таблица PipelineTemplates пуста. Проверьте инициализацию каталога ЦИМ-2.");
        }

        var templates = allTemplates
            .Where(x => x.LineType == lineTypeText)
            .ToList();

        if (!templates.Any())
        {
            // Защитный fallback: если нет точного типа, берём любой совместимый по DN.
            var fallback = allTemplates.FirstOrDefault(x => IsDnSupported(x.SupportedDN, dn));
            if (fallback != null)
            {
                return fallback;
            }

            return allTemplates.First();
        }

        var filtered = templates
            .Where(x => IsDnSupported(x.SupportedDN, dn))
            .OrderBy(x => simplifyTemplate ? ComplexityScore(x) : 0)
            .ToList();

        return filtered.FirstOrDefault() ?? templates.First();
    }

    private static int ComplexityScore(PipelineTemplate template)
    {
        var score = 0;
        if (template.HasElbow) score++;
        if (template.HasReducer) score++;
        if (template.HasPump) score += 2;
        if (template.HasFilter) score++;
        return score;
    }

    private static bool IsDnSupported(string supportedDnRaw, int dn)
    {
        if (string.IsNullOrWhiteSpace(supportedDnRaw))
        {
            return false;
        }

        return supportedDnRaw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(x => int.TryParse(x, out var value) && value == dn);
    }
}
