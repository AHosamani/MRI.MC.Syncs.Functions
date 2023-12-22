using MRI.PandA.Data.DataModel;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MRI.PandA.Data
{
    public class ApiCore
    {
        public static List<PropertyBase> DeSerializeMessage(string message)
        {
            return JsonConvert.DeserializeObject<List<PropertyBase>>(message);
        }

        public static string SerializeAndCompressOrchestrationInput<T>(T input)
        {
            var inputString = JsonConvert.SerializeObject(input);
            //var compressedString = StringCompressor.CompressString(inputString);
            return inputString;
        }
    }
}
