using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillVortex.Domain.Entities
{
    public class Talent
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        public string LinkedInUrl { get; set; }
        public string GitHubUrl { get; set; }
        public string PortfolioUrl { get; set; }
        public decimal HourlyRate { get; set; }
        public string CountryCode { get; set; }
        public int EnglishLevel { get; set; } // 1-5
        public bool HasPassedWelcomeCall { get; set; } = false;
        public DateTime? WelcomeCallDate { get; set; }

        // Manual checkbox for IA Verified badge
        public bool IsIAVerified { get; set; } = false;
        public DateTime? IAVerificationDate { get; set; }

        public List<string> Badges { get; set; } = new List<string>();
        public virtual ICollection<TalentSkill> Skills { get; set; } = new List<TalentSkill>();
        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}