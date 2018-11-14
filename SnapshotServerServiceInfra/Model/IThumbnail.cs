namespace Foxpict.Service.Infra.Model {
  public interface IThumbnail : Snapshot.Share.Common.Infra.Model.IThumbnail {
    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    byte[] BitmapBytes { get; set; }
  }
}
