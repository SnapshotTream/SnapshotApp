using System;
using System.Collections.Generic;
using NLog;
using RestSharp;
using Snapshot.Client.Bff.Mock.Data;
using Snapshot.Client.Bff.Sdk;
using Snapshot.Client.Bff.Sdk.Dao;
using Snapshot.Client.Bff.Sdk.Data.Dto;

namespace Snapshot.Client.Bff.Dao {
  /// <summary>
  /// サービスからコンテント情報の操作を行うDAOクラス
  /// </summary>
  public class ContentDao : DaoBase, IContentDao {

    readonly Logger mLogger;

    public ContentDao (DaoContext context) : base (context) {
      this.mLogger = LogManager.GetCurrentClassLogger ();
    }

    public byte[] LoadContentImage(long contentId,out string contentType) {
      var request = new RestRequest ("artifact/{id}/preview", Method.GET);
      request.AddUrlSegment ("id", contentId);
      var response = mClient.Execute (request);
      contentType = response.ContentType;
      return response.RawBytes;
    }

    /// <summary>
    /// コンテント情報を読み込みます
    /// </summary>
    /// <param name="contentId"></param>
    /// <returns></returns>
    public Content LoadContent (long contentId) {
      var request = new RestRequest ("artifact/{id}", Method.GET);
      request.AddUrlSegment ("id", contentId);

      var response = mClient.Execute<ServerResponseApi<Content>> (request);

      var content = response.Data.Value;

      content.LinkCategory = LinkGetCategory (content.Id, response.Data.Link);
      return content;
    }

    public void Update (Content content) {
      this.mLogger.Trace ("IN");
      try {
        var request = new RestRequest ("artifact/{id}", Method.PATCH);
        request.RequestFormat = DataFormat.Json;
        request.AddUrlSegment ("id", content.Id);
        request.AddJsonBody (content);
        var response = mClient.Execute<ServerResponseApi<Boolean>> (request);
      } catch (Exception expr) {
        mLogger.Error (expr);
      }
      this.mLogger.Trace ("OUT");
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="contentId"></param>
    public void UpdateRead (long contentId) {
      this.mLogger.Trace ("IN");
      var request = new RestRequest ("artifact/{id}/exec/read", Method.PUT);
      request.AddUrlSegment ("id", contentId);
      var response = mClient.Execute<ServerResponseApi<Boolean>> (request);
      this.mLogger.Trace ("OUT");
    }

    /// <summary>
    /// "artifact/{id}"のcategoryリンクデータを取得する
    /// </summary>
    /// <param name="contentId"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    private Category LinkGetCategory (long contentId, Dictionary<string, object> link) {
      // リンクデータが取得できない場合は、リンクデータのリクエストを実施しない
      if (!link.ContainsKey ("category")) return null;

      var request = new RestRequest ("artifact/{id}/category", Method.GET);
      request.AddUrlSegment ("id", contentId);
      var response = mClient.Execute<ServerResponseApi<Category>> (request);
      var category = response.Data.Value;
      category.Labels = response.Data.GetRelative<List<Label>> ("labels");
      return category;
    }

  }
}
