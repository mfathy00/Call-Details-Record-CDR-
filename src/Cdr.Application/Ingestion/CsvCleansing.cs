using System.Globalization;
using Cdr.Domain.Entities;

namespace Cdr.Application.Ingestion;

public static class CsvCleansing
{
    public static bool TryParseRow(string[] row, out CdrRecord? record, out string? error)
    {
        // Expected columns: caller_id, recipient, call_date, end_time, duration (s), cost (3 d.p.), reference, currency
        record = null; error = null;
        if (row.Length < 8) { error = "Row has fewer than 8 columns"; return false; }

        string caller = NormalizePhone(row[0]);
        string recipient = NormalizePhone(row[1]);

        if (!DateOnly.TryParse(row[2], CultureInfo.InvariantCulture, out var callDate))
        { error = $"Invalid call_date: {row[2]}"; return false; }
        if (!TimeOnly.TryParse(row[3], CultureInfo.InvariantCulture, out var endTime))
        { error = $"Invalid end_time: {row[3]}"; return false; }
        if (!int.TryParse(row[4], NumberStyles.Integer, CultureInfo.InvariantCulture, out var duration) || duration < 0)
        { error = $"Invalid duration: {row[4]}"; return false; }
        if (!decimal.TryParse(row[5], NumberStyles.Float, CultureInfo.InvariantCulture, out var cost) || cost < 0)
        { error = $"Invalid cost: {row[5]}"; return false; }
        string reference = row[6].Trim();
        string currency = row[7].Trim().ToUpperInvariant();
        if (currency.Length != 3) { error = $"Invalid currency: {currency}"; return false; }

        try
        {
            record = new CdrRecord(caller, recipient, callDate, endTime, duration, cost, reference, currency);
            return true;
        }
        catch (Exception ex) { error = ex.Message; return false; }
    }

    public static string NormalizePhone(string input)
        => new string(input.Where(char.IsDigit).ToArray());
}