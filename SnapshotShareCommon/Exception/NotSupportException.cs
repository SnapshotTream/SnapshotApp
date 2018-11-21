using System;
using System.Runtime.Serialization;

namespace Snapshot.Share.Common.Exception {
  public class NotSupportException : ApplicationException {
    public NotSupportException () { }

    public NotSupportException (string message) : base (message) { }

    public NotSupportException (string message, System.Exception innerException) : base (message, innerException) { }

    protected NotSupportException (SerializationInfo info, StreamingContext context) : base (info, context) { }
  }
}
