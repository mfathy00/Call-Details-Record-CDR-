using OpenTelemetry.Resources;

namespace Cdr.Api.Observability;

public static class Telemetry
{
    public static IServiceCollection AddAppTelemetry(this IServiceCollection services, IConfiguration cfg)
    {
        var environment = cfg["ASPNETCORE_ENVIRONMENT"] ?? "Development";
        
        services.AddOpenTelemetry()
            .ConfigureResource(r => r
                .AddService("Cdr.Api")
                .AddAttributes(new KeyValuePair<string, object>[]
                {
                    new("environment", environment),
                    new("version", "1.0.0"),
                }));
                
        return services;
    }
}