using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Szab.Hybrids;
using Szab.Scheduling.Representation;
using Szab.Extensions;

namespace Szab.Scheduling.MSRCPSP
{
    public class MSRCPSPAnnealedEASolver : AnnealedTournamentEvolutionarySolver<ScheduleSpecimen>
    {
        public ProjectSpecification ProjectData
        {
            get;
            private set;
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

        public MSRCPSPAnnealedEASolver(ProjectSpecification projectData, int maxGenerations)
        {
            this.ProjectData = projectData;
            this.MaxGenerations = maxGenerations;
        }
    }
}
