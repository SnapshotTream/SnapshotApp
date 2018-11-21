using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Snapshot.Client.Bff.Sdk;
using Snapshot.Client.Bff.Sdk.Dao;
using Snapshot.Client.Bff.Sdk.Data.Dto;
using Snapshot.Share.Common.Utils;

namespace Snapshot.Client.Bff.Mock.Controllers {
  /// <summary>
  /// イベントログ情報を扱うAPIを提供するコントローラーです。
  /// </summary>
  [Route ("api/bff/event")]
  [ApiController]
  public class EventLogController : ControllerBase {
    static readonly Logger LOGGER = LogManager.GetCurrentClassLogger ();

    readonly IEventLogDao mEventLogDao;

    readonly IContentDao mContentDao;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="eventLogDao"></param>
    public EventLogController (IEventLogDao eventLogDao,
      IContentDao contentDao) {
      this.mEventLogDao = eventLogDao;
      this.mContentDao = contentDao;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dateRange"></param>
    /// <param name="eventType"></param>
    /// <param name="withContentInfo"></param>
    /// <param name="witnCategoryInfo"></param>
    /// <returns></returns>
    [HttpGet ("s/{dateRange}/{eventType}")]
    public ActionResult<BffResponseApi<EventLog[]>> FindEventLog (string dateRange, string eventType, [FromQuery] bool withContentInfo, [FromQuery] bool witnCategoryInfo) {
      var dateRangeText = dateRange.Split ('-');
      var startDate = DateTimeOffset.FromUnixTimeSeconds (long.Parse (dateRangeText[0]));
      var endDate = DateTimeOffset.FromUnixTimeSeconds (long.Parse (dateRangeText[1]));
      var eventLogList = this.mEventLogDao.Find (startDate.LocalDateTime, endDate.LocalDateTime);
      var result = new BffResponseApi<EventLog[]> ();
      result.Value = eventLogList.ToArray ();

      if (eventType == "PREVIEW") {

        if (withContentInfo || witnCategoryInfo) {
          var contentByEventLogId = new Dictionary<long, Content> ();
          var categoryByEventLogId = new Dictionary<long, Category> ();

          foreach (var eventlog in eventLogList) {
            var eventValue = EventDataUtil.FromPreviewJson (eventlog);
            if (eventValue.TargetContentId > 0) {
              var content = mContentDao.LoadContent (eventValue.TargetContentId);
              contentByEventLogId.Add (eventlog.Id, content);

              // フラグが有効な場合、カテゴリ情報を読み込む
              if (witnCategoryInfo) {
                categoryByEventLogId.Add (eventlog.Id, content.LinkCategory);
              }
            }
          }

          // フラグが有効な場合のみ、コンテント一覧をリンクデータとしてレスポンスに含める
          if (withContentInfo) {
            result.Link.Add ("content", contentByEventLogId);
          }

          // フラグが有効な場合のみ、カテゴリ一覧をリンクデータとしてレスポンスに含める
          if (witnCategoryInfo) {
            result.Link.Add ("category", categoryByEventLogId);
          }
        }
      }

      return result;
    }
  }
}
