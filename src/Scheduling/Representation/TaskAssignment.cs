using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule.Representation
{
    public class TaskAssignment
    {
        public Task Task
        {
            get;
            protected set;
        }

        public Resource Resource
        {
            get;
            set;
        }

        public int StartOffset
        {
            get;
            set;
        }

        public int EndOffset
        {
            get
            {
                return StartOffset + Length;
            }
        }

        public float Cost
        {
            get
            {
                return this.Task.Length * this.Resource.Cost;
            }
        }

        public int Length
        {
            get
            {
                return this.Task.Length + this.StartOffset;
            }
        }

        public TaskAssignment(Task task, Resource resource, int offset)
        {
            if(task == null || resource == null)
            {
                throw new ArgumentNullException();
            }

            this.Task = task;
            this.Resource = resource;
            this.StartOffset = offset;
        }

        public TaskAssignment(TaskAssignment assignment) : this(assignment.Task, assignment.Resource, assignment.StartOffset)
        {

        }
    }
}
