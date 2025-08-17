using Cdr.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cdr.Infrastructure.Persistence;

public sealed class CdrDbContext : DbContext
{
    public CdrDbContext(DbContextOptions<CdrDbContext> options) : base(options) { }
    public DbSet<CdrRecord> CdrRecords => Set<CdrRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CdrDbContext).Assembly);
    }
}