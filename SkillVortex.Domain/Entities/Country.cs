using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillVortex.Domain.Entities
{
    public class Country
    {
        public string Code { get; set; } // ISO code
        public string Name { get; set; }
        public string Region { get; set; } // Latin America, etc.
        public string TimeZone { get; set; } // UTC offset
    }
}