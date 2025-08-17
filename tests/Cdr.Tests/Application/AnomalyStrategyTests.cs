using Cdr.Application.Anomalies;
using Cdr.Domain.Entities;
using FluentAssertions;

namespace Cdr.Tests.Application;

public class AnomalyStrategyTests
{
    private static CdrRecord R(int dur, decimal cost) => new("1","2", new DateOnly(2025,1,1), new TimeOnly(12,0), dur, cost, Guid.NewGuid().ToString(), "USD");

    [Fact]
    public async Task Threshold_Flags_HighCost()
    {
        var s = new ThresholdAnomalyStrategy(minCost: 10m, minDurationSeconds: 1000);
        var res = await s.DetectAsync(new[] { R(60, 5m), R(60, 15m) }, default);
        res.Should().HaveCount(1);
    }
}