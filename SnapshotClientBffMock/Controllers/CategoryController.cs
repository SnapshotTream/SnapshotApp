using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Snapshot.Client.Bff.Mock.Data;
using Snapshot.Client.Bff.Sdk;
using Snapshot.Client.Bff.Sdk.Dao;
using Snapshot.Client.Bff.Sdk.Data;
using Snapshot.Client.Bff.Sdk.Data.Dto;

namespace Snapshot.Client.Bff.Mock.Controllers {
  /// <summary>
  /// カテゴリ情報を取り扱うAPIを提供するコントローラーです。
  /// </summary>
  [Route ("api/bff")]
  [ApiController]
  public class CategoryController : ControllerBase {

    static readonly Logger LOGGER = LogManager.GetCurrentClassLogger ();

    readonly DaoContext mDaoContext;

    readonly ICategoryDao mCategoryDao;

    readonly IContentDao mContentDao;

    readonly int PPR = 500000; // ページネーションを実装するまでは十分大きな値を設定しておく

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="categoryDao"></param>
    public CategoryController (
      DaoContext daoContext,
      ICategoryDao categoryDao,
      IContentDao contentDao) {
      this.mDaoContext = daoContext;
      this.mCategoryDao = categoryDao;
      this.mContentDao = contentDao;
    }

    /// <summary>
    /// カテゴリ情報を取得します
    /// </summary>
    /// <param name="categoryId">カテゴリID</param>
    /// <returns></returns>
    [HttpGet ("category/{categoryId}")]
    public ActionResult<BffResponseApi<Category>> LoadCategory (long categoryId) {
      LOGGER.Trace ("IN");
      var result = new BffResponseApi<Category> ();
      result.Value = mCategoryDao.LoadCategory (categoryId);
      LOGGER.Trace ("OUT");
      return result;
    }

    /// <summary>
    /// カテゴリのアートワークを設定します。
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    [HttpPut ("category/{categoryId}/thumbnail")]
    public ActionResult<BffResponseApi<Category>> UpdateCategoryArtwork (long categoryId) {
      LOGGER.Trace ("IN");
      var categopry = mCategoryDao.UpdateCategoryArtwork (categoryId);

      var result = new BffResponseApi<Category> ();
      result.Value = categopry;
      LOGGER.Trace ("OUT");
      return result;
    }

    /// <summary>
    /// カテゴリの小階層カテゴリ一覧を取得します。
    /// </summary>
    /// <param name="parentCategoryId">カテゴリID</param>
    /// <param name="pageNo">ページ</param>
    /// <returns>小階層カテゴリ一覧</returns>
    [HttpGet ("category/tree/{parentCategoryId}")]
    public ActionResult<BffResponseApi<Category[]>> LoadCategoryTree (long parentCategoryId, [FromQuery] int pageNo) {
      LOGGER.Trace ("IN");
      var result = new BffResponseApi<Category[]> ();

      var category = mCategoryDao.LoadCategory (parentCategoryId);
      if (category == null) {
        throw new ApplicationException ("カテゴリ情報の取得に失敗しました。");
      }

      int page = 0;
      if (pageNo <= 0)
        page = 0;
      else
        page = pageNo;

      result.Value = category.LinkSubCategoryList.Skip (this.PPR * page).Take (this.PPR).ToArray ();
      LOGGER.Trace ("OUT");
      return result;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="parentCategoryId"></param>
    /// <returns></returns>
    [HttpGet ("category/tree/{parentCategoryId}/total")]
    public ActionResult<BffResponseApi<PagenationEntity>> LoadCategoryTreeTotal (long parentCategoryId) {
      // TODO: 総数はネットワーク側（サーバー側）で取得するべき
      var category = mCategoryDao.LoadCategory (parentCategoryId);
      var total = category.LinkSubCategoryList.LongCount ();

      var pagenation = new PagenationEntity ();
      pagenation.Page = this.calcPage (total);
      pagenation.Total = total;
      pagenation.WindowSize = this.PPR;

      var result = new BffResponseApi<PagenationEntity> ();
      result.Value = pagenation;
      return result;
    }

    /// <summary>
    /// カテゴリに属するコンテント一覧を取得します。
    /// </summary>
    /// <param name="categoryId">カテゴリID</param>
    /// <returns>コンテント一覧</returns>
    [HttpGet ("category/{categoryId}/contents")]
    public ActionResult<BffResponseApi<Content[]>> LoadCategoryContentList (long categoryId) {
      LOGGER.Trace ("IN");
      var result = new BffResponseApi<Content[]> ();

      var category = mCategoryDao.LoadCategory (categoryId);
      result.Value = category.LinkContentList.ToArray ();
      LOGGER.Trace ("OUT");
      return result;
    }

    /// <summary>
    /// ラベルを条件にカテゴリ一覧を取得します。
    /// </summary>
    /// <param name="cond_label">ラベルの検索条件(「,」区切りでラベルIDを指定します)</param>
    /// <returns>カテゴリ一覧</returns>
    [HttpGet ("categories/{cond_label}")]
    public ActionResult<BffResponseApi<Category[]>> FindCategoryByLabel (string cond_label) {
      LOGGER.Trace ("IN");
      var result = new BffResponseApi<Category[]> ();
      var cond_labels = from u in cond_label.Split (',') select long.Parse (u);
      LOGGER.Info ("パース済みラベルID一覧({0})", cond_label);

      // 検索するカテゴリはアルバムカテゴリとする
      result.Value = mCategoryDao.FindCategory (true, cond_labels.ToArray ()).ToArray ();
      LOGGER.Trace ("OUT");
      return result;
    }

    /// <summary>
    /// カテゴリ内の指定位置にあるコンテントを取得します。
    /// </summary>
    /// <param name="categoryId">カテゴリID</param>
    /// <param name="position">取得位置(1以上の値)</param>
    /// <returns>コンテント情報</returns>
    [HttpGet ("category/{categoryId}/contents/preview/{position}")]
    public ActionResult<BffResponseApi<Content>> LoadCategoryContentListPreview (long categoryId, int position) {
      LOGGER.Trace ("IN");
      if (position <= 0) {
        throw new ArgumentOutOfRangeException ("positionの値は1以上の値を設定してください。");
      }

      var result = new BffResponseApi<Content> ();

      var category = mCategoryDao.LoadCategory (categoryId);
      if (category.LinkContentList.Count < position)
        return NoContent ();

      var content = category.LinkContentList[position - 1];
      mContentDao.UpdateRead (content.Id);
      result.Value = mContentDao.LoadContent (content.Id);
      LOGGER.Trace ("OUT");
      return result;
    }

    /// <summary>
    /// カテゴリ内の指定位置にあるコンテント数を取得します。
    /// </summary>
    /// <param name="categoryId">カテゴリID</param>
    /// <returns>コンテント数</returns>
    [HttpGet ("category/{categoryId}/contents/total")]
    public ActionResult<BffResponseApi<PagenationEntity>> LoadCategoryContentListTotal (long categoryId) {
      LOGGER.Trace ("IN");
      var result = new BffResponseApi<PagenationEntity> ();

      var category = mCategoryDao.LoadCategory (categoryId);
      var pagenation = new PagenationEntity ();
      pagenation.Total = category.LinkContentList.Count;
      pagenation.Page = 0;
      pagenation.WindowSize = 5;

      result.Value = pagenation;
      LOGGER.Trace ("OUT");
      return result;
    }

    /// <summary>
    /// カテゴリ情報に既読済みコンテントIDを設定します。
    /// </summary>
    /// <param name="categoryId"></param>
    /// <param name="nextContentId"></param>
    /// <returns></returns>
    [HttpPut ("category/{categoryId}/nextcontent/{nextContentId}")]
    public ActionResult<BffResponseApi<bool>> UpdateNextContent (long categoryId, long nextContentId) {
      LOGGER.Trace ("IN");
      var result = new BffResponseApi<bool> ();

      var category = mCategoryDao.LoadCategory (categoryId);
      if (category.LinkContentList.Last ().Id == nextContentId) {
        Category entity = new Category ();
        entity.NextDisplayContentId = 0;

        mCategoryDao.Update (categoryId, entity);
        result.Value = false;
      } else {
        Category entity = new Category ();
        entity.NextDisplayContentId = nextContentId;

        mCategoryDao.Update (categoryId, entity);
        result.Value = true;
      }

      LOGGER.Trace ("OUT");
      return result;
    }

    /// <summary>
    /// コンテント情報の既読状態を更新します。
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    [HttpPut ("category/{categoryId}/readable")]
    public ActionResult<BffResponseApi<bool>> UpdateReadable (long categoryId) {
      LOGGER.Trace ("IN");
      var result = new BffResponseApi<bool> ();
      mCategoryDao.UpdateRead (categoryId);
      result.Value = true;
      LOGGER.Trace ("OUT");
      return result;
    }

    /// <summary>
    /// ShareCommonに持っていく。
    /// </summary>
    /// <param name="total"></param>
    /// <returns></returns>
    private long calcPage (long total) {
      if (total % this.PPR == 0) {
        return total / this.PPR;
      } else {
        return total / this.PPR + 1;
      }
    }
  }
}
