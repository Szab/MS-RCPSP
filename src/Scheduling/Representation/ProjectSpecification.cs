using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szab.Scheduling.Representation
{
    public class ProjectSpecification
    {
        public List<Task> Tasks { get; private set; }
        public List<Resource> Resources { get; private set; }

        public int MaxDuration
        {
            get
            {
                return this.Tasks.Sum(x => x.Length);
            }
        }

        public double MaxCost
        {
            get
            {
                return this.Tasks.Sum(x => x.AvailableResources.Max(y => y.Cost));
            }
        }


        public ProjectSpecification()
        {
            this.Tasks = new List<Task>();
            this.Resources = new List<Resource>();
        }
    }
}
