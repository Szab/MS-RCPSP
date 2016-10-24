using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionaryAlgorithm.Base
{
    public interface ISpeciman<T> where T : class
    {
        double RateQuality();
        T CrossOver(T otherSpeciman);
        void Mutate();
    }
}
