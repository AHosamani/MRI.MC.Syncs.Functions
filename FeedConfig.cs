using MRI.PandA.Syncs.Functions.DataAccess;
// using MRI.EncryptionLibrary;
// using MRI.EncryptionLibrary.Keys;

namespace MRI.PandA.Syncs.Data.Configuration
{
    public class FeedConfig
    {
        public string WebServiceUrl { get; set; }
        public string WebServiceUsername { get; set; }
        public string WebServicePassword { get; set; }
        public string ClientDatabase { get; set; }

        public string MixApiKey  {get; set; }

        public string AzureStorageConnectionString { get; set; }
        public string AzureStorageContainerName { get; set; }

        private readonly IDataAccessor _dataAccessor;

        public FeedConfig()
        {
            //Commenting out GetFeedInfo until I figure out how to correctly reference the EncryptionLibrary
            // GetFeedInfo(clientId, propertyId);
            WebServicePassword = "";
            WebServiceUsername = "";
            WebServiceUrl = "";
            ClientDatabase = "";

            MixApiKey = "";

            AzureStorageConnectionString = "";
            AzureStorageContainerName = "";

        }

        // public FeedConfig(IDataAccessor dataAccessor, string clientId, string propertyId)
        // {
        //     _dataAccessor = dataAccessor;
        //     GetFeedInfo(clientId, propertyId);
        // }

        // private void GetFeedInfo(string clientId, string propertyId)
        // {
        //     using(var command = new SqlCommand())
        //     {
        //         command.CommandText = @"SELECT [JsonData] FROM [DataFeedSourceSelections] dss
        //                                 JOIN [DataFeedSources] ds ON ds.Id = dss.DataFeedSourceId
        //                                 JOIN [Property] c ON c.[DataFeedSourceSelectionId] = dss.Id
        //                                 WHERE c.Id = @propertyId
        //                                 AND ds.Type = 2";

        //         command.Parameters.AddWithValue("@propertyId", propertyId);
        //         var propertyResult = _dataAccessor.ExecuteScalar<string>(command);
        //         var propertyConfig = propertyResult == null ? null : JsonConvert.DeserializeObject<DataSourceType.WebServiceWithDatabaseType>(propertyResult);

        //         command.Parameters.Clear();

        //         command.CommandText = @"SELECT [JsonData] FROM [DataFeedSourceSelections] dss
        //                                 JOIN [DataFeedSources] ds ON ds.Id = dss.DataFeedSourceId
        //                                 JOIN [Client] c ON c.[DataFeedSourceSelectionId] = dss.Id
        //                                 WHERE c.Id = @clientId
        //                                 AND ds.Type = 2";

        //         command.Parameters.AddWithValue("@clientId", clientId);
        //         var clientResult = _dataAccessor.ExecuteScalar<string>(command);
        //         var clientConfig = clientResult == null ? null : JsonConvert.DeserializeObject<DataSourceType.WebServiceWithDatabaseType>(clientResult);

        //         WebServiceUrl = string.IsNullOrWhiteSpace(propertyConfig?.WebServiceUrl) ? clientConfig?.WebServiceUrl : propertyConfig?.WebServiceUrl;
        //         WebServiceUsername = string.IsNullOrWhiteSpace(propertyConfig?.WebServiceUsername) ? clientConfig?.WebServiceUsername : propertyConfig?.WebServiceUsername;
        //         WebServicePassword = string.IsNullOrWhiteSpace(propertyConfig?.WebServicePassword) ? clientConfig?.WebServicePassword : propertyConfig?.WebServicePassword;
        //         ClientDatabase = string.IsNullOrWhiteSpace(propertyConfig?.ClientDatabase) ? clientConfig?.ClientDatabase : propertyConfig?.ClientDatabase;
                
        //         WebServicePassword = string.IsNullOrWhiteSpace(WebServicePassword) ? WebServicePassword : new AlgorithmManager(new KeyStore()).Decrypt(WebServicePassword).DecryptedValue;
        //     }
        // }
    }
}
