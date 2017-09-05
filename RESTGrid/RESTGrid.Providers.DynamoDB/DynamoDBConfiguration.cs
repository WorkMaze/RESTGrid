using System;
using System.Collections.Generic;
using System.Text;

namespace RESTGrid.Providers.DynamoDB
{
    public class DynamoDBConfiguration
    {
        public string Region { get; set; }
        public string AccessKey { get; set; }
        public string SecretAccessKey { get; set; }
    }
}
