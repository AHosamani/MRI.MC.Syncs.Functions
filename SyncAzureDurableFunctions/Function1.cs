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
using MRI.PandA.Data;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.AspNetCore.Mvc;
using MRI.PandA.Data.DataModel;
using System.Xml.Linq;
using System.Numerics;
using Microsoft.Extensions.Configuration;
using MRI.PandA.Syncs.Data.Configuration;
using System.Data.Common;
using SyncAzureDurableFunctions.MriApis;
using System.Runtime.InteropServices;
using MRI.PandA.Syncs.MriApis;
using Microsoft.Extensions.Configuration.Json;
using Castle.Core.Configuration;
using SyncAzureDurableFunctions.ConfigurationFile;

namespace SyncAzureDurableFunctions
{
    public class Function1
    {
        private readonly HttpClient _httpClientFactory;
        private readonly AppSettings _configuration;
        MRI.PandA.Syncs.Data.Configuration.FeedConfig _feedConfig = new();
        public Function1(HttpClient httpClient, AppSettings configuration, FeedConfig feedConfig)
        {
            _httpClientFactory = httpClient;
            _configuration = configuration;

            this._feedConfig.ClientDatabase = "MCX6_RMAPI";
            this._feedConfig.WebServicePassword = "Mri123!";
            this._feedConfig.WebServiceUsername = "VW_API";
            this._feedConfig.WebServiceUrl = "https://mrix6api-trunk.qasaas.mrisoftware.net/MRIAPIServices/api";
            this._feedConfig.MixApiKey = "89A2D1FE4C1ECEE7A2B5D4AAC86A6772076099A0A0505BE583A4730CA4A2807E";

        }

        [FunctionName("Function1")]
        public async Task RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var input = context.GetInput<List<string>>();
            IDbConnection connection = new SqlConnection(_configuration.DefaultConnection);
            PropertyDataService property = new PropertyDataService(connection);

            var inputdata = property.GetPropertyInfo();
            context.CallActivityAsync<dynamic>(nameof(Execute), inputdata);
        }

        [FunctionName(nameof(Execute))]
        public async Task Execute([ActivityTrigger] Dictionary<string, string> input, ILogger log)
        { 
            string[] ids = { input["ids"] };
            SyncAzureDurableFunctions.MriApis.MriApiClient mriApiClient = new SyncAzureDurableFunctions.MriApis.MriApiClient(_httpClientFactory, _feedConfig);
            IMriApiClient client_ = mriApiClient;
            PropertyApi.PropertyMriApi mriApiCall = new PropertyApi.PropertyMriApi(client_);
            await mriApiCall.ImportProperty(input["propertyId"], ids);

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
            List<string> propertyIds = new List<string>();
            foreach (QueueMessage message in (await queuedataClient.ReceiveMessagesAsync(maxMessages: 18)).Value)
            {
                string originalMessage = Encoding.UTF8.GetString(Convert.FromBase64String(message.MessageText));
                propertyIds.Add(originalMessage);
            }
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("Function1", propertyIds);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

    }

}