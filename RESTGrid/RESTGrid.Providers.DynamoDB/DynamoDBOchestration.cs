using RESTGrid.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using RESTGrid.Models;

namespace RESTGrid.Providers.DynamoDB
{

    public class DynamoDBOchestration : IOrchestration
    {
        private DynamoDBConfiguration _configuration;

        public DynamoDBOchestration(DynamoDBConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Queue> Enqueue()
        {
            throw new NotImplementedException();
        }

        public JObject GetTransformer(int transformerID)
        {
            throw new NotImplementedException();
        }

        public void PublishWorkflowStep(string workflowTypeName, Guid workflowID, JObject messageBodyJson, JObject customPropertiesJson, string stepIdentifier, bool stepSucceeded, bool workflowCompleted, int retries, bool active, string runStepIdentifier, string splitID)
        {
            throw new NotImplementedException();
        }

        public void SetWorkflowActive(JObject messageBodyJson, string customPropertyName, string customPropertyValue)
        {
            throw new NotImplementedException();
        }
    }
}
