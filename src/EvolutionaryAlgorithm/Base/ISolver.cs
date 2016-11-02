using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szab.MetaheuristicsBase
{
    public interface ISolver<T> where T : class
    {
        T Solve();
    }
}
