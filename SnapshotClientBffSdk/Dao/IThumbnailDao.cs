namespace Snapshot.Client.Bff.Sdk.Dao {
  public interface IThumbnailDao {
    /// <summary>
    /// サムネイル画像を取得します
    /// </summary>
    /// <param name="thumbnailKey">サムネイルのキー</param>
    /// <returns>サムネイル画像のバイト列</returns>
    byte[] Thumbnail (string thumbnailKey);
  }
}
