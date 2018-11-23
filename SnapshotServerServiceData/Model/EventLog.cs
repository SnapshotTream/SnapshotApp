using System;
using System.ComponentModel.DataAnnotations.Schema;
using Hyperion.Pf.Entity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Snapshot.Server.Service.Model {
  [Table ("svp_EventLog")]
  public class EventLog : Service.Infra.Model.IEventLog, IAuditableEntity {
    public long Id { get; set; }

    public DateTime Datetime { get; set; }

    public string EventType { get; set; }

    public string Owner { get; set; }

    public string User { get; set; }

    public string ValueFormat { get; set; }

    public string Value { get; set; }

    [JsonIgnore]
    public string CreatedBy { get; set; }

    [JsonIgnore]
    public DateTime CreatedDate { get; set; }

    [JsonIgnore]
    public string UpdatedBy { get; set; }

    [JsonIgnore]
    public DateTime UpdatedDate { get; set; }
  }
}
