using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Szab.Scheduling.Representation;
using Szab.Scheduling.Tools;
using Szab.Scheduling.MSRCPSP;
using System.Globalization;
using Szab.TabuSearch;

namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {
            List<double[]> partialQualities = new List<double[]>();

            string filePath = @"C:\Users\Szab\Desktop\MSRCPSP\100_5_22_15.def";
            ProjectSpecification project = FilesManager.ParseProjectData(filePath);

            //MSRCPSPTabuSolver solver = new MSRCPSPTabuSolver(project)
            //{
            //    NumberOfSteps = 500,
            //    TabuSize = 200,
            //    MaxStepsWithoutChange = 30
            //};

            MSRCPSPSolver solver = new MSRCPSPSolver(project, 100)
            {
                MutationProbability = 0.05,
                CrossoverProbability = 0.65,
                PercentInGroup = 0.05,
                PopulationSize = 100
            };

            solver.OnNextGeneration += delegate (int numGeneration, IEnumerable<ScheduleSpecimen> population)
            {
                double worst = double.NaN; //population.Min(x => x.RateQuality());
                double average = population.Average(x => x.RateQuality());
                double best = population.Max(x => x.RateQuality());
                Console.WriteLine("New generation: {0}, Best: {1}, Average: {2}, Worst: {3}", numGeneration + 1, 1 / best, 1 / average, 1 / worst);
                partialQualities.Add(new double[] { worst, average, best });
            };

            //solver.OnNextGeneration += delegate (int numGeneration, ScheduleSpecimen current, ScheduleSpecimen bestSolution)
            //{
            //    double worst = current.RateQuality();
            //    double average = double.NaN;
            //    double best = bestSolution.RateQuality();
            //    Console.WriteLine("New generation: {0}, Best: {1}, Average: {2}, Worst: {3}", numGeneration + 1, 1 / best, 1 / average, 1 / worst);
            //    partialQualities.Add(new double[] { worst, average, best });
            //};

            ScheduleSpecimen bestSpecimen = solver.Solve();
            Schedule schedule = ScheduleBuilder.BuildScheduleFromSpecimen(project, bestSpecimen);
            FilesManager.SaveResults(filePath, project, solver, schedule, partialQualities);


            Console.WriteLine();
        }
    }
}
