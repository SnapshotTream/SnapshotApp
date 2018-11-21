using System;
using System.Linq;
using Snapshot.Server.Service.Infra.Model;

namespace Snapshot.Server.Service.Infra.Repository {
  public interface IEventLogRepository : IRepositoryBase {
    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    IEventLog New ();

    /// <summary>
    ///
    /// </summary>
    /// /// <param name="id"></param>
    /// <returns></returns>
    IEventLog Load (long id);

    /// <summary>
    ///
    /// </summary>
    /// <param name="beginDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    IQueryable<IEventLog> FindEventLog (DateTime beginDate, DateTime endDate);

    /// <summary>
    ///
    /// </summary>
    /// <param name="beginDate"></param>
    /// <param name="endDate"></param>
    /// <param name="eventType"></param>
    /// <returns></returns>
    IQueryable<IEventLog> FindEventLog (DateTime beginDate, DateTime endDate, string eventType);
  }
}
