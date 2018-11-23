namespace Snapshot.Share.Common.Infra.Data.EventData {
  public class CreateEntityInfo {
    public string Name { get; set; }

    /// <summary>
    /// エンティティの種類
    /// /// </summary>
    /// <value></value>
    public string EntityName { get; set; }

    /// <summary>
    /// エンティティの主キー
    /// </summary>
    /// <value></value>
    public long EntityId { get; set; }
  }
}
