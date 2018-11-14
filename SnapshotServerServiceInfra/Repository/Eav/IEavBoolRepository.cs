using Snapshot.Server.Service.Infra.Model.Eav;

namespace Snapshot.Server.Service.Infra.Repository {
  public interface IEavBoolRepository<T> {
    IEavBool GetEavBool (T entity, string key);

    void SetEavBool (T entity, string key, bool value);
  }
}
