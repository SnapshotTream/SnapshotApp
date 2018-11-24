using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using RestSharp;
using Snapshot.Client.Bff.Mock.Data;
using Snapshot.Client.Bff.Sdk;
using Snapshot.Client.Bff.Sdk.Dao;
using Snapshot.Client.Bff.Sdk.Data.Dto;
using Snapshot.Share.Common.Infra.Translate;

namespace Snapshot.Client.Bff.Dao {
  public class LabelDao : DaoBase, ILabelDao {

    readonly Logger mLogger;

    public LabelDao (DaoContext context) : base (context) {
      this.mLogger = LogManager.GetCurrentClassLogger ();
    }

    public List<Label> LoadLabel () {
      var request = new RestRequest ("label", Method.GET);
      var response = mClient.Execute<ServerResponseApi<List<Label>>> (request);
      return response.Data.Value;
    }

    public Label LoadLabel (long labelId) {
      var request = new RestRequest ("label/{id}", Method.GET);
      request.AddUrlSegment ("id", labelId);

      var response = mClient.Execute<ServerResponseApi<Label>> (request);
      var label = response.Data.Value;

      label.LinkSubLabelList = LinkGetSubLabel (labelId, 0, 100000, response);
      label.LinkCategoryList = LinkGetCategory (labelId);
      label.LinkContentList = LinkGetContentList (labelId, 0, response);

      return label;
    }

    public List<Label> LoadRoot () {
      var request = new RestRequest ("label/{id}/l/children/", Method.GET);
      request.AddUrlSegment ("id", 0);
      var response = mClient.Execute<ServerResponseApi<List<Label>>> (request);
      mLogger.Info ($"ErrorMessage={@response.ErrorMessage}");
      return response.Data.Value;
    }

    public List<Category> LoadLabelLinkCategory (string query, int offset, int limit) {
      var request = new RestRequest ("label/{query}/category", Method.GET);
      request.AddUrlSegment ("query", query);
      //request.AddQueryParameter("offset", offset.ToString()); 実装したら使用する

      var response = mClient.Execute<ServerResponseApi<List<Category>>> (request);
      return response.Data.Value;
    }

    private List<Category> LinkGetCategory (long labelId) {
      var categoryList = new List<Category> ();
      var request_link_category_list = new RestRequest ("label/{id}/l/category-list", Method.GET);
      request_link_category_list.AddUrlSegment ("id", labelId);

      var response_link_category_list = mClient.Execute<ServerResponseApi<List<Category>>> (request_link_category_list);
      if (response_link_category_list.IsSuccessful) {
        foreach (var category in response_link_category_list.Data.Value) {
          categoryList.Add (category);
        }
      }

      return categoryList;
    }

    private List<Content> LinkGetContentList (long labelId, long offset, IRestResponse<ServerResponseApi<Label>> response) {
      var contentList = new List<Content> ();
      var request_link_contentList = new RestRequest ("label/{id}/l/content-list", Method.GET);
      request_link_contentList.AddUrlSegment ("id", labelId);

      var response_link_contentList = mClient.Execute<ResponseApi<List<Content>>> (request_link_contentList);
      if (response_link_contentList.IsSuccessful) {
        foreach (var content in response_link_contentList.Data.Value) {
          contentList.Add (content);
        }
      }

      return contentList;
    }

    private List<Label> LinkGetSubLabel (long labelId, int offset, int limit, IRestResponse<ServerResponseApi<Label>> response) {
      // リンク情報から、カテゴリ情報を取得する
      List<Label> labelList = new List<Label> ();
      var link_la = response.Data.Link["children"] as List<object>;
      foreach (var linkedCategoryId in link_la.Skip (offset).Select (p => (long) p).Take (limit)) {
        labelList.Add (LoadLinkedLabel (labelId, linkedCategoryId));
      }

      return labelList;
    }

    /// <summary>
    /// 任意ラベルのサブラベル情報を取得します
    /// </summary>
    /// <param name="labelId"></param>
    /// <param name="linkedLabelId"></param>
    /// <returns></returns>
    public Label LoadLinkedLabel (long labelId, long linkedLabelId) {
      var request_link_la = new RestRequest ("label/{id}/children/{label_id}", Method.GET);
      request_link_la.AddUrlSegment ("id", labelId);
      request_link_la.AddUrlSegment ("label_id", linkedLabelId);
      //request_link_la.AddQueryParameter("offset", offset.ToString());

      var response_link_la = mClient.Execute<ServerResponseApi<Label>> (request_link_la);
      if (!response_link_la.IsSuccessful)
        throw new ApplicationException ("DAOの実行に失敗しました");

      var linked_label = response_link_la.Data.Value;

      if (response_link_la.Data.Link.ContainsKey ("cc_available")) {
        var ccAvailable = response_link_la.Data.Link["cc_available"];
        if (Boolean.TrueString == ccAvailable.ToString ()) {
          linked_label.HasLinkSubCategoryFlag = true;
        }
      }

      return linked_label;
    }
  }
}
