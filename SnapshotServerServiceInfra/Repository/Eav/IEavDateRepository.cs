using System;
using Snapshot.Server.Service.Infra.Model.Eav;

namespace Snapshot.Server.Service.Infra.Repository {
  public interface IEavDateRepository<T> {
    IEavDate GetEavDate (T entity, string key);

    void SetEavDate (T entity, string key, DateTime? value);
  }
}
