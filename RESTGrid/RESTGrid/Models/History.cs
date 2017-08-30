using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace RESTGrid.Models
{
    public class History
    {
        public JObject CustomPropertiesJson { get; set; }

        public JObject EventJson { get; set; }

        public JObject Body { get; set; }

        public DateTime Timestamp { get; set; }

    }
}
