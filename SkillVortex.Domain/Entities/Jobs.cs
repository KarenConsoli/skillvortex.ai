using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillVortex.Domain.Entities
{
    public class Job
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string CompanyAlias { get; set; }
        public string JobRaw { get; set; } // Original JD text
        public string SeniorityRequired { get; set; }
        public string JobRole { get; set; }
        public string EmploymentType { get; set; } = "Contractor"; // Default for MVP
        public int TotalSeats { get; set; } = 1;
        public int FilledSeats { get; set; } = 0;
        public Guid MatcherId { get; set; } // User who created the job
        public virtual User Matcher { get; set; }
        public virtual ICollection<JobSkill> RequiredSkills { get; set; } = new List<JobSkill>();
        public virtual ICollection<JobSkill> OptionalSkills { get; set; } = new List<JobSkill>();
        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
        public string Status { get; set; } = "Open"; // Open, InterviewFlow, WaitingFeedback, Filled, OnHold, Canceled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public class JobSkill
    {
        public Guid JobId { get; set; }
        public virtual Job Job { get; set; }
        public Guid SkillId { get; set; }
        public virtual Skill Skill { get; set; }
        public bool IsRequired { get; set; }
    }
}