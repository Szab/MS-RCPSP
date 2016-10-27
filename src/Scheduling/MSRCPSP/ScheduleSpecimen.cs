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

            int length = schedule.Length;
            double cost = schedule.SummaryCost;

            return 1/(0.9 * (length/ProjectData.MaxDuration) + 0.1 * (cost/ProjectData.MaxCost));
        }

        public ScheduleSpecimen CrossOver(ScheduleSpecimen otherSpeciman)
        {
            ScheduleSpecimen child = new ScheduleSpecimen(this.ProjectData, this.Tasks.Length);

            int numTasks = this.Tasks.Length;
            int skip = random.Next(this.Tasks.Length);
            int take = random.Next(this.Tasks.Length / 2);
            int newPosition = random.Next(this.Tasks.Length);

            Task[] tasksToCopy = this.Tasks.Skip(skip).Take(take).ToArray();

            for (int i = 0, currentPosition = newPosition; i < tasksToCopy.Length; i++)
            {
                child.Tasks[newPosition] = tasksToCopy[i];
                newPosition = (newPosition + 1) % numTasks;
            }

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
                            break;
                        }
                    }
                }
            }

            return child;
        }

        // Mutation operator: swapping random pairs
        public void Mutate()
        {
            int pairs = random.Next(this.Tasks.Length);

            for(int i = 0; i < pairs; i++)
            {
                int firstIndex = random.Next(this.Tasks.Length);
                int secondIndex = random.Next(this.Tasks.Length);

                Task temp = this.Tasks[secondIndex];
                this.Tasks[secondIndex] = this.Tasks[firstIndex];
                this.Tasks[firstIndex] = temp;
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
