using Core.Domain.Contracts;
using Core.Domain.Tax.Assets;
using Core.Domain.Tax.Calculation;
using Core.Domain.Tax.Obligations;

using Microsoft.EntityFrameworkCore;

using TaxFlow.Infrastructure.Persistence.Configurations;

namespace TaxFlow.Infrastructure.Persistence;

public sealed class TaxFlowDbContext : DbContext
{
    public TaxFlowDbContext(DbContextOptions<TaxFlowDbContext> options) : base(options)
    {
    }

    public DbSet<AssetType> AssetTypes => Set<AssetType>();
    public DbSet<AttributeDefinition> AttributeDefinitions => Set<AttributeDefinition>();
    public DbSet<EnumDefinition> EnumDefinitions => Set<EnumDefinition>();
    public DbSet<EnumItem> EnumItems => Set<EnumItem>();
    public DbSet<TaxableAsset> TaxableAssets => Set<TaxableAsset>();
    public DbSet<ExtendedAttribute> ExtendedAttributes => Set<ExtendedAttribute>();
    public DbSet<TaxRule> TaxRules => Set<TaxRule>();
    public DbSet<TaxObligationSchedule> TaxObligationSchedules => Set<TaxObligationSchedule>();
    public DbSet<DeclarationDeadline> DeclarationDeadlines => Set<DeclarationDeadline>();
    public DbSet<PaymentDeadline> PaymentDeadlines => Set<PaymentDeadline>();
    public DbSet<LegalReference> LegalReferences => Set<LegalReference>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AssetTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AttributeDefinitionConfiguration());
        modelBuilder.ApplyConfiguration(new EnumDefinitionConfiguration());
        modelBuilder.ApplyConfiguration(new EnumItemConfiguration());
        modelBuilder.ApplyConfiguration(new TaxableAssetConfiguration());
        modelBuilder.ApplyConfiguration(new ExtendedAttributeConfiguration());
        modelBuilder.ApplyConfiguration(new TaxRuleConfiguration());
        modelBuilder.ApplyConfiguration(new TaxObligationScheduleConfiguration());
        modelBuilder.ApplyConfiguration(new DeclarationDeadlineConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentDeadlineConfiguration());
        modelBuilder.ApplyConfiguration(new LegalReferenceConfiguration());
    }
}
