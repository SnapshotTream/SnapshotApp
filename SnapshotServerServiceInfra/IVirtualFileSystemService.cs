using System.IO;
using Foxpict.Service.Infra.Model;

namespace Foxpict.Service.Infra {
  /// <summary>
  /// 仮想ファイルを操作するサービス
  /// </summary>
  public interface IVirtualFileSystemService {
    /// <summary>
    /// ファイルを元にシステム管理情報を作成します。
    /// </summary>
    /// <param name="workspace">登録先ワークスペース</param>
    /// <param name="file">登録対象のファイル</param>
    /// <returns>システム管理情報</returns>
    IFileMappingInfo PersistentFileMapping (IWorkspace workspace, FileInfo file);

    /// <summary>
    /// ACLファイルを作成します。
    /// </summary>
    /// <param name="file">対象ファイル（仮想領域内のファイルを指定します）</param>
    /// <param name="fileMappingInfo">対象ファイルのシステム管理情報</param>
    /// <returns></returns>
    IFileMappingInfo RegisterFile (FileInfo file, IFileMappingInfo fileMappingInfo);

  }
}
