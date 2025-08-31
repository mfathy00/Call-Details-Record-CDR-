using Cdr.Contracts.Dtos;
using Cdr.IntegrationTests.TestHost;
using FluentAssertions;
using System.Net.Http.Json;

namespace Cdr.IntegrationTests.Api;

public class QueryEndpointTests : IClassFixture<IntegrationWebApplicationFactory>
{
    private readonly HttpClient _client;
    public QueryEndpointTests(IntegrationWebApplicationFactory f) { _client = f.CreateClient(); }

    [Fact]
    public async Task Query_Returns_Ok()
    {
        var resp = await _client.GetAsync("/api/cdr/query?q=How%20many%20calls%20did%2001482%20123456%20make%20last%20week?");
        resp.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var dto = await resp.Content.ReadFromJsonAsync<NlQueryResponse>();
        dto!.Intent.Should().Be("count_calls");
    }
}
