using System;
using Hyperion.Pf.Entity;

namespace Snapshot.Share.Common.Infra.Model {
  public interface IEventLog : IEntity<long> {
    int EventNo { get; set; }

    DateTime EventDate { get; set; }

    string Sender { get; set; }

    string Message { get; set; }

    string Value { get; set; }
  }
}
