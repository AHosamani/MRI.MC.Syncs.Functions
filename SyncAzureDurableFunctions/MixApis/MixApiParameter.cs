namespace MRI.PandA.Syncs.Functions.MixApis;

public class MixApiParameter {
  public string Parameter { get; set; }
  public string Value { get; set; }

  public new string ToString() {
    return string.Format("{0}={1}", Parameter, Value);
  }
}