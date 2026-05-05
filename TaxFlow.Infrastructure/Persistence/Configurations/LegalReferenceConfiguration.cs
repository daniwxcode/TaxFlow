using Core.Domain.Tax.Obligations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaxFlow.Infrastructure.Persistence.Configurations;

internal sealed class LegalReferenceConfiguration : IEntityTypeConfiguration<LegalReference>
{
    public void Configure(EntityTypeBuilder<LegalReference> builder)
    {
        builder.ToTable("legal_references");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .HasColumnName("id");

        builder.Property(l => l.TextType)
            .HasColumnName("text_type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(l => l.Reference)
            .HasColumnName("reference")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(l => l.Title)
            .HasColumnName("title")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(l => l.Article)
            .HasColumnName("article")
            .HasMaxLength(200);

        builder.Property(l => l.PublicationDate)
            .HasColumnName("publication_date")
            .HasColumnType("date");

        builder.Property(l => l.EffectiveDate)
            .HasColumnName("effective_date")
            .HasColumnType("date");

        builder.Property(l => l.Url)
            .HasColumnName("url")
            .HasMaxLength(1000);

        builder.Property(l => l.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.ConfigureAuditable();
    }
}
