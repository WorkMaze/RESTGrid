using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace RESTGrid.Models
{
    public class QueueMetadata
    {
        public Guid ID { get; set; }

        public JObject CustomPropertiesJson { get; set; }

        public JObject MessageBodyJson { get; set; }

        public DateTime Timestamp { get; set; }

        public string SplitID { get; set; }

        public string StepIdentifier { get; set; }

        public bool Success { get; set; }

        public int Retries { get; set; }
    }
}
