using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Szab.EvolutionaryAlgorithm;
using Szab.EvolutionaryAlgorithm.SelectionSpecific;
using Szab.TabuSearch;

namespace Szab.Hybrids
{
    public abstract class TabuCorrectedEvolutionarySolver<T> : TournamentEvolutionarySolver<T> where T : class, ISpecimen<T>, ITabuSolution<T>
    {
        public TabuSolver<T> TabuSolver { get; set; }


        public override T Solve()
        {
            var bestSpecimen = base.Solve();

            if (TabuSolver != null)
            {
                TabuSolver.InitialSolution = bestSpecimen;
                bestSpecimen = TabuSolver.Solve();
            }

            return bestSpecimen;
        }
    }
}
