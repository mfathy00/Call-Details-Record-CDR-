namespace Cdr.Contracts.Dtos;

public sealed record UploadResultDto(
    int Accepted,
    int Rejected,
    int Duplicates,
    long DurationMs,
    string CorrelationId);