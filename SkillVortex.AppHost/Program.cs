using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server container
var sqlServer = builder.AddSqlServer("sqlserver",
    password: "Password123!",  // Set explicit password
    port: 1433)
    .AddDatabase("SkillVortexDb");

// Add API service with reference to SQL Server
var apiService = builder.AddProject<Projects.SkillVortex_ApiService>("apiservice")
    .WithReference(sqlServer);

// Add web frontend with reference to API
var webApp = builder.AddProject<Projects.SkillVortex_Web>("webfrontend")
    .WithReference(apiService);

builder.Build().Run();