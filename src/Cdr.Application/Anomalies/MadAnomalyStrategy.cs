using Cdr.Domain.Entities;
using Cdr.Contracts.Dtos;

namespace Cdr.Application.Anomalies;

public sealed class MadAnomalyStrategy : IAnomalyStrategy
{
    private readonly decimal _k;
    public string Name => "mad";
    public MadAnomalyStrategy(decimal k) => _k = k;

    static double Median(IList<double> a)
    {
        var b = a.OrderBy(x => x).ToArray();
        int n = b.Length; if (n == 0) return 0;
        return n % 2 == 1 ? b[n/2] : (b[n/2-1] + b[n/2]) / 2.0;
    }

    public Task<IReadOnlyList<AnomalyDto>> DetectAsync(IEnumerable<CdrRecord> records, CancellationToken ct)
    {
        var list = records.ToList();
        var costs = list.Select(r => (double)r.Cost).ToList();
        var durs  = list.Select(r => (double)r.DurationSeconds).ToList();
        var medC = Median(costs); var madC = Median(costs.Select(c => Math.Abs(c - medC)).ToList());
        var medD = Median(durs);  var madD = Median(durs.Select(d => Math.Abs(d - medD)).ToList());

        var res = new List<AnomalyDto>();
        foreach (var r in list)
        {
            double sc = madC == 0 ? 0 : Math.Abs((double)r.Cost - medC) / madC;
            double sd = madD == 0 ? 0 : Math.Abs((double)r.DurationSeconds - medD) / madD;
            double s = Math.Max(sc, sd);
            if ((decimal)s >= _k)
                res.Add(new AnomalyDto(r.Reference, r.CallerId, r.Recipient, r.CallDate, r.DurationSeconds, r.Cost, Name, (decimal)s, $"mad>={_k}"));
        }
        return Task.FromResult<IReadOnlyList<AnomalyDto>>(res);
    }
}