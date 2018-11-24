using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Snapshot.Client.Bff.Sdk;
using Snapshot.Client.Bff.Sdk.Dao;
using Snapshot.Client.Bff.Sdk.Data.Dto;
using Snapshot.Share.Common.Utils;
using Snapshot.Share.Common.Infra.Data.EventData;

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
    /// <param name="contentDao"></param>
    public EventLogController (IEventLogDao eventLogDao,
      IContentDao contentDao) {
      this.mEventLogDao = eventLogDao;
      this.mContentDao = contentDao;
    }

    /// <summary>
    /// プレビュー表示イベントを発行します
    /// </summary>
    /// <param name="contentId"></param>
    [HttpPost ()]
    public ActionResult<BffResponseApi<EventLog>> PreviewShowEventLog ([FromQuery] long contentId) {
      PreviewInfo previewInfo = new PreviewInfo ();
      previewInfo.TargetContentId = contentId;
      previewInfo.StartDisplayDate = DateTime.Now;

      EventLog eventLog = new EventLog ();
      eventLog.EventType = "PREVIEW";
      eventLog.Owner = "CLIENT";
      eventLog.User = "PRIVATE";
      eventLog.ValueFormat = "Preview-JSON";
      eventLog.Datetime = DateTime.Now;
      EventDataUtil.ToValue (eventLog, previewInfo);
      LOGGER.Info ("プレビュー表示イベントログ登録={0}", eventLog.Value);

      var response = new BffResponseApi<EventLog> ();
      response.Value = this.mEventLogDao.Create (eventLog);
      return response;
    }

    /// <summary>
    /// プレビュー非表示イベントを発行します
    /// </summary>
    /// <param name="eventLogId">更新するイベントログID</param>
    /// <param name="prevContentId">直前にプレビュー表示していたコンテントID(オプション)</param>
    [HttpPatch ("{eventLogId}")]
    public ActionResult<BffResponseApi<string>> PreviewHideEventLog (long eventLogId, [FromQuery (Name = "PrevContentId")] long? prevContentId) {
      LOGGER.Trace ("IN");
      var eventlog = this.mEventLogDao.Load (eventLogId);
      if (eventlog == null) {
        throw new ApplicationException ("イベントログの読み込みに失敗しました。");
      }

      var previewInfo = EventDataUtil.FromPreviewJson (eventlog);
      previewInfo.EndDisplayDate = DateTime.Now;
      if (prevContentId.HasValue) {
        previewInfo.PrevContentId = prevContentId.Value;
      }
      EventDataUtil.ToValue (eventlog, previewInfo);
      this.mEventLogDao.UpdateValue (eventlog);

      var response = new BffResponseApi<string> ();
      LOGGER.Trace ("OUT");
      return response;
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
      var eventLogList = this.mEventLogDao.Find (startDate.LocalDateTime, endDate.LocalDateTime, eventType);
      var result = new BffResponseApi<EventLog[]> ();
      result.Value = eventLogList.ToArray ();

      if (eventType == "PREVIEW") {

        if (withContentInfo || witnCategoryInfo) {
          var contentByEventLogId = new Dictionary<long, Content> ();
          var categoryByEventLogId = new Dictionary<long, Category> ();

          foreach (var eventlog in eventLogList) {
            var eventValue = EventDataUtil.FromPreviewJson (eventlog);
            if (eventValue.TargetContentId.HasValue && eventValue.TargetContentId.Value > 0) {
              var content = mContentDao.LoadContent (eventValue.TargetContentId.Value);
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

    /// <summary>
    /// イベントログを更新します
    /// </summary>
    /// <param name="eventLog">イベントログ</param>
    /// <returns></returns>
    [HttpPatch ()]
    public ActionResult<BffResponseApi<string>> UpdateEventLog ([FromBody] EventLog eventLog) {
      LOGGER.Trace ("IN");
      LOGGER.Info ("イベントログ({0})を更新します。", eventLog.Id);
      this.mEventLogDao.UpdateValue (eventLog);

      var response = new BffResponseApi<string> ();
      LOGGER.Trace ("OUT");
      return response;
    }
  }
}
