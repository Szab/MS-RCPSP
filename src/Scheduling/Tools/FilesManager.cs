using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Szab.Scheduling.Representation;
using System.Text.RegularExpressions;
using System.Globalization;
using Szab.EvolutionaryAlgorithm;
using Szab.Scheduling.MSRCPSP;
using Szab.TabuSearch;
using Szab.SimulatedAnnealing;

namespace Szab.Scheduling.Tools
{
    public static class FilesManager
    {
        private const int TASK_DEF_RELEVANT_COLUMNS = 4;
        private const int RES_DEF_RELEVANT_COLUMNS = 2;

        private static string ReadFile(string path)
        {
            string fileContents = "";

            using (StreamReader stream = new StreamReader(new FileStream(path, FileMode.Open)))
            {
                fileContents = stream.ReadToEnd();
            }

            return fileContents;   
        }

        private static void SaveToFile(string path, string data)
        {
            using (StreamWriter stream = new StreamWriter(new FileStream(path, FileMode.CreateNew)))
            {
                stream.Write(data);
            }
        }

        private static void AssignAvailableResources(ProjectSpecification project)
        {
            foreach(Task task in project.Tasks)
            {
                var matchingResources = project.Resources.Where(r => task.RequiredSkills.All(rs => r.Skills.Any(s => s.Name == rs.Name && s.Level >= rs.Level)));

                task.AvailableResources.AddRange(matchingResources.OrderBy(r => r.Cost));
            }
        }

        private static Resource GetResourceFromArray(string[] resourceData)
        {
            string name = resourceData[0];
            float salary = float.Parse(resourceData[1], CultureInfo.InvariantCulture);
            Resource resource = new Resource(name, salary);

            for(var i = 2; i < resourceData.Length; i = i + 2)
            {
                string skillName = resourceData[i].Substring(0, resourceData[i].Length - 1);
                int skillLevel = int.Parse(resourceData[i + 1]);

                Skill skill = new Skill(skillName, skillLevel);
                resource.Skills.Add(skill);
            }

            return resource;
        }

        private static void ParseResources(string resourcesString, ProjectSpecification specification)
        {
            string[] resourcesData = resourcesString.Split('\n');

            for (var i = 1; i < resourcesData.Length; i++)
            {
                string[] resourceData = Regex.Split(resourcesData[i].Trim(), @"\s+");

                if (resourceData.Length >= RES_DEF_RELEVANT_COLUMNS)
                { 
                    Resource resource = FilesManager.GetResourceFromArray(resourceData);
                    specification.Resources.Add(resource);
                }
            }
        }

        private static void FillPredecessors(string[] taskData, ProjectSpecification specification)
        {
            string taskName = taskData[0];
            Task task = specification.Tasks.FirstOrDefault(x => x.Name == taskName);

            for(var i = 4; i < taskData.Length; i++)
            {
                string predecessorId = taskData[i];

                if (!String.IsNullOrEmpty(predecessorId))
                {
                    Task predecessor = specification.Tasks.FirstOrDefault(x => x.Name == predecessorId);
                    task.Predecessors.Add(predecessor);
                }
            }
        }

        private static Task GetTaskFromArray(string[] taskData)
        {
            string name = taskData[0];
            int duration = int.Parse(taskData[1]);
            string requiredSkillName = taskData[2].Substring(0, taskData[2].Length - 1);
            int requiredSkillLevel = int.Parse(taskData[3]);

            Task task = new Task(name, duration);
            task.RequiredSkills.Add(new Skill(requiredSkillName, requiredSkillLevel));

            return task;
        }

        private static void ParseTasks(string tasksString, ProjectSpecification specification)
        {
            string[] tasksData = tasksString.Split('\n');

            for (var i = 1; i < tasksData.Length; i++)
            {
                string[] taskData = Regex.Split(tasksData[i].Trim(), @"\s+");

                if (taskData.Length >= TASK_DEF_RELEVANT_COLUMNS)
                {
                    Task task = FilesManager.GetTaskFromArray(taskData);
                    specification.Tasks.Add(task);
                }
            }

            for (var i = 1; i < tasksData.Length; i++)
            {
                string[] taskData = Regex.Split(tasksData[i].Trim(), @"\s+");

                if (taskData.Length >= TASK_DEF_RELEVANT_COLUMNS)
                {
                    FilesManager.FillPredecessors(taskData, specification);
                }
            }
        }

        public static ProjectSpecification ParseProjectData(string path)
        {
            string fileContent = FilesManager.ReadFile(path);
            string[] sections = Regex.Split(fileContent, "=+.+\n");
            int numSections = sections.Length;

            string tasksSection = String.IsNullOrEmpty(sections[numSections - 1]) ? sections[numSections - 2] : sections[numSections - 1];
            string resourcesSection = String.IsNullOrEmpty(sections[numSections - 1]) ? sections[numSections - 3] : sections[numSections - 2];

            ProjectSpecification projectSpecification = new ProjectSpecification();
            FilesManager.ParseResources(resourcesSection, projectSpecification);
            FilesManager.ParseTasks(tasksSection, projectSpecification);

            FilesManager.AssignAvailableResources(projectSpecification);

            return projectSpecification;
        }

        public static string SerializeSchedule(Schedule schedule)
        {
            string header = "Hour 	 Resource assignments (resource ID - task ID) ";
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(header);

            IEnumerable<TaskAssignment> assignments = schedule.GetAllAssignments();
            IEnumerable<int> offsets = assignments.Select(x => x.StartOffset).Distinct();

            foreach(int offset in offsets)
            {
                IEnumerable<TaskAssignment> assignmentsAtOffset = schedule.GetTaskAt(offset).Where(x => x.StartOffset == offset);
                IEnumerable<string> assignmentsAsString = assignmentsAtOffset.Select(x => x.Resource.Name + "-" + x.Task.Name);
                string line = offset.ToString() + " " + String.Join(" ", assignmentsAsString);
                builder.AppendLine(line);
            }

            return builder.ToString();
        }

        private static string SerializeResultQualities(List<double[]> qualities)
        {
            StringBuilder statisticsBuilder = new StringBuilder();

            statisticsBuilder.AppendLine("Generation;Worst;Average;Best");

            for (int i = 0; i < qualities.Count; i++)
            {
                string worstResult = qualities[i][0].ToString(CultureInfo.CurrentUICulture);
                string averageResult = qualities[i][1].ToString(CultureInfo.CurrentUICulture);
                string bestResult = qualities[i][2].ToString(CultureInfo.CurrentUICulture);

                string partialResult = String.Format("{0};{1};{2};{3}", i + 1, worstResult, averageResult, bestResult);
                statisticsBuilder.AppendLine(partialResult);
            }

            return statisticsBuilder.ToString();
        }

        private static string CreateRunSummary(string filePath, MSRCPSPEvolutionarySolver solver, Schedule result)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("File: " + filePath);
            builder.AppendLine("Metaheuristic: Evolutionary algorithm");
            builder.AppendLine("Number of generations: " + solver.MaxGenerations);
            builder.AppendLine("Population size: " + solver.PopulationSize);
            builder.AppendLine("Percent of population in a tournament group: " + solver.PercentInGroup);
            builder.AppendLine("Crossover probability: " + solver.CrossoverProbability);
            builder.AppendLine("Mutation probability: " + solver.MutationProbability);
            builder.AppendLine();
            builder.AppendLine("Result duration: " + result.Length);
            builder.AppendLine("Result cost: " + result.SummaryCost);
            builder.AppendLine("========================================");
            builder.Append(FilesManager.SerializeSchedule(result));

            return builder.ToString();
        }

        private static string CreateRunSummary(string filePath, MSRCPSPAnnealedEASolver solver, Schedule result)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("File: " + filePath);
            builder.AppendLine("Metaheuristic: Evolutionary algorithm");
            builder.AppendLine("Number of generations: " + solver.MaxGenerations);
            builder.AppendLine("Population size: " + solver.PopulationSize);
            builder.AppendLine("Percent of population in a tournament group: " + solver.PercentInGroup);
            builder.AppendLine("Crossover probability: " + solver.CrossoverProbability);
            builder.AppendLine("Mutation probability: " + solver.MutationProbability);
            builder.AppendLine();
            builder.AppendLine("Result duration: " + result.Length);
            builder.AppendLine("Result cost: " + result.SummaryCost);
            builder.AppendLine("========================================");
            builder.Append(FilesManager.SerializeSchedule(result));

            return builder.ToString();
        }

        private static string CreateRunSummary(string filePath, MSRCPSPTabuCorrectedEASolver solver, Schedule result)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("File: " + filePath);
            builder.AppendLine("Metaheuristic: Evolutionary algorithm");
            builder.AppendLine("Number of generations: " + solver.MaxGenerations);
            builder.AppendLine("Population size: " + solver.PopulationSize);
            builder.AppendLine("Percent of population in a tournament group: " + solver.PercentInGroup);
            builder.AppendLine("Crossover probability: " + solver.CrossoverProbability);
            builder.AppendLine("Mutation probability: " + solver.MutationProbability);
            builder.AppendLine();
            builder.AppendLine("Result duration: " + result.Length);
            builder.AppendLine("Result cost: " + result.SummaryCost);
            builder.AppendLine("========================================");
            builder.Append(FilesManager.SerializeSchedule(result));

            return builder.ToString();
        }

        private static string CreateRunSummary(string filePath, TabuSolver<ScheduleSpecimen> solver, Schedule result)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("File: " + filePath);
            builder.AppendLine("Metaheuristic: Tabu Search");
            builder.AppendLine("Number of steps: " + solver.NumberOfSteps);
            builder.AppendLine("Tabu size: " + solver.TabuSize);
            builder.AppendLine();
            builder.AppendLine("Result duration: " + result.Length);
            builder.AppendLine("Result cost: " + result.SummaryCost);
            builder.AppendLine("========================================");
            builder.Append(FilesManager.SerializeSchedule(result));

            return builder.ToString();
        }

        private static string CreateRunSummary(string filePath, SimulatedAnnealingSolver<ScheduleSpecimen> solver, Schedule result)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("File: " + filePath);
            builder.AppendLine("Metaheuristic: Simulated Annealing");
            builder.AppendLine("Initial temperature: " + solver.InitialTemperature);
            builder.AppendLine("Max iterations: " + solver.MaxIterations);
            builder.AppendLine();
            builder.AppendLine("Result duration: " + result.Length);
            builder.AppendLine("Result cost: " + result.SummaryCost);
            builder.AppendLine("========================================");
            builder.Append(FilesManager.SerializeSchedule(result));

            return builder.ToString();
        }

        public static void SaveResults(string path, ProjectSpecification project, MSRCPSPEvolutionarySolver solver, Schedule result, List<double[]> functionChange = null)
        {
            string folderName = DateTime.Now.ToString("yyyyMMdd hhmmss");
            string workingPath = Directory.GetCurrentDirectory() + "/" + folderName;
            string baseFileName = Path.GetFileName(path);

            Directory.CreateDirectory(workingPath);
            string serializedResult = FilesManager.SerializeSchedule(result);

            string qualitiesResult = null;
            if (functionChange != null)
            {
                qualitiesResult = FilesManager.SerializeResultQualities(functionChange);
            }
            string runSummary = FilesManager.CreateRunSummary(path, solver, result);

            FilesManager.SaveToFile(workingPath + "/" + baseFileName + ".sol", serializedResult);

            if (qualitiesResult != null)
            {
                FilesManager.SaveToFile(workingPath + "/QualitiesHistory.csv", qualitiesResult);
            }

            FilesManager.SaveToFile(workingPath + "/RunSummary.txt", runSummary);
        }

        public static void SaveResults(string path, ProjectSpecification project, MSRCPSPAnnealedEASolver solver, Schedule result, List<double[]> functionChange = null)
        {
            string folderName = DateTime.Now.ToString("yyyyMMdd hhmmss");
            string workingPath = Directory.GetCurrentDirectory() + "/" + folderName;
            string baseFileName = Path.GetFileName(path);

            Directory.CreateDirectory(workingPath);
            string serializedResult = FilesManager.SerializeSchedule(result);

            string qualitiesResult = null;
            if (functionChange != null)
            {
                qualitiesResult = FilesManager.SerializeResultQualities(functionChange);
            }
            string runSummary = FilesManager.CreateRunSummary(path, solver, result);

            FilesManager.SaveToFile(workingPath + "/" + baseFileName + ".sol", serializedResult);

            if (qualitiesResult != null)
            {
                FilesManager.SaveToFile(workingPath + "/QualitiesHistory.csv", qualitiesResult);
            }

            FilesManager.SaveToFile(workingPath + "/RunSummary.txt", runSummary);
        }

        public static void SaveResults(string path, ProjectSpecification project, MSRCPSPTabuCorrectedEASolver solver, Schedule result, List<double[]> functionChange = null)
        {
            string folderName = DateTime.Now.ToString("yyyyMMdd hhmmss");
            string workingPath = Directory.GetCurrentDirectory() + "/" + folderName;
            string baseFileName = Path.GetFileName(path);

            Directory.CreateDirectory(workingPath);
            string serializedResult = FilesManager.SerializeSchedule(result);

            string qualitiesResult = null;
            if (functionChange != null)
            {
                qualitiesResult = FilesManager.SerializeResultQualities(functionChange);
            }
            string runSummary = FilesManager.CreateRunSummary(path, solver, result);
            runSummary += "==========================================";
            runSummary += FilesManager.CreateRunSummary(path, solver.TabuSolver, result);

            FilesManager.SaveToFile(workingPath + "/" + baseFileName + ".sol", serializedResult);

            if (qualitiesResult != null)
            {
                FilesManager.SaveToFile(workingPath + "/QualitiesHistory.csv", qualitiesResult);
            }

            FilesManager.SaveToFile(workingPath + "/RunSummary.txt", runSummary);          
        }

        public static void SaveResults(string path, ProjectSpecification project, TabuSolver<ScheduleSpecimen> solver, Schedule result, List<double[]> functionChange = null)
        {
            string folderName = DateTime.Now.ToString("yyyyMMdd hhmmss");
            string workingPath = Directory.GetCurrentDirectory() + "/" + folderName;
            string baseFileName = Path.GetFileName(path);

            Directory.CreateDirectory(workingPath);
            string serializedResult = FilesManager.SerializeSchedule(result);

            string qualitiesResult = null;
            if (functionChange != null)
            {
                qualitiesResult = FilesManager.SerializeResultQualities(functionChange);
            }
            string runSummary = FilesManager.CreateRunSummary(path, solver, result);

            FilesManager.SaveToFile(workingPath + "/" + baseFileName + ".sol", serializedResult);

            if (qualitiesResult != null)
            {
                FilesManager.SaveToFile(workingPath + "/QualitiesHistory.csv", qualitiesResult);
            }

            FilesManager.SaveToFile(workingPath + "/RunSummary.txt", runSummary);
        }

        public static void SaveResults(string path, ProjectSpecification project, SimulatedAnnealingSolver<ScheduleSpecimen> solver, Schedule result, List<double[]> functionChange = null)
        {
            string folderName = DateTime.Now.ToString("yyyyMMdd hhmmss");
            string workingPath = Directory.GetCurrentDirectory() + "/" + folderName;
            string baseFileName = Path.GetFileName(path);

            Directory.CreateDirectory(workingPath);
            string serializedResult = FilesManager.SerializeSchedule(result);

            string qualitiesResult = null;
            if (functionChange != null)
            {
                qualitiesResult = FilesManager.SerializeResultQualities(functionChange);
            }

            string runSummary = FilesManager.CreateRunSummary(path, solver, result);

            FilesManager.SaveToFile(workingPath + "/" + baseFileName + ".sol", serializedResult);

            if (qualitiesResult != null)
            {
                FilesManager.SaveToFile(workingPath + "/QualitiesHistory.csv", qualitiesResult);
            }

            FilesManager.SaveToFile(workingPath + "/RunSummary.txt", runSummary);
        }
    }
}
