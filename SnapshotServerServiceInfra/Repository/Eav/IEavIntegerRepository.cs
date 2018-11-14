using Snapshot.Server.Service.Infra.Model.Eav;

namespace Snapshot.Server.Service.Infra.Repository {
  public interface IEavIntegerRepository<T> {
    IEavInteger GetEavInteger (T entity, string key);

    void SetEavInteger (T entity, string key, int value);
  }
}
