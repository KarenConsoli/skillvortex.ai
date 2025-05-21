using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SkillVortex.Infrastructure.Data
{
    public class SqlServerDbContextFactory : IDesignTimeDbContextFactory<SkillVortexDbContext>
    {
        public SkillVortexDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SkillVortexDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=SkillVortexDb;User Id=sa;Password=Password123!;TrustServerCertificate=True");

            return new SkillVortexDbContext(optionsBuilder.Options);
        }
    }
}