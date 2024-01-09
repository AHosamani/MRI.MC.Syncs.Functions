using Azure.Messaging;
using MRI.PandA.Data.DataModel;
using System.Text.Json;
using System.Collections.Generic;

namespace MRI.PandA.Data
{
    public class ApiCore
    {
        public static IEnumerable<PropertyBase> DeSerializeMessage(string message)
        {
            return JsonSerializer.Deserialize<IEnumerable<PropertyBase>>(message);
        }

        public static CloudEvent DeSerializeMessageCloudEvent(string message)
        {
            return JsonSerializer.Deserialize<CloudEvent>(message);
        }

        public static string SerializeAndCompressOrchestrationInput<T>(T input)
        {
            var inputString = JsonSerializer.Serialize(input);
            //var compressedString = StringCompressor.CompressString(inputString);
            return inputString;
        }
    }
}
