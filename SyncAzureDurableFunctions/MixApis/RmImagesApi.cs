namespace MRI.PandA.Syncs.Functions.MixApis;

public class RmImagesApi : MixApi {
  private const string PROPERTY_PARAMETER = "PROPERTYID";
  public RmImagesApi(string property) : base(property) {
    Name = "MRI_S-PMRM_GetRmImages";
    Parameters.Find(p => p.Parameter == (DEFAULT_PROPERTY_PARAMETER)).Parameter = PROPERTY_PARAMETER;
  }
}