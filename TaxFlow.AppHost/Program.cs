var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("taxflow-postgres")
    .WithImage("postgres:16-alpine")
    .WithEnvironment("POSTGRES_DB", "taxflow")
    .WithEnvironment("POSTGRES_USER", "taxflow")
    .WithEnvironment("POSTGRES_PASSWORD", "taxflow")
    .WithDataVolume("taxflow-postgres-data");

var taxflowDb = postgres.AddDatabase("taxflowdb", "taxflow");

builder.AddProject<Projects.TaxFlow_Api>("taxflow-api")
    .WithReference(taxflowDb)
    .WaitFor(taxflowDb);

builder.Build().Run();
