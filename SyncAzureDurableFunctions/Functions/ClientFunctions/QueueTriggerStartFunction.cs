using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;

namespace SyncAzureDurableFunctions.Functions.ClientFunctions
{
    public static class QueueTriggerStartFunction
    {
        [FunctionName("QueueTriggerStartFunction")]
        public static async Task Run(
            [ServiceBusTrigger("%QueueName%", Connection = "ServiceBusQueueConnectionString")]
            Message message,
            MessageReceiver messageReceiver,
            string lockToken,
            [DurableClient] IDurableOrchestrationClient starter, ILogger log)
        {

            string inputMessage = Encoding.UTF8.GetString(message.Body);
            log.LogInformation($"message - " + inputMessage);
            if (string.IsNullOrWhiteSpace(inputMessage))
            {
                await messageReceiver.DeadLetterAsync(lockToken, "Message content is empty.", "Message content is empty.");
            }

            if (inputMessage is null)
            {
                await messageReceiver.DeadLetterAsync(lockToken, "Missing Input.");
                log.LogInformation($"Message Dead Lettered: Missing Input.");
                return;
            }

            //string orchestrationInput = ApiCore.SerializeAndCompressOrchestrationInput(inputMessage);
            string orchestrationInput = inputMessage;
            string instanceId = await starter.StartNewAsync<string>("AzureServiceBusQueueOrchestrator", orchestrationInput);
            log.LogInformation($"Orchestration Started with ID: {instanceId}");

            var orchestrationStatus = await starter.GetStatusAsync(instanceId);
            var status = orchestrationStatus.RuntimeStatus.ToString().ToUpper();
            log.LogInformation($"Waiting to complete Orchestration function [Status:{status}][ID:{instanceId}]");

            while (status == "PENDING" || status == "RUNNING")
            {
                //await Task.Delay(1000);
                orchestrationStatus = await starter.GetStatusAsync(instanceId);
                status = orchestrationStatus.RuntimeStatus.ToString().ToUpper();
            }

            log.LogInformation($"AzureServiceBusQueueOrchestrator Function completed [Instance ID:{instanceId}]");

            if (!(bool)orchestrationStatus.Output)
            {
                await messageReceiver.AbandonAsync(lockToken);
            }
        }
    }
}
