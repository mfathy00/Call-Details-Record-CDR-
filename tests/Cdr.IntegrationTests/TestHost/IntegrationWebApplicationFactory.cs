using Cdr.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Cdr.IntegrationTests.TestHost;

public class IntegrationWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CdrDbContext>));
            if (descriptor != null) services.Remove(descriptor);
            services.AddDbContext<CdrDbContext>(o => o.UseInMemoryDatabase("test_db"));
        });
    }
}