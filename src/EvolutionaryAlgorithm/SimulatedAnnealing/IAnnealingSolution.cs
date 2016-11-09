using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Szab.MetaheuristicsBase;

namespace Szab.SimulatedAnnealing
{
    public interface IAnnealingSolution<T> : ISolution<T> where T : class, IAnnealingSolution<T>
    {
        IEnumerable<T> GetNeighbours();
    }
}
