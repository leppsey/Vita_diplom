using System;
using System.Collections.Generic;
using System.Linq;
using Isomerization.Domain.Data;
using Isomerization.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Isomerization.Domain.Cim2;

public class PipelineElementSelector
{
    private readonly IsomerizationContext _context;

    public PipelineElementSelector(IsomerizationContext context)
    {
        _context = context;
    }

    public List<PipelineElement> BuildElements(PipelineTemplate template, Cim2ExecutionContext executionContext)
    {
        var elements = new List<PipelineElement>();
        var requestedDn = executionContext.SelectedDn;
        var temperature = executionContext.Request.Cim1Result.Temperature;
        var pressureClass = executionContext.Request.PreferredPressureClass;

        var pipe = SelectPipe(requestedDn, pressureClass, temperature);
        var dn = pipe.DN;
        executionContext.SelectedDn = dn;

        elements.Add(new PipelineElement
        {
            ElementType = PipelineElementType.Pipe,
            Name = pipe.Name,
            DN = pipe.DN,
            Length = executionContext.Request.PipeLength,
            Zeta = 0,
            Material = pipe.Material,
            PressureClass = pipe.PressureClass,
            TemperatureMin = pipe.TemperatureMin,
            TemperatureMax = pipe.TemperatureMax,
            ModelPath = pipe.ModelPath
        });

        if (template.HasValve)
        {
            var valve = SelectValve(dn, pressureClass, temperature);
            if (valve != null)
            {
                elements.Add(ToElement(valve));
            }
        }

        if (template.HasElbow || executionContext.RequiresElbow)
        {
            var elbow = SelectElbow(dn, pressureClass, temperature);
            if (elbow != null)
            {
                elements.Add(ToElement(elbow));
            }
        }

        if (template.HasReducer || executionContext.RequiresReducer)
        {
            var reducer = _context.PipelineReducers.AsNoTracking()
                .OrderBy(x => x.DNOut == dn ? 0 : 1)
                .First();
            elements.Add(ToElement(reducer, dn));
        }

        if (template.HasFilter || executionContext.RequiresPumpAssembly)
        {
            var filter = SelectFilter(dn, pressureClass, temperature);
            if (filter != null)
            {
                elements.Add(ToElement(filter));
            }
        }

        if (template.HasPump || executionContext.RequiresPumpAssembly)
        {
            var pump = SelectPump(dn, temperature);
            if (pump != null)
            {
                elements.Add(ToElement(pump));
            }
        }

        return elements;
    }

    private PipelinePipe SelectPipe(int dn, string pressureClass, double temperature)
    {
        var pipes = _context.PipelinePipes.AsNoTracking().ToList();
        if (!pipes.Any())
        {
            throw new InvalidOperationException("Каталог PipelinePipes пуст. Заполните справочник труб.");
        }

        return pipes
            .OrderBy(x => x.DN == dn ? 0 : 1)
            .ThenBy(x => x.PressureClass == pressureClass ? 0 : 1)
            .ThenBy(x => IsTemperatureCompatible(x.TemperatureMin, x.TemperatureMax, temperature) ? 0 : 1)
            .ThenBy(x => Math.Abs(x.DN - dn))
            .First();
    }

    private PipelineValve? SelectValve(int dn, string pressureClass, double temperature)
    {
        var valves = _context.PipelineValves.AsNoTracking().ToList();
        return valves
            .OrderBy(x => x.DN == dn ? 0 : 1)
            .ThenBy(x => x.PressureClass == pressureClass ? 0 : 1)
            .ThenBy(x => IsTemperatureCompatible(x.TemperatureMin, x.TemperatureMax, temperature) ? 0 : 1)
            .ThenBy(x => Math.Abs(x.DN - dn))
            .FirstOrDefault();
    }

    private PipelineElbow? SelectElbow(int dn, string pressureClass, double temperature)
    {
        var elbows = _context.PipelineElbows.AsNoTracking().ToList();
        return elbows
            .OrderBy(x => x.DN == dn ? 0 : 1)
            .ThenBy(x => x.PressureClass == pressureClass ? 0 : 1)
            .ThenBy(x => IsTemperatureCompatible(x.TemperatureMin, x.TemperatureMax, temperature) ? 0 : 1)
            .ThenBy(x => Math.Abs(x.DN - dn))
            .FirstOrDefault();
    }

    private PipelineFilter? SelectFilter(int dn, string pressureClass, double temperature)
    {
        var filters = _context.PipelineFilters.AsNoTracking().ToList();
        return filters
            .OrderBy(x => x.DN == dn ? 0 : 1)
            .ThenBy(x => x.PressureClass == pressureClass ? 0 : 1)
            .ThenBy(x => IsTemperatureCompatible(x.TemperatureMin, x.TemperatureMax, temperature) ? 0 : 1)
            .ThenBy(x => Math.Abs(x.DN - dn))
            .FirstOrDefault();
    }

    private PipelinePump? SelectPump(int dn, double temperature)
    {
        var pumps = _context.PipelinePumps.AsNoTracking().ToList();
        return pumps
            .OrderBy(x => x.DN == dn ? 0 : 1)
            .ThenBy(x => IsTemperatureCompatible(x.TemperatureMin, x.TemperatureMax, temperature) ? 0 : 1)
            .ThenBy(x => Math.Abs(x.DN - dn))
            .FirstOrDefault();
    }

    private static bool IsTemperatureCompatible(double min, double max, double value) =>
        value >= min && value <= max;

    private static PipelineElement ToElement(PipelineValve source) => new()
    {
        ElementType = PipelineElementType.Valve,
        Name = source.Name,
        DN = source.DN,
        Zeta = source.Zeta,
        Length = 0,
        PressureClass = source.PressureClass,
        TemperatureMin = source.TemperatureMin,
        TemperatureMax = source.TemperatureMax,
        ModelPath = source.ModelPath
    };

    private static PipelineElement ToElement(PipelineElbow source) => new()
    {
        ElementType = PipelineElementType.Elbow,
        Name = source.Name,
        DN = source.DN,
        Zeta = source.Zeta,
        Length = 0,
        PressureClass = source.PressureClass,
        TemperatureMin = source.TemperatureMin,
        TemperatureMax = source.TemperatureMax,
        ModelPath = source.ModelPath
    };

    private static PipelineElement ToElement(PipelineReducer source, int dnOut) => new()
    {
        ElementType = PipelineElementType.Reducer,
        Name = source.Name,
        DN = dnOut,
        Zeta = source.Zeta,
        Length = 0,
        PressureClass = source.PressureClass,
        TemperatureMin = source.TemperatureMin,
        TemperatureMax = source.TemperatureMax,
        ModelPath = source.ModelPath
    };

    private static PipelineElement ToElement(PipelineFilter source) => new()
    {
        ElementType = PipelineElementType.Filter,
        Name = source.Name,
        DN = source.DN,
        Zeta = source.Zeta,
        Length = 0,
        PressureClass = source.PressureClass,
        TemperatureMin = source.TemperatureMin,
        TemperatureMax = source.TemperatureMax,
        ModelPath = source.ModelPath
    };

    private static PipelineElement ToElement(PipelinePump source) => new()
    {
        ElementType = PipelineElementType.Pump,
        Name = source.Name,
        DN = source.DN,
        Zeta = 0,
        Length = 0,
        PressureClass = "N/A",
        TemperatureMin = source.TemperatureMin,
        TemperatureMax = source.TemperatureMax,
        ModelPath = source.ModelPath
    };
}
