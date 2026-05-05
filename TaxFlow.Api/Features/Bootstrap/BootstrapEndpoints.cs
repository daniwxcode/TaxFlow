using Core.Bootstrap;
using Core.Domain.Contracts;
using Core.Domain.Tax.Assets;
using Core.Domain.Tax.Calculation;

using Microsoft.EntityFrameworkCore;

using TaxFlow.Api.Features.AssetTypes;
using TaxFlow.Infrastructure.Persistence;

namespace TaxFlow.Api.Features.Bootstrap;

public static class BootstrapEndpoints
{
    public static IEndpointRouteBuilder MapBootstrapEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/bootstrap");

        group.MapGet("/status", async (TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            var hasAssetTypes = await db.AssetTypes.AsNoTracking().AnyAsync(cancellationToken);
            return Results.Ok(new { seeded = hasAssetTypes });
        });

        group.MapGet("/asset-types", () =>
        {
            var preview = DefaultAssetTypes.InitialData()
                .Select(MapAssetTypeDetail)
                .ToList();
            return Results.Ok(preview);
        });

        group.MapPost("/seed", async (TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            if (await db.AssetTypes.AsNoTracking().AnyAsync(cancellationToken))
                return Results.Conflict(new { error = "Database already seeded." });

            db.AssetTypes.AddRange(DefaultAssetTypes.InitialData());
            await db.SaveChangesAsync(cancellationToken);

            return Results.Ok(new { seeded = true });
        });

        group.MapPost("/reseed", async (TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            var existing = await db.AssetTypes.ToListAsync(cancellationToken);
            if (existing.Count > 0)
            {
                db.AssetTypes.RemoveRange(existing);
                await db.SaveChangesAsync(cancellationToken);
            }

            db.AssetTypes.AddRange(DefaultAssetTypes.InitialData());
            await db.SaveChangesAsync(cancellationToken);

            return Results.Ok(new { seeded = true });
        });

        return endpoints;
    }

    private static AssetTypeDetailDto MapAssetTypeDetail(AssetType assetType)
    {
        var attributes = assetType.ExpectedAttributes
            .OrderBy(a => a.Key)
            .Select(MapAttributeDefinition)
            .ToList();

        var rules = assetType.TaxRules
            .OrderBy(r => r.Key)
            .Select(MapTaxRule)
            .ToList();

        return new AssetTypeDetailDto(
            assetType.Id,
            assetType.Name,
            assetType.Description,
            assetType.LiquidationMode,
            attributes,
            rules);
    }

    private static AttributeDefinitionDto MapAttributeDefinition(AttributeDefinition attribute)
    {
        return new AttributeDefinitionDto(
            attribute.Id,
            attribute.Key,
            attribute.Label,
            attribute.DataType,
            attribute.IsRequired,
            attribute.RegexPattern,
            attribute.EnumDefinition is null ? null : MapEnumDefinition(attribute.EnumDefinition));
    }

    private static EnumDefinitionDto MapEnumDefinition(EnumDefinition definition)
    {
        var items = definition.Items
            .OrderBy(i => i.Order)
            .Select(i => new EnumItemDto(i.Code, i.Label, i.Order))
            .ToList();

        return new EnumDefinitionDto(definition.Key, definition.Label, items);
    }

    private static TaxRuleDto MapTaxRule(TaxRule rule)
    {
        return new TaxRuleDto(
            rule.Id,
            rule.Key,
            rule.Label,
            rule.Expression,
            rule.Description,
            rule.Enabled);
    }
}
