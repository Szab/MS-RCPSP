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

            string filePath = @"C:\Users\Szab\Desktop\MSRCPSP\dataset_def\100_5_64_15.def";
            ProjectSpecification project = FilesManager.ParseProjectData(filePath);
            MSRCPSPSolver solver = new MSRCPSPSolver(project, 100)
            {
                MutationProbability = 0.15,
                CrossoverProbability = 0.60,
                PercentInGroup = 0.05,
                PopulationSize = 70
            };

            solver.OnNextGeneration += delegate (int numGeneration, IEnumerable<ScheduleSpecimen> population)
            {
                double worst = population.Min(x => x.RateQuality());
                double average = population.Average(x => x.RateQuality());
                double best = population.Max(x => x.RateQuality());
                Console.WriteLine("New generation: {0}, Best: {1}, Average: {2}, Worst: {3}", numGeneration + 1, 1/best, 1/average, 1/worst);
                partialQualities.Add(new double[] { worst, average, best });
            };

            ScheduleSpecimen bestSpecimen = solver.Solve();
            Schedule schedule = ScheduleBuilder.BuildScheduleFromSpecimen(project, bestSpecimen);
            FilesManager.SaveResults(filePath, project, solver, schedule, partialQualities);


            Console.WriteLine();
        }
    }
}
