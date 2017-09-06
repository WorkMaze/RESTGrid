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
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;

namespace RESTGrid.Providers.DynamoDB
{
    public class DynamoDBAdministration : IAdministration
    {
        private DynamoDBConfiguration _configuration;
        private AmazonDynamoDBClient _client;

        public DynamoDBAdministration(DynamoDBConfiguration configuration)
        {
            _configuration = configuration;
            RegionEndpoint regionEndpoint = RegionEndpoint.GetBySystemName(configuration.Region);
            AWSCredentials awsCredentails = new BasicAWSCredentials(configuration.AccessKey, configuration.SecretAccessKey);
            _client = new AmazonDynamoDBClient(awsCredentails, regionEndpoint);
        }

        public void CreateTransformer(JObject transformerJson)
        {
            Task<string> transformerIDTask = GetNextAvailableID();
            transformerIDTask.Wait();
            string transformerID = transformerIDTask.Result;

            CreateConfiguration(transformerID, Constants.TransformerType, transformerJson);
        }

        public void CreateWorkflowType(string workflowTypeName, JObject businessLogicJson)
        {
            CreateConfiguration(workflowTypeName, Constants.WorkflowType, businessLogicJson);
        }

        public WorkflowHistory GetHistory(string workflowID)
        {
            WorkflowHistory instance = null;
            Table table = Table.LoadTable(_client, Constants.HistoryTableName);

            var item = table.GetItemAsync(workflowID);
            item.Wait();

            if (item.Result != null)
            {
                instance = new WorkflowHistory();
                instance.Type = new WorkfowType();

                List<KeyValuePair<string, DynamoDBEntry>> rows = item.Result.ToList<KeyValuePair<string, DynamoDBEntry>>();

                foreach (KeyValuePair<string, DynamoDBEntry> row in rows)
                {
                    if (row.Key == Constants.WorkflowType)
                        instance.Type.Name = row.Value.AsString();
                    else if (row.Key == Constants.HistoryWorkflowId)
                        instance.WorkflowID = row.Value.AsGuid();
                    else if (row.Key == Constants.BusinessLogic)
                        instance.Type.BusinessLogicJson =  JObject.Parse(row.Value.AsString());
                    else
                    {
                        if (instance.HisoryObjects == null)
                            instance.HisoryObjects = new List<History>();

                        JObject historyObject = JObject.Parse(row.Value.AsString());

                        History history = historyObject.ToObject<History>();

                        instance.HisoryObjects.Add(history);
                        
                    }
                }
            }

            return instance;
        }

        private async void CreateConfiguration(string id,string type, JObject configuration)
        {
            string tableName = Constants.ConfigurationTableName;
            Table table = Table.LoadTable(_client, tableName);
            string jsonText = "{\""+Constants.ConfigurationId+"\""+id+"\",\""+ Constants.ConfigurationType + "\":\""+type+"\",\""+ Constants.ConfigurationValue + "\":" + JsonConvert.SerializeObject(configuration) + "}";
            Document item = Document.FromJson(jsonText);
            await table.PutItemAsync(item);
        }

        private async Task<string> GetNextAvailableID()
        {
            int availableID = 0;
            string tableName = Constants.ConfigurationTableName;
            Table table = Table.LoadTable(_client, tableName);

            ScanFilter scanFilter = new ScanFilter();
            scanFilter.AddCondition(Constants.ConfigurationType, ScanOperator.Equal, Constants.TransformerType);
            
            Search search = table.Scan(scanFilter);
            List<Document> documentList = new List<Document>();


            do
            {
                documentList = await search.GetNextSetAsync();

                foreach (Document document in documentList)
                {
                    int currentID = Int32.Parse(document[Constants.ConfigurationId]);

                    if (currentID > availableID)
                        availableID = currentID;
                }
            } while (!search.IsDone);


            return (availableID + 1).ToString();
        }
    }
}
