using Cdr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cdr.Infrastructure.Persistence.Configurations;

public sealed class CdrRecordConfig : IEntityTypeConfiguration<CdrRecord>
{
    public void Configure(EntityTypeBuilder<CdrRecord> b)
    {
        b.ToTable("cdr_records");
        b.HasKey(x => x.Id);
        
        // Map properties to database columns
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.CallerId).HasColumnName("caller_id").HasMaxLength(64).IsRequired();
        b.Property(x => x.Recipient).HasColumnName("recipient").HasMaxLength(64).IsRequired();
        b.Property(x => x.CallDate).HasColumnName("call_date");
        b.Property(x => x.EndTime).HasColumnName("end_time");
        b.Property(x => x.DurationSeconds).HasColumnName("duration_seconds");
        b.Property(x => x.Cost).HasColumnName("cost").HasColumnType("numeric(18,3)");
        b.Property(x => x.Reference).HasColumnName("reference").HasMaxLength(64).IsRequired();
        b.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        
        // Indexes
        b.HasIndex(x => x.Reference).IsUnique();
        b.HasIndex(x => new { x.CallDate, x.CallerId });
        b.HasIndex(x => x.Recipient);
    }
}