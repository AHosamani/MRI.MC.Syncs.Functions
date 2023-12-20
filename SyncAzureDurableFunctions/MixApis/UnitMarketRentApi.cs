namespace MRI.PandA.Syncs.Functions.MixApis;

public class UnitMarketRentApi : MixApi {
  private const string PROPERTY_PARAMETER = "PROPERTYID";
  public UnitMarketRentApi(string property) : base(property) {
    Name = "MRI_S-PMRM_UnitMarketRent";
    Parameters.Find(p => p.Parameter == (DEFAULT_PROPERTY_PARAMETER)).Parameter = PROPERTY_PARAMETER;
  }
}