using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

using MRI.PandA.Syncs.Data.Configuration;

namespace MRI.PandA.Syncs.Functions.MixApis;



public interface IMix_ApiClientFactory {
  MixApiClient Build(string clientId, FeedConfig config);
}

public class Mix_ApiClientFactory : IMixApiClientFactory {

  public readonly IHttpClientFactory _httpClientFactory;

  public const string XML_CONTENT = "application/xml";

  public Mix_ApiClientFactory(IHttpClientFactory clientFactory) {
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



public class MixApiClient {
  public readonly HttpClient _httpClient;
  public readonly FeedConfig _feedConfig;
  public readonly string _clientId;


  internal MixApiClient(HttpClient httpClient, FeedConfig config, string clientId) {
    _feedConfig = config;
    _httpClient = httpClient;
    _clientId = clientId;
  }


  public async Task<T> CallApi<T>(MixApi api, string url, string clientId, string database, string apiKey) {
    var interpretedUrl = url.StartsWith("http") ? url : "";
    var formattedUrl = string.Format($"{interpretedUrl}.asp?$api={api.ToString()}");

    var xmlStart = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<" + api.Name.ToLowerInvariant() + ">" + Environment.NewLine;
    var xmlFinish = "</" + api.Name.ToLowerInvariant() + ">";
    var xmlPart = xmlStart;
    var finalXml = new XmlDocument();

    // The below empty string was meant to just get things build - could possibly be a better solution
    var apiUsername = string.Format("{0}/{1}/{2}/{3}", clientId, database, _feedConfig.WebServiceUsername, string.IsNullOrWhiteSpace(apiKey) ?
        // ConfigurationManager.AppSettings["MriDirectPartnerKey"] : 
        "" :
        apiKey);

    var encodedAuthorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", apiUsername, _feedConfig.WebServicePassword)));
    var pagesCalled = 0;

    do {
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedAuthorization);
      _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

      var response = await _httpClient.GetAsync(formattedUrl.Replace("http://", "https://"));

      if (!response.IsSuccessStatusCode) {
        throw new Exception(string.Format("Error trying to call API: {0}. Reason: {1}. url = {2} ", api.Name, response.ReasonPhrase, formattedUrl));
      }

      var temp = response.Content.ReadAsStringAsync().Result;
      if (string.IsNullOrWhiteSpace(temp)) {
        throw new Exception(string.Format("Response from API [{0}] was blank. \tURL: {1}", api.Name, formattedUrl));
      }

      var returnedXml = XDocument.Parse(temp);
      //File.WriteAllText("_" + api + "_" + propertyInfo.RdtId + "_returnedXml_" + pagesCalled + ".xml", returnedXml.ToString());

      var attribute = returnedXml.Root.Attribute("NextPageLink");
      if (attribute != null) {
        formattedUrl = attribute.Value.ToString();
        temp = temp[temp.IndexOf(MixApi.ENTRY_TAG)..temp.IndexOf(xmlFinish)];
        xmlPart += temp;
        pagesCalled++;

        var entryTagTrimmed = MixApi.ENTRY_TAG.Replace('<', ' ').Replace('>', ' ').Trim();
        var source = temp.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
        var entries = from entryTag in source where entryTag == entryTagTrimmed select entryTag;

        if (entries.Count() < Convert.ToInt32(api.Parameters.Find(t => t.Parameter.Equals(MixApi.DEFAULT_ROWS_PER_REQUEST_PARAMETER)).Value)) {
          formattedUrl = null;
        }
      }
      else {
        if (xmlPart.Equals(xmlStart)) {
          xmlPart += temp;
        }
        formattedUrl = null;
      }
    } while (formattedUrl != null);

    if (pagesCalled == 0) {
      return default;
    }

    xmlPart += xmlFinish;

    //File.WriteAllText("_" + api + "_" + propertyInfo.RdtId + "_xmlPart.xml", xmlPart); //FOR TESTING

    finalXml.LoadXml(xmlPart);

    // Turn XML into a stream, so that it can be deserialized, as the Deserialize method cannot take an actual XML object
    using var stream = new MemoryStream();
    finalXml.Save(stream);
    stream.Position = 0;

    return (T)new XmlSerializer(typeof(T)).Deserialize(stream);
  }


  public static T CallApi<T>(MixApi api, HttpClient httpClient, string url, string clientId, string database, string userName, string password, string apiKey) {
    var interpretedUrl = url.StartsWith("http") ? url : "";
    var formattedUrl = string.Format($"{interpretedUrl}.asp?$api={api.ToString()}");

    var xmlStart = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<" + api.Name.ToLowerInvariant() + ">" + Environment.NewLine;
    var xmlFinish = "</" + api.Name.ToLowerInvariant() + ">";
    var xmlPart = xmlStart;
    var finalXml = new XmlDocument();

    // The below empty string was meant to just get things build - could possibly be a better solution
    var apiUsername = string.Format("{0}/{1}/{2}/{3}", clientId, database, userName, string.IsNullOrWhiteSpace(apiKey) ?
        // ConfigurationManager.AppSettings["MriDirectPartnerKey"] : 
        "" :
        apiKey);

    var encodedAuthorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", apiUsername, password)));
    var pagesCalled = 0;


    //not wrapped in using based on: https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
    //so should be passed to function. Will also allow for unit testing.
    var client = httpClient ?? new HttpClient();

    do {
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedAuthorization);
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

      var response = client.GetAsync(formattedUrl.Replace("http://", "https://")).Result;

      if (!response.IsSuccessStatusCode) {
        throw new Exception(string.Format("Error trying to call API: {0}. Reason: {1}. url = {2} ", api.Name, response.ReasonPhrase, formattedUrl));
      }

      var temp = response.Content.ReadAsStringAsync().Result;
      if (string.IsNullOrWhiteSpace(temp)) {
        throw new Exception(string.Format("Response from API [{0}] was blank. \tURL: {1}", api.Name, formattedUrl));
      }

      var returnedXml = XDocument.Parse(temp);
      //File.WriteAllText("_" + api + "_" + propertyInfo.RdtId + "_returnedXml_" + pagesCalled + ".xml", returnedXml.ToString());

      var attribute = returnedXml.Root.Attribute("NextPageLink");
      if (attribute != null) {
        formattedUrl = attribute.Value.ToString();
        temp = temp[temp.IndexOf(MixApi.ENTRY_TAG)..];
        temp = temp[..temp.IndexOf(xmlFinish)];
        xmlPart += temp;
        pagesCalled++;

        var entryTagTrimmed = MixApi.ENTRY_TAG.Replace('<', ' ').Replace('>', ' ').Trim();
        var source = temp.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
        var entries = from entryTag in source where entryTag == entryTagTrimmed select entryTag;

        if (entries.Count() < Convert.ToInt32(api.Parameters.Find(t => t.Parameter.Equals(MixApi.DEFAULT_ROWS_PER_REQUEST_PARAMETER)).Value)) {
          formattedUrl = null;
        }
      }
      else {
        if (xmlPart.Equals(xmlStart)) {
          xmlPart += temp;
        }
        formattedUrl = null;
      }
    } while (formattedUrl != null);

    if (pagesCalled == 0) {
      return default;
    }

    xmlPart += xmlFinish;

    //File.WriteAllText("_" + api + "_" + propertyInfo.RdtId + "_xmlPart.xml", xmlPart); //FOR TESTING

    finalXml.LoadXml(xmlPart);

    // Turn XML into a stream, so that it can be deserialized, as the Deserialize method cannot take an actual XML object
    using var stream = new MemoryStream();
    finalXml.Save(stream);
    stream.Position = 0;

    return (T)new XmlSerializer(typeof(T)).Deserialize(stream);
  }
}