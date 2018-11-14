using Snapshot.Server.Service.Infra.Model.Eav;

namespace Snapshot.Server.Service.Infra.Repository {
  public interface IEavTextRepository<T> {
    IEavText GetEavText (T entity, string key);

    void SetEavText (T entity, string key, string value);
  }
}
