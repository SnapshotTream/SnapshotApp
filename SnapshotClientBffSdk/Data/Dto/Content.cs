using System;
using Newtonsoft.Json;
using Snapshot.Share.Common.Infra.Model;

namespace Snapshot.Client.Bff.Sdk.Data.Dto {
  /// <summary>
  /// コンテント情報データモデル（クライアント）
  /// </summary>
  public class Content : IContent {
    public long Id { get; set; }

    public string Name { get; set; }

    public string ThumbnailKey { get; set; }

    public string IdentifyKey { get; set; }

    public string ContentHash { get; set; }

    public string Caption { get; set; }

    public string Comment { get; set; }

    public bool ArchiveFlag { get; set; }

    public bool ReadableFlag { get; set; }

    public int StarRating { get; set; }

    public int ReadableCount { get; set; }

    public DateTime? LastReadDate { get; set; }

    public DateTime? ReadableDate { get; set; }

    /// <summary>
    /// リンクしているカテゴリ情報
    /// </summary>
    /// <remarks>
    /// このプロパティは、フロントエンドへのシリアライズ対象外です。
    /// </remarks>
    /// <returns></returns>
    [JsonIgnore]
    public Category LinkCategory { get; set; }
  }
}
