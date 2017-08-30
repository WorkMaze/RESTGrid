using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RESTGrid.Interfaces;
using RESTGrid.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RESTGrid.Tasks
{
    internal static class Transformer
    {
        public static List<QueueMetadata> RunTask(QueueMetadata metadata, JObject taskProperties, string identifier, IOrchestration orchestration)
        {
            List<QueueMetadata> result = null;
            try
            {
                if (taskProperties== null)
                    throw new Exception("TaskProperties for Transformer cannot be NULL");

                TransformerConfiguration taskConfig = taskProperties.ToObject<TransformerConfiguration>();
                JObject transformerJson = orchestration.GetTransformer(taskConfig.TransformerID);


                QueueMetadata transformedMetadata = new QueueMetadata()
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

                transformedMetadata.MessageBodyJson = JUST.JsonTransformer.Transform(transformerJson, metadata.MessageBodyJson);
                result = new List<QueueMetadata>();
                result.Add(transformedMetadata);
                               
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

    internal class TransformerConfiguration
    {
        public int TransformerID { get; set; }
    }
}
