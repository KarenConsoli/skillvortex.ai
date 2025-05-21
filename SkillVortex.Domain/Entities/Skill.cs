using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillVortex.Domain.Entities
{
    public class Skill
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; } // AI, ML, Cloud, etc.

        // Navigation properties
        public virtual ICollection<TalentSkill> TalentSkills { get; set; } = new List<TalentSkill>();
        public virtual ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    }

    public class TalentSkill
    {
        public Guid TalentId { get; set; }
        public virtual Talent Talent { get; set; }
        public Guid SkillId { get; set; }
        public virtual Skill Skill { get; set; }
        public int YearsOfExperience { get; set; }
        public int ProficiencyLevel { get; set; } // 1-5
    }
}