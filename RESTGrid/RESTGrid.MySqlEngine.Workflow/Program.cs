using System;
using RESTGrid.Providers.MySql;

namespace RESTGrid.MySqlEngine.Workflow
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string connectionString = Environment.GetEnvironmentVariable("ConnectionString");

                if (string.IsNullOrEmpty(connectionString))
                    throw new Exception("ConnectionString not specified.");

                MySqlOrchestration orchestration = new MySqlOrchestration(connectionString);
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
