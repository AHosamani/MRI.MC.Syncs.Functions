namespace MRI.PandA.Data.DataModel;

public class Property {
  public string Id { get; set; }
  public string Name { get; set; }
  public string NamePMS { get; set; }
  public string PropertyType { get; set; }
  public string Email { get; set; }
  public string Phone { get; set; }
  public string PhonePMS { get; set; }
  public string EmergencyPhone { get; set; }
  public string Fax { get; set; }
  public string PropertyWebsite { get; set; }
  public string ManagementWebsite { get; set; }
  public int? YearBuilt { get; set; }
  public int? YearRemodeled { get; set; }
  public string ClientId { get; set; }
  public string ShortDescription { get; set; }
  public string ShortDescriptionPMS { get; set; }
  public string LongDescription { get; set; }
  public string LongDescriptionPMS { get; set; }
  public string LeaseTermsDescription { get; set; }
  public string RefId { get; set; }
  public int MCRefId { get; set; }
  public long? AddressId { get; set; }
  public string LocationDescription { get; set; }
  public string DirectionsDescription { get; set; }
  public bool IsActive { get; set; }
  public int? DimensionUnitId { get; set; }
  public string Location { get; set; }
  public string CurrencyCode { get; set; }
  public string WebFeatures { get; set; }
  public string GeneralPolicy { get; set; }
  public decimal SecurityDeposit { get; set; }
  public long? DataFeedSourceSelectionId { get; set; }
  public string CDNUserName { get; set; }
  public string CDNPassword { get; set; }
  public bool IsSingleFamily { get; set; }
  public string Suburb { get; set; }
  public short? FeeTerms { get; set; }
  public short? RentFrequency { get; set; }
}