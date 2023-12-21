using Castle.Core.Configuration;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MRI.PandA.Syncs.MriApis;
using SyncAzureDurableFunctions.ConfigurationFile;
using SyncAzureDurableFunctions.MriApis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MriApiClient = SyncAzureDurableFunctions.MriApis.MriApiClient;

[assembly: FunctionsStartup(typeof(SyncAzureDurableFunctions.Startup))]

namespace SyncAzureDurableFunctions
{
    public class Startup : FunctionsStartup
    {
        private static Microsoft.Extensions.Configuration.IConfiguration _configuration = null;

        public override void Configure(IFunctionsHostBuilder builder)
        {  
            var serviceProvider = builder.Services.BuildServiceProvider();
            _configuration = serviceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();

            builder.Services.Configure<AppSettings>(_configuration.GetSection("AppSettings"));
            builder.Services.AddSingleton<AppSettings>(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);
            builder.Services.AddSingleton<IMriApiClient, MriApiClient>();
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "local.settings.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"local.settings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }

    }
}
