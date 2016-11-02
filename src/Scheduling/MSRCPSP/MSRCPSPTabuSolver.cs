using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Szab.Scheduling.Representation;
using Szab.Scheduling.Tools;
using Szab.TabuSearch;

namespace Szab.Scheduling.MSRCPSP
{
    public class MSRCPSPTabuSolver : TabuSolver<ScheduleSpecimen>
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

        public MSRCPSPTabuSolver(ScheduleSpecimen initialSolution, ProjectSpecification project) : base(initialSolution)
        {
            this.ProjectData = project;
        }

        public MSRCPSPTabuSolver(ProjectSpecification project) : this(MSRCPSPTabuSolver.GetRandomSpecimen(project), project) 
        {
            
        }
    }
}
