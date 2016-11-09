using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Szab.MetaheuristicsBase;

namespace Szab.TabuSearch
{
    public class TabuSolver<T> : ISolver<T> where T : class, ITabuSolution<T>
    {
        public delegate void StepEventHandler(int numGeneration, T currentSolution, T currentBestSolution);
        public StepEventHandler OnNextStep;

        public T InitialSolution
        {
            get;
            private set;
        }

        public int NumberOfSteps
        {
            get;
            set;
        }

        public int TabuSize
        {
            get;
            set;
        }

        public int MaxStepsWithoutChange
        {
            get;
            set;
        }

        public virtual bool CheckIfFinished(int numStep, T currentSolution)
        {
            return numStep >= this.NumberOfSteps;
        }

        private bool CheckIfPresentInTabuList(T solution, List<T> tabuList)
        { 
            foreach (T tabu in tabuList)
            {
                if(tabu.CheckEquality(solution))
                {
                    return true;
                }
            }

            return false;
        } 

        private T GetBestNeighbour(IEnumerable<T> neigbours, int numStepsWithoutChange, double lastSolutionQuality)
        {
            Object lockObj = new Object();
            T currentSolution = null;
            double currentSolutionQuality = double.NegativeInfinity;

            Parallel.ForEach(neigbours, item =>
            {
                double partialQuality = item.RateQuality();

                lock (lockObj)
                {
                    bool pickBest = this.MaxStepsWithoutChange <= 0 || this.MaxStepsWithoutChange >= numStepsWithoutChange;

                    if (currentSolutionQuality < partialQuality && pickBest ||
                        currentSolutionQuality < partialQuality && partialQuality != lastSolutionQuality)
                    {
                        currentSolution = item;
                        currentSolutionQuality = partialQuality;
                    }
                }

            });

            return currentSolution;
        }

        public T Solve()
        {
            T finalSolution = this.InitialSolution;
            double finalSolutionQuality = finalSolution.RateQuality();

            T lastSolution = this.InitialSolution;
            double lastSolutionQuality = finalSolutionQuality;

            int numStep = 0;
            int numStepsWithoutChange = 0;

            List<T> tabuList = new List<T>();

            while (!this.CheckIfFinished(numStep, finalSolution))
            {
                // Get neighbours of the current solution
                IEnumerable<T> neigbours = lastSolution.GetNeighbours().Where(x => !this.CheckIfPresentInTabuList(x, tabuList));

                if (neigbours.Count() == 0)
                {
                    break;
                }

                // Calculate where to go next
                T currentSolution = this.GetBestNeighbour(neigbours, numStepsWithoutChange, lastSolutionQuality);
                double currentSolutionQuality = currentSolution.RateQuality();

                // Increase the counter if quality didn't change
                if (lastSolutionQuality == currentSolutionQuality)
                {
                    numStepsWithoutChange++;
                } 
                else
                {
                    numStepsWithoutChange = 0;
                }

                // Check if the current solution is the best one yet
                lastSolution = currentSolution;
                lastSolutionQuality = currentSolutionQuality;

                if (lastSolutionQuality > finalSolutionQuality)
                {
                    finalSolution = lastSolution;
                    finalSolutionQuality = lastSolutionQuality;
                }
                
                tabuList.Insert(0, lastSolution);
                tabuList = tabuList.Take(this.TabuSize).ToList();

                if(this.OnNextStep != null)
                {
                    this.OnNextStep(numStep, lastSolution, finalSolution);
                }

                numStep++;
            }

            return finalSolution;
        }
        
        public TabuSolver(T initialSolution)
        {
            if(initialSolution == null)
            {
                throw new ArgumentNullException();
            }

            this.InitialSolution = initialSolution;
            this.NumberOfSteps = 50;
            this.TabuSize = 10;
            this.MaxStepsWithoutChange = 0;
        }
    }
}
