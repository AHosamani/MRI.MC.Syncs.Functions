namespace MRI.PandA.Data.DataModel;

public class PropertyPreferences {
    public int Id { get; set; }
    public string PropertyId { get; set; }
    public bool UseGlobalAmenityRanks { get; set; }
    public bool UsePmsRanks { get; set; }
    public bool UsePCDisplay { get; set; }
    public bool SyncAmenitiesFromPms { get; set; }
    public bool SyncMediaFromPms { get; set; }
}