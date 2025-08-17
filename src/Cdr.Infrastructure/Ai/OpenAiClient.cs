using Cdr.Application.Abstractions;

namespace Cdr.Infrastructure.Ai;

public sealed class OpenAiClient : IAiClient
{
    public Task<string> CompleteAsync(string prompt, CancellationToken ct)
        => Task.FromResult("(stub) configure OpenAI/Azure OpenAI and return JSON only");
}