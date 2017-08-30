using System;
using System.Collections.Generic;
using System.Text;

namespace RESTGrid.Models
{
    public class WorkflowHistory
    {
        public Guid WorkflowID { get; set; }

        public string SplitID { get; set; }

        public WorkfowType Type { get; set; }

        public List<History> HisoryObjects { get; set; }
    }
}
