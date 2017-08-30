using RESTGrid.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using RESTGrid.Models;
using Newtonsoft.Json;
using System.Data;
using MySql.Data.MySqlClient;

namespace RESTGrid.Providers.MySql
{
    public class MySqlAdministration : IAdministration
    {
        private string _connectionString;

        public MySqlAdministration(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateTransformer(JObject transformerJson)
        {
            MySqlConnection connection = null;

            try
            {
                connection = new MySqlConnection(_connectionString);

                MySqlCommand command = new MySqlCommand("Administration_CreateTransformer", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@TransformerJson", JsonConvert.SerializeObject(transformerJson));
                command.Parameters["@TransformerJson"].Direction = ParameterDirection.Input;               

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

        public void CreateWorkflowType(string workflowTypeName, JObject businessLogicJson)
        {
            MySqlConnection connection = null;

            try
            {
                connection = new MySqlConnection(_connectionString);

                MySqlCommand command = new MySqlCommand("Administration_CreateWorkflowType", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@WorkflowTypeName", workflowTypeName);
                command.Parameters["@WorkflowTypeName"].Direction = ParameterDirection.Input;

                command.Parameters.AddWithValue("@BusinessLogic", JsonConvert.SerializeObject(businessLogicJson));
                command.Parameters["@BusinessLogic"].Direction = ParameterDirection.Input;

                
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

        public WorkflowHistory GetHistory(string workflowID)
        {
            WorkflowHistory instance = null;

            MySqlConnection connection = null;

            try
            {
                connection = new MySqlConnection(_connectionString);

                MySqlCommand command = new MySqlCommand("Administration_GetHistory", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@WorkflowID", workflowID);
                command.Parameters["@WorkflowID"].Direction = ParameterDirection.Input;

                connection.Open();

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (instance == null)
                    {
                        instance = new WorkflowHistory();

                        instance.WorkflowID = Guid.Parse(workflowID);
                        instance.SplitID = reader.GetString("SplitID");

                        instance.Type = new WorkfowType()
                        {
                            ID = reader.GetInt32("idWorkflowType"),
                            Name = reader.GetString("Name"),
                            BusinessLogicJson = JObject.Parse(reader.GetString("BusinessLogic"))
                        };

                        instance.HisoryObjects = new List<History>();
                    }

                    History history = new History()
                    {
                        Body = !reader.IsDBNull(0) ? JObject.Parse(reader.GetString(0)):null,
                        CustomPropertiesJson = !reader.IsDBNull(1) ? JObject.Parse(reader.GetString(1)) : null,
                        EventJson = !reader.IsDBNull(2) ? JObject.Parse(reader.GetString(2)) : null,
                        Timestamp = reader.GetDateTime("Timestamp")
                    };

                    instance.HisoryObjects.Add(history);

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
    }
}
