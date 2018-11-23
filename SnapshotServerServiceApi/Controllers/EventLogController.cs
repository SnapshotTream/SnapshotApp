using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Snapshot.Server.Service.Infra.Core;
using Snapshot.Server.Service.Infra.Model;
using Snapshot.Server.Service.Infra.Repository;
using Snapshot.Server.Service.Model;
using Snapshot.Share.Common.Utils;
using SnapshotShareCommon.Infra.Data.EventData;

namespace Snapshot.Server.Service.Api.Controllers {

  /// <summary>
  ///
  /// </summary>
  [Produces ("application/json")]
  [Route ("api/event")]
  [ApiController]
  public class EventLogController : Controller {
    private readonly Logger mLogger = LogManager.GetCurrentClassLogger ();

    private readonly IEventLogRepository mEventLogRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="eventLogRepository"></param>
    public EventLogController (IEventLogRepository eventLogRepository) {
      this.mEventLogRepository = eventLogRepository;
    }

    /// <summary>
    /// イベントログを登録します。
    /// </summary>
    /// <param name="eventLog">イベントログ</param>
    /// <returns></returns>
    [HttpPost ()]
    public ActionResult<ResponseAapi<IEventLog>> Save ([FromBody] EventLog eventLog) {
      mLogger.Trace ("IN");
      var entity = this.mEventLogRepository.New (eventLog);
      this.mEventLogRepository.Save ();

      mLogger.Debug ("イベントログ({0})を登録しました。", entity.Id);

      var response = new ResponseAapi<IEventLog> ();
      response.Value = entity;
      mLogger.Trace ("OUT");
      return response;
    }

    /// <summary>
    /// イベントログを保存します。
    /// </summary>
    /// <remarks>
    /// 更新できるフィールドは下記のデータに限ります。
    /// ・Value
    /// </remarks>
    /// <param name="id">イベントログID</param>
    /// <param name="eventLog">更新データ</param>
    /// <returns></returns>
    [HttpPatch ("{id}")]
    public ActionResult UpdateValue (long id, [FromBody] EventLog eventLog) {
      var entity = this.mEventLogRepository.Load (id);
      entity.Value = eventLog.Value;
      this.mEventLogRepository.Save ();
      return Ok ();
    }

    /// <summary>
    /// イベントログを取得します。
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet ("{id}")]
    public ActionResult<ResponseAapi<IEventLog>> GetEventLog (long id) {
      var response = new ResponseAapi<IEventLog> ();

      var eventlog = mEventLogRepository.Load (id);
      response.Value = eventlog;

      return response;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dateRange"></param>
    /// <returns></returns>
    [HttpGet ("s/{dateRange}")]
    public ActionResult<ResponseAapi<ICollection<IEventLog>>> GetEventLogByDate (string dateRange) {
      var response = new ResponseAapi<ICollection<IEventLog>> ();

      var dateRangeText = dateRange.Split ('-');
      var startDate = DateTimeOffset.FromUnixTimeSeconds (long.Parse (dateRangeText[0]));
      var endDate = DateTimeOffset.FromUnixTimeSeconds (long.Parse (dateRangeText[1]));

      var eventLogList = new List<IEventLog> ();
      eventLogList.AddRange (mEventLogRepository.FindEventLog (startDate.DateTime, endDate.DateTime));

      response.Value = eventLogList;
      return response;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="dateRange"></param>
    /// <param name="eventType"></param>
    /// <returns></returns>
    [HttpGet ("s/{dateRange}/{eventType}")]
    public ActionResult<ResponseAapi<ICollection<IEventLog>>> GetEventLogByDate (string dateRange, string eventType) {
      var response = new ResponseAapi<ICollection<IEventLog>> ();

      var dateRangeText = dateRange.Split ('-');
      var startDate = DateTimeOffset.FromUnixTimeSeconds (long.Parse (dateRangeText[0]));
      var endDate = DateTimeOffset.FromUnixTimeSeconds (long.Parse (dateRangeText[1]));

      var eventLogList = new List<IEventLog> ();
      eventLogList.AddRange (mEventLogRepository.FindEventLog (startDate.DateTime, endDate.DateTime, eventType));

      response.Value = eventLogList;
      return response;
    }
  }
}
