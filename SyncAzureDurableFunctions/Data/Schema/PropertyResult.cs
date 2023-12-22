using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static SyncAzureDurableFunctions.Data.Schema.ResultInfoAmenity;

namespace SyncAzureDurableFunctions.Data.Schema
{
    public class PropertyResult
    {

        [Serializable]
        public partial class Results
        {
            [XmlElement("Info")]
            public ResultsInfo[] Items;
        }

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
    }
}
