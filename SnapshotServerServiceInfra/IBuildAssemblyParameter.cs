using System;
using System.Collections.Generic;

namespace Foxpict.Service.Infra
{
    /// <summary>
    ///
    /// </summary>
    public interface IBuildAssemblyParameter
    {
        Dictionary<string, string> Params { get; }
    }
}
