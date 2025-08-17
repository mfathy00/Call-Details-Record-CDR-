using Cdr.Application.Abstractions;
using Cdr.Contracts.Dtos;

namespace Cdr.Application.Ingestion;

public sealed class UploadCdrCsvHandler
{
    private readonly IIngestWriter _writer;
    public UploadCdrCsvHandler(IIngestWriter writer) => _writer = writer;

    public Task<UploadResultDto> HandleAsync(UploadCdrCsvCommand cmd, CancellationToken ct)
        => _writer.UploadAsync(cmd.CsvStream, ct);
}