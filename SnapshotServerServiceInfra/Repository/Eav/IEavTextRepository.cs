using Foxpict.Service.Infra.Model.Eav;

namespace Foxpict.Service.Infra.Repository
{
    public interface IEavTextRepository<T>
    {
        IEavText GetEavText(T entity, string key);
        
        void SetEavText(T entity, string key, string value);
    }
}