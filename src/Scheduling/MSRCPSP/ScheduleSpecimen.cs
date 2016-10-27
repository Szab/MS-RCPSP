using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Szab.Scheduling.Representation;
using Szab.EvolutionaryAlgorithm.Base;

namespace Szab.Scheduling.MSRCPSP
{
    public class ScheduleSpecimen : ISpecimen<ScheduleSpecimen>
    {
        private static Random random = new Random(Guid.NewGuid().GetHashCode());

        public ProjectSpecification ProjectData
        {
            get;
            private set;
        }
        
        public Task[] Tasks
        {
            get;
        }

        public double RateQuality()
        {
            Schedule schedule = ScheduleBuilder.BuildScheduleFromSpecimen(this.ProjectData, this);

            return 1 / (double)schedule.Length;
        }

        public ScheduleSpecimen CrossOver(ScheduleSpecimen otherSpeciman)
        {
            ScheduleSpecimen child = new ScheduleSpecimen(this.ProjectData, this.Tasks.Length);

            int skip = random.Next(this.Tasks.Length);
            int take = random.Next(this.Tasks.Length);

            Task[] tasksToCopy = this.Tasks.Skip(skip).Take(take).ToArray();

            tasksToCopy.CopyTo(child.Tasks, skip);

            for (int i = 0; i < child.Tasks.Length; i++)
            {
                if (child.Tasks[i] == null)
                {
                    for (int j = 0; j < otherSpeciman.Tasks.Length; j++)
                    {
                        Task currentTask = otherSpeciman.Tasks[j];

                        if (!child.Tasks.Contains(currentTask))
                        {
                            child.Tasks[i] = currentTask;
                        }
                    }
                }
            }

            return child;
        }

        // Mutation operator: inverting a random sequence
        public void Mutate()
        {
            int startIndex = random.Next(this.Tasks.Length);
            int endIndex = random.Next(this.Tasks.Length);

            if (startIndex < endIndex)
            {
                int temp = startIndex;
                startIndex = endIndex;
                endIndex = startIndex;
            }

            for(int i = startIndex, j = endIndex; i < j; i++, j--)
            {
                Task temp = this.Tasks[i];
                this.Tasks[i] = this.Tasks[j];
                this.Tasks[j] = this.Tasks[i];
            }
        }

        public ScheduleSpecimen(ProjectSpecification projectData, int size)
        {
            this.ProjectData = projectData;
            this.Tasks = new Task[size];
        }

        public ScheduleSpecimen(ScheduleSpecimen otherSpecimen)
        {
            this.Tasks = new Task[otherSpecimen.Tasks.Length];
            this.Tasks.CopyTo(this.Tasks, 0);
        }
    }
}
