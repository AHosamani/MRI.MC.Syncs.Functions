using Azure.Messaging;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MRI.PandA.Data
{
    public class ServiceBusSenderHelper
    {
        /// <summary>
        /// The service bus client
        /// </summary>
        private readonly ServiceBusClient serviceBusClient;

        /// <summary>
        /// The service bus sender
        /// </summary>
        private readonly ServiceBusSender serviceBusSender;

        public ServiceBusSenderHelper(string serviceBusConnectionString, string queueName)
        {
            serviceBusClient = new ServiceBusClient(serviceBusConnectionString, new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            });
            serviceBusSender = serviceBusClient.CreateSender(queueName);
        }

        public async Task SendMessageAsync(CloudEvent cloudEvent)
        {
            ServiceBusMessage serviceBusMessage = new()
            {
                Body = new BinaryData(cloudEvent)
            };
            await this.serviceBusSender.SendMessageAsync(serviceBusMessage);
        }

        public async Task SendMessageAsync(IEnumerable<CloudEvent> cloudEvents)
        {
            IList<ServiceBusMessage> serviceBusMessages = new List<ServiceBusMessage>();
            foreach (var cloudEvent in cloudEvents)
            {
                ServiceBusMessage serviceBusMessage = new()
                {
                    Body = new BinaryData(cloudEvent)
                };
                serviceBusMessages.Add(serviceBusMessage);
            }
            await this.serviceBusSender.SendMessagesAsync(serviceBusMessages);
        }
    }

    public static class BlobClientExtensions
    {
        public static Task UploadTextAsync(this BlobClient client, string text)
        {
            var content = new BinaryData(text);
            return client.UploadAsync(content, true); // support overwrite as we use this to update blobs
        }

        public async static Task<string> DownloadTextAsync(this BlobClient client)
        {
            var res = await client.DownloadContentAsync();
            return res.Value.Content.ToString();
        }
    }
}
