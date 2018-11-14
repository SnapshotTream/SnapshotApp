using Foxpict.Service.Infra.Repository;

namespace Foxpict.Service.Infra
{
    public interface IContext
    {
        Type InjectionInstance<Type>() where Type : class;
    }
}
