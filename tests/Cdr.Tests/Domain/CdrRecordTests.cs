using Cdr.Domain.Entities;
using FluentAssertions;

namespace Cdr.Tests.Domain;

public class CdrRecordTests
{
    [Fact]
    public void Constructor_RoundsCost_To3dp()
    {
        var r = new CdrRecord("1","2", new DateOnly(2025,1,1), new TimeOnly(12,0), 60, 1.23456m, "ref1", "USD");
        r.Cost.Should().Be(1.235m);
    }

    [Fact]
    public void Constructor_Throws_OnNegativeDuration()
    {
        Action act = () => new CdrRecord("1","2", new DateOnly(2025,1,1), new TimeOnly(12,0), -1, 0m, "ref1", "USD");
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}