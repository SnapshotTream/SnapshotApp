using Microsoft.AspNetCore.Mvc;
using NLog;
using Snapshot.Client.Bff.Sdk;
using Snapshot.Client.Bff.Sdk.Dao;
using Snapshot.Client.Bff.Sdk.Data.Dto;

namespace Snapshot.Client.Bff.Mock.Controllers {
  /// <summary>
  /// BFFインターフェースのコントローラー
  /// </summary>
  [Route ("api/bff")]
  [ApiController]
  public class ContentController : ControllerBase {
    static readonly Logger LOGGER = LogManager.GetCurrentClassLogger ();

    readonly IContentDao mContentDao;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="contentDao"></param>
    public ContentController (
      IContentDao contentDao
    ) {
      this.mContentDao = contentDao;
    }

    /// <summary>
    /// コンテント情報を取得します
    /// </summary>
    /// <param name="contentId">コンテントID</param>
    /// <returns></returns>
    [HttpGet ("content/{contentId}")]
    public ActionResult<BffResponseApi<Content>> LoadContent (long contentId) {
      LOGGER.Trace ("IN");
      var result = new BffResponseApi<Content> ();
      result.Value = mContentDao.LoadContent (contentId);
      LOGGER.Trace ("OUT");
      return result;
    }

    /// <summary>
    /// コンテント情報を更新します
    /// </summary>
    /// <param name="contentId"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    [HttpPatch ("content/{contentId}")]
    public ActionResult<BffResponseApi<bool>> UpdateContent (long contentId, [FromBody] Content content) {
      LOGGER.Trace ("IN");
      var result = new BffResponseApi<bool> ();
      content.Id = contentId;
      mContentDao.Update (content);
      LOGGER.Trace ("OUT");
      return result;
    }

    /// <summary>
    /// コンテント情報の既読状態を更新します。
    /// </summary>
    /// <param name="contentId"></param>
    /// <returns></returns>
    [HttpPut ("content/{contentId}/readable")]
    public ActionResult<BffResponseApi<bool>> UpdateReadable (long contentId) {
      LOGGER.Trace ("IN");
      var result = new BffResponseApi<bool> ();
      mContentDao.UpdateRead (contentId);
      result.Value = true;
      LOGGER.Trace ("OUT");
      return result;
    }
  }
}
