using Core.Bootstrap;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

using TaxFlow.Api.Features.AssetTypes;
using TaxFlow.Api.Features.Bootstrap;
using TaxFlow.Api.Features.FormParameters;
using TaxFlow.Infrastructure.Persistence;
using TaxFlow.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceDefaults();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<TaxFlowDbContext>(options =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("taxflowdb") ??
        builder.Configuration.GetConnectionString("TaxFlowDb");

    options.UseNpgsql(connectionString);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaxFlowDbContext>();
    db.Database.Migrate();

    if (!db.AssetTypes.AsNoTracking().Any())
    {
        db.AssetTypes.AddRange(DefaultAssetTypes.InitialData());
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "TaxFlow API";
    });
}

app.MapGet("/", () => Results.Ok(new { status = "ok" }));
app.MapAssetTypeEndpoints();
app.MapBootstrapEndpoints();
app.MapFormParameterEndpoints();

app.MapDefaultEndpoints();

app.Run();
