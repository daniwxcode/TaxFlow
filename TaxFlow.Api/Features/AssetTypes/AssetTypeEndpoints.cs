using Core.Domain.Tax.Assets;

using Microsoft.EntityFrameworkCore;

using TaxFlow.Infrastructure.Persistence;

namespace TaxFlow.Api.Features.AssetTypes;

public static class AssetTypeEndpoints
{
    public static IEndpointRouteBuilder MapAssetTypeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/asset-types");

        group.MapGet("/", async (TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            var items = await db.AssetTypes
                .AsNoTracking()
                .OrderBy(a => a.Name)
                .Select(a => new AssetTypeDto(a.Id, a.Name, a.Description, a.LiquidationMode))
                .ToListAsync(cancellationToken);

            return Results.Ok(items);
        });

        group.MapGet("/{id:guid}", async (Guid id, TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            var item = await db.AssetTypes
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new AssetTypeDto(a.Id, a.Name, a.Description, a.LiquidationMode))
                .FirstOrDefaultAsync(cancellationToken);

            return item is null ? Results.NotFound() : Results.Ok(item);
        });

        group.MapPost("/", async (CreateAssetTypeRequest request, TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Results.BadRequest(new { error = "Name is required." });

            var assetType = AssetType.Create(request.Name, request.Description, request.LiquidationMode);

            db.AssetTypes.Add(assetType);
            await db.SaveChangesAsync(cancellationToken);

            var dto = new AssetTypeDto(assetType.Id, assetType.Name, assetType.Description, assetType.LiquidationMode);
            return Results.Created($"/asset-types/{assetType.Id}", dto);
        });

        return endpoints;
    }
}
