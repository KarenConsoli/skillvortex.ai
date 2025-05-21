using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillVortex.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Role { get; set; } // Talent, Scout, Matcher, SiteAdmin
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
