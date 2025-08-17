namespace Cdr.Contracts.Dtos;

public sealed record AnomalyDto(
    string Reference,
    string CallerId,
    string Recipient,
    DateOnly CallDate,
    int DurationSeconds,
    decimal Cost,
    string Strategy,
    decimal Score,
    string Reason);