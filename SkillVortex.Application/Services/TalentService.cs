using Microsoft.EntityFrameworkCore;
using SkillVortex.Domain.Entities;
using SkillVortex.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillVortex.Application.Services
{
    public class TalentService
    {
        private readonly SkillVortexDbContext _context;

        public TalentService(SkillVortexDbContext context)
        {
            _context = context;
        }

        public async Task<Talent> GetTalentByIdAsync(Guid id)
        {
            return await _context.Talents
                .Include(t => t.User)
                .Include(t => t.Skills)
                    .ThenInclude(ts => ts.Skill)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Talent>> GetVerifiedTalentsAsync()
        {
            return await _context.Talents
                .Include(t => t.User)
                .Where(t => t.HasPassedWelcomeCall && t.IsIAVerified)
                .ToListAsync();
        }

        public async Task<bool> SetIAVerificationStatusAsync(Guid talentId, bool isVerified)
        {
            var talent = await _context.Talents.FindAsync(talentId);
            if (talent == null) return false;

            talent.IsIAVerified = isVerified;
            talent.IAVerificationDate = isVerified ? DateTime.UtcNow : null;

            // Add or remove the IA-Verified badge
            if (isVerified && !talent.Badges.Contains("IA-Verified"))
            {
                talent.Badges.Add("IA-Verified");
            }
            else if (!isVerified && talent.Badges.Contains("IA-Verified"))
            {
                talent.Badges.Remove("IA-Verified");
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetWelcomeCallStatusAsync(Guid talentId, bool hasPassed, string notes = null)
        {
            var talent = await _context.Talents.FindAsync(talentId);
            if (talent == null) return false;

            talent.HasPassedWelcomeCall = hasPassed;
            talent.WelcomeCallDate = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(notes))
            {
                talent.Notes = string.IsNullOrEmpty(talent.Notes)
                    ? notes
                    : $"{talent.Notes}\n{DateTime.UtcNow:g}: {notes}";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Guid> CreateTalentAsync(Talent talent)
        {
            talent.Id = Guid.NewGuid();
            talent.CreatedAt = DateTime.UtcNow;

            _context.Talents.Add(talent);
            await _context.SaveChangesAsync();

            return talent.Id;
        }
    }
}