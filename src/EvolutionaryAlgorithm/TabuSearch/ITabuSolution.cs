using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Szab.MetaheuristicsBase;

namespace Szab.TabuSearch
{
    public interface ITabuSolution<T> : ISolution<T> where T : class, ITabuSolution<T>
    {
        IEnumerable<T> GetNeighbours();
    }
}
