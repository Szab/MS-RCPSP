using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Szab.EvolutionaryAlgorithm;
using Szab.MetaheuristicsBase;
using Szab.Scheduling.MSRCPSP;

namespace Application
{
    public class Benchmark
    {
        private ISolver<ScheduleSpecimen> solver;

        private double getStandardDeviation(List<double> doubleList)
        {
            double M = 0.0;
            double S = 0.0;
            int k = 1;
            foreach (double value in doubleList)
            {
                double tmpM = M;
                M += (value - tmpM) / k;
                S += (value - tmpM) * (value - M);
                k++;
            }
            return Math.Sqrt(S / (k - 2));
        }

        public BenchmarkResult Start(int attempts)
        {
            Console.WriteLine("Starting benchmark, {0} attempts.", attempts);
            List<ScheduleSpecimen> results = new List<ScheduleSpecimen>();
            int finished = 0;
            Parallel.For(0, attempts, i =>
            {
                ScheduleSpecimen partialResult = this.solver.Solve();
                
                lock(results)
                {
                    finished++;
                    results.Add(partialResult);
                    Console.WriteLine("Ukończono {0}/{1}", finished, attempts);
                }
            });

            ScheduleSpecimen best = results.Aggregate((x, y) => x.RateQuality() > y.RateQuality() ? x : y);
            double standardDev = this.getStandardDeviation(results.Select(x => 1 / x.RateQuality()).ToList());
            double avg = results.Average(x => 1 / x.RateQuality());

            BenchmarkResult result = new BenchmarkResult()
            {
                Results = results,
                BestSpecimen = best,
                StandardDeviation = standardDev,
                Average = avg
            };

            return result;
        }

        public Benchmark(ISolver<ScheduleSpecimen> solver)
        {
            this.solver = solver;
        }
    }
}
