namespace Cdr.Application.Ingestion;

public sealed record UploadCdrCsvCommand(Stream CsvStream);