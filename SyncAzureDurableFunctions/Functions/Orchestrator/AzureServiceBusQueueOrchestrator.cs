using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using MRI.PandA.Data;
using MRI.PandA.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyncAzureDurableFunctions.Functions.Orchestrator
{
    public static class AzureServiceBusQueueOrchestrator
    {
        [FunctionName("AzureServiceBusQueueOrchestrator")]
        public static async Task<bool> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            try
            {
                string inputMessage = context.GetInput<string>();
                List<PropertyBase> propertiesToSync = ApiCore.DeSerializeMessage(inputMessage);                

                var tasks = new Task<bool>[propertiesToSync.Count];

                for (int i = 0; i < propertiesToSync.Count; i++)
                {
                    log.LogInformation($"A new properties added named {propertiesToSync[i].Name} was uploaded to Azure Storage");
                    //Chain #1 
                    tasks[i] = context.CallActivityAsync<bool>("ImportAndProcessProperty", propertiesToSync[i]);

                }

                //Chain #2

                //Chain #3

                await Task.WhenAll(tasks);
                var allSuccessful = tasks.All(t => t.Result);

                log.LogInformation($"Done with the orchestration with Durable Context Id:  {context.InstanceId}");
                return allSuccessful;
            }
            catch (Exception ex)
            {
                //TODO Handle possible errors and do a retry if needed or retry a function
                log.LogError($"Something went wrong " + ex.Message);
                throw;
            }
        }
    }
}
