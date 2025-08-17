using System.Text;
using Cdr.Application.Abstractions;
using Cdr.Application.Ingestion;
using Cdr.Contracts.Dtos;
using Cdr.Domain.Entities;
using Cdr.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Cdr.Infrastructure.Ingestion;

public sealed class EfBatchIngestWriter : IIngestWriter
{
    private readonly CdrDbContext _db;
    private readonly ILogger<EfBatchIngestWriter> _logger;
    private readonly int _batchSize;

    public EfBatchIngestWriter(CdrDbContext db, ILogger<EfBatchIngestWriter> logger, IConfiguration cfg)
    {
        _db = db; _logger = logger; _batchSize = cfg.GetValue("Ingestion:BatchSize", 5000);
    }

    public async Task<UploadResultDto> UploadAsync(Stream csvStream, CancellationToken ct)
    {
        using var reader = new StreamReader(csvStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        string? header = await reader.ReadLineAsync(); // assume header row exists
        var sw = System.Diagnostics.Stopwatch.StartNew();
        int accepted=0, rejected=0, duplicates=0; var dedupe = new HashSet<string>();
        var batch = new List<CdrRecord>(_batchSize);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;
            var cells = SplitCsv(line);
            if (!CsvCleansing.TryParseRow(cells, out var rec, out var err))
            { rejected++; continue; }
            if (!dedupe.Add(rec!.Reference)) { duplicates++; continue; }

            if (await _db.CdrRecords.AnyAsync(x => x.Reference == rec.Reference, ct)) { duplicates++; continue; }

            batch.Add(rec);
            if (batch.Count >= _batchSize)
            {
                await _db.CdrRecords.AddRangeAsync(batch, ct);
                await _db.SaveChangesAsync(ct);
                accepted += batch.Count; batch.Clear();
            }
        }
        if (batch.Count > 0)
        {
            await _db.CdrRecords.AddRangeAsync(batch, ct);
            await _db.SaveChangesAsync(ct);
            accepted += batch.Count;
        }

        sw.Stop();
        return new UploadResultDto(accepted, rejected, duplicates, sw.ElapsedMilliseconds, Guid.NewGuid().ToString());
    }

    private static string[] SplitCsv(string line)
    {
        var res = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; }
                else inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes) { res.Add(sb.ToString()); sb.Clear(); }
            else sb.Append(c);
        }
        res.Add(sb.ToString());
        return res.ToArray();
    }
}