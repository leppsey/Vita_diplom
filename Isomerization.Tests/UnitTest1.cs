using Isomerization.Domain.Cim2;
using Isomerization.Domain.Data;
using Isomerization.Domain.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Isomerization.Tests;

public class Cim2ServicesTests
{
    [Fact]
    public void DiameterSelection_ReturnsStandardDnGreaterOrEqualCalculated()
    {
        var service = new PipelineDiameterSelectionService();
        var (calculatedDn, standardDn) = service.Select(9.0, 650.0, 2.5);

        Assert.True(calculatedDn > 0);
        Assert.True(standardDn >= calculatedDn);
    }

    [Fact]
    public void LineTypeResolver_UsesPumpTypeWhenPumpRequired()
    {
        var resolver = new PipelineLineTypeResolver();
        var cim1 = new Cim1Result { FlowRate = 10, Productivity = 9 };

        var type = resolver.Resolve(cim1, hasPump: true, requiresDirectionChange: false);

        Assert.Equal(PipelineLineType.ReactorInletWithPump, type);
    }

    [Fact]
    public void ValidationService_FailsWhenVelocityExceedsLimit()
    {
        var validator = new PipelineValidationService();
        var line = new PipelineLine
        {
            Elements = new List<PipelineElement>
            {
                new() { ElementType = PipelineElementType.Pipe, Name = "Pipe", DN = 80, Length = 10, TemperatureMin = -20, TemperatureMax = 300 }
            },
            CalculationResult = new PipelineCalculationResult
            {
                Velocity = 3.2,
                PressureLossTotal = 1000,
                EnergyConsumption = 400
            }
        };

        var request = new Cim2Request
        {
            Cim1Result = new Cim1Result { Temperature = 100 },
            AllowedVelocity = 2.5
        };

        var validation = validator.Validate(line, request);
        Assert.False(validation.IsValid);
        Assert.Contains(validation.Violations, x => x.Contains("Скорость потока"));
    }

    [Fact]
    public void Orchestrator_BuildsLineAndReturnsTemplate()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<IsomerizationContext>()
            .UseSqlite(connection)
            .Options;
        using var context = new IsomerizationContext(options);
        context.Database.EnsureCreated();

        SeedMinimalCatalog(context);

        var orchestrator = new Cim2OrchestratorService(
            new PipelineLineTypeResolver(),
            new PipelineDiameterSelectionService(),
            new RuleEngineService(context, new PipelineDiameterSelectionService()),
            new PipelineTemplateSelector(context),
            new PipelineElementSelector(context),
            new PipelineCalculationService(),
            new PipelineValidationService(),
            new PipelineRecommendationService(),
            new Pipeline3DTemplateSelector(context));

        var result = orchestrator.Execute(new Cim2Request
        {
            Cim1Result = new Cim1Result
            {
                ReactorId = 1,
                ReactorName = "R-1",
                FlowRate = 5,
                Productivity = 4.5,
                DensityGsm3 = 650,
                Temperature = 120,
                PressureKPa = 200000
            },
            AllowedVelocity = 2.5,
            PipeLength = 12,
            PreferredPressureClass = "PN16"
        });

        Assert.NotNull(result);
        Assert.NotEmpty(result.Line.Elements);
        Assert.False(string.IsNullOrWhiteSpace(result.Template3DPath));
    }

    private static void SeedMinimalCatalog(IsomerizationContext context)
    {
        context.PipelinePipes.Add(new PipelinePipe
        {
            Name = "Pipe DN65",
            DN = 65,
            Material = "Steel",
            Roughness = 0.0001,
            PressureClass = "PN16",
            TemperatureMin = -20,
            TemperatureMax = 250,
            WallThickness = 0.005,
            Standard = "GOST",
            ModelPath = "pipe65.obj"
        });
        context.PipelinePipes.Add(new PipelinePipe
        {
            Name = "Pipe DN80",
            DN = 80,
            Material = "Steel",
            Roughness = 0.0001,
            PressureClass = "PN16",
            TemperatureMin = -20,
            TemperatureMax = 250,
            WallThickness = 0.006,
            Standard = "GOST",
            ModelPath = "pipe.obj"
        });
        context.PipelineValves.Add(new PipelineValve
        {
            Name = "Valve DN65",
            Type = "Gate",
            DN = 65,
            Zeta = 0.2,
            PressureClass = "PN16",
            TemperatureMin = -20,
            TemperatureMax = 250,
            Standard = "GOST",
            ModelPath = "valve65.obj"
        });
        context.PipelineValves.Add(new PipelineValve
        {
            Name = "Valve DN80",
            Type = "Gate",
            DN = 80,
            Zeta = 0.2,
            PressureClass = "PN16",
            TemperatureMin = -20,
            TemperatureMax = 250,
            Standard = "GOST",
            ModelPath = "valve.obj"
        });
        context.PipelineTemplates.Add(new PipelineTemplate
        {
            Name = "Base",
            LineType = "ReactorInletLine",
            HasValve = true,
            SupportedDN = "65,80,100",
            Template3DPath = "tmpl.obj"
        });
        context.Pipeline3DTemplates.Add(new Pipeline3DTemplate
        {
            Name = "Base3D",
            LineType = "ReactorInletLine",
            SupportedDN = "65,80,100",
            RequiredElements = "Pipe,Valve",
            PreviewPath = "preview.png",
            ModelPath = "model.obj"
        });
        context.SaveChanges();
    }
}