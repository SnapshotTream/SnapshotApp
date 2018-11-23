using System;
using System.Collections.Generic;
using Snapshot.Client.Bff.Sdk.Data.Dto;

namespace Snapshot.Client.Bff.Sdk.Dao {
  public interface IEventLogDao {

    /// <summary>
    ///
    /// </summary>
    /// <param name="eventLogId"></param>
    /// <returns></returns>
    EventLog Load (long eventLogId);

    /// <summary>
    ///
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="eventType"></param>
    /// <returns></returns>
    List<EventLog> Find (DateTime startDate, DateTime endDate, string eventType);

    /// <summary>
    ///
    /// </summary>
    /// <param name="eventLog"></param>
    /// <returns></returns>
    EventLog Create(EventLog eventLog);

    /// <summary>
    /// イベントログを更新します。
    /// </summary>
    /// <param name="eventLog"></param>
    void UpdateValue(EventLog eventLog);
  }
}
