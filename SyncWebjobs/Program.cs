using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.Storage;
using Microsoft.Extensions.Configuration;
using MRI.PandA.Data;
using System.Data.SqlClient;
using System.Data;

namespace SyncWebjobs
{
    class Program
    {
        static async Task Main()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            var storageAccountConnectionString = configuration.GetConnectionString("StorageAccount");
            var queueName = "propertyqueue";

            IDbConnection connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            PropertyDataService property = new PropertyDataService(connection);
            var queues = property.GetAllPropertyIds();

            var storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);

            foreach (var id in queues)
            {
                CloudQueueMessage message = new CloudQueueMessage(id);
                queue.AddMessage(message);
            }
            var functionUrl = configuration.GetSection("Url").GetChildren().FirstOrDefault().Value;
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
            }
            queue.Clear();
        }
    }
}

