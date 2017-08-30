using System;
using System.Collections.Generic;
using System.Text;

namespace RESTGrid.Models
{
    public class BusinessLogic
    {
        public Task Start { get; set; }

        public List<Task> Tasks { get; set; }

        public Task FindTask(string identifier)
        {
            Task foundTask = null;

            if (this.Start != null && this.Start.Identifier.ToLower() == identifier.ToLower())
                foundTask = this.Start;
            else
            {
                if (this.Tasks != null && this.Tasks.Count > 0)
                {
                    foreach (Task task in this.Tasks)
                    {
                        if (task.Identifier.ToLower() == identifier.ToLower())
                        {
                            foundTask = task;
                            break;
                        }
                    }
                }
            }

            return foundTask;
        }
    }
}
