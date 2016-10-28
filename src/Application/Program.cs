using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Szab.Scheduling.Representation;
using Szab.Scheduling.Tools;
using Szab.Scheduling.MSRCPSP;
using System.Globalization;

namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {
            List<double[]> partialQualities = new List<double[]>();

            string filePath = @"C:\Users\Szab\Desktop\MSRCPSP\Datasets\200_40_130_9_D4.def";
            ProjectSpecification project = FilesManager.ParseProjectData(filePath);
            MSRCPSPSolver solver = new MSRCPSPSolver(project, 1000)
            {
                MutationProbability = 0.1,
                CrossoverProbability = 0.7,
                NumberOfGroups = 6,
                PopulationSize = 30
            };

            solver.OnNextGeneration += delegate (int numGeneration, IEnumerable<ScheduleSpecimen> population)
            {
                double worst = population.Min(x => x.RateQuality());
                double average = population.Average(x => x.RateQuality());
                double best = population.Max(x => x.RateQuality());
                Console.WriteLine("New generation: {0}", numGeneration + 1);
                partialQualities.Add(new double[] { worst, average, best });
            };

            ScheduleSpecimen bestSpecimen = solver.Solve();
            Schedule schedule = ScheduleBuilder.BuildScheduleFromSpecimen(project, bestSpecimen);
            FilesManager.SaveResults(filePath, project, solver, schedule, partialQualities);


            Console.WriteLine();
        }
    }
}
