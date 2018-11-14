using System;

namespace Snapshot.Server.Service.Infra.Model.Eav
{
    public interface IEavDate : IEavBase
    {
        DateTime? Value { get; set; }
    }
}