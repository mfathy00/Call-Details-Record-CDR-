using Cdr.Domain.Entities;
using Cdr.Contracts.Dtos;

namespace Cdr.Application.Anomalies;

public sealed class ZScoreAnomalyStrategy : IAnomalyStrategy
{
    private readonly decimal _z;
    public string Name => "zscore";
    public ZScoreAnomalyStrategy(decimal zThreshold) => _z = zThreshold;

    public Task<IReadOnlyList<AnomalyDto>> DetectAsync(IEnumerable<CdrRecord> records, CancellationToken ct)
    {
        var list = records.ToList();
        if (list.Count == 0) return Task.FromResult<IReadOnlyList<AnomalyDto>>(new List<AnomalyDto>());

        static (double mean, double std) Stats(IEnumerable<double> s)
        {
            var arr = s.ToArray();
            var mean = arr.Average();
            var var = arr.Select(x => (x - mean) * (x - mean)).Average();
            return (mean, Math.Sqrt(var));
        }

        var (costMean, costStd) = Stats(list.Select(r => (double)r.Cost));
        var (durMean, durStd)  = Stats(list.Select(r => (double)r.DurationSeconds));

        var result = new List<AnomalyDto>();
        foreach (var r in list)
        {
            double zc = costStd == 0 ? 0 : ((double)r.Cost - costMean) / costStd;
            double zd = durStd  == 0 ? 0 : ((double)r.DurationSeconds - durMean) / durStd;
            var maxz = Math.Max(Math.Abs(zc), Math.Abs(zd));
            if ((decimal)maxz >= _z)
            {
                result.Add(new AnomalyDto(r.Reference, r.CallerId, r.Recipient, r.CallDate, r.DurationSeconds, r.Cost, Name, (decimal)maxz, $"z>={_z}"));
            }
        }
        return Task.FromResult<IReadOnlyList<AnomalyDto>>(result);
    }
}