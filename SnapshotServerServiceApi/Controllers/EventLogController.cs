using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Snapshot.Server.Service.Infra.Core;
using Snapshot.Server.Service.Infra.Model;
using Snapshot.Server.Service.Infra.Repository;

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
    /// イベントログを取得します。
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet ("{id}")]
    public ActionResult<ResponseAapi<IEventLog>> GetEventLog (long id) {
      var response = new ResponseAapi<IEventLog> ();

      var eventlog = mEventLogRepository.Load (id);
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
    /// ///
    /// </summary>
    /// <param name="dateRange"></param>
    /// <returns></returns>
    [HttpGet ("s/{dateRange}/{eventType")]
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
