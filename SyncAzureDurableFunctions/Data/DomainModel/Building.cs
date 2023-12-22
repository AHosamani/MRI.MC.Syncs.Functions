namespace SyncAzureDurableFunctions.Data.DomainModel
{

    public class Building : IdentifiableContent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Address Address { get; set; }
        public int UnitCount { get; set; }
        public string EntryFloor { get; set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj as Building);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}