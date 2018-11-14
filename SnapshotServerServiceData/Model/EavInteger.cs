using System;
using System.ComponentModel.DataAnnotations.Schema;
using Foxpict.Service.Infra.Model.Eav;
using Hyperion.Pf.Entity;

namespace Foxpict.Service.Model
{

    [Table("svp_Eav_Int")]
    public class EavInteger : IEavInteger
    {
        public string EntityTypeName { get; set; }

        public string CategoryName { get; set; }

        public long EntityId { get; set; }

        public string Key { get; set; }

        public int Value { get; set; }
    }
}