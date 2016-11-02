using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szab.MetaheuristicsBase
{
    public interface ISolution<T> where T : class, ISolution<T>
    {
        double RateQuality();
        bool CheckEquality(T other);
    }
}
