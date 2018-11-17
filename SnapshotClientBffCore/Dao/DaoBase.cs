using RestSharp;
using Snapshot.Client.Bff.Mock.Data;

namespace Snapshot.Client.Bff.Dao {
  public abstract class DaoBase {
    protected readonly RestClient mClient;

    protected readonly DaoContext mDaoContext;

    public DaoBase (DaoContext context) {
      mDaoContext = context;
      mClient = new RestClient (context.ServerUrl);
    }
  }
}
