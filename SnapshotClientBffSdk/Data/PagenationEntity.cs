namespace Snapshot.Client.Bff.Sdk.Data {

  /// <summary>
  /// ページングデータ
  /// </summary>
  public class PagenationEntity {
    /// <summary>
    /// 要素の総数
    /// </summary>
    /// <value></value>
    public long Total { get; set; }

    /// <summary>
    /// ページ数
    /// </summary>
    /// <value></value>
    public long Page { get; set; }

    /// <summary>
    /// 1ページあたりの要素数
    /// </summary>
    /// <value></value>
    public int WindowSize { get; set; }
  }
}
