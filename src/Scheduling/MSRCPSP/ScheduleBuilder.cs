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

            // STEP 1: Check if task has already been scheduled - if yes, return; if no - schedule the task
            TaskAssignment currentAssignment = schedule.GetAssignmentByTask(task);

            if (currentAssignment != null)
            {
                return true;
            }


            // STEP 2: Check predecessors
            if (task.Predecessors.Count > 0)
            {
                // 2a: get a list of already scheduled predecessors
                List<TaskAssignment> assignedPredecessors = task.Predecessors.Select(x => schedule.GetAssignmentByTask(x))
                                                                             .Where(x => x != null).ToList();

                // 2b: check if all required predecessors have already been scheduled; if yes - schedule the task. if no - return and wait.
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

            // STEP 3: resolve collisions and assign resources
            Resource resource = null;
            List<Resource> availableResources = task.AvailableResources;

            // Repeat unless found a free resource
            while(resource == null)
            {
                TaskAssignment earliestEndingCollision = null;

                // Try to assign resources from the cheapest to the most expensive
                foreach(Resource availableResource in availableResources)
                {
                    // Check if the resource already has an assignment at the given time
                    TaskAssignment collidingAssignment = schedule.GetCollidingAssignment(availableResource, offset, offset + task.Length);

                    // If not, assign it
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

                // If no resource is available, move to the earliest time when any of the resources seem to be available
                if (resource == null)
                {
                    offset = earliestEndingCollision.EndOffset + 1;
                }
            }

            // Create an assignment and add it to the schedule
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
