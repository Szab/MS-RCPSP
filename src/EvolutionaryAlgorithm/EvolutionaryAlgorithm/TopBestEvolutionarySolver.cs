using Szab.EvolutionaryAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szab.EvolutionaryAlgorithm.SelectionSpecific
{
    public abstract class TopBestEvolutionarySolver<T> : EvolutionarySolver<T> where T : class, ISpecimen<T>
    {
        protected override IEnumerable<T> SelectNewPopulation(IEnumerable<Tuple<T, double>> qualities)
        {
            return qualities.OrderByDescending(x => x.Item2).Take(this.PopulationSize).Select(x => x.Item1);
        }

        public TopBestEvolutionarySolver() : base()
        {
        }
    }
}
