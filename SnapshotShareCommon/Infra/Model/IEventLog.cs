using System;
using Hyperion.Pf.Entity;

namespace Snapshot.Share.Common.Infra.Model {
  public interface IEventLog : IEntity<long> {
    string EventType { get; set; }

    DateTime Datetime { get; set; }

    string Owner { get; set; }

    string User { get; set; }

    string ValueFormat { get; set; }

    string Value { get; set; }
  }
}
