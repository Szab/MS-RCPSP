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

            MSRCPSPTabuSolver tabuSolver = new MSRCPSPTabuSolver(project)
            {
                NumberOfSteps = 500,
                TabuSize = 200,
                MaxStepsWithoutChange = 30
            };

            MSRCPSPSolver eaSolver = new MSRCPSPSolver(project, 100)
            {
                MutationProbability = 0.01,
                CrossoverProbability = 0.80,
                PercentInGroup = 0.03,
                PopulationSize = 400
            };

            eaSolver.OnNextGeneration += delegate (int numGeneration, IEnumerable<ScheduleSpecimen> population)
            {
                double worst = population.Min(x => x.RateQuality());
                double average = population.Average(x => x.RateQuality());
                double best = population.Max(x => x.RateQuality());
                Console.WriteLine("New generation: {0}, Best: {1}, Average: {2}, Worst: {3}", numGeneration + 1, 1 / best, 1 / average, 1 / worst);
                partialQualities.Add(new double[] { worst, average, best });
            };

            tabuSolver.OnNextGeneration += delegate (int numGeneration, ScheduleSpecimen current, ScheduleSpecimen bestSolution)
            {
                double worst = current.RateQuality();
                double average = double.NaN;
                double best = bestSolution.RateQuality();
                Console.WriteLine("New generation: {0}, Best: {1}, Average: {2}, Worst: {3}", numGeneration + 1, 1 / best, 1 / average, 1 / worst);
                partialQualities.Add(new double[] { worst, average, best });
            };

            MSRCPSPTabuSolver solver = tabuSolver;
            //MSRCPSPSolver solver = eaSolver;
            ScheduleSpecimen bestSpecimen = solver.Solve();
            Schedule schedule = ScheduleBuilder.BuildScheduleFromSpecimen(project, bestSpecimen);
            FilesManager.SaveResults(filePath, project, solver, schedule, partialQualities);


            Console.WriteLine();
        }
    }
}
