using Microsoft.EntityFrameworkCore;
using SkillVortex.Infrastructure.Data;

// Create and apply migrations
CreateAndMigrateDatabase();

Console.WriteLine("Database creation and migration completed successfully!");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();

void CreateAndMigrateDatabase()
{
    // Create connection string
    string connectionString = "Server=PUCHI\\SQLEXPRESS01;Database=SkillVortex;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    // Create DbContext options
    var optionsBuilder = new DbContextOptionsBuilder<SkillVortexDbContext>();
    optionsBuilder.UseSqlServer(connectionString);

    // Create and use DbContext
    using (var dbContext = new SkillVortexDbContext(optionsBuilder.Options))
    {
        // Ensure database is created
        bool wasCreated = dbContext.Database.EnsureCreated();
        Console.WriteLine($"Database was {(wasCreated ? "created" : "already existed")}");

        // Print count of each entity to verify schema
        Console.WriteLine($"Users: {dbContext.Users.Count()}");
        Console.WriteLine($"Skills: {dbContext.Skills.Count()}");
        Console.WriteLine($"Talents: {dbContext.Talents.Count()}");
    }
}
