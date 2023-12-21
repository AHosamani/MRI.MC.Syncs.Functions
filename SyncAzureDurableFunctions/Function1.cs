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

namespace SyncAzureDurableFunctions
{
    public class Function1
    {
        private readonly HttpClient _httpClientFactory;
        public Function1(HttpClient httpClient) { 
            _httpClientFactory = httpClient;
        }
        [FunctionName("Function1")]
        public void RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var input = context.GetInput<List<string>>();
            IDbConnection connection = new SqlConnection("Server=sql-eastus-pna-qa.database.windows.net;Database=sqldb-eastus-pna-portal-qa;User Id=zLZ3tOGLxjk3Wx69RvyY;Password='ETeIP2N5XhOPYL71R2mj';MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            PropertyDataService property = new PropertyDataService(connection);
           
                var inputdata = GetPropertyInfo();
                context.CallActivityAsync<dynamic>(nameof(MriApiCall), inputdata);
            
        }

        [FunctionName(nameof(MriApiCall))]
        public void MriApiCall([ActivityTrigger] Dictionary<string, string> input, ILogger log)
        {
            var feedConfig = new FeedConfig()
            {
                ClientDatabase = "MCX6_RMAPI",
                WebServicePassword = "Mri123!",
                WebServiceUsername = "VW_API",
                WebServiceUrl = "https://mrix6api-trunk.qasaas.mrisoftware.net/MRIAPIServices/api",

                MixApiKey = "89A2D1FE4C1ECEE7A2B5D4AAC86A6772076099A0A0505BE583A4730CA4A2807E",
            };
            
            string[] ids = { input["ids"] };
             SyncAzureDurableFunctions.MriApis.MriApiClient mriApiClient= new SyncAzureDurableFunctions.MriApis.MriApiClient(_httpClientFactory,feedConfig);
            IMriApiClient client_ = mriApiClient;
            PropertyApi.PropertyMriApi mriApiCall = new PropertyApi.PropertyMriApi(client_);
            mriApiCall.ImportProperty(input["propertyId"], ids);

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

        private Dictionary<string, string> GetPropertyInfo()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            
            list.Add("propertyId", "4b92f56b - 38e3 - 40c4 - 91f4 - 0c6de9317bc1");
            list.Add("ids", "duke") ;
            return list;
        }
    }

}