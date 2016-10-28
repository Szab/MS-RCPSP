using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szab.Scheduling.Representation
{
    public class Schedule
    {
        private List<TaskAssignment> tasks;

        public float SummaryCost
        {
            get
            {
                return this.tasks.Sum(x => x.Cost);
            }
        }

        public int Length
        {
            get
            {
                return this.tasks.Max(x => x.EndOffset);
            }
        }

        public IEnumerable<TaskAssignment> Last
        {
            get
            {
                return this.tasks.Where(x => x.EndOffset == this.Length);
            }
        }

        public void Add(Task task, Resource resource, int offset)
        {
            TaskAssignment newAssignment = new TaskAssignment(task, resource, offset);
            this.tasks.Add(newAssignment);
        }

        public void Add(TaskAssignment assignment)
        {
            if (assignment == null)
            {
                throw new ArgumentNullException("assignment");
            }

            this.tasks.Add(assignment);
        }

        public IEnumerable<TaskAssignment> Remove(Task task)
        {
            var assignments = tasks.Where(x => x.Task == task);
            this.tasks.RemoveAll(x => x.Task == task);

            return assignments;
        }

        public IEnumerable<TaskAssignment> Remove(TaskAssignment taskAssignment)
        {
            var assignments = tasks.Where(x => x == taskAssignment);
            this.tasks.RemoveAll(x => x == taskAssignment);

            return assignments;
        }

        public IEnumerable<TaskAssignment> RemoveAt(int time)
        {
            var assignments = this.GetTaskAt(time);
            this.tasks.RemoveAll(x => assignments.Contains(x));

            return assignments;
        }

        public IEnumerable<TaskAssignment> GetTaskAt(int time)
        {
            return this.tasks.Where(x => x.StartOffset >= time && x.EndOffset >= time);
        }

        public bool HasTaskAt(int time)
        {
            return this.tasks.Any(x => x.StartOffset >= time && x.EndOffset >= time);
        }

        public bool ContainsTask(Task task)
        {
            return this.tasks.Any(x => x.Task == task);
        }

        public TaskAssignment GetAssignmentByTask(Task task)
        {
            return this.tasks.Where(x => x.Task == task).FirstOrDefault();
        }

        public IEnumerable<TaskAssignment> GetAllAssignments()
        {
            return this.tasks.OrderBy(x => x.StartOffset);
        }

        public bool IsResourceAvailableAt(Resource resource, int time, int to)
        {
            return !this.tasks.Any(x => x.Resource == resource && (time <= x.StartOffset && to >= x.StartOffset ||
                                                                    time <= x.EndOffset && to >= x.EndOffset    ||
                                                                    time >= x.StartOffset && to <= x.EndOffset  ||
                                                                    time <= x.StartOffset && to >= x.EndOffset));
        }

        public TaskAssignment GetCollidingAssignment(Resource resource, int time, int to)
        {
            return this.tasks.Where(x => x.Resource == resource && (time <= x.StartOffset && to >= x.StartOffset||
                                                                    time <= x.EndOffset && to >= x.EndOffset    ||
                                                                    time >= x.StartOffset && to <= x.EndOffset  ||
                                                                    time <= x.StartOffset && to >= x.EndOffset)).FirstOrDefault();
        }

        public IEnumerable<TaskAssignment> GetTasksForResource(Resource resource)
        {
            return this.tasks.Where(x => x.Resource == resource);
        }
        
        public IEnumerable<TaskAssignment> GetTasksForResources(IEnumerable<Resource> resources)
        {
            return this.tasks.Where(x => resources.Contains(x.Resource));
        }

        public bool Validate()
        {
            throw new NotImplementedException();
        }

        public Schedule()
        {
            this.tasks = new List<TaskAssignment>();
        }
    }
}
