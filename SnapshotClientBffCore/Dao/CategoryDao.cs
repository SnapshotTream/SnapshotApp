using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using Snapshot.Client.Bff.Mock.Data;
using Snapshot.Client.Bff.Sdk;
using Snapshot.Client.Bff.Sdk.Dao;
using Snapshot.Client.Bff.Sdk.Data.Dto;
using Snapshot.Server.Service.Sdk.Data;
using Snapshot.Share.Common.Infra.Translate;

namespace Snapshot.Client.Bff.Dao {
  /// <summary>
  ///
  /// </summary>
  public class CategoryDao : DaoBase, ICategoryDao {
    public const int MAXLIMIT = 1000000;

    readonly Logger mLogger;

    public CategoryDao (DaoContext context) : base (context) {
      this.mLogger = LogManager.GetCurrentClassLogger ();
    }

    public Category LoadCategory (long categoryId, int offsetSubCategory = 0, int limitSubCategory = MAXLIMIT, int offsetContent = 0) {
      if (limitSubCategory == -1) limitSubCategory = MAXLIMIT;

      try {
        var request = new RestRequest ("category/{id}", Method.GET);
        request.AddUrlSegment ("id", categoryId);
        request.AddQueryParameter ("lla_order", "NAME_ASC");

        var response = mClient.Execute<ServerResponseApi<Category>> (request);
        if (!response.IsSuccessful) {
          mLogger.Error ("サーバーAPIの実行に失敗しました。{0}", response.ErrorMessage);
          return null;
        }

        var category = response.Data.Value;

        //関連データ
        category.Labels = response.Data.GetRelative<List<Label>>("labels");

        // リンクデータ
        category.LinkSubCategoryList = LinkGetSubCategory (categoryId, offsetSubCategory, limitSubCategory, response);
        category.LinkContentList = LinkGetContentList (categoryId, offsetContent, response);
        return category;
      } catch (Exception expr) {
        this.mLogger.Error (expr, "APIの実行に失敗しました");
      }
      return new Category ();
    }

    public Category LoadParentCategory (long categoryId) {
      var request = new RestRequest ("category/{id}/pc", Method.GET);
      request.AddUrlSegment ("id", categoryId);
      var response = mClient.Execute<ServerResponseApi<List<Category>>> (request);
      if (!response.IsSuccessful) {
        return null;
      }

      return response.Data.Value.FirstOrDefault ();
    }

    public Category UpdateCategoryArtwork (long categoryId) {
      var request = new RestRequest ("category/{id}/thumbnail", Method.PATCH);
      request.AddUrlSegment ("id", categoryId);
      request.AddQueryParameter ("mode", ModeType.AUTO_CONTENT.ToString ());

      var response = mClient.Execute<ServerResponseApi<Category>> (request);
      if (!response.IsSuccessful) {
        return null;
      }

      ApplyCategoryArtworkUrl (response.Data.Value);

      return response.Data.Value;
    }

    private List<Content> LinkGetContentList (long categoryId, long offset, IRestResponse<ServerResponseApi<Category>> response) {
      // リンク情報から、コンテント情報を取得する
      var contentList = new List<Content> ();

      var request_link_la = new RestRequest ("category/{id}/la", Method.GET);
      request_link_la.AddUrlSegment ("id", categoryId);

      var response_link_la = mClient.Execute<ResponseApi<List<Content>>> (request_link_la);
      if (response_link_la.IsSuccessful) {
        foreach (var content in response_link_la.Data.Value) {
          contentList.Add (content);
        }
      }

      return contentList;
    }

    private List<Category> LinkGetSubCategory (long categoryId, int offset, int limit, IRestResponse<ServerResponseApi<Category>> response) {
      // リンク情報から、カテゴリ情報を取得する
      List<Category> categoryList = new List<Category> ();
      var link_la = response.Data.Link["cc"] as List<object>;
      foreach (var linkedCategoryId in link_la.Skip (offset).Select (p => (long) p).Take (limit)) {
        categoryList.Add (LoadLinkedCategory (categoryId, linkedCategoryId));
      }

      return categoryList;
    }

    /// <summary>
    /// カテゴリ情報更新API
    /// </summary>
    /// <param name="categoryId">更新対象のカテゴリのキー</param>
    /// <param name="category">更新データ(更新するプロパティのみ含んだオブジェクト)</param>
    internal void Update (long categoryId, object category) {
      var request = new RestRequest ("category/{id}", Method.PUT);
      request.AddUrlSegment ("id", categoryId);
      //var s = JsonConvert.SerializeObject (category);
      request.AddJsonBody (JsonConvert.SerializeObject (category));

      var response = mClient.Execute (request);
      if (!response.IsSuccessful) {
        throw new ApplicationException ("DAOの実行に失敗しました");
      }
    }

    /// <summary>
    /// カテゴリ表示情報更新API
    /// </summary>
    /// <param name="categoryId"></param>
    internal void UpdateReading (long categoryId) {
      var request = new RestRequest ("category/{id}/read", Method.PUT);
      request.AddUrlSegment ("id", categoryId);

      var response = mClient.Execute (request);
      if (!response.IsSuccessful) {
        throw new ApplicationException ("DAOの実行に失敗しました");
      }
    }

    /// <summary>
    /// 任意のカテゴリのサブカテゴリ情報を取得します
    /// </summary>
    /// <param name="categoryId"></param>
    /// <param name="linkedCategoryId"></param>
    /// <returns></returns>
    public Category LoadLinkedCategory (long categoryId, long linkedCategoryId) {
      var request_link_la = new RestRequest ("category/{id}/cc/{category_id}", Method.GET);
      request_link_la.AddUrlSegment ("id", categoryId);
      request_link_la.AddUrlSegment ("category_id", linkedCategoryId);
      //request_link_la.AddQueryParameter("offset", offset.ToString());

      var response_link_la = mClient.Execute<ServerResponseApi<Category>> (request_link_la);
      if (!response_link_la.IsSuccessful)
        throw new ApplicationException ("DAOの実行に失敗しました");

      var linked_category = response_link_la.Data.Value;

      if (response_link_la.Data.Link.ContainsKey ("cc_available")) {
        var ccAvailable = response_link_la.Data.Link["cc_available"];
        if (Boolean.TrueString == ccAvailable.ToString ()) {
          linked_category.HasLinkSubCategoryFlag = true;
        }
      }

      return linked_category;
    }

    public List<Category> FindCategory (bool? albumCategory, long[] labelId) {
      if (!albumCategory.HasValue) // TODO: 現在はアルバムカテゴリのみ取得可能とする
        throw new NotSupportedException ("アルバムカテゴリのみ条件に含めることができます。");
      else if (albumCategory.Value != true) // TODO: 現在はアルバムカテゴリは必須とする
        throw new NotSupportedException ("アルバムカテゴリのみ条件に含めることができます。");
      if (labelId == null) // TODO: 現在はラベルの条件指定は必須です。
        throw new NotSupportedException ("ラベル一覧の指定は必須です。");

      var request = new RestRequest ("category/w_album/{labels_cond}", Method.GET);
      request.AddUrlSegment ("labels_cond", "+" + string.Join (",", labelId)); // ANDのみサポート

      var response = mClient.Execute<ServerResponseApi<List<Category>>> (request);
      if (!response.IsSuccessful)
        throw new ApplicationException ("DAOの実行に失敗しました");

      response.Data.Value.ForEach (prop => ApplyCategoryArtworkUrl (prop));

      return response.Data.Value;
    }
    private void ApplyCategoryArtworkUrl (Category category) {
      // EMPTY
    }

  }
}
