using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SkillVortex.Application.Services;
using SkillVortex.Infrastructure.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add database context
// Add database context with explicit connection string
builder.Services.AddDbContext<SkillVortexDbContext>(options =>
    options.UseSqlServer("Server=localhost,53091;Database=SkillVortexDb;User Id=sa;Password=Password123!;TrustServerCertificate=True"));

// Register application services
builder.Services.AddScoped<TalentService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<JobService>();

// Configure JSON serialization
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SkillVortex API",
        Version = "v1",
        Description = "API for SkillVortex talent matching platform"
    });
});

var app = builder.Build();

// Apply migrations automatically at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SkillVortexDbContext>();
    try
    {
        dbContext.Database.Migrate();
        Console.WriteLine("Database migration applied successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
    }
}

// Configure the HTTP request pipeline
app.UseDeveloperExceptionPage();

// Configure Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillVortex API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();