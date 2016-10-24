using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule.Representation
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
            return this.tasks.Where(x => x.StartOffset >= time && x.EndOffset <= time);
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
