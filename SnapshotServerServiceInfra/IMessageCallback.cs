using Foxpict.Service.Infra.Repository;
using SimpleInjector;

namespace Foxpict.Service.Infra
{
    /// <summary>
    /// メッセージングフレームワークで使用するコールバックメソッドを持つインターフェース
    /// </summary>
    public delegate void MessageCallback(IMessageContext context);

    public interface IMessageContext : IContext
    {
        int getInt();
        long getLong();
        string getString();

        object getObject();
    }
}
