using Newtonsoft.Json.Linq;
using RESTGrid.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RESTGrid.Interfaces
{
	public interface IAdministration
    {
        void CreateWorkflowType(string workflowTypeName, JObject businessLogicJson);

        void CreateTransformer(JObject transformerJson);

        WorkflowHistory GetHistory(string workflowID);

    }
}
