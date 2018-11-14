using Snapshot.Share.Common.Attributes;

namespace Snapshot.Share.Common.Types {
  public enum ThumbnailType {
    NON_SETTING = 0,

    /// <summary>
    /// ListIcon用サムネイル
    /// </summary>
    [ThumbnailInfo ("ListIcon", Width = 250)]
    LISTICON = 1,
  }
}
