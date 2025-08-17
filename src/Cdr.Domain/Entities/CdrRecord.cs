namespace Cdr.Domain.Entities;

public sealed class CdrRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string CallerId { get; private set; } = string.Empty;
    public string Recipient { get; private set; } = string.Empty;
    public DateOnly CallDate { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public int DurationSeconds { get; private set; }
    public decimal Cost { get; private set; }
    public string Reference { get; private set; } = string.Empty; // unique
    public string Currency { get; private set; } = ""; // ISO-4217 alpha-3

    public CdrRecord(
        string callerId,
        string recipient,
        DateOnly callDate,
        TimeOnly endTime,
        int durationSeconds,
        decimal cost,
        string reference,
        string currency)
    {
        if (string.IsNullOrWhiteSpace(callerId)) throw new ArgumentException("CallerId required");
        if (string.IsNullOrWhiteSpace(recipient)) throw new ArgumentException("Recipient required");
        if (durationSeconds < 0) throw new ArgumentOutOfRangeException(nameof(durationSeconds));
        if (cost < 0) throw new ArgumentOutOfRangeException(nameof(cost));
        if (string.IsNullOrWhiteSpace(reference)) throw new ArgumentException("Reference required");
        if (currency.Length != 3) throw new ArgumentException("Currency must be ISO-3");

        CallerId = callerId;
        Recipient = recipient;
        CallDate = callDate;
        EndTime = endTime;
        DurationSeconds = durationSeconds;
        Cost = decimal.Round(cost, 3, MidpointRounding.ToEven);
        Reference = reference;
        Currency = currency.ToUpperInvariant();
    }
}