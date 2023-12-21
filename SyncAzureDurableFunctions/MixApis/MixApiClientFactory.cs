using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Extensions.DependencyInjection;

using MRI.PandA.Syncs.Data.Configuration;

namespace MRI.PandA.Syncs.Functions.MixApis;

public interface IMixApiClientFactory {
  MixApiClient Build(string clientId, FeedConfig config);
}

public class MixApiClientFactory : IMixApiClientFactory {

  public readonly IHttpClientFactory _httpClientFactory;

  public const string XML_CONTENT = "application/xml";

  public MixApiClientFactory(IHttpClientFactory clientFactory) {
    _httpClientFactory = clientFactory;
  }

  public MixApiClient Build(string clientId, FeedConfig feedConfig) {
    var apiUri = new Uri(feedConfig.WebServiceUrl.TrimEnd('/'));
    var client = _httpClientFactory.CreateClient();
    client.BaseAddress = new Uri($"{apiUri.Scheme}://{apiUri.Host}");
    client.Timeout = new TimeSpan(0, 6, 0);
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}/{feedConfig.ClientDatabase}/{feedConfig.WebServiceUsername}:{feedConfig.WebServicePassword}")));
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(XML_CONTENT));

    return new MixApiClient(client, feedConfig, clientId);
  }
}