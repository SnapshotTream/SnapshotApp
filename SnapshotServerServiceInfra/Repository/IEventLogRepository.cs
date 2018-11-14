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
  }
}
