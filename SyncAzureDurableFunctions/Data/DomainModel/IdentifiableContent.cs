namespace SyncAzureDurableFunctions.Data.DomainModel;

public abstract class IdentifiableContent {
  public string Id { get; set; }
  public string ReferenceId { get; set; }

  public override bool Equals(object obj) {
    return obj is IdentifiableContent idObj && string.Equals(idObj.Id, Id) && string.Equals(idObj.ReferenceId, ReferenceId);
  }

  public override int GetHashCode() {
    return (new { Id, ReferenceId }).GetHashCode();
  }
}