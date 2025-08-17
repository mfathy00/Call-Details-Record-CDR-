using Cdr.Domain.Entities;
using Cdr.Contracts.Dtos;

namespace Cdr.Application.Anomalies;

public sealed class ThresholdAnomalyStrategy : IAnomalyStrategy
{
    private readonly decimal _minCost; private readonly int _minDurationSeconds;
    public string Name => "threshold";
    public ThresholdAnomalyStrategy(decimal minCost, int minDurationSeconds)
    { _minCost = minCost; _minDurationSeconds = minDurationSeconds; }

    public Task<IReadOnlyList<AnomalyDto>> DetectAsync(IEnumerable<CdrRecord> records, CancellationToken ct)
    {
        var result = records
            .Where(r => r.Cost >= _minCost || r.DurationSeconds >= _minDurationSeconds)
            .Select(r => new AnomalyDto(r.Reference, r.CallerId, r.Recipient, r.CallDate, r.DurationSeconds, r.Cost, Name, 1m, "Threshold exceeded"))
            .ToList();
        return Task.FromResult<IReadOnlyList<AnomalyDto>>(result);
    }
}