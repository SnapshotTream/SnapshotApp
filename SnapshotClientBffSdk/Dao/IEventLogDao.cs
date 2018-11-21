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
    /// <returns></returns>
    List<EventLog> Find (DateTime startDate, DateTime endDate);
  }
}
