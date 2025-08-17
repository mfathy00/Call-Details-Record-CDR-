using Cdr.Application.Anomalies;
using Cdr.Application.Ingestion;
using Cdr.Contracts.Dtos;

namespace Cdr.Api.Endpoints;

public static class CdrEndpoints
{
    public static IEndpointRouteBuilder MapCdrEndpoints(this IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/api/cdr");

        grp.MapPost("/upload", async (HttpRequest req, UploadCdrCsvHandler handler, CancellationToken ct) =>
        {
            if (!req.HasFormContentType) return Results.BadRequest("multipart/form-data expected");
            var form = await req.ReadFormAsync(ct);
            var file = form.Files.GetFile("file");
            if (file is null) return Results.BadRequest("file field missing");
            await using var stream = file.OpenReadStream();
            var result = await handler.HandleAsync(new UploadCdrCsvCommand(stream), ct);
            return Results.Accepted($"/api/cdr/upload/{result.CorrelationId}", result);
        })
        .WithSummary("Upload CDR CSV")
        .Produces<UploadResultDto>(StatusCodes.Status202Accepted);

        grp.MapGet("/anomalies", async (
            DateOnly? from,
            DateOnly? to,
            string? callerId,
            string? recipient,
            string? strategy,
            decimal? z,
            decimal? k,
            decimal? minCost,
            int? minDuration,
            DetectAnomaliesHandler handler,
            CancellationToken ct) =>
        {
            var q = new DetectAnomaliesQuery
            {
                From = from, To = to, CallerId = callerId, Recipient = recipient,
                Strategy = strategy ?? "threshold",
                Z = z ?? 3m, K = k ?? 6m, MinCost = minCost ?? 5m, MinDuration = minDuration ?? 600
            };
            var res = await handler.HandleAsync(q, ct);
            return Results.Ok(res);
        })
        .WithSummary("Detect anomalies");

        return app;
    }
}