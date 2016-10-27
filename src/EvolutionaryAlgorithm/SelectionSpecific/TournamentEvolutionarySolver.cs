﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Szab.EvolutionaryAlgorithm.Base;

namespace Szab.EvolutionaryAlgorithm.SelectionSpecific
{
    public abstract class TournamentEvolutionarySolver<T> : EvolutionarySolver<T> where T : class, ISpecimen<T>
    {
        private int numberOfGroups;
        public int NumberOfGroups
        {
            get
            {
                return this.numberOfGroups;
            }

            set
            {
                this.numberOfGroups = Math.Max(value, 1);
            }
        }

        public override IEnumerable<T> SelectNewPopulation(IEnumerable<T> population)
        {
            List<T> newPopulation = new List<T>();

            int step = (int)Math.Ceiling((double)population.Count() / this.NumberOfGroups);
            int winnersInGroup = (int)Math.Ceiling((double)this.PopulationSize / this.NumberOfGroups);

            for(var i = 0; i < population.Count(); i = i + step)
            {
                IEnumerable<T> subpopulation = population.Skip(i).Take(step).OrderByDescending(x => x.RateQuality())
                                                .Take(winnersInGroup);

                newPopulation.AddRange(subpopulation);
            }

            return newPopulation.Take(this.PopulationSize);
        }

        public TournamentEvolutionarySolver() : base()
        {
            this.numberOfGroups = 10;
        }
    }
}