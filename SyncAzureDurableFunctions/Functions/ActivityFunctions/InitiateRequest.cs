using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using MRI.PandA.Data;
using MRI.PandA.Syncs.Data.Configuration;
using SyncAzureDurableFunctions.Data.DomainModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static SyncAzureDurableFunctions.Data.Schema.PropertyResult;
using Property = SyncAzureDurableFunctions.Data.DomainModel.Property;

namespace SyncAzureDurableFunctions.Functions.ActivityFunctions
{
    public class InitiateRequest
    {
        private readonly BlobServiceClient blobServiceClient;

        private const string XML_CONTENT = "application/xml";

        private static readonly FeedConfig config = new FeedConfig()
        {
            ClientDatabase = "\"****\",",
            WebServicePassword = "****",
            WebServiceUsername = "****",
            WebServiceUrl = "****",
            MixApiKey = "\"****\",",
        };

        public InitiateRequest(BlobServiceClient blobServiceClien)
        {
            this.blobServiceClient = blobServiceClien;
        }

        [FunctionName("A_FetchPropertyInfo")]
        public async Task<Property> FetchPropertyInfo([ActivityTrigger] string payload, ILogger logger)
        {
            using (var client = HttpClientFactory.Create())
            {
                var apiUri = new Uri(config.WebServiceUrl);
                client.BaseAddress = new Uri($"{apiUri.Scheme}://{apiUri.Host}");
                client.Timeout = new TimeSpan(0, 3, 0);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.ClientDatabase}/{config.MixApiKey}/{config.WebServiceUsername}:{config.WebServicePassword}")));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(XML_CONTENT));
                var response = await client.PostAsync(apiUri, new StringContent(payload));

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError($"Error trying to call API {apiUri}. Reason: {response.ReasonPhrase}");
                }

                var propertyInfo = (Results)new XmlSerializer(typeof(Results)).Deserialize(response.Content.ReadAsStreamAsync().Result);

                if (!propertyInfo.Items.Any())
                {
                    logger.LogWarning("Property API did not return any results.");
                }

                var property = BuildProperty(propertyInfo.Items.FirstOrDefault());
                try
                {
                    var propertyJson = JsonSerializer.Serialize(property, new JsonSerializerOptions { WriteIndented = true });

                    // Get a reference to the container
                    var pmxResponseContainer = blobServiceClient.GetBlobContainerClient("nss");
                    await pmxResponseContainer.CreateIfNotExistsAsync();

                    // Generate a unique filename based on the current date and time
                    string fileName = $"file_{DateTime.UtcNow:yyyyMMddHHmmss}_nss.json";
                    
                    var blob = pmxResponseContainer.GetBlobClient(fileName);

                    // Upload the file to the blob storage
                    await blob.UploadTextAsync(propertyJson);

                    var blobUri = blob.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddDays(1)).ToString();

                    logger.LogWarning($"API {apiUri} Results: Success");
                    logger.LogWarning($"blobUri {blobUri}");
                }
                catch (Exception ex)
                {
                    logger.LogWarning("Error while parsing.");
                }
                return property;
            }

        }

        private static Property BuildProperty(ResultsInfo propertyInfo)
        {
            return new Property()
            {
                Name = propertyInfo.PropertyName,
                ReferenceId = propertyInfo.RmPropId,
                Address = new Address()
                {
                    AddressLines = new List<string>() {
                      propertyInfo.Address1,
                      propertyInfo.Address2,
                      propertyInfo.Address3
                    },
                    City = propertyInfo.City,
                    State = propertyInfo.State,
                    PostalCode = propertyInfo.Zip,
                    County = propertyInfo.County,
                    Country = propertyInfo.Country
                },

                Amenities = propertyInfo.Amenities.Select(a => new Amenity()
                {
                    Id = a.AmenityCode,
                    Name = a.DisplayName,
                    Description = a.Description,
                    Rank = a.DisplayOrder,
                    Code = a.AmenityCode
                }).ToList(),

                Buildings = propertyInfo.Buildings.Select(b => new Building()
                {
                    Name = b.BuildingId,
                    Description = b.BuildingDescription,
                    Address = new Address()
                    {
                        AddressLines = new List<string>() {
                            b.BuildingAddress1,
                            b.BuildingAddress2,
                            b.BuildingAddress3
                        },
                        City = b.BuildingCity,
                        State = b.BuildingState,
                        PostalCode = b.BuildingZip,
                        County = b.BuildingCounty,
                        Country = b.BuildingCountry
                    }
                }).ToList()
            };
        }
    }
}
