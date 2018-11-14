using Snapshot.Server.Service.Infra.Model;
using NLog;
using SimpleInjector;

namespace Snapshot.Server.Service.Core.Service.Service {
  public class VirtualFileUpdateWatchService {
    private readonly Logger mLogger;

    FileUpdateWatchServiceBase mFileUpdateWatchImpl;

    public VirtualFileUpdateWatchService (Container container) {
      mFileUpdateWatchImpl = new FileUpdateWatchServiceBase (container);
    }

    public void StartWatch (IWorkspace workspace) {
      mFileUpdateWatchImpl.StartWatchByVirtualPath (workspace);
    }
  }
}
