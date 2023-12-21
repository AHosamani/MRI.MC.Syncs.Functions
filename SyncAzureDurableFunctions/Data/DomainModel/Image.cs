namespace SyncAzureDurableFunctions.Data.DomainModel;

public class Image : IdentifiableContent {
  public string IsFloorPlan { get; set; }
  public string PropertyID { get; set; }
  public string FloorplanID { get; set; }
  public string BuildingID { get; set; }
  public string UnitID { get; set; }
  public string ExternalLink { get; set; }
}