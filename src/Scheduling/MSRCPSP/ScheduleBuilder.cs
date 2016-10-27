using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Szab.Scheduling.Representation;

namespace Szab.Scheduling.MSRCPSP
{
    public static class ScheduleBuilder
    {
        private static bool CheckIfResourceIsValidForTask(Resource resource, Task task)
        {
            bool isValid = true;

            for(int i = 0; i < task.RequiredSkills.Count && isValid; i++)
            {
                Skill requiredSkill = task.RequiredSkills[i];

                for(int j = 0; j < resource.Skills.Count; j++)
                {
                    Skill ownedSkill = resource.Skills[j];

                    if(ownedSkill.Name == requiredSkill.Name && ownedSkill.Level >= requiredSkill.Level)
                    {
                        isValid = true;
                        break;
                    }

                    isValid = false;
                }
            }

            return isValid;
        }

        private static Resource GetAvailableResourceForTask(ProjectSpecification projectData, Schedule schedule, int offset, Task task)
        {
            IEnumerable<Skill> requiredSkills = task.RequiredSkills;
            List<Resource> resources = projectData.Resources.ToList();

            for(int i = 0; i < resources.Count; i++)
            {
                Resource currentResource = resources[i];

                bool isValid = ScheduleBuilder.CheckIfResourceIsValidForTask(currentResource, task);

                if (isValid)
                {
                    //bool isFree = schedule.IsResourceAvailableAt(currentResource, offset, offset + task.Length);

                    //if (isFree)
                    //{
                    //    return currentResource;
                    //}
                    return currentResource;
                }
            }

            return null;
        } 

        private static int ScheduleTask(ProjectSpecification projectData, Task task, Schedule schedule)
        {
            int offset = 0;
            TaskAssignment existingAssignment = schedule.GetAssignmentByTask(task);

            if (existingAssignment != null)
            {
                return existingAssignment.EndOffset;
            }

            if (task.Predecessors.Count > 0)
            {
                for (int i = 0; i < task.Predecessors.Count; i++)
                {
                    Task predecessor = task.Predecessors[i];
                    int newOffset = ScheduleBuilder.ScheduleTask(projectData, predecessor, schedule);

                    offset = offset > newOffset ? offset : newOffset;
                }
            }

            Resource resource = null;
            offset--;
            while(resource == null)
            {
                offset++;
                resource = ScheduleBuilder.GetAvailableResourceForTask(projectData, schedule, offset, task);
            }

            TaskAssignment taskAssignment = new TaskAssignment(task, resource, offset);
            schedule.Add(taskAssignment);

            return taskAssignment.EndOffset;
        }

        public static Schedule BuildScheduleFromSpecimen(ProjectSpecification projectData, ScheduleSpecimen specimen)
        {
            Schedule schedule = new Schedule();

            for(int i = 0; i < specimen.Tasks.Length; i++)
            {
                Task currentTask = specimen.Tasks[i];
                ScheduleBuilder.ScheduleTask(projectData, currentTask, schedule);
            }

            return schedule;
        }
    }
}
