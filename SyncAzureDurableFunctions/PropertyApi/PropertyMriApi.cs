using MRI.PandA.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SyncAzureDurableFunctions.Data.DomainModel;
using SyncAzureDurableFunctions.Data.Schema;
using SyncAzureDurableFunctions.MriApis;
using System.IO;
using System.Text.Json;

namespace SyncAzureDurableFunctions.PropertyApi
{
    public class PropertyMriApi
    {
        public readonly IMriApiClient _mriApiClient;
        private MRI.PandA.Syncs.MriApis.MriApiClient client;
        private MRI.PandA.Syncs.MriApis.MriApiClient.IMriApiClient client1;

        public PropertyMriApi(IMriApiClient mriApiClient) {
            _mriApiClient = mriApiClient;
        }

        public PropertyMriApi(MRI.PandA.Syncs.MriApis.MriApiClient client)
        {
            this.client = client;
        }

        public PropertyMriApi(MRI.PandA.Syncs.MriApis.MriApiClient.IMriApiClient client1)
        {
            this.client1 = client1;
        }

        public async Task ImportProperty(string propertyId, string[] refIds)
        {
            // log.Info($"Importing property information. Ids: {string.Join(", ", ids)}");
            var payloadXml = new XDocument(
              new XElement("GetInfo",
                new XElement("Type", "Property"),
                new XElement("Ids",
                  refIds.Select(i => new XElement("Id", i))
                )
              )
            );

            var propertyInfo = await _mriApiClient.MakeRequest<PropertyResult.Results>($"MRIRMConnectGetInfo", payloadXml.ToString());
            if (!propertyInfo.Items.Any())
            {
                var message = "Property API did not return any results.";
                // destination.UpdateSyncStatus(clientId, propertyId, importType.ToString(), "Scheduled", message);
                throw new Exception(message);
            }

            var property = BuildProperty(propertyInfo.Items.FirstOrDefault());

            // check for implicit processing of unit types.

            await SavePropertyToJsonFile(propertyId, property);
        }

        private async Task SavePropertyToJsonFile(string propertyId, Data.DomainModel.Property property)
        {
            var json = JsonSerializer.Serialize(property, new JsonSerializerOptions { WriteIndented = true });

            //using(var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            //{
            //    await _blobContainerClient.UploadBlobAsync($"{_guid}/{propertyId}/property", stream);
            //}

            var localDataPath = @"C:\MRIPandASyncsTestData\Data\Property";
            Directory.CreateDirectory(localDataPath);
            var savePath = Path.Combine(localDataPath, propertyId);
            if (!string.IsNullOrWhiteSpace(json))
            {
                await File.WriteAllTextAsync(savePath, json);
            }
        }
        //
        // Todo - Move this method to MRI.PandA.Data
        //
        private List<string> GetBuildingIdsForProperty(string propertyId)
        {
            using var command = new SqlCommand("SELECT Id FROM Building WHERE PropertyId = @propertyId");
            command.Parameters.AddWithValue("@propertyId", propertyId);

            return new List<string> { }; //_dataAccessor.Execute(command).Select(r => r.GetString("Id")).ToList();
        }



        private static Data.DomainModel.Property BuildProperty(PropertyResult.ResultsInfo propertyInfo)
        {
            return new Data.DomainModel.Property()
            {
                Name = propertyInfo.PropertyName,
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
