using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snapshot.Server.Service.Model
{
    [Table("svp_EventLog")]
    public class EventLog : Service.Infra.Model.IEventLog
    {
        public long Id { get; set; }

        public DateTime Datetime { get; set; }

        public string EventType { get; set; }

        public string Owner { get; set; }

        public string User { get; set; }

        public string ValueFormat { get; set; }

        public string Value { get; set; }
    }
}
