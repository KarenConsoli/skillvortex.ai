using Microsoft.EntityFrameworkCore;
using SkillVortex.Domain.Entities;
using SkillVortex.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillVortex.Application.Services
{
    public class ApplicationService
    {
        private readonly SkillVortexDbContext _context;

        public ApplicationService(SkillVortexDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Application> GetApplicationByIdAsync(Guid id)
        {
            return await _context.Applications
                .Include(a => a.Talent)
                    .ThenInclude(t => t.User)
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Domain.Entities.Application>> GetApplicationsByTalentIdAsync(Guid talentId)
        {
            return await _context.Applications
                .Include(a => a.Job)
                .Where(a => a.TalentId == talentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Application>> GetApplicationsByJobIdAsync(Guid jobId)
        {
            return await _context.Applications
                .Include(a => a.Talent)
                    .ThenInclude(t => t.User)
                .Where(a => a.JobId == jobId)
                .ToListAsync();
        }

        public async Task<Guid> ApplyToJobAsync(Guid talentId, Guid jobId)
        {
            // Check if talent is verified
            var talent = await _context.Talents.FindAsync(talentId);
            if (talent == null || !talent.HasPassedWelcomeCall)
                throw new InvalidOperationException("Only verified talents can apply to jobs");

            // Check if job exists and is open
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null || job.Status != "Open")
                throw new InvalidOperationException("Job is not available for applications");

            // Check if already applied
            var existingApplication = await _context.Applications
                .FirstOrDefaultAsync(a => a.TalentId == talentId && a.JobId == jobId);

            if (existingApplication != null)
                return existingApplication.Id;

            // Create new application
            var application = new Domain.Entities.Application
            {
                Id = Guid.NewGuid(),
                TalentId = talentId,
                JobId = jobId,
                Status = Domain.Entities.ApplicationStatus.Applied,
                AppliedDate = DateTime.UtcNow
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return application.Id;
        }

        public async Task<bool> UpdateApplicationStatusAsync(Guid applicationId, Domain.Entities.ApplicationStatus newStatus)
        {
            var application = await _context.Applications
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null) return false;

            // Validate state transition
            if (!IsValidTransition(application.Status, newStatus))
            {
                return false;
            }

            application.Status = newStatus;
            application.StatusUpdatedDate = DateTime.UtcNow;

            // If status is Hired, update job's filled seats
            if (newStatus == Domain.Entities.ApplicationStatus.Hired)
            {
                application.Job.FilledSeats++;

                // If all seats are filled, update job status
                if (application.Job.FilledSeats >= application.Job.TotalSeats)
                {
                    application.Job.Status = "Filled";
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private bool IsValidTransition(Domain.Entities.ApplicationStatus current, Domain.Entities.ApplicationStatus next)
        {
            switch (current)
            {
                case Domain.Entities.ApplicationStatus.Applied:
                    return next == Domain.Entities.ApplicationStatus.InterviewScheduled ||
                           next == Domain.Entities.ApplicationStatus.Rejected;
                case Domain.Entities.ApplicationStatus.InterviewScheduled:
                    return next == Domain.Entities.ApplicationStatus.WaitingFeedback ||
                           next == Domain.Entities.ApplicationStatus.PositionCanceled;
                case Domain.Entities.ApplicationStatus.WaitingFeedback:
                    return next == Domain.Entities.ApplicationStatus.Hired ||
                           next == Domain.Entities.ApplicationStatus.Rejected;
                default:
                    return false;
            }
        }
    }
}