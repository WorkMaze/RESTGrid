using RESTGrid.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using RESTGrid.Models;
using MySql.Data.MySqlClient;
using System.Data;
using Newtonsoft.Json;

namespace RESTGrid.Providers.MySql
{
    public class MySqlOrchestration : IOrchestration
    {
        private string _connectionString;

        public MySqlOrchestration(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Queue> Enqueue()
        {
            List<Queue> instance = null;

            MySqlConnection connection = null;

            try
            {
                connection = new MySqlConnection(_connectionString);

                MySqlCommand command = new MySqlCommand("Orchestration_Enqueue", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                MySqlDataReader reader = command.ExecuteReader();

                Queue queue = null;
                while (reader.Read())
                {
                    if (instance == null)
                        instance = new List<Queue>();
                    
                    string workflowID = reader.GetString("idWorkflow");

                    if (queue == null || queue.WorkflowID == null || queue.WorkflowID.ToString().ToLower() != workflowID.ToLower())
                    {
                        if (queue != null)
                            instance.Add(queue);

                        queue = new Queue();
                        queue.WorkflowID = Guid.Parse(workflowID);
                        queue.Type = new WorkfowType()
                        {
                            ID = reader.GetInt32("idWorkflowType"),
                            Name = reader.GetString("WorkflowTypeName"),
                            BusinessLogicJson = JObject.Parse(reader.GetString("BusinessLogic"))
                        };
                        queue.Metadata = new List<QueueMetadata>();
                    }

                    QueueMetadata queueMetadata = new QueueMetadata()
                    {
                        CustomPropertiesJson = !reader.IsDBNull(3) ? JObject.Parse(reader.GetString(3)) : null,
                        ID = reader.GetGuid("QueueID"),
                        MessageBodyJson = !reader.IsDBNull(7) ? JObject.Parse(reader.GetString(7)) : null,
                        Retries= reader.GetInt32("Retries"),
                        SplitID = reader.GetString("SplitID"),
                        StepIdentifier = !reader.IsDBNull(5) ? reader.GetString(5) : null,
                        Success = reader.GetBoolean("Success"),
                        Timestamp = reader.GetDateTime("Timestamp")
                    };

                    queue.Metadata.Add(queueMetadata);
                   
                }
                if(instance != null)
                    instance.Add(queue);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if(connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return instance;
        }

        public JObject GetTransformer(int transformerID)
        {
            JObject instance = null;

            MySqlConnection connection = null;

            try
            {
                connection = new MySqlConnection(_connectionString);

               
                MySqlCommand command = new MySqlCommand("SELECT TransformerJson FROM transformer WHERE idTransformer = " + transformerID.ToString(), connection);
                command.CommandType = CommandType.Text;

                connection.Open();

                MySqlDataReader reader = command.ExecuteReader();                

                while (reader.Read())
                {
                    instance = JObject.Parse(reader.GetString("TransformerJson"));

                    break;
                }

                reader.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }

            return instance;
        }

        public void PublishWorkflowStep(string workflowTypeName, Guid workflowID, JObject messageBodyJson, JObject customPropertiesJson, string stepIdentifier, bool stepSucceeded, bool workflowCompleted, int retries, bool active, string runStepIdentifier, string splitID)
        {
            MySqlConnection connection = null;          

            try
            {
                connection = new MySqlConnection(_connectionString);

                MySqlCommand command = new MySqlCommand("Orchestration_PublishWorkflowStep", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@WorkflowTypeName", workflowTypeName);
                command.Parameters["@WorkflowTypeName"].Direction = ParameterDirection.Input;

                command.Parameters.AddWithValue("@WorkflowID", workflowID);
                command.Parameters["@WorkflowID"].Direction = ParameterDirection.Input;

                command.Parameters.AddWithValue("@Success", stepSucceeded);
                command.Parameters["@Success"].Direction = ParameterDirection.Input;

                command.Parameters.AddWithValue("@WorkflowCompleted", workflowCompleted);
                command.Parameters["@WorkflowCompleted"].Direction = ParameterDirection.Input;

                command.Parameters.AddWithValue("@Retries", retries);
                command.Parameters["@Retries"].Direction = ParameterDirection.Input;

                command.Parameters.AddWithValue("@Active", active);
                command.Parameters["@Active"].Direction = ParameterDirection.Input;

                command.Parameters.Add("@MessageBody", MySqlDbType.JSON);
                command.Parameters["@MessageBody"].Direction = ParameterDirection.Input;
                command.Parameters["@MessageBody"].IsNullable = true;
                if (messageBodyJson != null)
                    command.Parameters["@MessageBody"].Value = JsonConvert.SerializeObject( messageBodyJson);

                command.Parameters.Add("@CustomProperties", MySqlDbType.JSON);
                command.Parameters["@CustomProperties"].Direction = ParameterDirection.Input;
                command.Parameters["@CustomProperties"].IsNullable = true;
                if (customPropertiesJson != null)
                    command.Parameters["@CustomProperties"].Value = JsonConvert.SerializeObject(customPropertiesJson);

                command.Parameters.Add("@StepIdentifier", MySqlDbType.String);
                command.Parameters["@StepIdentifier"].Direction = ParameterDirection.Input;
                command.Parameters["@StepIdentifier"].IsNullable = true;
                if (stepIdentifier != null)
                    command.Parameters["@StepIdentifier"].Value = stepIdentifier;

                command.Parameters.Add("@RunStepIdentifier", MySqlDbType.String);
                command.Parameters["@RunStepIdentifier"].Direction = ParameterDirection.Input;
                command.Parameters["@RunStepIdentifier"].IsNullable = true;
                if (runStepIdentifier != null)
                    command.Parameters["@RunStepIdentifier"].Value = runStepIdentifier;

                command.Parameters.Add("@In_SplitID", MySqlDbType.String);
                command.Parameters["@In_SplitID"].Direction = ParameterDirection.Input;
                command.Parameters["@In_SplitID"].IsNullable = true;
                if (splitID != null)
                    command.Parameters["@In_SplitID"].Value = splitID;

                connection.Open();

                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        public void SetWorkflowActive(JObject messageBodyJson, string customPropertyName, string customPropertyValue)
        {
            MySqlConnection connection = null;

            try
            {
                connection = new MySqlConnection(_connectionString);

                MySqlCommand command = new MySqlCommand("Orchestration_SetActive", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@In_CustomPropertyName", customPropertyName);
                command.Parameters["@In_CustomPropertyName"].Direction = ParameterDirection.Input;

                command.Parameters.AddWithValue("@In_CustomPropertyValue", customPropertyValue);
                command.Parameters["@In_CustomPropertyValue"].Direction = ParameterDirection.Input;


                command.Parameters.Add("@MessageBody", MySqlDbType.JSON);
                command.Parameters["@MessageBody"].Direction = ParameterDirection.Input;
                command.Parameters["@MessageBody"].IsNullable = true;
                if (messageBodyJson != null)
                    command.Parameters["@MessageBody"].Value = JsonConvert.SerializeObject(messageBodyJson);

                

                connection.Open();

                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
