using Microsoft.EntityFrameworkCore;
using SkillVortex.Domain.Entities;
using SkillVortex.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillVortex.Application.Services
{
    public class JobService
    {
        private readonly SkillVortexDbContext _context;

        public JobService(SkillVortexDbContext context)
        {
            _context = context;
        }

        public async Task<Job> GetJobByIdAsync(Guid id)
        {
            return await _context.Jobs
                .Include(j => j.Matcher)
                .Include(j => j.RequiredSkills)
                    .ThenInclude(rs => rs.Skill)
                .Include(j => j.OptionalSkills)
                    .ThenInclude(os => os.Skill)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<IEnumerable<Job>> GetOpenJobsAsync()
        {
            return await _context.Jobs
                .Include(j => j.RequiredSkills)
                    .ThenInclude(rs => rs.Skill)
                .Where(j => j.Status == "Open")
                .ToListAsync();
        }

        public async Task<Guid> CreateJobAsync(Job job)
        {
            job.Id = Guid.NewGuid();
            job.CreatedAt = DateTime.UtcNow;
            job.Status = "Open";

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            return job.Id;
        }

        public async Task<bool> UpdateJobStatusAsync(Guid jobId, string status)
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null) return false;

            var validStatuses = new[] { "Open", "InterviewFlow", "WaitingFeedback", "Filled", "OnHold", "Canceled" };
            if (!validStatuses.Contains(status))
                return false;

            job.Status = status;
            job.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}