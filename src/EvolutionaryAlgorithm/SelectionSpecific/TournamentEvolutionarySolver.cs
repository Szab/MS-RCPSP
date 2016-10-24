using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvolutionaryAlgorithm.Base;

namespace EvolutionaryAlgorithm.SelectionSpecific
{
    public abstract class TournamentEvolutionarySolver<T> : EvolutionarySolver<T> where T : class, ISpeciman<T>
    {
        public override IEnumerable<T> SelectNewPopulation(IEnumerable<T> population)
        {
            throw new NotImplementedException();
        }
    }
}
