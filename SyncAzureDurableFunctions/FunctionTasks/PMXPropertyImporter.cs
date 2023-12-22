using Microsoft.Extensions.Logging;
using MRI.PandA.Syncs.Data.Configuration;
using SyncAzureDurableFunctions.MriApis;
using System;
using System.Threading.Tasks;
using static SyncAzureDurableFunctions.Data.Schema.PropertyResult;

namespace SyncAzureDurableFunctions.FunctionTasks
{
    public class PMXPropertyImporter
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;
        private readonly string HARD_CODED_CLIENT = "MRIQWEB";

        public PMXPropertyImporter(IServiceProvider serviceProvider, ILogger log)
        {
            this.serviceProvider = serviceProvider;
            this.logger = log;
        }

        public async Task<Results> Run(string xmlRequestData)
        {
            try
            {
                FeedConfig feedConfig = new FeedConfig
                {
                    ClientDatabase = "MCX6_RMAPI",
                    WebServicePassword = "Mri123!",
                    WebServiceUsername = "VW_API",
                    WebServiceUrl = "https://mrix6api-trunk.qasaas.mrisoftware.net/MRIAPIServices/api",
                    MixApiKey = "89A2D1FE4C1ECEE7A2B5D4AAC86A6772076099A0A0505BE583A4730CA4A2807E"
                };

                var mriApiClientFactory = (IMriApiClientFactory)serviceProvider.GetService(typeof(IMriApiClientFactory));
                var mriApiClient = mriApiClientFactory.Build(HARD_CODED_CLIENT, feedConfig);

                // CHANGE THE HARD CODED DATA TO DYNAMIC PROPERTIES etc.. pass to the input parameter
                xmlRequestData = @"<GetInfo>
	                                <Type>Property</Type>
	                                <Ids>
		                                <Id>nss</Id>
		                                <Id>duke</Id>
	                                </Ids>
                                    </GetInfo>";

                return await mriApiClient.MakeRequest<Results>("MRIRMConnectGetInfo", xmlRequestData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + "Exception caught at PMXPropertyImporter");
                throw;
            }
        }
    }
}
