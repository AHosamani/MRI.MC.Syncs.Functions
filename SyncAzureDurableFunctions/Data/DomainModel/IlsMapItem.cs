namespace SyncAzureDurableFunctions.Data.DomainModel
{

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