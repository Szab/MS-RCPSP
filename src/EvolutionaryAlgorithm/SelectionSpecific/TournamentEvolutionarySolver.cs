using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Szab.EvolutionaryAlgorithm.Base;

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

        public override IEnumerable<T> SelectNewPopulation(IEnumerable<Tuple<T, double>> qualities)
        {
            List<T> newPopulation = new List<T>();

            int step = (int)Math.Ceiling(qualities.Count() * this.PercentInGroup);
            int groups = (int)Math.Ceiling(qualities.Count() / (double)step);
            int winnersInGroup = (int)Math.Ceiling(this.PopulationSize / (double)groups);

            for(var i = 0; i < qualities.Count(); i = i + step)
            {
                IEnumerable<T> subpopulation = qualities.Skip(i).Take(step).OrderByDescending(x => x.Item2)
                                                .Take(winnersInGroup).Select(x => x.Item1);

                newPopulation.AddRange(subpopulation);
            }

            return newPopulation.Take(this.PopulationSize);
        }

        public TournamentEvolutionarySolver() : base()
        {
            this.populationInGroup = 10;
        }
    }
}
