namespace Isomerization.Domain.Cim2;

public class PipelineLineTypeResolver
{
    public PipelineLineType Resolve(Cim1Result cim1, bool hasPump, bool requiresDirectionChange)
    {
        if (hasPump)
        {
            return PipelineLineType.ReactorInletWithPump;
        }

        if (requiresDirectionChange)
        {
            return PipelineLineType.ReactorOutletWithElbow;
        }

        return cim1.FlowRate >= cim1.Productivity
            ? PipelineLineType.ReactorInletLine
            : PipelineLineType.ReactorOutletLine;
    }
}
