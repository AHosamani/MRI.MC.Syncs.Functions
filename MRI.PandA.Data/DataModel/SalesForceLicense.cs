using System;

namespace MRI.PandA.Data.DataModel;

public class SalesForceLicense {
    public int Id { get; set; }
    public string ExternalId { get; set; }
    public string PropertyId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
    public string AssetId { get; set; }
    public string Source { get; set; }
}