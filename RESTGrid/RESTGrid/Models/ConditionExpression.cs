using System;
using System.Collections.Generic;
using System.Text;

namespace RESTGrid.Models
{
    public class ConditionExpression
    {
        public string Evaluator { get; set; }
        public string Evaluated { get; set; }
        public string Operator { get; set; }
    }
}
