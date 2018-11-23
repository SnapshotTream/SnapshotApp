using NLog;
using RestSharp;
using Snapshot.Client.Bff.Mock.Data;

namespace Snapshot.Client.Bff.Dao {
  public abstract class DaoBase {
    static readonly Logger LOGGER = LogManager.GetCurrentClassLogger ();

    protected readonly RestClient mClient;

    protected readonly DaoContext mDaoContext;

    public DaoBase (DaoContext context) {
      mDaoContext = context;
      mClient = new RestClient (context.ServerUrl);
    }

    protected void OutputResponseErrorMessage (IRestResponse response) {
      if (!response.IsSuccessful) {
        LOGGER.Info ("処理結果={0}", response.IsSuccessful);
        LOGGER.Warn ("エラーメッセージ={0}", response.ErrorMessage);
      }
    }
  }
}
