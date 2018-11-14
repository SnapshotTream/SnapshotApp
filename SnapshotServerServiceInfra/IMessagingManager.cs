using Hyperion.Pf.Entity;
using Snapshot.Server.Service.Infra.Extention;

namespace Snapshot.Server.Service.Infra {
  /// <summary>
  /// メッセージマネージャのインターフェースを使用するのは拡張機能からのみを想定
  /// </summary>
  public interface IMessagingManager {
    void RegisterMessage (string messageName, IExtentionMetaInfo extention, MessageCallback callback);
    void UnegisterMessage (string messageName, IExtentionMetaInfo extention, MessageCallback callback);
  }
}
