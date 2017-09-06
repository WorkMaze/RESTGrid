using System;
using System.Collections.Generic;
using System.Text;

namespace RESTGrid.Providers.DynamoDB
{
    public static class Constants
    {
        public const string ConfigurationTableName = "RESTGrid_Configuration";
        public const string HistoryTableName = "RESTGrid_History";
        public const string QueueTableName = "RESTGrid_Queue";

        public const string ConfigurationId = "Id";
        public const string ConfigurationType = "Type";
        public const string ConfigurationValue= "Value";

        public const string TransformerType = "Transformer";
        public const string WorkflowType = "WorkflowType";
        public const string BusinessLogic = "BusinessLogic";

        public const string HistoryWorkflowId = "WorkflowID";

        public const string QueueId = "QueueID";
        public const string QueueActive = "Active";

        public const string QueueMessageBody = "MessageBody";
        public const string QueueCustomProperties = "CustomProperties";

        public const string QueueStepIdentifier = "StepIdentifier";
        public const string QueueStepSucceeded = "StepSucceeded";
        public const string QueueWorkflowCompleted = "WorkflowCompleted ";
        public const string QueueRetries = "Retries";
        public const string QueueRunStepIdentifier = "RunStepIdentifier";
        public const string QueueSlitID = "SplitD";
        public const string Timestamp = "Timestamp";
    }
}
