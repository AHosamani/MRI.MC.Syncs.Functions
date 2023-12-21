using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Extensions.DependencyInjection;

using MRI.PandA.Syncs.Data.Configuration;

namespace SyncAzureDurableFunctions.MriApis;

public interface IMriApiClientFactory
{
    MriApiClient Build(string clientId, FeedConfig config);
}

public class MriApiClientFactory : IMriApiClientFactory
{

    public readonly IHttpClientFactory _httpClientFactory;

    public const string XML_CONTENT = "application/xml";

    public MriApiClientFactory(IHttpClientFactory clientFactory)
    {
        _httpClientFactory = clientFactory;
    }

    public MriApiClient Build(string clientId, FeedConfig config)
    {
        var apiUri = new Uri(config.WebServiceUrl);
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri($"{apiUri.Scheme}://{apiUri.Host}");
        httpClient.Timeout = new TimeSpan(0, 3, 0);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}/{config.ClientDatabase}/{config.WebServiceUsername}:{config.WebServicePassword}")));
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(XML_CONTENT));

        return new MriApiClient(httpClient, config);
    }
}
