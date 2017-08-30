using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RESTGrid.Interfaces;
using RESTGrid.Providers.MySql;
using Newtonsoft.Json.Linq;
using RESTGrid.Models;

namespace RESTGrid.MySqlEngine.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Orchestration")]
    public class OrchestrationController : Controller
    {
        string _connectionString;
        private IOrchestration _orchestration;

        public OrchestrationController(IOptions<MySqlConfiguration> mySqlModel)
        {
            _connectionString = mySqlModel.Value.ConnectionString;
            _orchestration = new MySqlOrchestration(_connectionString);
        }

        [HttpPut("Workflow/{customPropertyName}/{customPropertyValue}")]
        public ActionResult Put(string customPropertyName, string customPropertyValue, [FromBody]object value)
        {
            ActionResult result = null;

            try
            {
                JObject messageBodyJson = null;

                if(value != null)
                    messageBodyJson = (JObject)value;

                _orchestration.SetWorkflowActive(messageBodyJson, customPropertyName, customPropertyValue);

                result = NoContent();
            }
            catch (Exception ex)
            {
                result = StatusCode(500, new { Error = ex.Message });
            }

            return result;
        }

        [HttpPost("Workflow/{workflowTypeName}")]
        public ActionResult Post(string workflowTypeName, [FromBody]WorkflowTypeFromBody value)
        {
            ActionResult result = null;

            try
            {
                JObject messageBodyJson = null;
                JObject customPropertiesJson = null;

                if (value != null)
                {
                    messageBodyJson = value.MessageBody;
                    customPropertiesJson = value.CustomProperties;
                }

                _orchestration.PublishWorkflowStep(workflowTypeName,Guid.NewGuid(),messageBodyJson,customPropertiesJson,null,true,false,0,true,
                    null,"0");

                result = NoContent();
            }
            catch (Exception ex)
            {
                result = StatusCode(500, new { Error = ex.Message });
            }

            return result;
        }
    }
}