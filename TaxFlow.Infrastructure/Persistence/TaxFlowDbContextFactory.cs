using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using System;
using System.IO;

namespace TaxFlow.Infrastructure.Persistence;

public sealed class TaxFlowDbContextFactory : IDesignTimeDbContextFactory<TaxFlowDbContext>
{
    public TaxFlowDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            Environment.GetEnvironmentVariable("TAXFLOW_CONNECTION_STRING") ??
            configuration.GetConnectionString("TaxFlowDb");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Connection string manquante. Définissez TAXFLOW_CONNECTION_STRING ou ConnectionStrings:TaxFlowDb.");

        var optionsBuilder = new DbContextOptionsBuilder<TaxFlowDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new TaxFlowDbContext(optionsBuilder.Options);
    }
}
