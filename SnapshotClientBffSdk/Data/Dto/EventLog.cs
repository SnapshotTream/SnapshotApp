using System;
using Snapshot.Share.Common.Infra.Model;

namespace Snapshot.Client.Bff.Sdk.Data.Dto {
  public class EventLog : IEventLog {
    public string EventType { get; set; }
    public DateTime Datetime { get; set; }
    public string Owner { get; set; }
    public string User { get; set; }
    public string ValueFormat { get; set; }
    public string Value { get; set; }
    public long Id { get; set; }
  }
}
