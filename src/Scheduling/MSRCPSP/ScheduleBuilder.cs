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
        private static bool ScheduleTask(ProjectSpecification projectData, ScheduleSpecimen specimen, Task task, Schedule schedule)
        {
            int offset = 1;

            TaskAssignment currentAssignment = schedule.GetAssignmentByTask(task);

            if (currentAssignment != null)
            {
                return true;
            }

            if (task.Predecessors.Count > 0)
            {
                List<TaskAssignment> assignedPredecessors = task.Predecessors.Select(x => schedule.GetAssignmentByTask(x))
                                                                             .Where(x => x != null).ToList();

                if (assignedPredecessors.Count < task.Predecessors.Count)
                {
                    return false;
                }
                else
                {
                    int latestPredecessorOffset = assignedPredecessors.Max(x => x.EndOffset);
                    offset = latestPredecessorOffset;
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
                    else if (earliestEndingCollision == null || collidingAssignment.EndOffset <= earliestEndingCollision.EndOffset)
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

            return true;
        }

        public static Schedule BuildScheduleFromSpecimen(ProjectSpecification projectData, ScheduleSpecimen specimen)
        {
            Schedule schedule = new Schedule();


            while (schedule.GetAllAssignments().Count() != specimen.Tasks.Length)
            {
                for (int i = 0; i < specimen.Tasks.Length; i++)
                {
                    Task currentTask = specimen.Tasks[i];
                    ScheduleBuilder.ScheduleTask(projectData, specimen, currentTask, schedule);
                }
            }

            return schedule;
        }
    }
}
