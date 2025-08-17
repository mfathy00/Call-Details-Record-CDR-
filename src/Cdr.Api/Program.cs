using Cdr.Api.Endpoints;
using Cdr.Api.Observability;
using Cdr.Application.Ingestion;
using Cdr.Infrastructure;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add HTTP logging
builder.Services.AddHttpLogging(o => o.LoggingFields = HttpLoggingFields.All);

// Add Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Add Telemetry
builder.Services.AddAppTelemetry(builder.Configuration);

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!);

// Add Application Services
builder.Services.AddScoped<UploadCdrCsvHandler>();
builder.Services.AddScoped<Cdr.Application.Anomalies.DetectAnomaliesHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseHttpLogging();

// Health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/ready");

// API endpoints
app.MapCdrEndpoints();

// Database initialization
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Cdr.Infrastructure.Persistence.CdrDbContext>();
    try
    {
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        // Log the error but don't fail the startup
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Failed to apply database migrations. The application will continue but may not function correctly.");
    }
}

app.Run();

public partial class Program { }