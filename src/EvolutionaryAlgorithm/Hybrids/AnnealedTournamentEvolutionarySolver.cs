using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Szab.EvolutionaryAlgorithm;
using Szab.EvolutionaryAlgorithm.SelectionSpecific;
using Szab.Extensions;

namespace Szab.Hybrids
{

    public abstract class AnnealedTournamentEvolutionarySolver<T> : TournamentEvolutionarySolver<T> where T : class, ISpecimen<T>
    {
        public double InitialTemperature
        {
            get;
            set;
        }

        public double MinTemperature
        {
            get;
            set;
        }

        public int MaxGenerations
        {
            get;
            set;
        }

        protected override bool CheckIfFinished(int numGeneration, IEnumerable<T> population)
        {
            return numGeneration >= this.MaxGenerations;
        }

        private double CalculateProbability(double best, T current, double currentTemperature)
        {
            double bestQuality = 1 / best;
            double currentQuality = 1 / current.RateQuality();

            return Math.Pow(Math.E, (bestQuality - currentQuality) / currentTemperature);
        }

        protected override IEnumerable<T> SelectNewPopulation(IEnumerable<Tuple<T, double>> qualities, int numGeneration)
        {
            List<Tuple<T, double>> qualitiesList = qualities.ToList();
            double bestQuality = qualities.Max(x => x.Item2);

            double a = (Math.Log(this.InitialTemperature, Math.E) - Math.Log(0.45, Math.E)) / this.MaxGenerations;
            double b = Math.Pow(Math.E, -(numGeneration * a));
            double currentTemperature = this.InitialTemperature * b;

            List<Tuple<T, double>> newPopulation = new List<Tuple<T, double>>();

            int specimenInGroup = (int)(qualitiesList.Count * this.PercentInGroup);

            // Perform annealed tournament
            for (int i = 0; i < this.PopulationSize; i++)
            {
                qualitiesList.Shuffle();
                IEnumerable<Tuple<T, double>> tournamentGroup = qualitiesList.Take(specimenInGroup).OrderByDescending(x => x.Item2);
                Tuple<T, double> winner = null;

                foreach (var contestant in tournamentGroup)
                {
                    double probability = this.CalculateProbability(bestQuality, contestant.Item1, currentTemperature);
                    double roll = random.NextDouble();

                    if (roll <= probability)
                    {
                        winner = contestant;
                        break;
                    }
                }

                if (winner != null)
                {
                    qualitiesList.Remove(winner);
                    newPopulation.Add(winner);
                }
            }

            // Get remaining from normal tournament
            while (newPopulation.Count < this.PopulationSize)
            {
                qualitiesList.Shuffle();
                Tuple<T, double> winner = qualitiesList.Take(specimenInGroup).OrderByDescending(x => x.Item2).First();
                qualitiesList.Remove(winner);
                newPopulation.Add(winner);
            }

            return newPopulation.OrderByDescending(x => x.Item2).Select(x => x.Item1).Take(this.PopulationSize);
        }
    }
}
