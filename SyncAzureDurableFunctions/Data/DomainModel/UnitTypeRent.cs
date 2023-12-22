using System;
using System.Collections.Generic;

namespace SyncAzureDurableFunctions.Data.DomainModel;

public class UnitTypeRent : IdentifiableContent {
  public string UnitTypeId { get; set; }
  public string Term { get; set; }
  public decimal Amount { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
}