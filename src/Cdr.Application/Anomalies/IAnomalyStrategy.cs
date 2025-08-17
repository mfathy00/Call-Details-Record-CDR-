using Cdr.Domain.Entities;
using Cdr.Contracts.Dtos;

namespace Cdr.Application.Anomalies;

public interface IAnomalyStrategy
{
    Task<IReadOnlyList<AnomalyDto>> DetectAsync(IEnumerable<CdrRecord> records, CancellationToken ct);
    string Name { get; }
}