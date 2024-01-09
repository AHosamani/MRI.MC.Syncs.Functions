using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MarketConnect.Data.Models
{
    public partial class ResultsInfo
    {
        public string RmPropId;
        public string PropertyName;
        public string Address1;
        public string Address2;
        public string Address3;
        public string City;
        public string State;
        public string Zip;
        public string ManagerName;
        public string ManagerPhone;
        public string Country;
        public string County;
        [XmlArrayItem("Building", typeof(ResultsInfoBuilding))]
        public ResultsInfoBuilding[] Buildings;
        [XmlArrayItem("Amenity", typeof(ResultsInfoAmenity))]
        public ResultsInfoAmenity[] Amenities;
    }

    public partial class ResultsInfoBuilding
    {
        public string BuildingId;
        public string BuildingAddress1;
        public string BuildingAddress2;
        public string BuildingAddress3;
        public string BuildingCity;
        public string BuildingCounty;
        public string BuildingCountry;
        public string BuildingDescription;
        public string BuildingState;
        public string BuildingZip;
    }

    public partial class ResultsInfoAmenity
    {
        public string AmenityCode;
        public string ChargeCode;
        public string GlobalChargeCode;
        public string Display;
        public int DisplayOrder;
        public string LastUpdate;
        public string Amount;
        public string GlobalAmount;
        public string Description;
        public string DisplayName;
    }

    public partial class ResultsInfoChargeCode
    {
        public string ChargeCode;
        public string ChargeDescription;
    }
    public class IlsMapItem
    {
        public string IlsType { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as IlsMapItem;
            return item.IlsType == IlsType && item.Key == Key;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}
