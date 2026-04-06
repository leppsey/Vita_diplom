using System;
using System.Linq;
using Isomerization.Domain.Data;
using Isomerization.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Isomerization.Domain.Cim2;

public class Pipeline3DTemplateSelector
{
    private readonly IsomerizationContext _context;

    public Pipeline3DTemplateSelector(IsomerizationContext context)
    {
        _context = context;
    }

    public Pipeline3DTemplate Select(PipelineLineType lineType, int dn)
    {
        var lineTypeText = lineType.ToString();
        var all = _context.Pipeline3DTemplates.AsNoTracking().Where(x => x.LineType == lineTypeText).ToList();
        if (!all.Any())
        {
            throw new InvalidOperationException($"Для типа линии {lineTypeText} не найдено 3D-шаблонов.");
        }

        return all.FirstOrDefault(x => SupportsDn(x.SupportedDN, dn)) ?? all.First();
    }

    private static bool SupportsDn(string supportedDnRaw, int dn)
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
