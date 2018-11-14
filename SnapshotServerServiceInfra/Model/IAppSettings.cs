namespace Foxpict.Service.Infra.Model {
  public interface IAppSettings {
    string ENV_HEROKU_DATABASE_URL { get; }
  }
}
