using Core.Domain.Contracts;
using Core.Domain.Enums;
using Core.Domain.Tax.Assets;
using Core.Domain.Tax.Calculation;
using Core.Domain.Tax.Obligations;
using Core.Domain.Tax.Penalties;

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

        group.MapGet("/{id:guid}/details", async (Guid id, TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            var assetType = await db.AssetTypes
                .AsNoTracking()
                .Include(a => a.ExpectedAttributes)
                    .ThenInclude(a => a.EnumDefinition)
                        .ThenInclude(e => e.Items)
                .Include(a => a.TaxRules)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            return assetType is null
                ? Results.NotFound()
                : Results.Ok(MapAssetTypeDetail(assetType));
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

        group.MapPut("/{id:guid}", async (Guid id, UpdateAssetTypeRequest request, TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Results.BadRequest(new { error = "Name is required." });

            var assetType = await db.AssetTypes.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (assetType is null)
                return Results.NotFound();

            assetType.Rename(request.Name);
            assetType.UpdateDescription(request.Description);
            assetType.UpdateLiquidationMode(request.LiquidationMode);

            await db.SaveChangesAsync(cancellationToken);

            var dto = new AssetTypeDto(assetType.Id, assetType.Name, assetType.Description, assetType.LiquidationMode);
            return Results.Ok(dto);
        });

        group.MapDelete("/{id:guid}", async (Guid id, TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            var assetType = await db.AssetTypes.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (assetType is null)
                return Results.NotFound();

            db.AssetTypes.Remove(assetType);
            await db.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        });

        group.MapGet("/{id:guid}/attributes", async (Guid id, TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            var assetType = await db.AssetTypes
                .AsNoTracking()
                .Include(a => a.ExpectedAttributes)
                    .ThenInclude(a => a.EnumDefinition)
                        .ThenInclude(e => e.Items)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (assetType is null)
                return Results.NotFound();

            var attributes = assetType.ExpectedAttributes
                .OrderBy(a => a.Key)
                .Select(MapAttributeDefinition)
                .ToList();

            return Results.Ok(attributes);
        });

        group.MapPost("/{id:guid}/attributes", async (Guid id, CreateAttributeDefinitionRequest request, TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            var assetType = await db.AssetTypes
                .Include(a => a.ExpectedAttributes)
                    .ThenInclude(a => a.EnumDefinition)
                        .ThenInclude(e => e.Items)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (assetType is null)
                return Results.NotFound();

            AttributeDefinition attribute;

            if (request.DataType == AttributeDataType.Enum)
            {
                if (request.EnumDefinition is null)
                    return Results.BadRequest(new { error = "EnumDefinition is required for enum attributes." });

                if (request.EnumDefinition.Items is null || request.EnumDefinition.Items.Count == 0)
                    return Results.BadRequest(new { error = "EnumDefinition must include at least one item." });

                var enumItems = request.EnumDefinition.Items
                    .Select(i => EnumDefinition.CreateItem(i.Code, i.Label, i.Order));

                var enumDefinition = EnumDefinition.Create(
                    request.EnumDefinition.Key,
                    request.EnumDefinition.Label,
                    enumItems);

                attribute = AttributeDefinition.Create(enumDefinition, request.IsRequired);

                if (!string.IsNullOrWhiteSpace(request.RegexPattern))
                    attribute.SetRegexPattern(request.RegexPattern);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.Key) || string.IsNullOrWhiteSpace(request.Label))
                    return Results.BadRequest(new { error = "Key and label are required." });

                if (request.EnumDefinition is not null)
                    return Results.BadRequest(new { error = "EnumDefinition is only allowed for enum attributes." });

                attribute = AttributeDefinition.Create(
                    request.Key,
                    request.Label,
                    request.DataType,
                    request.IsRequired,
                    request.RegexPattern);
            }

            try
            {
                assetType.AddExpectedAttribute(attribute);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }

            await db.SaveChangesAsync(cancellationToken);
            return Results.Created($"/asset-types/{id}/attributes/{attribute.Key}", MapAttributeDefinition(attribute));
        });

        group.MapPut("/{id:guid}/attributes/{key}", async (Guid id, string key, UpdateAttributeDefinitionRequest request, TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            var assetType = await db.AssetTypes
                .Include(a => a.ExpectedAttributes)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (assetType is null)
                return Results.NotFound();

            var attribute = assetType.ExpectedAttributes
                .FirstOrDefault(a => a.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (attribute is null)
                return Results.NotFound();

            try
            {
                attribute.UpdateLabel(request.Label)
                    .SetRegexPattern(request.RegexPattern);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }

            await db.SaveChangesAsync(cancellationToken);
            return Results.Ok(MapAttributeDefinition(attribute));
        });

        group.MapDelete("/{id:guid}/attributes/{key}", async (Guid id, string key, TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            var assetType = await db.AssetTypes
                .Include(a => a.ExpectedAttributes)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (assetType is null)
                return Results.NotFound();

            var removed = assetType.RemoveExpectedAttribute(key);
            if (!removed)
                return Results.NotFound();

            await db.SaveChangesAsync(cancellationToken);
            return Results.NoContent();
        });

        group.MapGet("/{id:guid}/tax-rules", async (Guid id, TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            var assetType = await db.AssetTypes
                .AsNoTracking()
                .Include(a => a.TaxRules)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (assetType is null)
                return Results.NotFound();

            var rules = assetType.TaxRules
                .OrderBy(r => r.Key)
                .Select(MapTaxRule)
                .ToList();

            return Results.Ok(rules);
        });

        group.MapPost("/{id:guid}/tax-rules", async (Guid id, CreateTaxRuleRequest request, TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            var assetType = await db.AssetTypes
                .Include(a => a.TaxRules)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (assetType is null)
                return Results.NotFound();

            if (string.IsNullOrWhiteSpace(request.Key) || string.IsNullOrWhiteSpace(request.Label))
                return Results.BadRequest(new { error = "Key and label are required." });

            if (string.IsNullOrWhiteSpace(request.Expression))
                return Results.BadRequest(new { error = "Expression is required." });

            var rule = new TaxRule
            {
                Key = request.Key.Trim(),
                Label = request.Label.Trim(),
                Expression = request.Expression,
                Description = request.Description,
                Enabled = request.Enabled
            };

            try
            {
                assetType.AddTaxRule(rule);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }

            await db.SaveChangesAsync(cancellationToken);
            return Results.Created($"/asset-types/{id}/tax-rules/{rule.Key}", MapTaxRule(rule));
        });

        group.MapPut("/{id:guid}/tax-rules/{key}", async (Guid id, string key, UpdateTaxRuleRequest request, TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            var assetType = await db.AssetTypes
                .Include(a => a.TaxRules)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (assetType is null)
                return Results.NotFound();

            var rule = assetType.FindTaxRule(key);
            if (rule is null)
                return Results.NotFound();

            if (string.IsNullOrWhiteSpace(request.Label) || string.IsNullOrWhiteSpace(request.Expression))
                return Results.BadRequest(new { error = "Label and expression are required." });

            rule.Label = request.Label.Trim();
            rule.Expression = request.Expression;
            rule.Description = request.Description;
            rule.Enabled = request.Enabled;

            await db.SaveChangesAsync(cancellationToken);
            return Results.Ok(MapTaxRule(rule));
        });

        group.MapDelete("/{id:guid}/tax-rules/{key}", async (Guid id, string key, TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            var assetType = await db.AssetTypes
                .Include(a => a.TaxRules)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (assetType is null)
                return Results.NotFound();

            var removed = assetType.RemoveTaxRule(key);
            if (!removed)
                return Results.NotFound();

            await db.SaveChangesAsync(cancellationToken);
            return Results.NoContent();
        });

        group.MapGet("/{id:guid}/tax-rules/{key}/obligations", async (Guid id, string key, TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            var assetType = await db.AssetTypes
                .AsNoTracking()
                .Include(a => a.TaxRules)
                    .ThenInclude(r => r.ObligationSchedule)
                        .ThenInclude(s => s.DeclarationDeadlines)
                            .ThenInclude(d => d.LegalReferences)
                .Include(a => a.TaxRules)
                    .ThenInclude(r => r.ObligationSchedule)
                        .ThenInclude(s => s.PaymentDeadlines)
                            .ThenInclude(p => p.LegalReferences)
                .Include(a => a.TaxRules)
                    .ThenInclude(r => r.ObligationSchedule)
                        .ThenInclude(s => s.LegalReferences)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (assetType is null)
                return Results.NotFound();

            var rule = assetType.FindTaxRule(key);
            if (rule?.ObligationSchedule is null)
                return Results.NotFound();

            return Results.Ok(MapSchedule(rule.ObligationSchedule));
        });

        group.MapPut("/{id:guid}/tax-rules/{key}/obligations", async (Guid id, string key, UpsertTaxObligationScheduleRequest request, TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            var assetType = await db.AssetTypes
                .Include(a => a.TaxRules)
                    .ThenInclude(r => r.ObligationSchedule)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (assetType is null)
                return Results.NotFound();

            var rule = assetType.FindTaxRule(key);
            if (rule is null)
                return Results.NotFound();

            if (request.Declarations is null || request.Payments is null)
                return Results.BadRequest(new { error = "Declarations and payments are required (can be empty arrays)." });

            var schedule = new TaxObligationSchedule
            {
                Name = request.Name,
                Description = request.Description,
                FiscalYear = request.FiscalYear
            };

            if (request.LegalReferences is not null)
            {
                foreach (var reference in request.LegalReferences)
                {
                    schedule.AddLegalReference(MapLegalReference(reference));
                }
            }

            foreach (var declaration in request.Declarations)
            {
                if (string.IsNullOrWhiteSpace(declaration.Key) || string.IsNullOrWhiteSpace(declaration.Label))
                    return Results.BadRequest(new { error = "Declaration key and label are required." });

                if (declaration.Order < 1)
                    return Results.BadRequest(new { error = "Declaration order must be at least 1." });

                var deadline = new DeclarationDeadline
                {
                    Key = declaration.Key.Trim(),
                    Label = declaration.Label.Trim(),
                    DueDate = declaration.DueDate,
                    Periodicity = declaration.Periodicity,
                    Regime = declaration.Regime,
                    Order = declaration.Order,
                    GracePeriod = MapDuration(declaration.GracePeriod),
                    DeclarationType = declaration.DeclarationType,
                    RequiresDocuments = declaration.RequiresDocuments,
                    FormReference = string.IsNullOrWhiteSpace(declaration.FormReference) ? null : declaration.FormReference.Trim()
                };

                if (declaration.PenaltyDefinition is not null)
                    deadline.WithPenalty(MapPenaltyDefinition(declaration.PenaltyDefinition));

                if (declaration.LegalReferences is not null)
                {
                    foreach (var reference in declaration.LegalReferences)
                        deadline.AddLegalReference(MapLegalReference(reference));
                }

                schedule.AddDeclarationDeadline(deadline);
            }

            foreach (var payment in request.Payments)
            {
                if (string.IsNullOrWhiteSpace(payment.Key) || string.IsNullOrWhiteSpace(payment.Label))
                    return Results.BadRequest(new { error = "Payment key and label are required." });

                if (payment.Order < 1)
                    return Results.BadRequest(new { error = "Payment order must be at least 1." });

                if (payment.Fraction <= 0 || payment.Fraction > 1)
                    return Results.BadRequest(new { error = "Payment fraction must be between 0 and 1." });

                var deadline = new PaymentDeadline
                {
                    Key = payment.Key.Trim(),
                    Label = payment.Label.Trim(),
                    DueDate = payment.DueDate,
                    PaymentType = payment.PaymentType,
                    Fraction = payment.Fraction,
                    Order = payment.Order,
                    Periodicity = payment.Periodicity,
                    Regime = payment.Regime,
                    GracePeriod = MapDuration(payment.GracePeriod),
                    LinkedDeclarationKey = string.IsNullOrWhiteSpace(payment.LinkedDeclarationKey) ? null : payment.LinkedDeclarationKey.Trim(),
                    AllowsPartialPayment = payment.AllowsPartialPayment,
                    MinimumPayment = payment.MinimumPayment,
                    FixedAmount = payment.FixedAmount
                };

                if (payment.PenaltyDefinition is not null)
                    deadline.WithPenalty(MapPenaltyDefinition(payment.PenaltyDefinition));

                if (payment.LegalReferences is not null)
                {
                    foreach (var reference in payment.LegalReferences)
                        deadline.AddLegalReference(MapLegalReference(reference));
                }

                schedule.AddPaymentDeadline(deadline);
            }

            if (rule.ObligationSchedule is not null)
            {
                db.Remove(rule.ObligationSchedule);
            }

            rule.ConfigureObligationSchedule(schedule);
            await db.SaveChangesAsync(cancellationToken);

            return Results.Ok(MapSchedule(schedule));
        });

        group.MapDelete("/{id:guid}/tax-rules/{key}/obligations", async (Guid id, string key, TaxFlowDbContext db, CancellationToken cancellationToken) =>
        {
            var assetType = await db.AssetTypes
                .Include(a => a.TaxRules)
                    .ThenInclude(r => r.ObligationSchedule)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (assetType is null)
                return Results.NotFound();

            var rule = assetType.FindTaxRule(key);
            if (rule?.ObligationSchedule is null)
                return Results.NotFound();

            db.Remove(rule.ObligationSchedule);
            await db.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
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

    private static TaxObligationScheduleDto MapSchedule(TaxObligationSchedule schedule)
    {
        var declarations = schedule.DeclarationDeadlines
            .OrderBy(d => d.Order)
            .ThenBy(d => d.DueDate)
            .Select(MapDeclarationDeadline)
            .ToList();

        var payments = schedule.PaymentDeadlines
            .OrderBy(p => p.Order)
            .ThenBy(p => p.DueDate)
            .Select(MapPaymentDeadline)
            .ToList();

        var references = schedule.LegalReferences
            .Select(MapLegalReference)
            .ToList();

        return new TaxObligationScheduleDto(
            schedule.Name,
            schedule.Description,
            schedule.FiscalYear,
            declarations,
            payments,
            references);
    }

    private static DeclarationDeadlineDto MapDeclarationDeadline(DeclarationDeadline deadline)
    {
        return new DeclarationDeadlineDto(
            deadline.Key,
            deadline.Label,
            deadline.DueDate,
            MapDuration(deadline.GracePeriod),
            deadline.Periodicity,
            deadline.Regime,
            deadline.Order,
            deadline.DeclarationType,
            deadline.RequiresDocuments,
            deadline.FormReference,
            deadline.PenaltyDefinition is null ? null : MapPenaltyDefinition(deadline.PenaltyDefinition),
            deadline.LegalReferences.Select(MapLegalReference).ToList());
    }

    private static PaymentDeadlineDto MapPaymentDeadline(PaymentDeadline deadline)
    {
        return new PaymentDeadlineDto(
            deadline.Key,
            deadline.Label,
            deadline.DueDate,
            deadline.Fraction,
            deadline.Order,
            deadline.PaymentType,
            deadline.Periodicity,
            deadline.Regime,
            MapDuration(deadline.GracePeriod),
            deadline.LinkedDeclarationKey,
            deadline.AllowsPartialPayment,
            deadline.MinimumPayment,
            deadline.FixedAmount,
            deadline.PenaltyDefinition is null ? null : MapPenaltyDefinition(deadline.PenaltyDefinition),
            deadline.LegalReferences.Select(MapLegalReference).ToList());
    }

    private static DurationDto MapDuration(Duration duration) =>
        new(duration.Value, duration.Unit);

    private static Duration MapDuration(DurationRequest request) =>
        new(request.Value, request.Unit);

    private static PenaltyDefinitionDto MapPenaltyDefinition(PenaltyDefinition definition) =>
        new(
            definition.Type,
            definition.TriggerEvent,
            definition.FixedAmount,
            MapDuration(definition.GracePeriod),
            MapDuration(definition.Period),
            definition.AnnualRate,
            definition.PeriodRate,
            definition.PeriodRateIncrement,
            definition.Cap,
            definition.Minimum,
            definition.Capitalize);

    private static PenaltyDefinition MapPenaltyDefinition(PenaltyDefinitionRequest request)
    {
        var definition = new PenaltyDefinition
        {
            Type = request.Type,
            TriggerEvent = request.TriggerEvent,
            FixedAmount = request.FixedAmount,
            GracePeriod = MapDuration(request.GracePeriod),
            Period = MapDuration(request.Period),
            AnnualRate = request.AnnualRate,
            PeriodRate = request.PeriodRate,
            PeriodRateIncrement = request.PeriodRateIncrement,
            Cap = request.Cap,
            Minimum = request.Minimum,
            Capitalize = request.Capitalize
        };

        definition.Validate();
        return definition;
    }

    private static LegalReferenceDto MapLegalReference(LegalReference reference) =>
        new(
            reference.TextType,
            reference.Reference,
            reference.Title,
            reference.Article,
            reference.PublicationDate,
            reference.EffectiveDate,
            reference.Url,
            reference.Notes);

    private static LegalReference MapLegalReference(LegalReferenceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Reference) || string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Legal reference requires reference and title.");

        return new LegalReference
        {
            TextType = request.TextType,
            Reference = request.Reference.Trim(),
            Title = request.Title.Trim(),
            Article = string.IsNullOrWhiteSpace(request.Article) ? null : request.Article.Trim(),
            PublicationDate = request.PublicationDate,
            EffectiveDate = request.EffectiveDate ?? request.PublicationDate,
            Url = string.IsNullOrWhiteSpace(request.Url) ? null : request.Url.Trim(),
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
        };
    }
}
