using System.IO;

namespace Snapshot.Server.Service.Infra.Model {
  public interface IWorkspace : Snapshot.Share.Common.Infra.Model.IWorkspace {
    /// <summary>
    /// 指定した文字列からワークスペースのパス部分を削除した文字列を返します。
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    string TrimWorekspacePath (string path);

    /// <summary>
    /// インポート領域内のファイルかどうかを検証します。
    /// </summary>
    /// <param name="fileSystemInfo"></param>
    /// <returns></returns>
    bool ContainsImport (FileSystemInfo fileSystemInfo);

    /// <summary>
    /// 仮想領域内のファイルかどうかを検証します。
    /// </summary>
    /// <param name="fileSystemInfo"></param>
    /// <returns></returns>
    bool ContainsVirtual (FileSystemInfo fileSystemInfo);

    /// <summary>
    /// 物理領域内のファイルかどうかを検証します。
    /// </summary>
    /// <param name="fileSystemInfo"></param>
    /// <returns></returns>
    bool ContainsPhysical (FileSystemInfo fileSystemInfo);
  }
}
