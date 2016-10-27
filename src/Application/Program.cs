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
            List<double> worst = new List<double>();
            List<double> average = new List<double>();
            List<double> best = new List<double>();

            ProjectSpecification project = FilesManager.ParseProjectData(@"C:\Users\Szab\Desktop\MSRCPSP\def_small\15_3_5_3.def");
            MSRCPSPSolver solver = new MSRCPSPSolver(project, 1000)
            {
                MutationProbability = 0.2,
                CrossoverProbability = 0.7,
                NumberOfGroups = 5,
                PopulationSize = 30
            };

            solver.OnNextGeneration += delegate (int numGeneration, IEnumerable<ScheduleSpecimen> population)
            {
                Console.WriteLine("New generation: {0}", numGeneration + 1);
                worst.Add(population.Min(x => x.RateQuality()));
                average.Add(population.Average(x => x.RateQuality()));
                best.Add(population.Max(x => x.RateQuality()));
            };

            ScheduleSpecimen bestSpecimen = solver.Solve();
            Schedule schedule = ScheduleBuilder.BuildScheduleFromSpecimen(project, bestSpecimen);
            string result = FilesManager.SaveToFile(schedule);

            StringBuilder statisticsBuilder = new StringBuilder();

            for(int i = 0; i < best.Count; i++)
            {
                string worstResult = worst[i].ToString(CultureInfo.CurrentUICulture);
                string averageResult = average[i].ToString(CultureInfo.CurrentUICulture);
                string bestResult = best[i].ToString(CultureInfo.CurrentUICulture);

                string partialResult = String.Format("{0};{1};{2}", worstResult, averageResult, bestResult);
                statisticsBuilder.AppendLine(partialResult);
            }

            string statisticsResult = statisticsBuilder.ToString();


            Console.WriteLine();
        }
    }
}
