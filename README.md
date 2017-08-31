# RESTGrid

A simple Workflow\ETL system in .NET Core which uses REST services to interact with the outside world.

+ The system has the ability to define workflows using a JSON format. 
+ The workflows can be synchronous or long running & asynchronous.
+ Each step in the workflow is a call to a REST service.
+ Steps can be Synchronous where in the next step is called after the previous step is executed.
+ Steps can also be asynchronous where in after the execution of a step, the workflow goes into a waiting stage and waits for an external call.
+ A workflow can be started, re-started using a REST API.
+ Ability to define JSON transformations.
+ Interface based system and hence can be hooked to different data sources.

## System Components

### Orchestration engine 
This is the core of the system which enqueues tasks, reads the business logic of the workflow and runs tasks.

### REST API 
This is used by external systems to start\re-start workflows as well as users to administer\set-up workflows.

### Backend 
This is the store where the configuration as well as historical data is stored.
RESTGrid is the main project which defines the interfaces, objects and the orchestration logic for the system. Since this is an interface based projects, we can hook up any backend as long as we implement the interfaces defined.

RESTGrid is available as a Nuget package:-
 *https://www.nuget.org/packages/RESTGrid/*
## MySql provider for RESTGrid

At the moment we have the MySql provider as part of the project.

The following docker containers are available:-
 #### Orchestration engine - https://hub.docker.com/r/workmaze/restgrid.mysqlengine.workflow/
 #### REST API - https://hub.docker.com/r/workmaze/restgrid.mysqlengine.api/

## Business Logic
Business Logic is a JSON which defines the workflow. Below is an example of the business logic:-

```{
 "Start": {
    "Identifier": "CreateUser",
    "Next": [
     "CreateRole"
    ],
    "RunCondition": null,
    "Type": "Transformer",
    "TaskProperties": {
     "TransformerID": "3"
    }
 },
 "Tasks": [
    {
     "Identifier": "CreateRole",
     "Next": [
       "AddApplication",
       "Notify"
     ],
     "Type": "Splitter",
     "TaskProperties": {
       "ArrayPath": "$.Organization.Employee"
     }
    },
    {
     "Identifier": "AddApplication",
     "Type": "Sync",
     "Next": [
       "Approve"
     ],
     "RunCondition": {
       "Evaluated": "CreditCard",       "Evaluator": "#valueof($.MessageBodyJson.Organization.Employee.PaymentMode)",
       "Operator": "stringequals"
     },
     "TaskProperties": {
       "Url": "http://localhost:5001/",
       "Method": "POST",
       "Headers": null,
       "Body": "#valueof($.MessageBodyJson.Organization.Employee.Details)",
       "QueryString": "api/table/user"
     },
     "TaskRetries": 0
    },
    {
     "Identifier": "Notify",
     "Next": [
       "Approve"
     ],
     "Type": "Async",
     "RunCondition": {
       "Evaluated": "Cash",
       "Evaluator": "#valueof($.MessageBodyJson.Organization.Employee.PaymentMode)",
       "Operator": "stringequals"
     },
     "TaskProperties": {
       "Url": "http://localhost:5001/",
       "Method": "POST",
       "Headers": null,
       "Body": "#valueof($.MessageBodyJson.Organization.Employee.Details)",
       "QueryString": "api/table/user"
     },
     "TaskRetries": 0
    },
    {
     "Identifier": "Approve",
     "Type": "Sync",
     "TaskProperties": {
       "Url": "http://localhost:5001/",
       "Method": "POST",
       "Headers": null,
       "Body": {
         "Message": "Your payment has been approved"
       },
       "QueryString": "api/table/user"
     },
     "TaskRetries": 0
    }
 ]
}
```

