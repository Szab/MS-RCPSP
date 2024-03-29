﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Szab.EvolutionaryAlgorithm.SelectionSpecific;
using Szab.Scheduling.Representation;
using Szab.Extensions;

namespace Szab.Scheduling.MSRCPSP
{
    public class MSRCPSPEvolutionarySolver : TournamentEvolutionarySolver<ScheduleSpecimen>
    {
        public ProjectSpecification ProjectData
        {
            get;
            private set;
        }

        public int MaxGenerations
        {
            get;
            set;
        }

        protected override bool CheckIfFinished(int numGeneration, IEnumerable<ScheduleSpecimen> population)
        {
            return numGeneration >= this.MaxGenerations;
        }

        protected override IEnumerable<ScheduleSpecimen> CreateInitialPopulation()
        {
            List<Task> availableTasks = this.ProjectData.Tasks.ToList();
            List<ScheduleSpecimen> initialPopulation = new List<ScheduleSpecimen>();

            for(int i = 0; i < this.PopulationSize; i++)
            {
                initialPopulation.Add(ScheduleSpecimen.GetRandom(this.ProjectData, availableTasks.Count));
            }

            return initialPopulation;
        }

        public MSRCPSPEvolutionarySolver(ProjectSpecification projectData, int maxGenerations)
        {
            this.ProjectData = projectData;
            this.MaxGenerations = maxGenerations;
        }
    }
}
