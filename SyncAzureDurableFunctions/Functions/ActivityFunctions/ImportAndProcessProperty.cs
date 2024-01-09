using MarketConnect.Data.Repository;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MarketConnect.Data.Models;
using System;
using System.Threading.Tasks;

namespace SyncAzureDurableFunctions.Functions.ActivityFunctions
{
    public class ImportAndProcessProperty
    {
        /// <summary>
        /// The mri API client factory
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<ImportAndProcessProperty> logger;

        MarketConnectDestinationRepository marketConnectDestinationRepository;

        public ImportAndProcessProperty(IServiceProvider serviceProvider, ILogger<ImportAndProcessProperty> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            marketConnectDestinationRepository = new MarketConnectDestinationRepository();
        }

        [FunctionName("A_ImportAndProcessProperty")]
        public async Task<bool> ImportAndProcessPropertyActivity([ActivityTrigger] ResultsInfo propertyInfo, ILogger log, ExecutionContext executionContext)
        {
            //Config for Azure Service Bus
            var azureServiceBusConfig = new ConfigurationBuilder()
                 .SetBasePath(executionContext.FunctionAppDirectory)
                 .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                 .AddEnvironmentVariables()
            .Build();
            
            try
            {
                log.LogInformation($"Updating property info for: {propertyInfo.RmPropId}");
                //marketConnectDestinationRepository.UpdatePropertyInformation(propertyInfo, "");

            }
            catch (Exception ex)
            {
                log.LogInformation($"Something went wrong. Exception : {ex.InnerException}. Stopping process for import API");
                throw;
            }

            
            try
            {
                // Process the above rmPropertyData for insert to our db

                // TODO await

                //await importer.Run(propertyDataService, property, new string[] { property.RefId });
            }
            catch (Exception ex)
            {
                log.LogInformation($"Something went wrong. Exception : {ex.InnerException}. Stopping process for import data");
                throw;
            }

            // TODO
            return true;
        }
    }
}
