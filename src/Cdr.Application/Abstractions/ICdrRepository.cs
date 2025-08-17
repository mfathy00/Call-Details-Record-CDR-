using Cdr.Domain.Entities;
using Cdr.Domain.Specifications;

namespace Cdr.Application.Abstractions;

public interface ICdrRepository
{
    Task<bool> ExistsByReferenceAsync(string reference, CancellationToken ct);
    Task AddRangeAsync(IEnumerable<CdrRecord> records, CancellationToken ct);
    IAsyncEnumerable<CdrRecord> QueryAsync(CdrSpecification spec, CancellationToken ct);
}