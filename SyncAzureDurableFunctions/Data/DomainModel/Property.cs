using SyncAzureDurableFunctions.DataAccess;
using System.Collections.Generic;

namespace SyncAzureDurableFunctions.Data.DomainModel;

public class Property : IdentifiableContent {
  public bool SingleFamilyProperty { get; set; }
  public string Name { get; set; }
  public string ShortDescription { get; set; }
  public string LongDescription { get; set; }
  public string CurrencyCode { get; set; }
  public Address Address { get; set; }
  public decimal SecurityDeposit { get; set; }
  public ICollection<UnitType> UnitTypes { get; set; }
  public ICollection<Property> PhaseProperties { get; set; }
  public int? YearBuilt { get; set; }
  // public List<Policy> Policies { get; set; }
  public string PropertyType { get; set; }
  public ICollection<Building> Buildings { get; set; }
   public List<Image> Media { get; set; }
  public ICollection<Amenity> Amenities { get; set; }
  public ICollection<UnitTypeRent> UnitTypeRents { get; set; }
  public HashSet<string> OnlineUnitIds { get; set; }
  public int PmsId { get; set; }
  public IRow ExtraData { get; set; }

  public override bool Equals(object obj) => base.Equals(obj as Property);

  public override int GetHashCode() => base.GetHashCode();
}