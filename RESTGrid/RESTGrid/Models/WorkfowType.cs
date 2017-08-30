using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace RESTGrid.Models
{
    public class WorkfowType
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public JObject BusinessLogicJson { get; set; }
    }
}
