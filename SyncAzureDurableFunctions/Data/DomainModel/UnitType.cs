using System.Collections.Generic;

namespace SyncAzureDurableFunctions.Data.DomainModel;

public class UnitType : IdentifiableContent {
  public string Name { get; set; }
  public string Description { get; set; }
  public decimal Bedrooms { get; set; }
  public decimal Bathrooms { get; set; }
  public decimal SecurityDeposit { get; set; }
  public string SquareUnit { get; set; }
  public decimal SquareMeasure { get; set; }
  public long? FloorplanId { get; set; }
  public List<Unit> Units { get; set; }
  public List<Amenity> Amenities { get; set; }
}