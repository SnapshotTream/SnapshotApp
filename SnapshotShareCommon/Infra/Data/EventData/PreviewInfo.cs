using System;

namespace Snapshot.Share.Common.Infra.Data.EventData {
  public class PreviewInfo {
    public long? TargetContentId { get; set; }

    public long? PrevContentId { get; set; }

    public DateTime? StartDisplayDate { get; set; }

    public DateTime? EndDisplayDate { get; set; }
  }
}
