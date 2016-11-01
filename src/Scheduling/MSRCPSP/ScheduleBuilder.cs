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
        private static int ScheduleTask(ProjectSpecification projectData, ScheduleSpecimen specimen, Task task, Schedule schedule)
        {
            int offset = 1;
            TaskAssignment existingAssignment = schedule.GetAssignmentByTask(task);

            if (existingAssignment != null)
            {
                return existingAssignment.EndOffset;
            }

            if (task.Predecessors.Count > 0)
            {
                List<Task> predecessors = task.Predecessors.OrderBy(x => Array.IndexOf(specimen.Tasks, x)).ToList();

                for (int i = 0; i < predecessors.Count; i++)
                {
                    Task predecessor = predecessors[i];
                    int newOffset = ScheduleBuilder.ScheduleTask(projectData, specimen, predecessor, schedule);

                    offset = offset > newOffset ? offset : newOffset;
                }
            }

            Resource resource = null;
            List<Resource> availableResources = task.AvailableResources;

            while(resource == null)
            {
                TaskAssignment earliestEndingCollision = null;

                foreach(Resource availableResource in availableResources)
                {
                    TaskAssignment collidingAssignment = schedule.GetCollidingAssignment(availableResource, offset, offset + task.Length);

                    if (collidingAssignment == null)
                    {
                        resource = availableResource;
                        break;
                    }
                    else if (earliestEndingCollision == null || collidingAssignment.EndOffset < earliestEndingCollision.EndOffset)
                    {
                        earliestEndingCollision = collidingAssignment;
                    }
                }

                if (resource == null)
                {
                    offset = earliestEndingCollision.EndOffset + 1;
                }
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
                ScheduleBuilder.ScheduleTask(projectData, specimen, currentTask, schedule);
            }

            return schedule;
        }
    }
}
