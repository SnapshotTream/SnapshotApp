using Newtonsoft.Json;
using Snapshot.Share.Common.Exception;
using Snapshot.Share.Common.Infra.Data.EventData;
using Snapshot.Share.Common.Infra.Model;

namespace Snapshot.Share.Common.Utils {
  public static class EventDataUtil {
    /// <summary>
    ///
    /// </summary>
    /// <param name="eventlog"></param>
    /// <returns></returns>
    public static PreviewInfo FromPreviewJson (IEventLog eventlog) {
      // Guard
      if (eventlog.ValueFormat != "Preview-JSON") {
        throw new NotSupportException ();
      }

      PreviewInfo value = new PreviewInfo ();
      JsonConvert.PopulateObject (eventlog.Value, value);
      return value;
    }

    public static CreateEntityInfo FromCreateEntityJson (IEventLog eventlog) {
      // Guard
      if (eventlog.ValueFormat != "CreateEntity-JSON") {
        throw new NotSupportException ();
      }

      CreateEntityInfo value = new CreateEntityInfo ();
      JsonConvert.PopulateObject (eventlog.Value, value);
      return value;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="target"></param>
    /// <param name="valueObject"></param>
    public static void ToValue (IEventLog target, object valueObject) {
      if (target.ValueFormat == "Preview-JSON") {
        target.Value = JsonConvert.SerializeObject ((PreviewInfo) valueObject);
      } else if (target.ValueFormat == "CreateEntity-JSON") {
        target.Value = JsonConvert.SerializeObject ((CreateEntityInfo) valueObject);
      } else {
        throw new NotSupportException ();
      }
    }
  }
}
