using Cdr.Application.Abstractions;
using Cdr.Contracts.Dtos;
using Cdr.Domain.Specifications;

namespace Cdr.Application.Nl;

public sealed class NlQueryHandler
{
    private readonly ICdrRepository _repo;
    public NlQueryHandler(ICdrRepository repo) => _repo = repo;

    public async Task<NlQueryResponse> HandleAsync(string query, CancellationToken ct)
    {
        var parsed = NlQueryParser.Parse(query);
        var spec = new CdrSpecification();

        if (parsed.Filters.TryGetValue("callerId", out var caller))
            spec = spec with { CallerId = caller };
        if (parsed.Filters.TryGetValue("recipientCountry", out var country) && country == "USA")
            spec = spec with { RecipientStartsWith = "+1" };
        if (parsed.Filters.TryGetValue("range", out var range))
        {
            if (range == "last_week")
            {
                var end = DateOnly.FromDateTime(DateTime.UtcNow);
                var start = end.AddDays(-7);
                spec = spec with { From = start, To = end };
            }
            else if (range == "june")
            {
                var year = DateTime.UtcNow.Year;
                var start = new DateOnly(year, 6, 1);
                var end = start.AddMonths(1).AddDays(-1);
                spec = spec with { From = start, To = end };
            }
        }

        if (parsed.Intent == "count_calls")
        {
            var count = await _repo.CountAsync(spec, ct);
            var res = new Dictionary<string, object> { ["count"] = count };
            return new NlQueryResponse(parsed.Intent, parsed.Filters, res);
        }
        if (parsed.Intent == "total_cost")
        {
            var total = await _repo.SumCostAsync(spec, ct);
            var res = new Dictionary<string, object> { ["totalCost"] = total };
            return new NlQueryResponse(parsed.Intent, parsed.Filters, res);
        }

        return parsed;
    }
}
