using Newtonsoft.Json;
using RESTGrid.Providers.DynamoDB;
using System;

namespace RESTGrid.DynamoDBEngine.Workflow
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string config = Environment.GetEnvironmentVariable("DynamoDBConfiguration");

                if (string.IsNullOrEmpty(config))
                    throw new Exception("DynamoDBConfiguration not specified.");

                DynamoDBConfiguration dynamoDBConfig = JsonConvert.DeserializeObject<DynamoDBConfiguration>(config);
                DynamoDBOchestration orchestration = new DynamoDBOchestration(dynamoDBConfig);
                OrchestrationEngine engine = new OrchestrationEngine(orchestration);
                Console.WriteLine("Running orchestration engine...");
                while (true)
                {
                    engine.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
