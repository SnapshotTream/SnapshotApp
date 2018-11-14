using Foxpict.Service.Infra.Model.Eav;

namespace Foxpict.Service.Infra.Repository
{
    public interface IEavIntegerRepository<T>
    {
        IEavInteger GetEavInteger(T entity, string key);
        
        void SetEavInteger(T entity, string key, int value);
    }
}