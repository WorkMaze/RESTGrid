using RESTGrid.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using RESTGrid.Models;

namespace RESTGrid.Providers.DynamoDB
{
    public class DynamoDBAdministration : IAdministration
    {
        private DynamoDBConfiguration _configuration;

        public DynamoDBAdministration(DynamoDBConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void CreateTransformer(JObject transformerJson)
        {
            throw new NotImplementedException();
        }

        public void CreateWorkflowType(string workflowTypeName, JObject businessLogicJson)
        {
            throw new NotImplementedException();
        }

        public WorkflowHistory GetHistory(string workflowID)
        {
            throw new NotImplementedException();
        }
    }
}
