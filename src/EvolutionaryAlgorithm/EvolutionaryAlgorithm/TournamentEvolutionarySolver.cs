using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Szab.EvolutionaryAlgorithm;
using Szab.Extensions;

namespace Szab.EvolutionaryAlgorithm.SelectionSpecific
{
    public abstract class TournamentEvolutionarySolver<T> : EvolutionarySolver<T> where T : class, ISpecimen<T>
    {
        private double populationInGroup;
        public double PercentInGroup
        {
            get
            {
                return this.populationInGroup;
            }

            set
            {
                this.populationInGroup = Math.Min(Math.Max(0.01, value), 1);
            }
        }

        protected override IEnumerable<T> SelectNewPopulation(IEnumerable<Tuple<T, double>> qualities, int numGeneration)
        {
            List<Tuple<T, double>> qualitiesList = qualities.ToList();
            List<Tuple<T, double>> newPopulation = new List<Tuple<T, double>>();

            int specimenInGroup = (int)(qualitiesList.Count * this.PercentInGroup);

            while(newPopulation.Count < this.PopulationSize)
            {
                qualitiesList.Shuffle();
                Tuple<T, double> winner = qualitiesList.Take(specimenInGroup).OrderByDescending(x => x.Item2).First();
                qualitiesList.Remove(winner);
                newPopulation.Add(winner);
            }

            return newPopulation.OrderByDescending(x => x.Item2).Select(x => x.Item1).Take(this.PopulationSize);
        }

        public TournamentEvolutionarySolver() : base()
        {
            this.populationInGroup = 10;
        }
    }
}
