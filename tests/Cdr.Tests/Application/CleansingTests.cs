using Cdr.Application.Ingestion;
using FluentAssertions;

namespace Cdr.Tests.Application;

public class CleansingTests
{
    [Theory]
    [InlineData("+44 123 456 789", "44123456789")]
    [InlineData("00123-456", "00123456")]
    public void NormalizePhone_StripsNonDigits(string input, string expected)
        => CsvCleansing.NormalizePhone(input).Should().Be(expected);
}