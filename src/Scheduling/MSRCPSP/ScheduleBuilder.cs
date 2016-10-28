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
        private static int ScheduleTask(ProjectSpecification projectData, Task task, Schedule schedule)
        {
            int offset = 1;
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
            List<Resource> availableResources = task.AvailableResources;
            //IEnumerable<TaskAssignment> relevantAssignments = schedule.GetTasksForResources(availableResources).OrderBy(x => x.StartOffset);

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
                    else if (earliestEndingCollision == null || collidingAssignment.StartOffset < earliestEndingCollision.StartOffset)
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
                ScheduleBuilder.ScheduleTask(projectData, currentTask, schedule);
            }

            return schedule;
        }
    }
}
