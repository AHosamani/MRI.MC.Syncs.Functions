using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncAzureDurableFunctions.ConfigurationFile
{
    public class AppSettings
    {
        public string DBConnection { get; set; }
        public string InstallPath { get; set; }
        public string Archive { get; set; }
        public string Data { get; set; }
        public string MixApiKey { get; set; }
        public string SystemName { get; set; }
        public int RightMoveNetworkId { get; set; }
        public string CertStoreName { get; set; }
        public string CertStoreLocation { get; set; }
        public string RightMoveCertFriendlyName { get; set; }
        public int RightMoveThrottleCallsPerMinute { get; set; }
        public string RightMoveApiUrlDefault { get; set; }
        public string MarketConnectUrlAvailability { get; set; }
        public string DomainGroupAuthTokenUrl { get; set; }
        public string DomainGroupAuthClientId { get; set; }
        public string DomainGroupAuthClientSecret { get; set; }
        public string DomainGroupApiUrl { get; set; }
    }
}
