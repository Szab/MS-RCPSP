using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szab.Scheduling.Representation
{
    public class Skill
    {
        public string Name
        {
            get;
            protected set;
        }

        public int Level
        {
            get;
            protected set;
        }

        public Skill(string name, int level)
        {
            this.Name = name ?? Guid.NewGuid().ToString();
            this.Level = level;
        }

        public Skill(Skill skill) : this(skill.Name, skill.Level)
        {
        }
    }
}
