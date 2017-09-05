using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RESTGrid.Interfaces;
using RESTGrid.Providers.DynamoDB;
using Microsoft.Extensions.Options;

namespace RESTGrid.DynamoDBEngine.API.Controllers
{
    [Produces("application/json")]
    [Route("api/Administration")]
    public class AdministrationController : Controller
    {
        private DynamoDBConfiguration _model;
        private IAdministration _administration;

        public AdministrationController(IOptions<DynamoDBConfiguration> model)
        {
            _model = model.Value;
            _administration = new DynamoDBAdministration(_model);
        }

        [HttpGet("History/{workflowID}")]
        public ActionResult Get(string workflowID)
        {
            ActionResult result = null;

            try
            {

                Models.WorkflowHistory history = _administration.GetHistory(workflowID);

                result = Ok(history);
            }
            catch (Exception ex)
            {
                result = StatusCode(500, new { Error = ex.Message });
            }

            return result;
        }

        [HttpPost("WorkflowType/{workflowTypeName}")]
        public ActionResult PostWorkflowType(string workflowTypeName, [FromBody]object value)
        {
            ActionResult result = null;

            try
            {
                JObject businessLogicJson = (JObject)value;

                _administration.CreateWorkflowType(workflowTypeName, businessLogicJson);

                result = NoContent();
            }
            catch (Exception ex)
            {
                result = StatusCode(500, new { Error = ex.Message });
            }

            return result;
        }

        [HttpPost("Transformer")]
        public ActionResult PostTransformer([FromBody]object value)
        {
            ActionResult result = null;

            try
            {
                JObject transformer = (JObject)value;

                _administration.CreateTransformer(transformer);

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