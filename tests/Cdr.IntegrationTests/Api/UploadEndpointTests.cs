using System.Text;
using Cdr.IntegrationTests.TestHost;
using FluentAssertions;

namespace Cdr.IntegrationTests.Api;

public class UploadEndpointTests : IClassFixture<IntegrationWebApplicationFactory>
{
    private readonly HttpClient _client;
    public UploadEndpointTests(IntegrationWebApplicationFactory f) { _client = f.CreateClient(); }

    [Fact]
    public async Task Upload_Returns_Accepted()
    {
        var content = new MultipartFormDataContent();
        var csv = "caller_id,recipient,call_date,end_time,duration (s),cost (3 d.p.),reference,currency\n" +
                  "1,2,2025-01-01,12:00,60,0.123,ref1,USD\n";
        content.Add(new StringContent(csv, Encoding.UTF8, "text/csv"), "file", "techtest_cdr.csv");
        var resp = await _client.PostAsync("/api/cdr/upload", content);
        resp.StatusCode.Should().Be(System.Net.HttpStatusCode.Accepted);
    }
}