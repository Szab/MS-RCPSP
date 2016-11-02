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
        public StepEventHandler OnNextGeneration;

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

        public T Solve()
        {
            T finalSolution = this.InitialSolution;
            double finalSolutionQuality = finalSolution.RateQuality();

            T lastSolution = this.InitialSolution;
            double lastSolutionQuality = finalSolutionQuality;

            int numStep = 0;
            int numStepsWithoutChange = 0;

            List<T> tabuList = new List<T>();
            Object lockObj = new Object();

            while (!this.CheckIfFinished(numStep, finalSolution))
            {
                IEnumerable<T> neigbours = lastSolution.GetNeighbours().Where(x => !this.CheckIfPresentInTabuList(x, tabuList));

                if (neigbours.Count() == 0)
                {
                    break;
                }

                T currentSolution = null;
                double currentSolutionQuality = double.NegativeInfinity;

                Parallel.ForEach(neigbours, item =>
                {
                    double partialQuality = item.RateQuality();

                    lock (lockObj)
                    {
                        bool pickBest = this.MaxStepsWithoutChange <= 0 || this.MaxStepsWithoutChange <= numStepsWithoutChange;

                        if (currentSolutionQuality < partialQuality && pickBest ||
                            currentSolutionQuality < partialQuality && partialQuality != lastSolutionQuality)
                        {
                            currentSolution = item;
                            currentSolutionQuality = partialQuality;
                        }
                    }

                });

                if (lastSolutionQuality == currentSolutionQuality)
                {
                    numStepsWithoutChange++;
                } 
                else
                {
                    numStepsWithoutChange = 0;
                }

                lastSolution = currentSolution;
                lastSolutionQuality = currentSolutionQuality;

                if (lastSolutionQuality > finalSolutionQuality)
                {
                    finalSolution = lastSolution;
                    finalSolutionQuality = lastSolutionQuality;
                }
                
                tabuList.Insert(0, lastSolution);
                tabuList = tabuList.Take(this.TabuSize).ToList();

                if(this.OnNextGeneration != null)
                {
                    this.OnNextGeneration(numStep, lastSolution, finalSolution);
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
