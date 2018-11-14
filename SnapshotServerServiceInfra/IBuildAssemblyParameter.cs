using System;
using System.Collections.Generic;

namespace Snapshot.Server.Service.Infra {
  /// <summary>
  ///
  /// </summary>
  public interface IBuildAssemblyParameter {
    Dictionary<string, string> Params { get; }
  }
}
