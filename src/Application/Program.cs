﻿using System;
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

            string filePath = @"C:\Users\Szab\Desktop\MSRCPSP\dataset_def\100_20_46_15.def";
            ProjectSpecification project = FilesManager.ParseProjectData(filePath);

            MSRCPSPTabuSolver tabuSolver = new MSRCPSPTabuSolver(project)
            {
                NumberOfSteps = 3000,
                TabuSize = 50,
                MaxStepsWithoutChange = 0
            };

            MSRCPSPEvolutionarySolver eaSolver = new MSRCPSPEvolutionarySolver(project, 100)
            {
                MutationProbability = 0.015,
                CrossoverProbability = 0.60,
                PercentInGroup = 0.05,
                PopulationSize = 100
            };

            MSRCPSPSimulatedAnnealingSolver saSolver = new MSRCPSPSimulatedAnnealingSolver(project)
            {
                MaxIterations = 1000,
                InitialTemperature = 5000,
            };

            //eaSolver.OnNextGeneration += delegate (int numGeneration, IEnumerable<ScheduleSpecimen> population)
            //{
            //    double worst = population.Min(x => x.RateQuality());
            //    double average = population.Average(x => x.RateQuality());
            //    double best = population.Max(x => x.RateQuality());
            //    Console.WriteLine("New generation: {0}, Best: {1}, Average: {2}, Worst: {3}", numGeneration + 1, 1 / best, 1 / average, 1 / worst);
            //    partialQualities.Add(new double[] { worst, average, best });
            //};

            ////tabuSolver.OnNextStep += delegate (int numGeneration, ScheduleSpecimen current, ScheduleSpecimen bestSolution)
            ////{
            ////    double worst = current.RateQuality();
            ////    double average = double.NaN;
            ////    double best = bestSolution.RateQuality();
            ////    Console.WriteLine("New generation: {0}, Best: {1}, Average: {2}, Worst: {3}", numGeneration + 1, 1 / best, 1 / average, 1 / worst);
            ////    partialQualities.Add(new double[] { worst, average, best });
            ////};

            ////saSolver.OnNextStep += delegate (int numGeneration, ScheduleSpecimen current, ScheduleSpecimen bestSolution, double probability)
            ////{
            ////    double worst = current.RateQuality();
            ////    double average = probability;
            ////    double best = bestSolution.RateQuality();
            ////    Console.WriteLine("New generation: {0}, Best: {1}, Average: {2}, Worst: {3}", numGeneration + 1, 1 / best, average, 1 / worst);
            ////    partialQualities.Add(new double[] { worst, average, best });
            ////};

            //MSRCPSPTabuSolver solver = tabuSolver;
            //MSRCPSPEvolutionarySolver solver = eaSolver;
            ////MSRCPSPSimulatedAnnealingSolver solver = saSolver;
            //ScheduleSpecimen bestSpecimen = solver.Solve();
            //Schedule schedule = ScheduleBuilder.BuildScheduleFromSpecimen(project, bestSpecimen);
            //FilesManager.SaveResults(filePath, project, solver, schedule, partialQualities);

            Benchmark bench = new Benchmark(tabuSolver);
            BenchmarkResult res = bench.Start(10);

            Console.WriteLine("Results for {0}", filePath);
            Console.WriteLine("=============================");
            foreach (ScheduleSpecimen result in res.Results)
            {
                Console.WriteLine("{0}: {1}", res.Results.IndexOf(result), 1 / result.RateQuality());
            }
            Console.WriteLine("=============================");
            Console.WriteLine("Best: {0}", res.Best);
            Console.WriteLine("Average: {0}", res.Average);
            Console.WriteLine("Std dev: {0}", res.StandardDeviation);

            FilesManager.SaveResults(filePath, project, eaSolver, ScheduleBuilder.BuildScheduleFromSpecimen(project, res.BestSpecimen));

            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
