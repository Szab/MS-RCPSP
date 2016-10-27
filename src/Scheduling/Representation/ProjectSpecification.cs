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

        public void SortData()
        {
            this.Tasks = this.Tasks.OrderBy(x => x.Length).ToList();
            this.Resources = this.Resources.OrderBy(x => x.Cost).ToList();
        }

        public int MaxDuration
        {
            get
            {
                return this.Tasks.Sum(x => x.Length);
            }
        }

        public int MaxCost
        {
            get
            {
                // TODO
                return int.MaxValue;
            }
        }


        public ProjectSpecification()
        {
            this.Tasks = new List<Task>();
            this.Resources = new List<Resource>();
        }
    }
}
