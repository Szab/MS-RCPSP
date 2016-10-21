﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule.Representation
{
    public class Resource
    {
        public string Name { get; set; }
        public float Cost { get; set; }
        public List<Skill> Skills
        {
            get;
            protected set;
        }

        public Resource(string name, float cost, IEnumerable<Skill> skills = null)
        {
            this.Name = name ?? Guid.NewGuid().ToString();
            this.Cost = cost;
            this.Skills = new List<Skill>();

            if(skills != null)
            {
                this.Skills.AddRange(skills);
            }
        }

        public Resource(Resource resource) : this(resource.Name, resource.Cost, resource.Skills)
        {
        }
    }
}
