using SyncAzureDurableFunctions.DataAccess;
using System.Collections.Generic;

namespace SyncAzureDurableFunctions.Data.DomainModel
{
    public class Amenity : IdentifiableContent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rank { get; set; }
        public decimal Amount { get; set; }
        public string Code { get; set; }
        public ICollection<IlsMapItem> IlsMappings { get; set; }
        public IRow ExtraData { get; set; }
        public override bool Equals(object obj) => base.Equals(obj as Amenity);

        public override int GetHashCode() => base.GetHashCode();
    }
}