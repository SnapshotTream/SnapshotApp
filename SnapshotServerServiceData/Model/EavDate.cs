using System;
using System.ComponentModel.DataAnnotations.Schema;
using Snapshot.Server.Service.Infra.Model.Eav;
using Hyperion.Pf.Entity;

namespace Snapshot.Server.Service.Model
{

    [Table("svp_Eav_Date")]
    public class EavDate : IEavDate
    {
        public string EntityTypeName { get; set; }

        public string CategoryName { get; set; }

        public long EntityId { get; set; }

        public string Key { get; set; }

        public DateTime? Value { get; set; }
    }
}