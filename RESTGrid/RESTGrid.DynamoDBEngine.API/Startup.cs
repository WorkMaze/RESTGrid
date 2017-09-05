using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RESTGrid.Providers.DynamoDB;
using Newtonsoft.Json;

namespace RESTGrid.DynamoDBEngine.API
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables("RESTGrid_");
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            string config = Configuration.GetValue<string>("DynamoDBConfiguration");

            DynamoDBConfiguration dynamoDBConfig = JsonConvert.DeserializeObject<DynamoDBConfiguration>(config);

            services.Configure<DynamoDBConfiguration>(x => x.AccessKey = dynamoDBConfig.AccessKey);
            services.Configure<DynamoDBConfiguration>(x => x.Region = dynamoDBConfig.Region);
            services.Configure<DynamoDBConfiguration>(x => x.SecretAccessKey = dynamoDBConfig.SecretAccessKey);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
