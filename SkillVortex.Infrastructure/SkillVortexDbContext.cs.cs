using Microsoft.EntityFrameworkCore;
using SkillVortex.Domain.Entities;

namespace SkillVortex.Infrastructure.Data
{
    public class SkillVortexDbContext : DbContext
    {
        // Default constructor for migrations
        public SkillVortexDbContext()
        {
        }

        // Constructor with options for runtime
        public SkillVortexDbContext(DbContextOptions<SkillVortexDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Talent> Talents { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Domain.Entities.Application> Applications { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<TalentSkill> TalentSkills { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Default Aspire SQL Server credentials
                optionsBuilder.UseSqlServer("Server=localhost;Database=SkillVortexDb;User Id=sa;Password=Password123!;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure primary keys
            modelBuilder.Entity<Country>()
                .HasKey(c => c.Code);

            // Configure composite keys
            modelBuilder.Entity<TalentSkill>()
                .HasKey(ts => new { ts.TalentId, ts.SkillId });

            modelBuilder.Entity<JobSkill>()
                .HasKey(js => new { js.JobId, js.SkillId });

            // Configure relationships - modify cascade behavior
            modelBuilder.Entity<Talent>()
                .HasOne(t => t.User)
                .WithOne()
                .HasForeignKey<Talent>(t => t.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Job>()
                .HasOne(j => j.Matcher)
                .WithMany()
                .HasForeignKey(j => j.MatcherId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Application relationships with modified cascade behavior
            modelBuilder.Entity<Domain.Entities.Application>()
                .HasOne(a => a.Talent)
                .WithMany(t => t.Applications)
                .HasForeignKey(a => a.TalentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Domain.Entities.Application>()
                .HasOne(a => a.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure relationship between Job and JobSkill
            modelBuilder.Entity<JobSkill>()
                .HasOne(js => js.Job)
                .WithMany(j => j.RequiredSkills)
                .HasForeignKey(js => js.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship between Skill and TalentSkill
            modelBuilder.Entity<TalentSkill>()
                .HasOne(ts => ts.Talent)
                .WithMany(t => t.Skills)
                .HasForeignKey(ts => ts.TalentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TalentSkill>()
                .HasOne(ts => ts.Skill)
                .WithMany(s => s.TalentSkills)
                .HasForeignKey(ts => ts.SkillId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure string list conversion for Badges
            modelBuilder.Entity<Talent>()
                .Property(t => t.Badges)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );
        }
    }
}