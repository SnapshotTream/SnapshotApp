using Snapshot.Client.Bff.Sdk.Data.Dto;

namespace Snapshot.Client.Bff.Sdk.Dao {
  public interface IContentDao {
    Content LoadContent (long contentId);

    /// <summary>
    /// コンテント情報を更新します
    /// /// </summary>
    /// <param name="content"></param>
    void Update (Content content);

    void UpdateRead (long contentId);
  }
}
