using Foxpict.Service.Infra.Extention;
using Hyperion.Pf.Entity;

namespace Foxpict.Service.Infra
{
    /// <summary>
    /// メッセージマネージャのインターフェースを使用するのは拡張機能からのみを想定
    /// </summary>
    public interface IMessagingManager
    {
        void RegisterMessage(string messageName, IExtentionMetaInfo extention, MessageCallback callback);
        void UnegisterMessage(string messageName, IExtentionMetaInfo extention, MessageCallback callback);
    }
}
