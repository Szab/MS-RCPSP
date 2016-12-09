using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Szab.Hybrids;
using Szab.Extensions;
using Szab.Scheduling.Representation;

namespace Szab.Scheduling.MSRCPSP
{
    public class MSRCPSPTabuCorrectedEASolver: TabuCorrectedEvolutionarySolver<ScheduleSpecimen>
    {
        public ProjectSpecification ProjectData
        {
            get;
            private set;
        }

        public int MaxGenerations
        {
            get;
            set;
        }

        protected override bool CheckIfFinished(int numGeneration, IEnumerable<ScheduleSpecimen> population)
        {
            return numGeneration >= this.MaxGenerations;
        }

        protected override IEnumerable<ScheduleSpecimen> CreateInitialPopulation()
        {
            List<Task> availableTasks = this.ProjectData.Tasks.ToList();
            List<ScheduleSpecimen> initialPopulation = new List<ScheduleSpecimen>();

            for (int i = 0; i < this.PopulationSize; i++)
            {
                ScheduleSpecimen newSpecimen = new ScheduleSpecimen(this.ProjectData, availableTasks.Count);
                availableTasks.Shuffle();
                availableTasks.CopyTo(newSpecimen.Tasks, 0);

                initialPopulation.Add(newSpecimen);
            }

            return initialPopulation;
        }

        public MSRCPSPTabuCorrectedEASolver(ProjectSpecification projectData, int maxGenerations)
        {
            this.ProjectData = projectData;
            this.MaxGenerations = maxGenerations;
        }
    }
}
