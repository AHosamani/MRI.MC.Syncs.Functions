using MarketConnect.Data.Models;

namespace MarketConnect.Data.Repository
{
    public interface IMarketConnectDestinationRepository
    {
        bool PropertyHasCurrentLicense(string propertyId);
       
        void UpdatePropertyInformation(ResultsInfo propertyInfo, string propertyId);

        long InsertAddress(string line1, string line2, string line3, string city, string state, string postalcode1, string county, string country);

        void UpdateAddress(long addressId, string line1, string line2, string line3, string city, string state, string postalcode1, string county, string country);

        void InsertOrUpdateGlobalAmenityInformation(ResultsInfoAmenity[] amenities, string propertyId);

        void InsertOrUpdatePropertyAmenityInformation(ResultsInfo propertyResults, string propertyId);

        void InsertChargeCodes(ResultsInfoChargeCode[] chargeCodeResults, string propertyId, string clientId);

        void InsertOrUpdateBuildingInformation(ResultsInfo propertyResults, string propertyId);

        //void UpdateSyncStatus(string clientId, string propertyId, string syncType, string stateName, string message);

    }
}
