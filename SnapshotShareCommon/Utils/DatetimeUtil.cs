using System;

namespace Snapshot.Share.Common.Utils {
  public static class DatetimeUtil {
    public static long ToUnixTime (DateTime dt) {
      var dto = new DateTimeOffset (dt.Ticks, new TimeSpan (+09, 00, 00));
      return dto.ToUnixTimeSeconds ();
    }
    public static DateTime FromUnixTime (long unixTime) {
      return DateTimeOffset.FromUnixTimeSeconds (unixTime).LocalDateTime;
    }
  }
}
