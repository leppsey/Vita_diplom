using System.Collections.Generic;

namespace Isomerization.Domain.Cim2;

public class Cim2OrchestratorService
{
    private readonly PipelineLineTypeResolver _lineTypeResolver;
    private readonly PipelineDiameterSelectionService _diameterSelectionService;
    private readonly RuleEngineService _ruleEngineService;
    private readonly PipelineTemplateSelector _templateSelector;
    private readonly PipelineElementSelector _elementSelector;
    private readonly PipelineCalculationService _calculationService;
    private readonly PipelineValidationService _validationService;
    private readonly PipelineRecommendationService _recommendationService;
    private readonly Pipeline3DTemplateSelector _template3DSelector;

    public Cim2OrchestratorService(
        PipelineLineTypeResolver lineTypeResolver,
        PipelineDiameterSelectionService diameterSelectionService,
        RuleEngineService ruleEngineService,
        PipelineTemplateSelector templateSelector,
        PipelineElementSelector elementSelector,
        PipelineCalculationService calculationService,
        PipelineValidationService validationService,
        PipelineRecommendationService recommendationService,
        Pipeline3DTemplateSelector template3DSelector)
    {
        _lineTypeResolver = lineTypeResolver;
        _diameterSelectionService = diameterSelectionService;
        _ruleEngineService = ruleEngineService;
        _templateSelector = templateSelector;
        _elementSelector = elementSelector;
        _calculationService = calculationService;
        _validationService = validationService;
        _recommendationService = recommendationService;
        _template3DSelector = template3DSelector;
    }

    public Cim2Result Execute(Cim2Request request)
    {
        var lineType = _lineTypeResolver.Resolve(request.Cim1Result, request.RequiresPump, request.RequiresDirectionChange);
        var (calculatedDn, standardDn) =
            _diameterSelectionService.Select(request.Cim1Result.FlowRate, request.Cim1Result.DensityGsm3, request.AllowedVelocity);

        var executionContext = new Cim2ExecutionContext
        {
            Request = request,
            LineType = lineType,
            CalculatedDn = calculatedDn,
            SelectedDn = standardDn,
            RequiresElbow = request.RequiresDirectionChange,
            RequiresPumpAssembly = request.RequiresPump
        };

        _ruleEngineService.ApplyRules(executionContext);

        var template = _templateSelector.Select(executionContext.LineType, executionContext.SelectedDn, executionContext.SimplifyTemplate);
        var elements = _elementSelector.BuildElements(template, executionContext);

        var line = new PipelineLine
        {
            Name = template.Name,
            LineType = executionContext.LineType,
            Elements = elements
        };

        line.CalculationResult = _calculationService.Calculate(line, request);
        line.ValidationResult = _validationService.Validate(line, request);

        var recommendations = _recommendationService.BuildRecommendations(line, request);
        if (executionContext.RuleRecommendations.Count > 0)
        {
            recommendations.AddRange(executionContext.RuleRecommendations);
        }
        line.Recommendations = recommendations;

        var template3d = _template3DSelector.Select(executionContext.LineType, executionContext.SelectedDn);
        line.Selected3DTemplate = template3d.ModelPath;

        return new Cim2Result
        {
            Line = line,
            TemplateName = template.Name,
            Template3DPath = template3d.ModelPath,
            Recommendations = new List<string>(line.Recommendations)
        };
    }
}
