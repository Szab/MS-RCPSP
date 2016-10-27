using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Szab.Scheduling.Representation;
using Szab.Scheduling.Tools;
using Szab.Scheduling.MSRCPSP;

namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {
            List<double> worst = new List<double>();
            List<double> average = new List<double>();
            List<double> best = new List<double>();

            ProjectSpecification project = FilesManager.ParseProjectData(@"C:\Users\Szab\Desktop\MSRCPSP\100_5_22_15.def");
            MSRCPSPSolver solver = new MSRCPSPSolver(project, 2)
            {
                MutationProbability = 0.05,
                CrossoverProbability = 0.8,
                NumberOfGroups = 10,
                PopulationSize = 20
            };

            //solver.OnNextGeneration += delegate (int numGeneration, IEnumerable<ScheduleSpecimen> population)
            //{
            //    worst.Add(population.Min(x => x.RateQuality()));
            //    average.Add(population.Average(x => x.RateQuality()));
            //    best.Add(population.Max(x => x.RateQuality()));
            //};

            ScheduleSpecimen bestSpecimen = solver.Solve();
            Schedule schedule = ScheduleBuilder.BuildScheduleFromSpecimen(project, bestSpecimen);
            string result = FilesManager.SaveToFile(schedule);
            Console.WriteLine();
        }
    }
}
