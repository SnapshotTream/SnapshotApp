using RestSharp;
using Snapshot.Client.Bff.Dao;
using Snapshot.Client.Bff.Mock.Data;
using Snapshot.Client.Bff.Sdk.Dao;

namespace Snapshot.Client.Bff.Core.Dao {
  public class ThumbnailDao : DaoBase, IThumbnailDao {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public ThumbnailDao (DaoContext context) : base (context) { }

    public byte[] Thumbnail (string thumbnailKey) {
      var request = new RestRequest ("thumbnail/{key}", Method.GET);
      request.AddUrlSegment ("key", thumbnailKey);

      var response = mClient.Execute (request);
      return response.RawBytes;
    }
  }
}
