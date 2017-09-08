using RESTGrid.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using RESTGrid.Models;
using Amazon.DynamoDBv2;
using Amazon;
using Amazon.Runtime;
using Amazon.DynamoDBv2.DocumentModel;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using System.Threading;
using System.Runtime.CompilerServices;

namespace RESTGrid.Providers.DynamoDB
{

    public class DynamoDBOchestration : IOrchestration
    {
        private DynamoDBConfiguration _configuration;
        private AmazonDynamoDBClient _client;

        public DynamoDBOchestration(DynamoDBConfiguration configuration)
        {
            _configuration = configuration;
            RegionEndpoint regionEndpoint = RegionEndpoint.GetBySystemName(configuration.Region);
            AWSCredentials awsCredentails = new BasicAWSCredentials(configuration.AccessKey, configuration.SecretAccessKey);
            _client = new AmazonDynamoDBClient(awsCredentails, regionEndpoint);
        }

        private Queue GetQueueByWorkflowID(List<Queue> queueList, string workflowID)
        {
            Queue found = null;

            if(queueList != null)
            {
                foreach(Queue queue in queueList)
                {
                    if(queue.WorkflowID.ToString() == workflowID)
                    {
                        found = queue;
                        break;

                    }
                }
            }

            return found;
        }

        public List<Queue> Enqueue()
        {
            string tableName = Constants.QueueTableName;
            Table table = Table.LoadTable(_client, tableName);

            ScanFilter scanFilter = new ScanFilter();
            scanFilter.AddCondition(Constants.QueueActive, ScanOperator.Equal, "True");
            
            Search search = table.Scan(scanFilter);

            List<Queue> list = new List<Queue>();

            string queueID = null;
            JObject businessLogicJson = null;
            string workflowTypeName = null;
            Guid workflowID = Guid.Empty;
            JObject customPropertiesJson = null; 
            JObject messageBodyJson = null;

            string stepIdentifier = null;
            bool stepSucceeded = true;
           
            int retries = 0;

            DateTime timeStamp = DateTime.UtcNow;
            string runStepIdentifier = null;
            string splitID = null;

            do
            {
                Task<List<Document>> documentListTask = search.GetNextSetAsync();
                documentListTask.Wait();

                List<Document> documentList = documentListTask.Result;

                foreach (Document document in documentList)
                {
                    queueID = document[Constants.QueueId].AsString();
                    businessLogicJson = JObject.Parse(document[Constants.BusinessLogic].AsDocument().ToJson());
                    workflowTypeName = document[Constants.WorkflowType].AsString();
                    workflowID = document[Constants.HistoryWorkflowId].AsGuid();

                    if(document.ContainsKey(Constants.QueueCustomProperties))
                        customPropertiesJson = JObject.Parse(document[Constants.QueueCustomProperties].AsDocument().ToJson());

                    if (document.ContainsKey(Constants.QueueMessageBody))
                        messageBodyJson = JObject.Parse(document[Constants.QueueMessageBody].AsDocument().ToJson());

                    if (document.ContainsKey(Constants.QueueStepIdentifier))
                        stepIdentifier = document[Constants.QueueStepIdentifier].AsString();

                    if (document.ContainsKey(Constants.QueueRunStepIdentifier))
                        runStepIdentifier = document[Constants.QueueRunStepIdentifier].AsString();

                    if (document.ContainsKey(Constants.QueueSlitID))
                        splitID = document[Constants.QueueSlitID].AsString();

                    if (document.ContainsKey(Constants.QueueRetries))
                        retries = document[Constants.QueueRetries].AsInt();

                    if (document.ContainsKey(Constants.QueueStepSucceeded))
                        stepSucceeded = document[Constants.QueueStepSucceeded].AsBoolean();

                    if (document.ContainsKey(Constants.Timestamp))
                        timeStamp = Convert.ToDateTime( document[Constants.Timestamp].AsString());

                    QueueMetadata metadata = new QueueMetadata()
                    {
                        CustomPropertiesJson = customPropertiesJson,
                        ID = Guid.Parse( queueID),
                        MessageBodyJson = messageBodyJson,
                        SplitID = splitID,
                        StepIdentifier = stepIdentifier,
                        Success = stepSucceeded,
                        Retries = retries,
                        Timestamp = timeStamp
                        
                    };

                    Queue queue = GetQueueByWorkflowID(list, workflowID.ToString());

                    if (queue == null)
                        queue = new Queue()
                        {
                            Type = new WorkfowType()
                            {
                                BusinessLogicJson = businessLogicJson,
                                Name = workflowTypeName
                            },
                            WorkflowID = workflowID,
                            Metadata = new List<QueueMetadata>()
                        };

                    queue.Metadata.Add(metadata);

                    list.Add(queue);
                    DeleteFromQueue(queueID,true);
                }
            } while (!search.IsDone);

            return list;
        }

       

        public JObject GetTransformer(int transformerID)
        {
            JObject instance = null;

            Table table = Table.LoadTable(_client, Constants.ConfigurationTableName);

            var item = table.GetItemAsync(transformerID.ToString(), Constants.TransformerType);
            item.Wait();

                      
            if (item.Result != null)
            {                

                List<KeyValuePair<string, DynamoDBEntry>> rows = item.Result.ToList<KeyValuePair<string, DynamoDBEntry>>();

                foreach (KeyValuePair<string, DynamoDBEntry> row in rows)
                {
                    if (row.Key == Constants.ConfigurationValue)
                        instance = JObject.Parse( row.Value.AsDocument().ToJson());
                    
                   
                }
            }

            return instance;
        }

        public void PublishWorkflowStep(string workflowTypeName, Guid workflowID, JObject messageBodyJson, JObject customPropertiesJson, string stepIdentifier, bool stepSucceeded, bool workflowCompleted, int retries, bool active, string runStepIdentifier, string splitID)
        {
            string queueID = Guid.NewGuid().ToString();

            Table table = Table.LoadTable(_client, Constants.ConfigurationTableName);

            var item = table.GetItemAsync(workflowTypeName, Constants.WorkflowType);
            item.Wait();

            JObject businessLogicJson = null;

            if (item.Result != null)
            {

                List<KeyValuePair<string, DynamoDBEntry>> rows = item.Result.ToList<KeyValuePair<string, DynamoDBEntry>>();

                foreach (KeyValuePair<string, DynamoDBEntry> row in rows)
                {
                    if (row.Key == Constants.ConfigurationValue)
                        businessLogicJson = JObject.Parse(row.Value.AsDocument().ToJson());
                }
            }

            if(!workflowCompleted)
                PublishToQueue(queueID, businessLogicJson, workflowTypeName, workflowID, messageBodyJson, customPropertiesJson, stepIdentifier, stepSucceeded, workflowCompleted, retries, active, runStepIdentifier, splitID);
            PublishEvent(queueID, businessLogicJson, workflowTypeName, workflowID, messageBodyJson, customPropertiesJson, stepIdentifier, stepSucceeded, workflowCompleted, retries, active, runStepIdentifier, splitID);
        }

        public void SetWorkflowActive(JObject messageBodyJson, string customPropertyName, string customPropertyValue)
        {
           
            string tableName = Constants.QueueTableName;
            Table table = Table.LoadTable(_client, tableName);

           
            Search search = table.Scan(new Expression());

            string queueID = null;
            JObject businessLogicJson = null;
            string workflowTypeName = null;
            Guid workflowID = Guid.Empty;
            JObject customPropertiesJson = null;

            string stepIdentifier = null;
            bool stepSucceeded = true;
            bool workflowCompleted = false;
            int retries = 0;
            bool active = true;
            string runStepIdentifier = null;
            string splitID = null;

            do
            {
                Task<List<Document>> documentListTask = search.GetNextSetAsync();
                documentListTask.Wait();

                List<Document> documentList = documentListTask.Result;

                foreach (Document document in documentList)
                {
                    string customPropertieString = document[Constants.QueueCustomProperties].AsDocument().ToJson();

                    if (customPropertieString.Contains(customPropertyName) && customPropertieString.Contains(customPropertyValue))
                    {

                        queueID = document[Constants.QueueId].AsString();
                        businessLogicJson = JObject.Parse(document[Constants.BusinessLogic].AsDocument().ToJson());
                        workflowTypeName = document[Constants.WorkflowType].AsString();
                        workflowID = document[Constants.HistoryWorkflowId].AsGuid();
                        customPropertiesJson = JObject.Parse(customPropertieString);

                        if(messageBodyJson == null)
                        {
                            messageBodyJson = JObject.Parse(document[Constants.QueueMessageBody].AsDocument().ToJson());
                        }

                        if(document.ContainsKey(Constants.QueueStepIdentifier))
                            stepIdentifier = document[Constants.QueueStepIdentifier].AsString();

                        if (document.ContainsKey(Constants.QueueRunStepIdentifier))
                            runStepIdentifier = document[Constants.QueueRunStepIdentifier].AsString();
                        splitID = document[Constants.QueueSlitID].AsString();


                        PublishToQueue(queueID, businessLogicJson, workflowTypeName, workflowID, messageBodyJson, customPropertiesJson, stepIdentifier, stepSucceeded, workflowCompleted, retries, active, runStepIdentifier, splitID);
                        DeleteFromQueue(queueID,false);
                        PublishEvent(queueID, businessLogicJson, workflowTypeName, workflowID, messageBodyJson, customPropertiesJson, stepIdentifier, stepSucceeded, workflowCompleted, retries, active, runStepIdentifier, splitID);
                    }
                }
            } while (!search.IsDone);


           

        }

        private async void DeleteFromQueue(string queueID,bool active)
        {
            string tableName = Constants.QueueTableName;
            Table table = Table.LoadTable(_client, tableName);

            await table.DeleteItemAsync(queueID, active.ToString());
        }

        private async void PublishToQueue(string queueID,JObject businessLogicJson,string workflowTypeName, Guid workflowID, JObject messageBodyJson, JObject customPropertiesJson, string stepIdentifier, bool stepSucceeded, bool workflowCompleted, int retries, bool active, string runStepIdentifier, string splitID)
        {
            string tableName = Constants.QueueTableName;
            Table table = Table.LoadTable(_client, tableName);
            string jsonText = "{\"" + Constants.QueueId + "\":\"" + queueID + "\",\""
                + Constants.QueueActive + "\":\"" + active.ToString() + "\",\""
                + Constants.HistoryWorkflowId + "\":\"" + workflowID.ToString() + "\",\""
                + Constants.WorkflowType + "\":\"" + workflowTypeName + "\",\""
                + Constants.BusinessLogic + "\":" + JsonConvert.SerializeObject(businessLogicJson) + ",\""
                + Constants.QueueMessageBody + "\":" + JsonConvert.SerializeObject(messageBodyJson) + ",\""
                + Constants.QueueCustomProperties + "\":" + JsonConvert.SerializeObject(customPropertiesJson) + ",\""
                + Constants.QueueRetries + "\":" + retries.ToString() + ",\""
                + ((stepIdentifier == null) ? string.Empty: Constants.QueueStepIdentifier + "\":\"" +  stepIdentifier.ToString() + "\",\"")
                + ((runStepIdentifier == null)? string.Empty : Constants.QueueRunStepIdentifier + "\":\"" + runStepIdentifier.ToString() + "\",\"")
                + Constants.QueueSlitID + "\":\"" + splitID.ToString() + "\",\""
                + Constants.Timestamp + "\":\"" + DateTime.UtcNow.ToString() + "\",\""
                + Constants.QueueStepSucceeded + "\":\"" + stepSucceeded.ToString() + "\",\""
                + Constants.QueueWorkflowCompleted + "\":\"" + workflowCompleted.ToString() + "\"}";
            Document item = Document.FromJson(jsonText);
            await table.PutItemAsync(item);
        }

        private async void PublishEvent(string queueID, JObject businessLogicJson, string workflowTypeName, Guid workflowID, JObject messageBodyJson, JObject customPropertiesJson, string stepIdentifier, bool stepSucceeded, bool workflowCompleted, int retries, bool active, string runStepIdentifier, string splitID)
        {
            string tableName = Constants.HistoryTableName;
            Table table = Table.LoadTable(_client, tableName);

            History history = new History()
            {
                Body = messageBodyJson,
                CustomPropertiesJson = customPropertiesJson,
                SplitID = splitID,
                Timestamp = DateTime.UtcNow
            };

            string workflowState = string.IsNullOrEmpty(stepIdentifier) ? "Started" : "InProcess";
            workflowState = workflowCompleted ? "Completed" : workflowState;

            string runStepString = string.IsNullOrEmpty(runStepIdentifier) ? string.Empty : "\"RunStep\":\"" + runStepIdentifier + "\"";

            string eventString = "{\"WorkflowState\":\"" + workflowState + "\"," + runStepString + "}";

            history.EventJson = JObject.Parse(eventString);

            string UniqueId = string.IsNullOrEmpty(stepIdentifier) ? "Start_" : stepIdentifier + "_";

            string jsonText = "{\"" + Constants.HistoryWorkflowId + "\":\"" + workflowID.ToString() + "\",\""
                + Constants.WorkflowType + "\":\"" + workflowTypeName + "\",\""
                + UniqueId + Guid.NewGuid().ToString() + "\":" + JsonConvert.SerializeObject(history) + "}";

            
            Document item = Document.FromJson(jsonText);
            await table.UpdateItemAsync(item);
        }
    }
}
