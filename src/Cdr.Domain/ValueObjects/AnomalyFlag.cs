namespace Cdr.Domain.ValueObjects;

public sealed record AnomalyFlag(
    string Strategy,
    string Reason,
    decimal Score,
    IReadOnlyDictionary<string, string>? Metadata);