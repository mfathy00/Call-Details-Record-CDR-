using Cdr.Application.Abstractions;
using Cdr.Infrastructure.Ingestion;
using Cdr.Infrastructure.Persistence;
using Cdr.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Cdr.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<CdrDbContext>(o =>
            o.UseNpgsql(cfg.GetConnectionString("DefaultConnection"))
             .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddScoped<ICdrRepository, CdrRepository>();
        services.AddScoped<IIngestWriter, EfBatchIngestWriter>();
        return services;
    }
}