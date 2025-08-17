using Cdr.Application.Abstractions;
using Cdr.Contracts.Dtos;
using Cdr.Domain.Specifications;

namespace Cdr.Application.Anomalies;

public sealed class DetectAnomaliesQuery
{
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
    public string? CallerId { get; init; }
    public string? Recipient { get; init; }
    public string Strategy { get; init; } = "threshold";
    public decimal Z { get; init; } = 3m;
    public decimal K { get; init; } = 6m;
    public decimal MinCost { get; init; } = 5m;
    public int MinDuration { get; init; } = 600;
}

public sealed class DetectAnomaliesHandler
{
    private readonly ICdrRepository _repo;
    public DetectAnomaliesHandler(ICdrRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<AnomalyDto>> HandleAsync(DetectAnomaliesQuery q, CancellationToken ct)
    {
        var spec = new CdrSpecification { From = q.From, To = q.To, CallerId = q.CallerId, Recipient = q.Recipient };
        var list = new List<Cdr.Domain.Entities.CdrRecord>();
        await foreach (var r in _repo.QueryAsync(spec, ct)) list.Add(r);

        IAnomalyStrategy strategy = q.Strategy.ToLowerInvariant() switch
        {
            "zscore" => new ZScoreAnomalyStrategy(q.Z),
            "mad"    => new MadAnomalyStrategy(q.K),
            _        => new ThresholdAnomalyStrategy(q.MinCost, q.MinDuration)
        };
        return await strategy.DetectAsync(list, ct);
    }
}