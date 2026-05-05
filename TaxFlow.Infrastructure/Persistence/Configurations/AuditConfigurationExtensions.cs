using Core.Domain.Contracts.Abstracts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaxFlow.Infrastructure.Persistence.Configurations;

internal static class AuditConfigurationExtensions
{
    internal static void ConfigureAuditable<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : AuditableEntity
    {
        builder.Property(e => e.Created)
            .HasColumnName("created")
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasColumnName("created_by")
            .IsRequired();

        builder.Property(e => e.LastModified)
            .HasColumnName("last_modified")
            .IsRequired();

        builder.Property(e => e.LastModifiedBy)
            .HasColumnName("last_modified_by");
    }

    internal static void ConfigureSoftDelete<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : SoftAuditableEntity
    {
        builder.Property(e => e.Deleted)
            .HasColumnName("deleted");

        builder.Property(e => e.DeletedBy)
            .HasColumnName("deleted_by");

        builder.Property(e => e.LastDeletedOn)
            .HasColumnName("last_deleted_on");

        builder.Property(e => e.LastDeletedby)
            .HasColumnName("last_deleted_by");

        builder.Property(e => e.LastRecovered)
            .HasColumnName("last_recovered");

        builder.Property(e => e.LastRecoveredBy)
            .HasColumnName("last_recovered_by");
    }

    internal static void ConfigureTemporal<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : TemporalAuditableEntity
    {
        builder.Property(e => e.ValidFrom)
            .HasColumnName("valid_from")
            .IsRequired();

        builder.Property(e => e.ValidTo)
            .HasColumnName("valid_to");
    }
}
