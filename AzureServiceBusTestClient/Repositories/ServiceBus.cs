using Microsoft.Azure.ServiceBus;
using System.Text;
using System.Text.Json;

namespace AzureServiceBusTestClient.Repositories
{
    public interface IServiceBus
    {
        Task SendMessageAsync(IEnumerable<PropertyDetails> properties);
    }

    public class ServiceBus : IServiceBus
    {
        private readonly IConfiguration _configuration;

        public ServiceBus(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMessageAsync(IEnumerable<PropertyDetails> properties)
        {
            IQueueClient client = new QueueClient(_configuration["AzureServiceBusConnectionString"], _configuration["QueueName"]);

            //Serialize car details object
            var messageBody = JsonSerializer.Serialize(properties);

            //Set content type and Guid
            var message = new Message(Encoding.UTF8.GetBytes(messageBody))
            {
                MessageId = Guid.NewGuid().ToString(),
                ContentType = "application/json"
            };

            await client.SendAsync(message);
        }
    }
}
