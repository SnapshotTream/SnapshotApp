using Snapshot.Server.Service.Infra.Repository;

namespace Snapshot.Server.Service.Infra {
  public interface IContext {
    Type InjectionInstance<Type> () where Type : class;
  }
}
