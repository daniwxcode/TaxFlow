using Microsoft.EntityFrameworkCore;

using TaxFlow.Api.Features.AssetTypes;
using TaxFlow.Infrastructure.Persistence;
using TaxFlow.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceDefaults();

builder.Services.AddDbContext<TaxFlowDbContext>(options =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("taxflowdb") ??
        builder.Configuration.GetConnectionString("TaxFlowDb");

    options.UseNpgsql(connectionString);
});

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { status = "ok" }));
app.MapAssetTypeEndpoints();

app.MapDefaultEndpoints();

app.Run();
