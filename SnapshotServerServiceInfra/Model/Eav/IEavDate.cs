using System;

namespace Foxpict.Service.Infra.Model.Eav
{
    public interface IEavDate : IEavBase
    {
        DateTime? Value { get; set; }
    }
}