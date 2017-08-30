using Newtonsoft.Json.Linq;
using RESTGrid.Interfaces;
using RESTGrid.Models;
using RESTGrid.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace RESTGrid
{
    public class OrchestrationEngine
    {
        private IOrchestration _orchestration;

        public OrchestrationEngine(IOrchestration orchestration)
        {
            _orchestration = orchestration;
        }

        public void Run()
        {
            List<Models.Queue> enqueuedList = _orchestration.Enqueue();

            if (enqueuedList == null)
                return;

            foreach (Models.Queue queue in enqueuedList)
            {
                BusinessLogic businessLogic = queue.Type.BusinessLogicJson.ToObject<BusinessLogic>();

                if (businessLogic == null)
                    continue;

                bool workflowCompleted = true;

                foreach (QueueMetadata metadata in queue.Metadata)
                {
                    //Case when the workflow has been started and no task has been run yet.
                    if (metadata.StepIdentifier == null)
                    {
                        Task nextTask = businessLogic.Start;
                        if (!nextTask.SatisfiesRunCondition(metadata))
                            continue;

                        workflowCompleted = false;                                              

                        List<QueueMetadata> resultList = RunTask(metadata, nextTask);

                        foreach(QueueMetadata result in resultList)
                            PublishToQueue(queue, metadata, result, businessLogic.Start.Identifier, workflowCompleted, nextTask.Type != TaskType.Async);
                    }
                    //A task has been run in the workflow previously.
                    else
                    {
                        Task currentTask = businessLogic.FindTask(metadata.StepIdentifier);
                        
                        List<string> nextTaskList = currentTask.Next;

                        if (nextTaskList != null)
                        {                           

                            if ((!metadata.Success) && ((!currentTask.TaskRetries.HasValue) || (metadata.Retries >= currentTask.TaskRetries.Value)))
                            {
                                workflowCompleted = false;
                                PublishToQueue(queue, metadata, metadata, "None", workflowCompleted, false);
                            }
                            else
                            {
                                foreach (string nextTaskIdentifier in nextTaskList)
                                {
                                    Task nextTask = businessLogic.FindTask(nextTaskIdentifier);

                                    if (!nextTask.SatisfiesRunCondition(metadata))
                                        continue;

                                    workflowCompleted = false;

                                    List<QueueMetadata> resultList = RunTask(metadata, nextTask);

                                    foreach (QueueMetadata result in resultList)
                                        PublishToQueue(queue, metadata, result, nextTaskIdentifier, workflowCompleted, nextTask.Type != TaskType.Async);

                                }
                            }
                        }
                       
                    }
                }

                if (workflowCompleted)
                {
                    PublishToQueue(queue, null, null, "None", workflowCompleted, true);
                }
            }
        }

        private List<QueueMetadata> RunTask(QueueMetadata metadata,Task task)
        {
            List<QueueMetadata> result = null;

            JObject metadataJson = JObject.FromObject(metadata);
            JObject taskpropertiesJson = JUST.JsonTransformer.Transform((JObject)task.TaskProperties.DeepClone(), metadataJson);

            JObject taskProperties = taskpropertiesJson;
            string identifier = task.Identifier;

            if (task.Type == TaskType.Splitter)
            {
                result = Splitter.RunTask(metadata, taskProperties,identifier);
            }
            else if (task.Type == TaskType.Transformer)
            {
                result = Transformer.RunTask(metadata, taskProperties, identifier, _orchestration);
            }
            else
            {
                result = RESTConnector.RunTask(metadata, taskProperties, identifier);
            }

            return result;
        }

        private void PublishToQueue(Models.Queue currentQueue, QueueMetadata currentMetadata,QueueMetadata result, string runTask, bool workflowCompleted,
            bool active)
        {
            if (result == null)
            {
                _orchestration.PublishWorkflowStep(currentQueue.Type.Name, currentQueue.WorkflowID,null, null,
                        runTask, true, workflowCompleted, 0, active, runTask,null);
            }
            else
            {
                if (result.Success)
                    _orchestration.PublishWorkflowStep(currentQueue.Type.Name, currentQueue.WorkflowID, result.MessageBodyJson, result.CustomPropertiesJson,
                        runTask, true, workflowCompleted, 0, active, runTask, result.SplitID);
                else
                    _orchestration.PublishWorkflowStep(currentQueue.Type.Name, currentQueue.WorkflowID, currentMetadata.MessageBodyJson, result.CustomPropertiesJson,
                        currentMetadata.StepIdentifier, false, workflowCompleted, currentMetadata.Retries + 1, active, runTask, currentMetadata.SplitID);
            }
        }
    }
}
