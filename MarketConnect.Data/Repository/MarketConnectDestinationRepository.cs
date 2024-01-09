using Dapper;
using MarketConnect.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MarketConnect.Data.Repository
{
    public class MarketConnectDestinationRepository : BaseRepository, IMarketConnectDestinationRepository
    {
        public bool PropertyHasCurrentLicense(string propertyId)
        {
            using (IDbConnection conn = DefaultConnection)
            {
                conn.Open();
                string sql = @"SELECT TOP 1 id
                               FROM   salesforcelicenses
                               WHERE  propertyid = @propertyId
                                       AND [status] = @status
                                       AND Cast(startdate AS DATE) <= Cast(@date AS DATE)
                                       AND Cast(enddate AS DATE) >= Cast(@date AS DATE) ";

                var queryResult = conn.Query(sql, new { propertyId, Status = "Active", Date = DateTime.UtcNow });
                return queryResult != null ? true : false;
            }
        }

        public virtual void UpdatePropertyInformation(ResultsInfo propertyInfo, string propertyId)
        {

            using (IDbConnection conn = DefaultConnection)
            {
                conn.Open();
                string sql = @"SELECT AddressId FROM Property WHERE Id = @propertyId";
                var addressId = conn.Query<long>(sql, new { propertyId }).FirstOrDefault();

                if (addressId == 0)
                {
                    addressId = InsertAddress(propertyInfo.Address1, propertyInfo.Address2, propertyInfo.Address3, propertyInfo.City, propertyInfo.State, propertyInfo.Zip, propertyInfo.County, propertyInfo.Country);
                }
                else
                {
                    UpdateAddress(addressId, propertyInfo.Address1, propertyInfo.Address2, propertyInfo.Address3, propertyInfo.City, propertyInfo.State, propertyInfo.Zip, propertyInfo.County, propertyInfo.Country);
                }

                string updateQuery = @"UPDATE Property SET
                                            NamePms = @name,
                                            AddressId = @addressId
                                        WHERE Id = @propertyId";

                conn.Query(updateQuery, new { name = propertyInfo.PropertyName, propertyId, addressId });
            }
        }

        public long InsertAddress(string line1, string line2, string line3, string city, string state, string postalcode1, string county, string country)
        {
            using (IDbConnection conn = DefaultConnection)
            {
                conn.Open();
                string sql = @"BEGIN TRAN
                                            INSERT INTO [Address](Line1PMS, Line2PMS, Line3PMS, CityPMS, StatePMS, PostalCode1PMS, CountyPMS, CountryPMS, Latitude, Longitude)
                                            VALUES(@line1, @line2, @line3, @city, @state, @postalcode1, @county, @country, @latitude, @longitude)
                                            SELECT SCOPE_IDENTITY() AS AddressId;
                                        COMMIT TRAN";

                return (long)conn.Query(sql, new { line1, line2, line3, city, state, postalcode1, county, country, latitude = 0, longitude = 0 })?.First().Get("AddressId");
            }
        }

        public void UpdateAddress(long addressId, string line1, string line2, string line3, string city, string state, string postalcode1, string county, string country)
        {
            using (IDbConnection conn = DefaultConnection)
            {
                conn.Open();
                string sql = @"UPDATE [Address] SET 
                                            Line1PMS = @line1,
                                            Line2PMS = @line2,
                                            Line3PMS = @line3,
                                            CityPMS = @city,
                                            StatePMS = @state,
                                            PostalCode1PMS = @postalcode1,
                                            CountyPMS = @county,
                                            CountryPMS = @country
                                    WHERE Id = @addressId";

                var res = conn.Query<string>(sql, new { line1, line2, line3, city, state, postalcode1, county, country, addressId });
            }
        }

        public virtual void InsertOrUpdateGlobalAmenityInformation(ResultsInfoAmenity[] amenities, string propertyId)
        {
            using (IDbConnection conn = DefaultConnection)
            {
                conn.Open();
                var defaultIlsMappingJson = JsonConvert.SerializeObject(GetDefaultIlsMappings());
                if (amenities != null)
                {
                    foreach (var amenity in amenities)
                    {
                        string sql = @"SELECT Id, Export, ExportPms, DisplayName
                                      FROM GlobalAmenityDefaults
                                      WHERE PropertyId = @propertyId AND Code = @code";
                        var originalAmenity = conn.QueryFirstOrDefault(sql, new { propertyId, code = amenity.AmenityCode });

                        var parameters = new
                        {
                            propertyId,
                            code = amenity.AmenityCode,
                            description = amenity.Description,
                            export = amenity.Display.Trim() == "Y",
                            exportPms = string.IsNullOrWhiteSpace(amenity.Display) ? false : (bool?)(amenity.Display.Trim() == "Y"),
                            displayName = amenity.DisplayName ?? string.Empty,
                            rankPms = amenity.DisplayOrder,
                            ilsMappingJson = defaultIlsMappingJson,
                            source = "MRI"
                        };

                        if (originalAmenity == null)
                        {
                            conn.Query(
                                @"INSERT INTO GlobalAmenityDefaults([Export], [ExportPms], [Code], [Description], [DisplayName], [DisplayNamePms], [Source], [Rank], [RankPms], [IlsMappingJson], [PropertyId])
                                                            VALUES(@export, @exportPms, @code, @description, @displayName, @displayName, @source, 
                                                     (SELECT ISNULL(MAX([Rank]) + 1, 1) FROM GlobalAmenityDefaults WHERE PropertyId = @propertyId), @rankPms, @ilsMappingJson, @propertyId)",
                                parameters);
                        }
                        else
                        {
                            conn.Query(
                                @"UPDATE GlobalAmenityDefaults SET
                                    Export = CASE WHEN Export = ExportPms THEN @export ELSE Export END,
                                    ExportPms = @exportPms,
                                    Description = @description,
                                    DisplayName = CASE WHEN DisplayName = DisplayNamePms THEN @displayName ELSE DisplayName END,
                                    DisplayNamePms = @displayName,
                                    RankPms = @rankPms
                                    WHERE Code = @code",
                                parameters);
                        }
                    }
                }
            }
        }

        public virtual void InsertOrUpdatePropertyAmenityInformation(ResultsInfo propertyResults, string propertyId)
        {
            var amenityInfo = propertyResults.Amenities ?? new ResultsInfoAmenity[0];
            //TODO: remove from repository
            //InsertOrUpdateGlobalAmenityInformation(amenityInfo, propertyId);

            using (IDbConnection conn = DefaultConnection)
            {
                conn.Open();
                string sql = @"SELECT pa.Id, 
                                            gad.Id AS GlobalId, 
                                            gad.Code
                                        FROM GlobalAmenityDefaults gad 
                                        LEFT JOIN PropertyAmenities pa ON pa.GlobalId = gad.Id
                                        WHERE gad.PropertyId = @propertyId
                                        AND gad.IsIndustryAmenity = 0
                                        AND gad.[Source] NOT IN ('Custom', 'Market Connect')
                                        ORDER BY pa.[Rank], gad.[Rank], gad.RankPMS, gad.[Description]";
                var currentAmenities = conn.Query(sql, new { propertyId });

                foreach (var amenity in currentAmenities.Where(c => c.Id != null && !amenityInfo.Any(a => a.AmenityCode == c.Code)))
                {
                    string deleteQuery = "DELETE FROM PropertyAmenities WHERE Id = @amenityId";
                    conn.Query(deleteQuery, new { amenityId = amenity.Get("Id") });
                }

                foreach (var amenity in amenityInfo)
                {
                    var currentAmenity = currentAmenities.First(c => c.Code == amenity.AmenityCode);
                    string mergeQuery = @"MERGE PropertyAmenities WITH (HOLDLOCK) AS pa
                                            USING (SELECT @amenityId AS Id) AS amenity
                                            ON pa.Id = amenity.Id
                                            WHEN NOT MATCHED THEN
                                                INSERT (GlobalId, Export, DisplayName, PropertyId, [Rank])
                                            VALUES(@globalId, @export, @displayName, @propertyId, 
                                                    (SELECT ISNULL(MAX([Rank])+1,1) FROM PropertyAmenities WHERE PropertyId = @propertyId));";

                    conn.Query(mergeQuery, new
                    {
                        amenityId = currentAmenity.Id,
                        export = amenity.Display.Trim() == "Y",
                        globalId = currentAmenity.GlobalId,
                        displayName = amenity.DisplayName ?? string.Empty,
                        propertyId
                    });
                }
            }
        }

        public virtual void InsertChargeCodes(ResultsInfoChargeCode[] chargeCodeResults, string propertyId, string clientId)
        {
            if (chargeCodeResults == null)
            {
                return;
            }
            foreach (var chargeCode in chargeCodeResults)
            {
                using (IDbConnection conn = DefaultConnection)
                {
                    conn.Open();
                    string insertQuery = "INSERT INTO ChargeCodes ([ClientId],[PropertyId],[ChargeCategory],[PMSChargeCode],[ChargeDescription]) " +
                                            "SELECT @clientId, @propId,'Application',@ChargeCode,@ChargeCodeDescription " +
                                            "WHERE NOT EXISTS(SELECT 1 FROM ChargeCodes WHERE PropertyId = @propId AND PMSChargeCode = @ChargeCode)";
                    conn.Query(insertQuery, new { clientId, propId = propertyId, ChargeCode = chargeCode.ChargeCode, ChargeCodeDescription = (object)chargeCode.ChargeDescription ?? DBNull.Value });
                }
            }
        }

        public virtual void InsertOrUpdateBuildingInformation(ResultsInfo propertyResults, string propertyId)
        {

            foreach (var result in propertyResults.Buildings ?? new ResultsInfoBuilding[0])
            {
                using (IDbConnection conn = DefaultConnection)
                {
                    string mergeQuery = @"MERGE Building WITH (HOLDLOCK) AS b
                                                USING (SELECT @buildingId as Id) AS c ON c.Id = b.Id AND b.PropertyId = @propertyId
                                                WHEN MATCHED THEN
                                                    UPDATE SET ShortDescriptionPms = @shortDescription
                                                WHEN NOT MATCHED THEN
                                                    INSERT (Id, PropertyId, ShortDescriptionPMS) VALUES(@buildingId, @propertyId, @shortDescription);
                                           SELECT AddressId FROM Building WHERE PropertyId = @propertyId AND Id = @buildingId";

                    var addressId = conn.Query<long>(mergeQuery, new { propertyId, buildingId = result.BuildingId, shortDescription = result.BuildingDescription }).FirstOrDefault();

                    if (addressId == 0)
                    {
                        addressId = InsertAddress(result.BuildingAddress1, result.BuildingAddress2, result.BuildingAddress3, result.BuildingCity, result.BuildingState, result.BuildingZip, result.BuildingCounty, result.BuildingCountry);
                    }
                    else
                    {
                        UpdateAddress(addressId, result.BuildingAddress1, result.BuildingAddress2, result.BuildingAddress3, result.BuildingCity, result.BuildingState, result.BuildingZip, result.BuildingCounty, result.BuildingCountry);
                    }

                    string updateQuery = @"UPDATE Building SET
                                            AddressId = @addressId
                                        WHERE PropertyId = @propertyId AND Id = @buildingId";

                    conn.Query(updateQuery, new { addressId, propertyId, buildingId = result.BuildingId });
                }
            }
        }

        private List<IlsMapItem> GetDefaultIlsMappings()
        {
            var ilsMappings = new List<IlsMapItem>();

            var mitsBasedIlsTypes = new string[] { "Irvine", "Mits 1.5", "Mits 1.5.1", "Mits 2.0", "Mits 2.0 - No Option" };

            var defaultMits20Key = "Other";
            var defaultMits20Value = "Other";

            foreach (var ilsType in mitsBasedIlsTypes)
            {
                ilsMappings.Add(new IlsMapItem
                {
                    IlsType = ilsType,
                    Key = defaultMits20Key,
                    Value = defaultMits20Value
                });
            }
            return ilsMappings;
        }
    }
}
