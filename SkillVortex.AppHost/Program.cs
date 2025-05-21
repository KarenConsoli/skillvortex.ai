using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server with the correct API
var sqlServer = builder.AddSqlServer("sqlserver")
    .AddDatabase("SkillVortexDb");

// Add API service with reference to SQL Server
var apiService = builder.AddProject<Projects.SkillVortex_ApiService>("apiservice")
    .WithReference(sqlServer);

// Add web frontend with reference to API
var webApp = builder.AddProject<Projects.SkillVortex_Web>("webfrontend")
    .WithReference(apiService);

builder.Build().Run();