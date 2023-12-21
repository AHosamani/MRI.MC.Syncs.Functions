using System.Collections.Generic;

namespace SyncAzureDurableFunctions.Data.DomainModel
{
    public class Address
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Display { get; set; }
        public ICollection<string> AddressLines { get; set; }
        public string Town { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string SecondPostalCode { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public string Directions { get; set; }
    }
}
