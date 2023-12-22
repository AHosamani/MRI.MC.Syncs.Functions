using System;

namespace SyncAzureDurableFunctions.Data.DomainModel;

public class UnitAvailability : IdentifiableContent {
  public string PropertyID { get; set; }
  public string FloorplanID { get; set; }
  public string BuildingID { get; set; }
  public string UnitID { get; set; }
  public string IsAvailableNow { get; set; }
  public DateTime? MakeReadyDate { get; set; }
  public DateTime? VacateDate { get; set; }
  public string AmenityTotal { get; set; }
  public int VacancyClass { get; set; }
}