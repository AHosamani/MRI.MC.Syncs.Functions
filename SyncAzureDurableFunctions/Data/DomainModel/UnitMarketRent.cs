namespace SyncAzureDurableFunctions.Data.DomainModel;

public class UnitMarketRent : IdentifiableContent {
  public string PropertyID { get; set; }
  public string FloorplanID { get; set; }
  public string BuildingID { get; set; }
  public string UnitID { get; set; }
  public string Term { get; set; }
  public string StartDate { get; set; }
  public string EndDate { get; set; }
  public string UnitRent { get; set; }
}