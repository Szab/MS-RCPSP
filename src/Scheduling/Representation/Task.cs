﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szab.Scheduling.Representation
{
    public class Task
    {
        public string Name { get; set; }

        public List<Task> Predecessors
        {
            get;
            protected set;
        }

        public List<Skill> RequiredSkills
        {
            get;
            protected set;
        }

        public List<Resource> AvailableResources
        {
            get;
            protected set;
        }

        public int Length
        {
            get;
            set;
        }

        public Task(string name, int length, IEnumerable<Task> predecessors, IEnumerable<Skill> requiredSkills)
        {
            this.Name = name ?? Guid.NewGuid().ToString();
            this.Predecessors = new List<Task>();
            this.RequiredSkills = new List<Skill>();
            this.AvailableResources = new List<Resource>();
            this.Length = length;

            if (predecessors != null)
            {
                this.Predecessors.AddRange(predecessors);
            }

            if (requiredSkills != null)
            {
                this.RequiredSkills.AddRange(requiredSkills);
            }
        }

        public Task(string name, int length) : this(name, length, null, null)
        {
        }

        public Task(string name, int length, IEnumerable<Task> predecessors) : this(name, length, predecessors, null)
        {
        }

        public Task(string name, int length, IEnumerable<Skill> requiredSkills) : this(name, length, null, requiredSkills)
        {
        }

        public Task(Task task) : this(task.Name, task.Length, task.Predecessors, task.RequiredSkills)
        {
        }
    }
}
