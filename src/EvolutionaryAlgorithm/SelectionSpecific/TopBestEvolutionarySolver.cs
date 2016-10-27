using Szab.EvolutionaryAlgorithm.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szab.EvolutionaryAlgorithm.SelectionSpecific
{
    public abstract class TopBestEvolutionarySolver<T> : EvolutionarySolver<T> where T : class, ISpecimen<T>
    {
        public override IEnumerable<T> SelectNewPopulation(IEnumerable<T> population)
        {
            return population.OrderByDescending(x => x.RateQuality()).Take(this.PopulationSize);
        }

        public TopBestEvolutionarySolver() : base()
        {
        }
    }
}
