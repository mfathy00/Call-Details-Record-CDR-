using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Cdr.Application.Ingestion;

namespace Cdr.Benchmarks.Parsers;

public class CsvParserBenchmarks
{
    [Params(10, 100, 1000)] public int Lines;

    private string[] _rows = Array.Empty<string>();
    [GlobalSetup]
    public void Setup()
    {
        var line = "1,2,2025-01-01,12:00,60,0.123,ref1,USD";
        _rows = Enumerable.Repeat(line, Lines).ToArray();
    }

    [Benchmark]
    public void ParseRows()
    {
        foreach (var line in _rows)
        {
            var cells = SimpleSplit(line);
            CsvCleansing.TryParseRow(cells, out var rec, out var err);
        }
    }

    private static string[] SimpleSplit(string line) => line.Split(',');
}

public static class Program
{
    public static void Main(string[] args) => BenchmarkRunner.Run<CsvParserBenchmarks>();
}