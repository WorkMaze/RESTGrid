using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace RESTGrid.Models
{
    public class WorkflowTypeFromBody
    {
        public JObject MessageBody { get; set; }

        public JObject CustomProperties { get; set; }
    }
}
