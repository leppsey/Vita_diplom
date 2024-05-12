namespace Isomerization.Domain.Task2;

public record struct Task2CalculationResults
{
    public double S { get; set; }
    public double U { get; set; }
    public double tR { get; set; }
    public double teta { get; set; }
    public double k1 { get; set; }
    public double k2 { get; set; }
    public double deltaX { get; set; }
    public double deltaT { get; set; }
    public double M { get; set; }
    public double N { get; set; }
    public double CBmax { get; set; }
    public double eA { get; set; }
    public double eps { get; set; }
    public double q { get; set; }
}