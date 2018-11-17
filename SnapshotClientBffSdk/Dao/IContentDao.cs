using Snapshot.Client.Bff.Sdk.Data.Dto;

namespace Snapshot.Client.Bff.Sdk.Dao {
  public interface IContentDao {
    Content LoadContent (long contentId);

    void Update (Content content);

    void UpdateRead (long contentId);
  }
}