using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Szab.Scheduling.Representation;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Szab.Scheduling.Tools
{
    public static class FilesManager
    {
        private const int TASK_DEF_RELEVANT_COLUMNS = 4;
        private const int RES_DEF_RELEVANT_COLUMNS = 2;

        private static string readFile(string path)
        {
            string fileContents = "";

            using (StreamReader stream = new StreamReader(new FileStream(path, FileMode.Open)))
            {
                fileContents = stream.ReadToEnd();
            }

            return fileContents;   
        }

        private static Resource getResourceFromArray(string[] resourceData)
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

        private static void parseResources(string resourcesString, ProjectSpecification specification)
        {
            string[] resourcesData = resourcesString.Split('\n');

            for (var i = 1; i < resourcesData.Length; i++)
            {
                string[] resourceData = Regex.Split(resourcesData[i].Trim(), @"\s+");

                if (resourceData.Length >= RES_DEF_RELEVANT_COLUMNS)
                { 
                    Resource resource = FilesManager.getResourceFromArray(resourceData);
                    specification.Resources.Add(resource);
                }
            }
        }

        private static void fillPredecessors(string[] taskData, ProjectSpecification specification)
        {
            string taskName = taskData[0];
            Task task = specification.Tasks.Where(x => x.Name == taskName).FirstOrDefault();

            for(var i = 4; i < taskData.Length; i++)
            {
                string predecessorId = taskData[i];

                if (!String.IsNullOrEmpty(predecessorId))
                {
                    Task predecessor = specification.Tasks.Where(x => x.Name == predecessorId).FirstOrDefault();
                    task.Predecessors.Add(predecessor);
                }
            }
        }

        private static Task getTaskFromArray(string[] taskData)
        {
            string name = taskData[0];
            int duration = int.Parse(taskData[1]);
            string requiredSkillName = taskData[2].Substring(0, taskData[2].Length - 1);
            int requiredSkillLevel = int.Parse(taskData[3]);

            Task task = new Task(name, duration);
            task.RequiredSkills.Add(new Skill(requiredSkillName, requiredSkillLevel));

            return task;
        }

        private static void parseTasks(string tasksString, ProjectSpecification specification)
        {
            string[] tasksData = tasksString.Split('\n');

            for (var i = 1; i < tasksData.Length; i++)
            {
                string[] taskData = Regex.Split(tasksData[i].Trim(), @"\s+");

                if (taskData.Length >= TASK_DEF_RELEVANT_COLUMNS)
                {
                    Task task = FilesManager.getTaskFromArray(taskData);
                    specification.Tasks.Add(task);
                }
            }

            for (var i = 1; i < tasksData.Length; i++)
            {
                string[] taskData = Regex.Split(tasksData[i].Trim(), @"\s+");

                if (taskData.Length >= TASK_DEF_RELEVANT_COLUMNS)
                {
                    FilesManager.fillPredecessors(taskData, specification);
                }
            }
        }

        public static ProjectSpecification ParseProjectData(string path)
        {
            string fileContent = FilesManager.readFile(path);
            string[] sections = Regex.Split(fileContent, "=+\n");
            int numSections = sections.Length;

            string tasksSection = String.IsNullOrEmpty(sections[numSections - 1]) ? sections[numSections - 2] : sections[numSections - 1];
            string resourcesSection = String.IsNullOrEmpty(sections[numSections - 1]) ? sections[numSections - 3] : sections[numSections - 2];

            ProjectSpecification projectSpecification = new ProjectSpecification();
            FilesManager.parseResources(resourcesSection, projectSpecification);
            FilesManager.parseTasks(tasksSection, projectSpecification);

            projectSpecification.SortData();

            return projectSpecification;
        }

        public static string SaveToFile(Schedule schedule)
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
    }
}
