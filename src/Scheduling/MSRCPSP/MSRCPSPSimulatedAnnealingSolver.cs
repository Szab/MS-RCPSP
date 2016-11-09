using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Szab.Scheduling.Representation;
using Szab.SimulatedAnnealing;
using Szab.Extensions;

namespace Szab.Scheduling.MSRCPSP
{
    public class MSRCPSPSimulatedAnnealingSolver : SimulatedAnnealingSolver<ScheduleSpecimen>
    {
        public ProjectSpecification ProjectData
        {
            get;
            private set;
        }

        private static ScheduleSpecimen GetRandomSpecimen(ProjectSpecification projectData)
        {
            List<ScheduleSpecimen> specimens = new List<ScheduleSpecimen>();

            for (int i = 0; i < 30; i++)
            {
                List<Task> availableTasks = projectData.Tasks.ToList();
                availableTasks.Shuffle();

                ScheduleSpecimen specimen = new ScheduleSpecimen(projectData, availableTasks.Count);
                availableTasks.CopyTo(specimen.Tasks, 0);
                specimens.Add(specimen);
            }

            ScheduleSpecimen best = specimens.Aggregate((x, y) => x.RateQuality() > y.RateQuality() ? x : y);

            return best;
        }

        public MSRCPSPSimulatedAnnealingSolver(ScheduleSpecimen initialSolution, ProjectSpecification project) : base(initialSolution)
        {
            this.ProjectData = project;
        }

        public MSRCPSPSimulatedAnnealingSolver(ProjectSpecification project) : this(MSRCPSPSimulatedAnnealingSolver.GetRandomSpecimen(project), project) 
        {

        }
    }
}
