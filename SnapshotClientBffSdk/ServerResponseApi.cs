using Newtonsoft.Json;
using Snapshot.Share.Common.Infra.Translate;

namespace Snapshot.Client.Bff.Sdk {
  public class ServerResponseApi<T> : ResponseApi<T> {
    public RT GetRelative<RT> (string key) {
      return JsonConvert.DeserializeObject<RT> (this.Rel[key]);
    }
  }
}
