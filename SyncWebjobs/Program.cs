using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SyncWebjobs;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Azure.Storage.Queues;

namespace ConsoleApp4
{
    class Program
    {
        static async Task Main()
        {

            var storageAccountConnectionString = "DefaultEndpointsProtocol=https;AccountName=triptisa;AccountKey=uXRaSzNBsHMTDlb9CIueE79MtCzER56cy4mi5m/1BQXcYztrXuA7z/vc+v7m69qlgQod+ILa4yla+ASto+rYXQ==;EndpointSuffix=core.windows.net";
            var queueName = "propertyqueue";
            PropertyService propertyService = new PropertyService();
            IEnumerable<string> queues = propertyService.GetAllPropertyIds();
            var storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);
           

            foreach (var id in queues)
            {
                CloudQueueMessage message = new CloudQueueMessage(id);
                queue.AddMessage(message);
            }
            var queuedataClient = new QueueClient(storageAccountConnectionString, queueName);         // Retrieve all messages from the queue        var messages = await ReceiveAllMessagesAsync(queueClient);
            var functionUrl = "http://localhost:7287/api/Function1_HttpStart";
            var data = await queuedataClient.ReceiveMessagesAsync(maxMessages:18) ;
            var jsonData = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
            var httpContent = new StringContent(jsonData);
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(functionUrl, httpContent);

                // Check the response status if needed
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Azure Function triggered successfully.");
                }
                else
                {
                    Console.WriteLine($"Error triggering Azure Function. Status code: {response.StatusCode}");
                }
                
            }
            queue.Clear();
        }
    }
}
