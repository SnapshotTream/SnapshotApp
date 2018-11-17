using Microsoft.AspNetCore.Mvc;
using NLog;
using Snapshot.Client.Bff.Sdk;
using Snapshot.Client.Bff.Sdk.Dao;
using Snapshot.Client.Bff.Sdk.Data.Dto;

namespace Snapshot.Client.Bff.Mock.Controllers {
  /// <summary>
  /// ラベル情報を取り扱うAPIを提供するコントローラーです。
  /// </summary>
  [Route ("api/bff")]
  [ApiController]
  public class LabelController : ControllerBase {
    static readonly Logger LOGGER = LogManager.GetCurrentClassLogger ();

    readonly ILabelDao mLabelDao;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="labelDao"></param>
    public LabelController (
      ILabelDao labelDao
    ) {
      this.mLabelDao = labelDao;
    }

    /// <summary>
    /// ラベル一覧を取得します
    /// </summary>
    /// <returns>ラベル一覧</returns>
    [HttpGet ("labels")]
    public ActionResult<BffResponseApi<Label[]>> LoadLabels () {
      LOGGER.Trace ("IN");
      var result = new BffResponseApi<Label[]> ();
      result.Value = mLabelDao.LoadLabel ().ToArray ();
      LOGGER.Trace ("OUT");
      return result;
    }
  }
}
