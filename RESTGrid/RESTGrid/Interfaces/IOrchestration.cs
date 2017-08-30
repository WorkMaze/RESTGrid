using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace RESTGrid.Interfaces
{
    public interface IOrchestration
    {
		
        void PublishWorkflowStep(string workflowTypeName, Guid workflowID, JObject messageBodyJson, JObject customPropertiesJson, string stepIdentifier,
    bool stepSucceeded, bool workflowCompleted, int retries, bool active, string runStepIdentifier, string splitID);

        void SetWorkflowActive(JObject messageBodyJson, string customPropertyName, string customPropertyValue);

        List<RESTGrid.Models.Queue> Enqueue();

        JObject GetTransformer(int transformerID);
    }
}
