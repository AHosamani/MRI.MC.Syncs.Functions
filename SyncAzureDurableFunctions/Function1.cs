using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Storage;
using Azure.Storage.Queues.Models;
using Azure.Storage.Queues;
using Microsoft.Azure.Storage.Queue;
using System.Text;

namespace SyncAzureDurableFunctions
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName(nameof(SayHello))]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation("Saying hello to {name}.", name);
            return $"Hello {name}!";

        }


        [FunctionName("Function1_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var storageAccountConnectionString = "DefaultEndpointsProtocol=https;AccountName=triptisa;AccountKey=uXRaSzNBsHMTDlb9CIueE79MtCzER56cy4mi5m/1BQXcYztrXuA7z/vc+v7m69qlgQod+ILa4yla+ASto+rYXQ==;EndpointSuffix=core.windows.net";
            var queueName = "propertyqueue";
            var queuedataClient = new QueueClient(storageAccountConnectionString, queueName);
            var functionUrl = "http://localhost:7287/api/Function1_HttpStart";
            var data1 = queuedataClient.ReceiveMessagesAsync(maxMessages: 18);
            foreach (QueueMessage message in (await queuedataClient.ReceiveMessagesAsync(maxMessages: 18)).Value)
            {
                string originalMessage = Encoding.UTF8.GetString(Convert.FromBase64String(message.MessageText));
            }
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("Function1", null);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}