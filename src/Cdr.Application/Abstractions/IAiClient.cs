namespace Cdr.Application.Abstractions;

public interface IAiClient
{
    Task<string> CompleteAsync(string prompt, CancellationToken ct);
}