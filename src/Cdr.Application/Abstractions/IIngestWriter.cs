using Cdr.Contracts.Dtos;

namespace Cdr.Application.Abstractions;

public interface IIngestWriter
{
    Task<UploadResultDto> UploadAsync(Stream csvStream, CancellationToken ct);
}