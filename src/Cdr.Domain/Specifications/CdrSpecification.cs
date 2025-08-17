namespace Cdr.Domain.Specifications;

public sealed class CdrSpecification
{
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
    public string? CallerId { get; init; }
    public string? Recipient { get; init; }
}