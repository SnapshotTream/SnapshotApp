using Snapshot.Server.Service.Infra.Model;

namespace Snapshot.Server.Service.Infra.Repository {
  public interface IThumbnailAppMetaInfoRepository : IRepositoryBase {
    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    IAppMetaInfo New ();

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    IAppMetaInfo Load (long id);

    /// <summary>
    ///
    /// </summary>
    /// <param name="keyName"></param>
    /// <returns></returns>
    IAppMetaInfo LoadByKey (string keyName);
  }
}
