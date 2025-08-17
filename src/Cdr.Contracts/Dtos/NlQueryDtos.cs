namespace Cdr.Contracts.Dtos;

public sealed record NlQueryResponse(
    string Intent,
    IReadOnlyDictionary<string, string> Filters,
    IReadOnlyDictionary<string, object> Result);