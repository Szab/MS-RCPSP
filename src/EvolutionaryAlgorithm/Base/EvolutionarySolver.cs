using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szab.EvolutionaryAlgorithm.Base
{
    public abstract class EvolutionarySolver<T> where T : class, ISpecimen<T>
    {

        private double mutationProbability;
        private double crossoverProbability;

        public delegate void StepEventHandler(int numGeneration, IEnumerable<T> currentPopulation);
        public StepEventHandler OnNextGeneration;

        public double MutationProbability
        {
            get { return this.mutationProbability; }
            set { this.mutationProbability = Math.Min(Math.Max(value, 0), 1);  }
        }

        public double CrossoverProbability
        {
            get { return this.crossoverProbability; }
            set { this.crossoverProbability = Math.Min(Math.Max(value, 0), 1); }
        }

        public int PopulationSize
        {
            get;
            set;
        }

        public abstract bool CheckIfFinished(int numGeneration, IEnumerable<T> population);
        public abstract IEnumerable<T> SelectNewPopulation(IEnumerable<T> population);
        public abstract IEnumerable<T> CreateInitialPopulation();
        
        protected void CrossOverPopulation(List<T> population, Random randomGenerator)
        {          
            for (int i = 0; i < population.Count - 1; i = i + 2)
            {
                double rand = randomGenerator.NextDouble();

                if (rand < this.CrossoverProbability)
                {
                    T firstParent = population[i];
                    T secondParent = population[i + 1];

                    population.Add(firstParent.CrossOver(secondParent));
                    population.Add(secondParent.CrossOver(firstParent));
                }
            }
        }

        protected void MutatePopulation(List<T> population, Random randomGenerator)
        {
            for (int i = 0; i < population.Count; i++)
            {
                double rand = randomGenerator.NextDouble();

                if (rand < this.MutationProbability)
                {
                    population[i].Mutate();
                }
            }
        }

        public T Solve()
        {
            int numGeneration = 0;
            List<T> population = this.CreateInitialPopulation().ToList();
            Random random = new Random(Guid.NewGuid().GetHashCode());
            
            while (!this.CheckIfFinished(numGeneration, population))
            {
                if (this.OnNextGeneration != null)
                {
                    this.OnNextGeneration(numGeneration, population);
                }

                this.MutatePopulation(population, random);
                this.CrossOverPopulation(population, random);

                int populationSize = population.Count;
                IEnumerable<T> newPopulation = this.SelectNewPopulation(population).Take(this.PopulationSize);
                population.Clear();
                population.AddRange(newPopulation);
                numGeneration++;
            }

            return population.Aggregate((x, y) => x.RateQuality() > y.RateQuality() ? x : y);
        }

        public EvolutionarySolver()
        {
            this.mutationProbability = 0.01;
            this.crossoverProbability = 0.7;
            this.PopulationSize = 1000;
        }
    }
}
