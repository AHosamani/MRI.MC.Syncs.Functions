using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;

using MRI.PandA.Syncs.Data.Configuration;

namespace MRI.PandA.Syncs.MriApis;

public class MriApiClient {

  public readonly HttpClient _client;
  public readonly FeedConfig _feedConfig;
  public const string XML_CONTENT = "application/xml";

  public interface IMriApiClient {
    T MakeRequest<T>(string url, string payload);
  }

  public MriApiClient(string clientId, FeedConfig config) {
    _feedConfig = config;

    var apiUri = new Uri(config.WebServiceUrl);
    _client = new HttpClient();
    _client.BaseAddress = new Uri($"{apiUri.Scheme}://{apiUri.Host}");
    _client.Timeout = new TimeSpan(0, 6, 0);
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}/{_feedConfig.ClientDatabase}/{_feedConfig.WebServiceUsername}:{_feedConfig.WebServicePassword}")));
    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(XML_CONTENT));
  }


  public T MakeRequest<T>(string url, string payload) {
    var response = _client.PostAsync($"{new Uri(_feedConfig.WebServiceUrl).AbsolutePath}/{url}", new StringContent(payload)).Result;
    if (!response.IsSuccessStatusCode) {
      throw new Exception($"Error trying to call API {url}. Reason: {response.ReasonPhrase}");
    }

    return (T)new XmlSerializer(typeof(T)).Deserialize(response.Content.ReadAsStreamAsync().Result);
  }
}
