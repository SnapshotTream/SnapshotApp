using System;
using System.ComponentModel.DataAnnotations.Schema;
using Foxpict.Service.Infra.Model.Eav;
using Hyperion.Pf.Entity;

namespace Foxpict.Service.Model
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