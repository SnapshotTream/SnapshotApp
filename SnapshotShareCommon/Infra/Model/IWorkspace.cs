using System;
using Hyperion.Pf.Entity;

namespace Snapshot.Share.Common.Infra.Model {
  /// <summary>
  /// ワークスペースモデルのインターフェースです。
  /// </summary>
  /// <typeparam name="long"></typeparam>
  public interface IWorkspace : IEntity<long> {
    /// <summary>
    /// ワークスペース名称
    /// </summary>
    /// <returns></returns>
    string Name { get; set; }

    /// <summary>
    /// 物理領域のパス
    /// </summary>
    /// <returns></returns>
    string PhysicalPath { get; set; }

    /// <summary>
    /// 仮想領域のパス
    /// </summary>
    /// <returns></returns>
    string VirtualPath { get; set; }

    /// <summary>
    /// インポート用領域のパス
    /// </summary>
    /// <returns></returns>
    string ImportPath { get; set; }

    /// <summary>
    /// 最終フルビルド実行日時
    /// </summary>
    /// <returns></returns>
    DateTime? LastFullBuildDate { get; set; }
  }
}
