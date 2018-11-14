using System;
using Hyperion.Pf.Entity;

namespace Snapshot.Share.Common.Infra.Model {
  /// <summary>
  /// カテゴリモデル
  /// </summary>
  /// <typeparam name="long"></typeparam>
  public interface ICategory : IEntity<long> {
    /// <summary>
    /// カテゴリ名称
    /// </summary>
    /// <returns></returns>
    string Name { get; set; }

    /// <summary>
    /// カテゴリの表示回数
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// カテゴリ内のコンテント一覧を表示した回数です。
    /// カテゴリ内のコンテントを表示した場合にはカウントアップしません。
    /// </remarks>
    int ReadableCount { get; set; }

    /// <summary>
    /// 最終カテゴリ表示日時
    /// </summary>
    /// <value></value>
    DateTime? LastReadDate { get; set; }

    /// <summary>
    /// 既読日時
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// 未読フラグをセットした日時。
    ///
    /// </remarks>
    DateTime? ReadableDate { get; set; }

    /// <summary>
    /// 未読フラグ
    /// </summary>
    /// <value></value>
    bool ReadableFlag { get; set; }

    string ArtworkThumbnailKey { get; set; }

    /// <summary>
    /// スターレーティング
    /// </summary>
    /// <value></value>
    int StarRating { get; set; }

    /// <summary>
    /// アルバムカテゴリフラグ
    /// </summary>
    /// <value></value>
    bool AlbumFlag { get; set; }

    /// <summary>
    /// カテゴリ内のコンテントをナビゲーション表示した際の次回表示コンテントの位置
    /// </summary>
    /// <value></value>
    long? NextDisplayContentId { get; set; }

    /// <summary>
    /// 不明・・・。
    /// </summary>
    /// <value></value>
    string BookmarkValue { get; set; }
  }
}
