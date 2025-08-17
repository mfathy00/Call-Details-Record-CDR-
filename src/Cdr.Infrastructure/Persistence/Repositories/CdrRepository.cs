using Cdr.Application.Abstractions;
using Cdr.Domain.Entities;
using Cdr.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Cdr.Infrastructure.Persistence.Repositories;

public sealed class CdrRepository : ICdrRepository
{
    private readonly CdrDbContext _db;
    public CdrRepository(CdrDbContext db) => _db = db;

    public Task<bool> ExistsByReferenceAsync(string reference, CancellationToken ct)
        => _db.CdrRecords.AnyAsync(x => x.Reference == reference, ct);

    public async Task AddRangeAsync(IEnumerable<CdrRecord> records, CancellationToken ct)
    {
        await _db.CdrRecords.AddRangeAsync(records, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async IAsyncEnumerable<CdrRecord> QueryAsync(CdrSpecification spec, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        var q = _db.CdrRecords.AsNoTracking().AsQueryable();
        if (spec.From.HasValue) q = q.Where(x => x.CallDate >= spec.From.Value);
        if (spec.To.HasValue) q = q.Where(x => x.CallDate <= spec.To.Value);
        if (!string.IsNullOrWhiteSpace(spec.CallerId)) q = q.Where(x => x.CallerId == spec.CallerId);
        if (!string.IsNullOrWhiteSpace(spec.Recipient)) q = q.Where(x => x.Recipient == spec.Recipient);
        await foreach (var item in q.AsAsyncEnumerable().WithCancellation(ct)) yield return item;
    }
}