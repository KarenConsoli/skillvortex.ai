using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillVortex.Domain.Entities
{
    public enum ApplicationStatus
    {
        Applied,
        InterviewScheduled,
        WaitingFeedback,
        Hired,
        Rejected,
        PositionCanceled,
        PositionOnHold
    }

    public class Application
    {
        public Guid Id { get; set; }
        public Guid TalentId { get; set; }
        public Talent Talent { get; set; }
        public Guid JobId { get; set; }
        public Job Job { get; set; }
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;
        public DateTime? InterviewDate { get; set; }
        public string Notes { get; set; }
        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
        public DateTime? StatusUpdatedDate { get; set; }
    }
}