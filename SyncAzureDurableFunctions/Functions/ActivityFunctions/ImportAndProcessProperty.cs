using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MRI.PandA.Data.DataModel;
using SyncAzureDurableFunctions.FunctionTasks;
using System;
using System.Threading.Tasks;
using static SyncAzureDurableFunctions.Data.Schema.PropertyResult;

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

        public ImportAndProcessProperty(IServiceProvider serviceProvider, ILogger<ImportAndProcessProperty> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        [FunctionName("ImportAndProcessProperty")]
        public async Task<bool> ImportAndProcessPropertyActivity([ActivityTrigger] PropertyBase property, ILogger log, ExecutionContext executionContext)
        {
            //Config for Azure Service Bus
            var azureServiceBusConfig = new ConfigurationBuilder()
                 .SetBasePath(executionContext.FunctionAppDirectory)
                 .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                 .AddEnvironmentVariables()
                 .Build();
            
            try
            {
                // Call PMX API to get XML
                var importer = new PMXPropertyImporter(serviceProvider, logger);
                Results rmPropertyData = await importer.Run("nss");

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

                // await importer.Run(propertyDataService, property, new string[] { property.RefId });
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
