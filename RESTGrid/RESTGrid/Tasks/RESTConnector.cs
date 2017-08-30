using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RESTGrid.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RESTGrid.Tasks
{
    internal class RESTConnector
    {
        public static List<QueueMetadata> RunTask(QueueMetadata metadata, JObject taskProperties, string identifier)
        {
            List<QueueMetadata> result = null;
            try
            {
                if (taskProperties == null)
                    throw new Exception("TaskProperties for Transformer cannot be NULL");

                RESTConfig taskConfig = taskProperties.ToObject<RESTConfig>();
                
                QueueMetadata newMetadata = new QueueMetadata()
                {
                    CustomPropertiesJson = metadata.CustomPropertiesJson,
                    ID = Guid.NewGuid(),
                    MessageBodyJson = metadata.MessageBodyJson,
                    Retries = metadata.Retries,
                    SplitID = metadata.SplitID,
                    StepIdentifier = identifier,
                    Success = true,
                    Timestamp = DateTime.UtcNow
                };

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(taskConfig.Url);
                
                if (taskConfig.Headers != null)
                {
                    foreach(KeyValuePair<string,string> header in taskConfig.Headers)
                        client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpContent content = new StringContent(JsonConvert.SerializeObject( taskConfig.Body), Encoding.UTF8, "application/json");

                Task<HttpResponseMessage> response = null;

                if (taskConfig.Method.ToLower() == "get")
                {
                    response = client.GetAsync(taskConfig.QueryString);
                }
                else if (taskConfig.Method.ToLower() == "put")
                {
                    response = client.PutAsync(taskConfig.QueryString, content);
                }
                else if (taskConfig.Method.ToLower() == "post")
                {
                    response = client.PostAsync(taskConfig.QueryString, content);
                }
                else if (taskConfig.Method.ToLower() == "delete")
                {
                    response = client.DeleteAsync(taskConfig.QueryString);
                }
                else
                    throw new Exception("Method : " + taskConfig.Method + " not implemented in RESTConnector.");

                response.Wait();

                if (response.Result.IsSuccessStatusCode)
                {
                    Task<string> resultString = response.Result.Content.ReadAsStringAsync();
                    resultString.Wait();

                    JObject jObj = metadata.MessageBodyJson;

                    try
                    {
                        JObject token = JObject.Parse(resultString.Result);

                        JProperty property = jObj.Property(identifier);
                        if(property == null)
                            jObj.Add(new JProperty(identifier, token));
                    }
                    catch (JsonReaderException ex)
                    {
                        JProperty property = jObj.Property(identifier);
                        if (property == null)
                            jObj.Add(new JProperty(identifier, resultString.Result));
                    }
 

                    newMetadata.MessageBodyJson = jObj;
                }
                else
                    throw new Exception("Response from RESTService : " + response.Result.StatusCode.ToString() + " : " + response.Result.ReasonPhrase);

                result = new List<QueueMetadata>();
                result.Add(newMetadata);
            }
            catch (Exception ex)
            {
                result = new List<QueueMetadata>();
                QueueMetadata errorMetaData = new QueueMetadata()
                {
                    CustomPropertiesJson = metadata.CustomPropertiesJson,
                    ID = Guid.NewGuid(),
                    MessageBodyJson = metadata.MessageBodyJson,
                    Retries = metadata.Retries,
                    SplitID = metadata.SplitID,
                    StepIdentifier = identifier,
                    Success = false,
                    Timestamp = DateTime.UtcNow

                };

                Dictionary<string, string> customPropertiesSerialized = errorMetaData.CustomPropertiesJson.ToObject<Dictionary<string, string>>();
                if (customPropertiesSerialized.ContainsKey("Error"))
                    customPropertiesSerialized.Remove("Error");

                customPropertiesSerialized.Add("Error", ex.Message);

                errorMetaData.CustomPropertiesJson = JObject.FromObject(customPropertiesSerialized);

                result.Add(errorMetaData);
            }


            return result;
        }
    }

    internal class RESTConfig
    {
        public string Url { get; set; }

        public string Method { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public JToken Body { get; set; }

        public string QueryString { get; set; }
    }
}
