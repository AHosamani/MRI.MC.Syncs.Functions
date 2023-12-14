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

namespace ConsoleApp4
{
    class Program
    {
        static async Task Main()
        {

            var storageAccountConnectionString = "";
            var queueName = "";
            PropertyService propertyService = new PropertyService();
            IEnumerable<string> queues =propertyService.GetAllPropertyIds();
            var storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);
            foreach (var id in queues)
            {
                CloudQueueMessage message = new CloudQueueMessage(id);
                queue.AddMessage(message);
            }

            var functionUrl = "https://durablefunction20231211134111.azurewebsites.net/api/Function1_HttpStart";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(functionUrl, null);

                // Check the response status if needed
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Azure Function triggered successfully.");
                }
                else
                {
                    Console.WriteLine($"Error triggering Azure Function. Status code: {response.StatusCode}");
                }
                queue.Delete();
            }
        }
    }
}
