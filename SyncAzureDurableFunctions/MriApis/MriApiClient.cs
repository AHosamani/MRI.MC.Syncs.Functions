using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.DependencyInjection;
using MRI.PandA.Syncs.Data.Configuration;

namespace SyncAzureDurableFunctions.MriApis;

public interface IMriApiClient
{
    Task<T> MakeRequest<T>(string url, string payload);
}

public class MriApiClient : IMriApiClient
{

    public readonly HttpClient _client;
    public readonly FeedConfig _feedConfig;

    /// <summary>
    /// Do not use this, unless you cannot use MriApiClientFactory.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="config"></param>
    internal MriApiClient(HttpClient client, FeedConfig config)
    {
        _feedConfig = config;
        _client = client;
    }


    public async Task<T> MakeRequest<T>(string url, string payload)
    {
        var response = await _client.PostAsync($"{new Uri(_feedConfig.WebServiceUrl).AbsolutePath}/{url}", new StringContent(payload));
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error trying to call API {url}. Reason: {response.ReasonPhrase}");
        }

        return (T)new XmlSerializer(typeof(T)).Deserialize(response.Content.ReadAsStreamAsync().Result);
    }
}
