using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Szab.MetaheuristicsBase;
using Szab.Metaheuristics.Tools;

namespace Szab.EvolutionaryAlgorithm
{
    public abstract class EvolutionarySolver<T> : ISolver<T> where T : class, ISpecimen<T>
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

        protected abstract bool CheckIfFinished(int numGeneration, IEnumerable<T> population);
        protected abstract IEnumerable<T> SelectNewPopulation(IEnumerable<Tuple<T, double>> qualities);
        protected abstract IEnumerable<T> CreateInitialPopulation();
        
        protected List<T> CrossOverPopulation(List<T> population, Random randomGenerator)
        {
            Object addingLock = new Object();
            List<T> children = new List<T>();

            Parallel.For(0, population.Count, index =>
            {
                int secondParentIndex = randomGenerator.Next(population.Count);
                double rand = randomGenerator.NextDouble();

                if (population[secondParentIndex] == this)
                {
                    secondParentIndex = (secondParentIndex + 1) % population.Count;
                }

                if (rand < this.CrossoverProbability)
                {
                    T firstParent = population[index];
                    T secondParent = population[secondParentIndex];

                    lock (addingLock)
                    {
                        children.Add(firstParent.CrossOver(secondParent));
                        children.Add(secondParent.CrossOver(firstParent));
                    }
                }
            });

            return children;
        }

        protected void MutatePopulation(List<T> population, Random randomGenerator)
        {
            Parallel.ForEach(population, specimen =>
            {
                double rand;

                lock (randomGenerator)
                {
                    rand = randomGenerator.NextDouble();
                }

                if (rand < this.MutationProbability)
                {
                    specimen.Mutate();
                }
            });
        }

        protected IEnumerable<Tuple<T, double>> CalculateQualities(List<T> population)
        {
            Object addingLock = new Object();
            List<Tuple<T, double>> qualities = new List<Tuple<T, double>>();

            Parallel.ForEach(population, (item) =>
            {
                double quality = item.RateQuality();
                Tuple<T, double> qualityItem = new Tuple<T, double>(item, quality);

                lock (addingLock)
                {
                    qualities.Add(qualityItem);
                }
            });

            return qualities;
        }

        protected virtual void PerformStep(List<T> population, Random random)
        {
            IEnumerable<Tuple<T, double>> qualities = this.CalculateQualities(population);
            IEnumerable<T> newPopulation = this.SelectNewPopulation(qualities).Take(this.PopulationSize);
            population.Clear();
            population.AddRange(newPopulation);

            List<T> children = this.CrossOverPopulation(population, random);
            population.AddRange(children);
            this.MutatePopulation(population, random);

            int populationSize = population.Count;
        }

        public virtual T Solve()
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

                this.PerformStep(population, random);

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
