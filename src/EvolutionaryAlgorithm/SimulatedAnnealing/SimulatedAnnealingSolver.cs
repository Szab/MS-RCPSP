using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Szab.MetaheuristicsBase;
using Szab.Extensions;

namespace Szab.SimulatedAnnealing
{
    public class SimulatedAnnealingSolver<T> : ISolver<T> where T : class, IAnnealingSolution<T>
    {
        private Random rand = new Random(Guid.NewGuid().GetHashCode());

        public delegate void StepEventHandler(int numGeneration, T currentSolution, T currentBestSolution, double probability);
        public StepEventHandler OnNextStep;

        public T InitialSolution
        {
            get;
            private set;
        }

        public double InitialTemperature
        {
            get;
            set;
        }

        public int MaxIterations
        {
            get;
            set;
        }

        public double MinTemperature
        {
            get;
            set;
        }

        private double CalculateProbability(T last, T current, double currentTemperature)
        {
            double lastQuality = 1/last.RateQuality();
            double currentQuality = 1/current.RateQuality();

            return Math.Pow(Math.E, (lastQuality - currentQuality) / currentTemperature);
        }

        public T Solve()
        {
            T best = this.InitialSolution;
            double bestQuality = 1/best.RateQuality();

            T current = best;
            double currentQuality = bestQuality;

            double temperature = this.InitialTemperature;

            int step = 0;

            while(step < this.MaxIterations)
            {
                step++;

                List<T> neighbours = best.GetNeighbours().ToList();
                neighbours.Shuffle();

                //int neighbourIndex = rand.Next(neighbours.Count);

                T neighbour = neighbours[0];
                double neighbourQuality = 1/neighbour.RateQuality();

                double changeProbability = this.CalculateProbability(current, neighbour, temperature);
                double changeRoll = rand.NextDouble();

                if (neighbourQuality <= currentQuality)
                {
                    current = neighbour;
                    currentQuality = neighbourQuality;
                }
                else if (changeRoll < changeProbability)
                {
                    current = neighbour;
                    currentQuality = neighbourQuality;
                }

                if (neighbourQuality < bestQuality)
                {
                    best = neighbour;
                    bestQuality = neighbourQuality;
                }

                //temperature = this.InitialTemperature * (1 - step/this.MaxIterations);
                double a = (Math.Log(this.InitialTemperature, Math.E) - Math.Log(0.45, Math.E)) / this.MaxIterations;
                double b = Math.Pow(Math.E, -(step * a));
                temperature = this.InitialTemperature * b;

                if (this.OnNextStep != null)
                {
                    this.OnNextStep(step, current, best, temperature);
                }
            }

            return best;
        }

        public SimulatedAnnealingSolver(T initialSolution)
        {
            if(initialSolution == null)
            {
                throw new ArgumentNullException();
            }

            this.InitialTemperature = 10;
            this.MaxIterations = 200;
            this.MinTemperature = 0.01;
            this.InitialSolution = initialSolution;
        }
    }
}
