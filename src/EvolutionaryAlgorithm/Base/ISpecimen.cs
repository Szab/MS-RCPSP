using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szab.EvolutionaryAlgorithm.Base
{
    public interface ISpecimen<T> where T : class
    {
        double RateQuality();
        T CrossOver(T otherSpeciman);
        void Mutate();
    }
}
