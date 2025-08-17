using System.Text.RegularExpressions;
using Cdr.Contracts.Dtos;

namespace Cdr.Application.Nl;

public static class NlQueryParser
{
    // Simple intents: count calls by caller in a time window ("last week", "in June 2025")
    public static NlQueryResponse Parse(string q)
    {
        q = q.Trim().ToLowerInvariant();
        var filters = new Dictionary<string, string>();
        if (Regex.IsMatch(q, "how many|count"))
        {
            // naive phone extraction
            var m = Regex.Match(q, @"(\+?\d[\d\s]{5,})");
            var phone = m.Success ? m.Groups[1].Value.Replace(" ", "") : string.Empty;
            if (!string.IsNullOrEmpty(phone)) filters["callerId"] = phone;
            // naive "last week" handling
            if (q.Contains("last week")) filters["range"] = "last_week";
            return new NlQueryResponse("count_calls", filters, new Dictionary<string, object>());
        }
        return new NlQueryResponse("unknown", filters, new Dictionary<string, object>());
    }
}