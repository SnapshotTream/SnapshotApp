using Snapshot.Client.Bff.Sdk.Data.Dto;

namespace Snapshot.Client.Bff.Sdk.Dao {
  public interface IContentDao {

    /// <summary>
    /// コンテントの画像ファイルのデータを取得します
    /// </summary>
    /// <param name="contentId">コンテントID</param>
    /// <param name="mimeType">データのMIMEタイプを出力します</param>
    /// <returns>画像ファイルのバイト列</returns>
    byte[] LoadContentImage(long contentId,out string mimeType);

    Content LoadContent (long contentId);

    /// <summary>
    /// コンテント情報を更新します
    /// /// </summary>
    /// <param name="content"></param>
    void Update (Content content);

    void UpdateRead (long contentId);
  }
}
