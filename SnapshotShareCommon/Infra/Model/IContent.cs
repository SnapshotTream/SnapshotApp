using System;
using Hyperion.Pf.Entity;

namespace Snapshot.Share.Common.Infra.Model {
  public interface IContent : IEntity<long> {
    /// <summary>
    /// コンテント名称
    /// </summary>
    /// <value></value>
    string Name { get; set; }

    /// <summary>
    /// 識別子
    /// </summary>
    /// <value></value>
    string IdentifyKey { get; set; }

    /// <summary>
    /// コンテントハッシュ
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// 該当データから生成したハッシュ文字列
    /// </remarks>
    string ContentHash { get; set; }

    /// <summary>
    /// サムネイル
    /// </summary>
    /// <value></value>
    string ThumbnailKey { get; set; }

    /// <summary>
    /// キャプション
    /// </summary>
    /// <value></value>
    string Caption { get; set; }

    /// <summary>
    /// コメント
    /// </summary>
    /// <value></value>
    string Comment { get; set; }

    /// <summary>
    /// アーカイブフラグ
    /// /// </summary>
    /// <value></value>
    bool ArchiveFlag { get; set; }

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

    /// <summary>
    /// スターレーティング
    /// </summary>
    /// <value></value>
    int StarRating { get; set; }
  }
}
