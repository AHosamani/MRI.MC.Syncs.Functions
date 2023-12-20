namespace MRI.PandA.Syncs.Functions.MixApis;

public class MarketConnectModelRentOffers : MixApi {
  private const string PROPERTY_PARAMETER = "PROPERTYID";
  public MarketConnectModelRentOffers(string property) : base(property) {
    Name = "MRI_S-PMRM_MarketConnectModelRentOffers";
    Parameters.Find(p => p.Parameter == (DEFAULT_PROPERTY_PARAMETER)).Parameter = PROPERTY_PARAMETER;
  }
}
