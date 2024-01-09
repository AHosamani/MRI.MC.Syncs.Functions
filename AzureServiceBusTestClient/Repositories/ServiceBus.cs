using Azure.Messaging;
using Microsoft.Azure.ServiceBus;
using MRI.PandA.Data;
using System.Text;
using System.Text.Json;

namespace AzureServiceBusTestClient.Repositories
{
    public interface IServiceBus
    {
        Task SendMessageAsync(IEnumerable<PropertyDetails> properties, bool importAll);
    }

    public class ServiceBus : IServiceBus
    {
        private readonly IConfiguration configuration;

        public ServiceBus(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendMessageAsync(IEnumerable<PropertyDetails> properties, bool importAll)
        {
            try
            {
                IList<CloudEvent> events = new List<CloudEvent>();
                //foreach (var property in properties)
                //{
                //    CloudEvent @event = GetCloudEvent(property, importAll);
                //    events.Add(@event);
                //}

                CloudEvent @event = GetCloudEvent(properties, importAll);
                events.Add(@event);

                ServiceBusSenderHelper sbHelper = new ServiceBusSenderHelper(configuration["AzureServiceBusConnectionString"], configuration["QueueName"]);
                await sbHelper.SendMessageAsync(events);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SendMessageUsingQueueClientAsync(IEnumerable<PropertyDetails> properties, bool importAll)
        {
            try
            {
                IQueueClient client = new QueueClient(configuration["AzureServiceBusConnectionString"], configuration["QueueName"]);

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
            catch (Exception)
            {
                throw;
            }
        }

        private static CloudEvent GetCloudEvent(IEnumerable<PropertyDetails> properties, bool importAll)
        {
            CloudEvent @event = new("MRI/MC/2.0", string.Empty, null)
            {
                Id = Guid.NewGuid().ToString(),
                Subject = "Background Jobs",
                Time = DateTimeOffset.UtcNow,
                Type = "Import",
            };
            @event.ExtensionAttributes.Add("importall", importAll);
            @event.Data = new BinaryData(properties);
            return @event;
        }
    }
}
