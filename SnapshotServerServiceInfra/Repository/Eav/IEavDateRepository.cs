using System;
using Foxpict.Service.Infra.Model.Eav;

namespace Foxpict.Service.Infra.Repository
{
    public interface IEavDateRepository<T>
    {
        IEavDate GetEavDate(T entity, string key);
        
        void SetEavDate(T entity, string key, DateTime? value);
    }
}