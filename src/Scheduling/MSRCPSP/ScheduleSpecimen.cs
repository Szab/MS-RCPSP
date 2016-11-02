using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Szab.Scheduling.Representation;
using Szab.EvolutionaryAlgorithm;
using Szab.TabuSearch;

namespace Szab.Scheduling.MSRCPSP
{
    public class ScheduleSpecimen : ISpecimen<ScheduleSpecimen>, ITabuSolution<ScheduleSpecimen>
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

            return 1.0 / length;
        }

        // Crossover operator: copy a sequence from parent to child and copy remaining tasks from other parent in their original order.
        public ScheduleSpecimen CrossOver(ScheduleSpecimen otherSpeciman)
        {
            ScheduleSpecimen child = new ScheduleSpecimen(this.ProjectData, this.Tasks.Length);

            int maxElementsToCopy = this.Tasks.Length / 2;
            int endPosition = random.Next(this.Tasks.Length);
            int startPosition = random.Next(Math.Max(0, endPosition - maxElementsToCopy + 1), endPosition);

            Task[] tasksToCopy = this.Tasks.Skip(startPosition).Take(endPosition - startPosition + 1).ToArray();
            tasksToCopy.CopyTo(child.Tasks, startPosition);

            for(int i = 0; i < otherSpeciman.Tasks.Length; i++)
            {
                Task currentTask = otherSpeciman.Tasks[i];

                if (!tasksToCopy.Contains(currentTask))
                {
                    for (int j = 0; j < otherSpeciman.Tasks.Length; j++)
                    {
                        int newIndex = (i + j) % otherSpeciman.Tasks.Length;

                        if(child.Tasks[newIndex] == null)
                        {
                            child.Tasks[newIndex] = currentTask;
                            break;
                        }
                    }
                }
            }

            return child;
        }

        // Mutation operator: inversion of a random sequence
        public void Mutate()
        {
            int startIndex = random.Next(this.Tasks.Length - 1);
            int endIndex = random.Next(startIndex + 1, this.Tasks.Length);

            for(int i = startIndex, j = endIndex - 1; i <= j; i++, j--)
            {
                Task first = this.Tasks[i];
                Task second = this.Tasks[j];

                this.Tasks[i] = second;
                this.Tasks[j] = first;
            }
        }

        public bool CheckEquality(ScheduleSpecimen other)
        {
            for(int i = 0; i < this.Tasks.Length; i++)
            {
                if(this.Tasks[i] != other.Tasks[i])
                {
                    return false;
                }
            }

            return true;
        }

        public IEnumerable<ScheduleSpecimen> GetNeighbours()
        {
            int index = random.Next(this.Tasks.Length);
            List<ScheduleSpecimen> neighbours = new List<ScheduleSpecimen>();

            for(int i = 0; i < this.Tasks.Length; i++)
            {
                ScheduleSpecimen neighbour = new ScheduleSpecimen(this);
                neighbour.ProjectData = this.ProjectData;
                Task temp = neighbour.Tasks[index];
                neighbour.Tasks[index] = neighbour.Tasks[i];
                neighbour.Tasks[i] = temp;
                neighbours.Add(neighbour);
            }

            return neighbours;
        }

        public ScheduleSpecimen(ProjectSpecification projectData, int size)
        {
            this.ProjectData = projectData;
            this.Tasks = new Task[size];
        }

        public ScheduleSpecimen(ScheduleSpecimen otherSpecimen) : this(otherSpecimen.ProjectData, otherSpecimen.Tasks.Count())
        {
            this.Tasks = new Task[otherSpecimen.Tasks.Length];
            otherSpecimen.Tasks.CopyTo(this.Tasks, 0);
        }
    }
}
