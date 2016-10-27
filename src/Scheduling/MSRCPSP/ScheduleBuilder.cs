using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Szab.Scheduling.Representation;
using Szab.Scheduling.Tools;

namespace Szab.Scheduling.MSRCPSP
{
    public static class ScheduleBuilder
    {
        private static Resource GetAvailableResourceForTask(ProjectSpecification projectData, Schedule schedule, int offset, Task task)
        {
            List<Resource> availableResources = task.AvailableResources.ToList();
            availableResources.Shuffle();

            foreach (Resource resource in availableResources)
            {
                bool isFree = schedule.IsResourceAvailableAt(resource, offset, offset + task.Length);

                if (isFree)
                {
                    return resource;
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
