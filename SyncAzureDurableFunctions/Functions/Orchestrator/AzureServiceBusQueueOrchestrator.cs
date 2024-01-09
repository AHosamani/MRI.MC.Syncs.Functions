using Azure.Messaging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using MRI.PandA.Data;
using MRI.PandA.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using static SyncAzureDurableFunctions.Data.Schema.PropertyResult;
using Property = SyncAzureDurableFunctions.Data.DomainModel.Property;

namespace SyncAzureDurableFunctions.Functions.Orchestrator
{
    public static class AzureServiceBusQueueOrchestrator
    {
        [FunctionName("AzureServiceBusQueueOrchestrator")]
        public static async Task<Results> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            try
            {
                string inputMessage = context.GetInput<string>();
                CloudEvent cloudEvent = ApiCore.DeSerializeMessageCloudEvent(inputMessage);
                IEnumerable<PropertyBase> properties = ApiCore.DeSerializeMessage(cloudEvent.Data.ToString());

                foreach (var property in properties)
                {
                    try
                    {
                        log.LogInformation($"Processing property {property.RefId}");

                        var idList = new XElement("Ids");
                        idList.Add(new XElement("Id", property.RefId));
                        var payloadXml = new XDocument(new XElement("GetInfo", new XElement("Type", "Property"), idList));

                        var propertyInfoTask = context.CallActivityAsync<Property>("A_FetchPropertyInfo", payloadXml.ToString());
                        //var propertyProcessingTask = context.CallActivityAsync<Results>("A_ImportAndProcessProperty", propertyInfoTask);

                        await propertyInfoTask;

                        if (propertyInfoTask.Result != null)
                        {
                            var propertyProcessingTask = await context.CallActivityAsync<Results>("A_ImportAndProcessProperty", propertyInfoTask);
                        }

                    }
                    catch (Exception ex)
                    {
                        if (!context.IsReplaying)
                        {
                            log.LogError($"Failed while processing property", ex);
                        }
                    }
                }

                log.LogInformation($"Done with the orchestration with Durable Context Id:  {context.InstanceId}");
                return default;
            }
            catch (FunctionFailedException ex)
            {
                log.LogError($"Something went wrong " + ex.Message);
                return default;
            }
            catch (Exception ex)
            {
                log.LogError($"Something went wrong " + ex.Message);
                return default;
            }
        }
    }
}
