# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/Cdr.Api/Cdr.Api.csproj", "src/Cdr.Api/"]
COPY ["src/Cdr.Infrastructure/Cdr.Infrastructure.csproj", "src/Cdr.Infrastructure/"]
COPY ["src/Cdr.Application/Cdr.Application.csproj", "src/Cdr.Application/"]
COPY ["src/Cdr.Domain/Cdr.Domain.csproj", "src/Cdr.Domain/"]
COPY ["src/Cdr.Contracts/Cdr.Contracts.csproj", "src/Cdr.Contracts/"]
COPY ["Directory.Build.props", "./"]

RUN dotnet restore "src/Cdr.Api/Cdr.Api.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "src/Cdr.Api/Cdr.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "src/Cdr.Api/Cdr.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copy published app
COPY --from=publish --chown=appuser:appuser /app/publish .

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Cdr.Api.dll"]