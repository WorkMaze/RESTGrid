using Newtonsoft.Json.Linq;
using RESTGrid.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RESTGrid.Tasks
{
    internal static class Splitter
    {
        public static List<QueueMetadata> RunTask(QueueMetadata metadata, JObject taskProperties, string identifier)
        {
            List<QueueMetadata> result = null;
            try
            {
                if (taskProperties == null)
                    throw new Exception("TaskProperties for Splitter cannot be NULL");

                SplitterConfiguration taskConfig = taskProperties.ToObject<SplitterConfiguration>();

                IEnumerable<JObject> messages = JUST.JsonTransformer.SplitJson(metadata.MessageBodyJson, taskConfig.ArrayPath);

                result = new List<QueueMetadata>();

                int i = 0;
                foreach (JObject message in messages)
                {
                    
                    QueueMetadata splitMetadata = new QueueMetadata()
                    {
                        CustomPropertiesJson = metadata.CustomPropertiesJson,
                        ID = Guid.NewGuid(),
                        MessageBodyJson = message,
                        Retries = metadata.Retries,
                        SplitID = metadata.SplitID + "." + i.ToString(),
                        StepIdentifier = identifier,
                        Success = true,
                        Timestamp = DateTime.UtcNow

                    };
                    i++;
                    result.Add(splitMetadata);
                }

                

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

    internal class SplitterConfiguration
    {
        public string ArrayPath { get; set; }
    }
}
