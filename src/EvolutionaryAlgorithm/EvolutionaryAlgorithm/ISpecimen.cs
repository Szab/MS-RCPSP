using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Szab.MetaheuristicsBase;

namespace Szab.EvolutionaryAlgorithm
{
    public interface ISpecimen<T> : ISolution<T> where T : class, ISpecimen<T>
    {
        T CrossOver(T otherSpeciman);
        void Mutate();
    }
}
