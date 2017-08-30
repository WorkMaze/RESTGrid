using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace RESTGrid.Models
{
    public class Task
    {
        public string Identifier { get; set; }       

        public JObject TaskProperties { get; set; }
        
        public JObject RunCondition { get; set; }
        
        public List<string> Next { get; set; }

        public int? TaskRetries { get; set; }

        public TaskType Type { get; set; }

        public bool HasNextTasks()
        {
            bool hasNextTasks = false;

            if (this.Next != null && this.Next.Count > 0)
                hasNextTasks = true;

            return hasNextTasks;
        }

        public bool SatisfiesRunCondition(QueueMetadata queueMetadata)
        {
            bool conditionSatisfied = false;

            if (this.RunCondition == null)
                conditionSatisfied = true;
            else
            {
                JObject input = JObject.FromObject(queueMetadata);
                JObject ceJson = JUST.JsonTransformer.Transform(this.RunCondition, input);

                ConditionExpression condition = ceJson.ToObject<ConditionExpression>();

                if ((condition.Operator.ToLower() == "stringequals") && (condition.Evaluator == condition.Evaluated))
                {
                    conditionSatisfied = true;
                }
                if ((condition.Operator.ToLower() == "stringcontains") && (condition.Evaluator.Contains(condition.Evaluated)))
                {
                    conditionSatisfied = true;
                }
                if (condition.Operator.ToLower().Contains("math"))
                {
                    decimal lshDecimal = Convert.ToDecimal(condition.Evaluator);
                    decimal rhsDecimal = Convert.ToDecimal(condition.Evaluated);

                    if ((condition.Operator.ToLower() == "mathequals") && (lshDecimal == rhsDecimal))
                        conditionSatisfied = true;
                    if ((condition.Operator.ToLower() == "mathgreaterthan") && (lshDecimal > rhsDecimal))
                        conditionSatisfied = true;
                    if ((condition.Operator.ToLower() == "mathlessthan") && (lshDecimal < rhsDecimal))
                        conditionSatisfied = true;
                    if ((condition.Operator.ToLower() == "mathgreaterthanorequalto") && (lshDecimal >= rhsDecimal))
                        conditionSatisfied = true;
                    if ((condition.Operator.ToLower() == "mathlessthanorequalto") && (lshDecimal <= rhsDecimal))
                        conditionSatisfied = true;
                }
            }

            return conditionSatisfied;
        }


    }

    public enum TaskType
    {
        Sync , Async, Transformer, Splitter
    }
}
