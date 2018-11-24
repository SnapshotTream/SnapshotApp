using Microsoft.AspNetCore.Mvc;
using Snapshot.Client.Bff.Sdk.Dao;

namespace Snapshot.Client.Entry.App.Controllers {

  [Route ("api/bff/resource")]
  [ApiController]
  public class ResourceController : ControllerBase {

    readonly IContentDao mContentDao;

    readonly IThumbnailDao mThumbnailDao;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="contentDao"></param>
    /// <param name="thumbnailDao"></param>
    public ResourceController (IContentDao contentDao, IThumbnailDao thumbnailDao) {
      this.mContentDao = contentDao;
      this.mThumbnailDao = thumbnailDao;
    }

    /// <summary>
    /// サムネイル画像を取得します。
    /// </summary>
    /// <param name="thumbnailKey"></param>
    /// <returns></returns>
    [HttpGet ("thumbnail/{thumbnailKey}")]
    public IActionResult FetchThumbnail (string thumbnailKey) {
      return new FileContentResult (mThumbnailDao.Thumbnail (thumbnailKey), "image/png");
    }

    /// <summary>
    /// コンテントの画像を取得します。
    /// </summary>
    /// <param name="contentId"></param>
    /// <returns></returns>
    [HttpGet ("content/{contentId}")]
    public IActionResult FetchPreviewImage (long contentId) {
      string contentType;
      var bytes = mContentDao.LoadContentImage (contentId, out contentType);
      return new FileContentResult (bytes, contentType);
    }
  }
}
