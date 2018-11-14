using Snapshot.Server.Service.Infra.Model;

namespace Snapshot.Server.Service.Infra.Repository {
  /// <summary>
  ///
  /// </summary>
  public interface IWorkspaceRepository : IRepositoryBase {
    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    IWorkspace New ();

    /// <summary>
    ///
    /// /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    IWorkspace Load (long id);
  }
}
