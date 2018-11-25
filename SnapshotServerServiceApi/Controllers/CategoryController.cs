using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using Snapshot.Server.Extention.Sdk;
using Snapshot.Server.Service.Apo.Builder;
using Snapshot.Server.Service.Infra.Core;
using Snapshot.Server.Service.Infra.Exception;
using Snapshot.Server.Service.Infra.Model;
using Snapshot.Server.Service.Infra.Repository;
using Snapshot.Server.Service.Model;
using Snapshot.Server.Service.Sdk.Data;
using Snapshot.Share.Common.Collections;

namespace Foxpict.Service.Web.Controllers {
  /// <summary>
  /// カテゴリ操作コントローラ
  /// </summary>
  [Produces ("application/json")]
  [Route ("api/[controller]")]
  [ApiController]
  public class CategoryController : Controller {
    private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger ();

    private readonly ApiResponseBuilder mBuilder;

    private readonly ICategoryRepository mCategoryRepository;

    private readonly IContentRepository mContentRepository;

    private readonly ExtentionManager mExtentionManager;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="extentionManager"></param>
    /// <param name="categoryRepository"></param>
    /// <param name="contentRepository"></param>
    public CategoryController (ApiResponseBuilder builder, ExtentionManager extentionManager, ICategoryRepository categoryRepository, IContentRepository contentRepository) {
      this.mBuilder = builder;
      this.mCategoryRepository = categoryRepository;
      this.mExtentionManager = extentionManager;
      this.mContentRepository = contentRepository;
    }

    /// <summary>
    /// カテゴリのアートワークを設定します。
    /// </summary>
    /// <param name="category_id">カテゴリID</param>
    /// <param name="queryParam">クエリーパラメータ</param>
    /// <returns></returns>
    [HttpPatch ("{category_id}/thumbnail")]
    [ProducesResponseType (200)]
    public ActionResult<ResponseAapi<ICategory>> UpdateCategoryArtwork (long category_id, [FromQuery] UpdateCategoryArtworkParam queryParam) {
      if (queryParam == null) {
        throw new ApplicationException ("クエリーパラメータは必須です。");
      }

      var category = mCategoryRepository.Load (category_id);
      if (queryParam.Mode == ModeType.AUTO_CONTENT) {
        if (category.GetContentList ().Count > 0) {
          // コンテントのNameを自然順に並び替えて、最初の要素からアートワーク用のサムネイル画像を取得する。

          var contentList = category.GetContentList ().ToArray ();
          Array.Sort (contentList, new ContentNaturalComparer ());

          var content = contentList[0];
          if (!string.IsNullOrEmpty (content.ThumbnailKey)) {
            category.ArtworkThumbnailKey = content.ThumbnailKey;
            mCategoryRepository.Save ();
          } else {
            LOGGER.Warn ("コンテント(ID:{0})にサムネイルキーが設定されていないため、カテゴリにアートワークを設定できませんでした。", content.Id);
          }
        }
      } else {
        LOGGER.Warn ("不明なモード({0})のため、アートワークの設定を行いませんでした。", queryParam.Mode);
      }

      var response = new ResponseAapi<ICategory> ();
      mBuilder.AttachCategoryEntity (category, response);
      return response;
    }

    /// <summary>
    /// カテゴリの既読状態を更新します
    /// </summary>
    /// <param name="category_id">カテゴリID</param>
    /// <returns></returns>
    [HttpPatch ("{category_id}/readable")]
    [ProducesResponseType (200)]
    public ResponseAapi<Boolean> UpdateReadableStatus (long category_id) {
      var category = mCategoryRepository.Load (category_id);
      if (category == null) throw new InterfaceOperationException ("カテゴリ情報が見つかりません。");

      if (!category.ReadableFlag) {
        category.ReadableFlag = true;
        category.ReadableDate = DateTime.Now;
      }
      category.LastReadDate = DateTime.Now;
      category.ReadableCount = category.ReadableCount + 1;

      mCategoryRepository.Save ();

      var response = new ResponseAapi<Boolean> ();
      response.Value = true;
      return response;
    }

    /// <summary>
    /// 任意のカテゴリを取得します
    /// </summary>
    /// <param name="id">カテゴリID</param>
    /// <param name="param"></param>
    [HttpGet ("{id}")]
    [ProducesResponseType (200)]
    public ActionResult<ResponseAapi<ICategory>> GetCategory (long id, [FromQuery] CategoryParam param) {
      var response = new ResponseAapi<ICategory> ();

      var category = mCategoryRepository.Load (id);
      mBuilder.AttachCategoryEntity (category, response);

      // リンクデータの生成
      // "la"
      if (param.lla_order == CategoryParam.LLA_ORDER_NAME_ASC) {
        response.Link.Add ("la", category.GetContentList ().OrderBy (prop => prop.Name).Select (prop => prop.Id).ToArray ());
      } else if (param.lla_order == CategoryParam.LLA_ORDER_NAME_DESC) {
        response.Link.Add ("la", category.GetContentList ().OrderByDescending (prop => prop.Name).Select (prop => prop.Id).ToArray ());
      } else {
        response.Link.Add ("la", category.GetContentList ().Select (prop => prop.Id).ToArray ());
      }

      // "cc"
      var ccQuery = this.mCategoryRepository.FindChildren (category);
      response.Link.Add ("cc", ccQuery.Select (prop => prop.Id).ToArray ());

      // 拡張機能の呼び出し
      this.mExtentionManager.Execute (ExtentionCutpointType.API_GET_CATEGORY, category);

      return response;
    }

    /// <summary>
    /// カテゴリの親階層カテゴリを取得します
    /// </summary>
    /// <param name="id">カテゴリID</param>
    /// <returns></returns>
    /// <response code="200">カテゴリと関連付けられた親階層カテゴリを取得しました</response>
    /// <response code="400">指定した項目が取得できませんでした</response>
    [HttpGet ("{id}/pc")]
    [ProducesResponseType (200)]
    [ProducesResponseType (400)]
    public ActionResult<ResponseAapi<ICategory>> GetCategoryLink_pc (long id) {
      var response = new ResponseAapi<ICategory> {
        Value = GetCategoryLink (id, "pc").FirstOrDefault ()
      };

      if (response.Value == null) {
        return NotFound ();
      }

      return response;
    }

    /// <summary>
    /// カテゴリに含まれる子階層カテゴリ一覧を取得します
    /// </summary>
    /// <param name="id">カテゴリID</param>
    /// <response code="200">カテゴリと関連付けられた子階層カテゴリ一覧を取得しました</response>
    [HttpGet ("{id}/cc")]
    [ProducesResponseType (200)]
    public ActionResult<ResponseAapi<ICollection<ICategory>>> GetCategoryLink_cc (long id) {
      var response = new ResponseAapi<ICollection<ICategory>> {
        Value = GetCategoryLink (id, "cc")
      };

      return response;
    }

    /// <summary>
    /// カテゴリに含まれるコンテント一覧を取得します
    /// </summary>
    /// <param name="id">カテゴリID</param>
    /// <response code="200">カテゴリと関連付けられたコンテント一覧を取得しました</response>
    [HttpGet ("{id}/la")]
    [ProducesResponseType (200)]
    public ActionResult<ResponseAapi<ICollection<IContent>>> GetCategoryLink_la (long id) {
      LOGGER.Info ("REQUEST - {0}", id);

      var categoryList = new List<IContent> ();
      var response = new ResponseAapi<ICollection<IContent>> ();

      var category = this.mCategoryRepository.Load (id);
      categoryList.AddRange (
        category.GetContentList ().OrderBy (prop => prop.Name).Select (prop => prop).Take (100000)
      );
      response.Value = categoryList;
      return response;
    }

    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    ///    GET api/category/{id}/albc/{link_id}
    /// </remarks>
    /// <param name="id">カテゴリID</param>
    /// <param name="link_id"></param>
    /// <returns></returns>
    [HttpGet ("{id}/albc/{link_id}")]
    public ResponseAapi<Category> GetCategoryLink_albc (long id, long link_id) {
      LOGGER.Info ("REQUEST - {0}/albc/{1}", id, link_id);

      var response = new ResponseAapi<Category> ();
      response.Value = new Category { Id = link_id, Name = "リンクカテゴリ " + link_id };
      return response;
    }

    /// <summary>
    /// カテゴリと関連付けされた子階層カテゴリを取得します
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="id"></param>
    /// <param name="link_id"></param>
    /// <response code="200">カテゴリと関連付けられた子階層カテゴリを取得しました</response>
    /// <response code="400">指定した項目が取得できませんでした</response>
    [HttpGet ("{id}/cc/{link_id}")]
    [ProducesResponseType (200)]
    [ProducesResponseType (400)]
    public ActionResult<ResponseAapi<ICategory>> GetCategoryLink_cc (long id, long link_id) {
      LOGGER.Info ("REQUEST - {0}/cc/{1}", id, link_id);
      var response = new ResponseAapi<ICategory> ();

      var linkedCategory = this.mCategoryRepository.FindChildren (id).Where (prop => prop.Id == link_id).SingleOrDefault ();
      if (linkedCategory != null) {
        var linkCategory = mCategoryRepository.Load (link_id);
        mBuilder.AttachCategoryEntity (linkCategory, response);

        var sub = this.mCategoryRepository.FindChildren (linkedCategory).FirstOrDefault ();
        if (sub != null) {
          response.Link.Add ("cc_available", true);
        }
      } else {
        return NotFound ();
      }

      return response;
    }

    /// <summary>
    /// カテゴリと関連付けされたコンテントを取得します
    /// </summary>
    /// <remarks>
    ///    コンテント情報取得と同じ情報量を返します。
    /// </remarks>
    /// <param name="id"></param>
    /// <param name="link_id"></param>
    /// <response code="200">カテゴリと関連付けられたコンテントを取得しました</response>
    /// <response code="400">指定した項目が取得できませんでした</response>
    [HttpGet ("{id}/la/{link_id}")]
    [ProducesResponseType (200)]
    [ProducesResponseType (400)]
    public ActionResult<ResponseAapi<IContent>> GetCategoryLink_la (long id, long link_id) {
      LOGGER.Info ("REQUEST - {0}/la/{1}", id, link_id);

      var response = new ResponseAapi<IContent> ();
      var linkedContent = this.mCategoryRepository.Load (id).GetContentList ().Where (prop => prop.Id == link_id).SingleOrDefault ();

      if (linkedContent != null) {
        mBuilder.AttachContentEntity (link_id, response);
      } else {
        return NotFound ();
      }

      return response;
    }

    /// <summary>
    /// ラベルの組み合わせすべてに一致するアルバムカテゴリ一覧を取得します
    /// </summary>
    /// <param name="labels">ラベル組み合わせ</param>
    /// <returns>アルバムカテゴリ一覧</returns>
    [HttpGet ("w_album/+{labels}")]
    [ProducesResponseType (200)]
    public ActionResult<ResponseAapi<ICollection<ICategory>>> SearchAlbumCategory1 (string labels) {
      LOGGER.Info ("REQUEST - {0}", labels);
      var response = new ResponseAapi<ICollection<ICategory>> ();
      var categoryList = new List<ICategory> ();

      // クエリパラメータからラベルIDを取り出します
      List<ILabel> labelentity = new List<ILabel> ();
      string[] labelIdstr = labels.Split (',');
      foreach (var labelId in labelIdstr) {
        labelentity.Add (
          new Label { Id = long.Parse (labelId) }
        );
      }
      var query = this.mCategoryRepository.FindCategory (labelentity)
        .Where (category => category.AlbumFlag == true);
      categoryList.AddRange (
        query.Take (1000000)
      );

      response.Value = categoryList;
      return response;
    }

    /// <summary>
    /// ラベルの組み合わせすべてに一致するアルバムカテゴリ一覧を取得します
    /// </summary>
    /// <param name="labels">ラベル組み合わせ</param>
    /// <returns>アルバムカテゴリ一覧</returns>
    [HttpGet ("w_album/-{labels}")]
    [ProducesResponseType (200)]
    public ActionResult<ResponseAapi<ICollection<ICategory>>> SearchAlbumCategory2 (string labels) {
      LOGGER.Info ("REQUEST - {0}", labels);
      var response = new ResponseAapi<ICollection<ICategory>> ();
      var categoryList = new List<ICategory> ();

      // クエリパラメータからラベルIDを取り出します
      List<ILabel> labelentity = new List<ILabel> ();
      string[] labelIdstr = labels.Split (',');
      foreach (var labelId in labelIdstr) {
        labelentity.Add (
          new Label { Id = long.Parse (labelId) }
        );
      }
      var query = this.mCategoryRepository.FindCategoryOr (labelentity)
        .Where (category => category.AlbumFlag == true);
      categoryList.AddRange (
        query.Take (1000000)
      );

      response.Value = categoryList;
      return response;
    }

    /// <summary>
    /// 新しいカテゴリを登録します（未実装）
    /// </summary>
    /// <param name="value"></param>
    [HttpPost]
    public void Post ([FromBody] string value) { }

    /// <summary>
    /// カテゴリの表示回数を更新します
    /// </summary>
    /// <param name="id">更新対象のカテゴリを示すキー</param>
    /// <response code="200">カテゴリの表示回数を更新しました</response>
    /// <response code="400">指定した項目が取得できませんでした</response>
    [HttpPut ("{id}/read")]
    [ProducesResponseType (200)]
    [ProducesResponseType (400)]
    public ActionResult PutReadableCategory (long id) {
      LOGGER.Info ("REQUEST - {0}/read", id);

      var category = mCategoryRepository.Load (id);
      if (category != null) {
        if (!category.ReadableFlag) {
          category.ReadableFlag = true;
          category.ReadableDate = DateTime.Now;
        }
        category.LastReadDate = DateTime.Now;
        category.ReadableCount = category.ReadableCount + 1;
        mCategoryRepository.Save ();
        return Ok ();
      } else {
        return NotFound ();
      }
    }

    /// <summary>
    /// カテゴリ情報を永続化します
    /// </summary>
    /// <remarks>
    /// 永続化できるプロパティは下記の通りです。
    /// ・Name
    /// ・StarRating
    /// ・NextDisplayContentId
    /// </remarks>
    /// <param name="id">カテゴリID</param>
    /// <param name="value">永続化エンティティ</param>
    [HttpPatch ("{id}")]
    public ResponseAapi<Boolean> UpdateEntity (long id, [FromBody] Category value) {
      LOGGER.Info ("REQUEST - {0} Body={1}", id, value);

      var targetCategory = mCategoryRepository.Load (id);
      if (targetCategory == null) throw new InterfaceOperationException ("カテゴリ情報が見つかりません");

      if (value.Name != null)
        targetCategory.Name = value.Name;
      if (value.StarRating >= 0)
        targetCategory.StarRating = value.StarRating;
      if (value.NextDisplayContentId.HasValue)
        targetCategory.NextDisplayContentId = value.NextDisplayContentId.Value;

      mCategoryRepository.Save ();

      var response = new ResponseAapi<Boolean> ();
      response.Value = true;
      return response;
    }

    /// <summary>
    /// カテゴリを削除します（未実装）
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete ("{id}")]
    public void Delete (int id) { }

    /// <summary>
    /// カテゴリ情報リンク取得
    /// </summary>
    /// <param name="id">カテゴリID</param>
    /// <param name="link_type">リンクタイプを指定します。</param>
    /// <remarks>
    /// GET api/category/{id}/{link_type}
    /// link_type =
    /// "pc" : 親階層のカテゴリ情報を取得します
    /// "cc" : 子階層のカテゴリ情報リストを取得します。
    /// </remarks>
    /// <returns></returns>
    private ICollection<ICategory> GetCategoryLink (long id, string link_type) {
      var categoryList = new List<ICategory> ();

      if (link_type == "pc") {
        var category = this.mCategoryRepository.Load (id);
        var parentCategory = this.mCategoryRepository.Load (category.GetParentCategory ().Id);
        if (parentCategory != null) categoryList.Add (parentCategory);
      } else if (link_type == "cc") {
        var category = this.mCategoryRepository.Load (id);
        categoryList.AddRange (
          this.mCategoryRepository.FindChildren (category).Take (1000000)
        );
      }

      return categoryList;
    }

  }

  /// <summary>
  /// コンテント情報のNameプロパティを使用した比較を行う
  /// </summary>
  public class ContentNaturalComparer : NaturalComparer, IComparer<IContent> {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ContentNaturalComparer () { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Compare (IContent x, IContent y) {
      return base.Compare (x.Name, y.Name);
    }
  }
}
