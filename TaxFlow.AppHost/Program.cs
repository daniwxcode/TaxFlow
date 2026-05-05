using Aspire.Hosting;
using System;

SetDefaultEnv("ASPNETCORE_URLS", "http://localhost:18888");
SetDefaultEnv("ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL", "http://localhost:18889");
SetDefaultEnv("ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL", "http://localhost:18890");
SetDefaultEnv("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true");

var builder = DistributedApplication.CreateBuilder(args);

var pgUser = builder.AddParameter("postgres-user", "taxflow");
var pgPassword = builder.AddParameter("postgres-password", "taxflow");

var postgres = builder
    .AddPostgres("taxflow-postgres", pgUser, pgPassword)
    .WithImage("postgres:16-alpine")
    .WithEnvironment("POSTGRES_DB", "taxflow")
    .WithDataVolume("taxflow-postgres-data");

var taxflowDb = postgres.AddDatabase("taxflowdb", "taxflow");

builder.AddProject("taxflow-api", "../TaxFlow.Api/TaxFlow.Api.csproj")
    .WithReference(taxflowDb);

builder.Build().Run();

static void SetDefaultEnv(string key, string value)
{
    if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key)))
        Environment.SetEnvironmentVariable(key, value);
}
