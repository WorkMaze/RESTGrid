using System;
using System.Collections.Generic;
using System.Text;

namespace RESTGrid.Models
{
    public class Queue
    {
        public Guid WorkflowID { get; set; }

        public WorkfowType Type { get; set; }

        public List<QueueMetadata> Metadata { get; set; }
    }
}
