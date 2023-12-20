namespace MRI.PandA.Data.DataModel;

public class Client {
    public string Id { get; set; }
    public string Name { get; set; }
    public int DataFeedSourceSelectionId { get; set; }
    public string CDNUserName { get; set; }
    public string CDNPassword { get; set; }
}