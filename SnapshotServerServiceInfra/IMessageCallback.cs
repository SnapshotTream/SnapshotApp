using SimpleInjector;
using Snapshot.Server.Service.Infra.Repository;

namespace Snapshot.Server.Service.Infra {
  /// <summary>
  /// メッセージングフレームワークで使用するコールバックメソッドを持つインターフェース
  /// </summary>
  public delegate void MessageCallback (IMessageContext context);

  public interface IMessageContext : IContext {
    int getInt ();
    long getLong ();
    string getString ();

    object getObject ();
  }
}
