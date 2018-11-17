using System.Collections.Generic;
using Snapshot.Client.Bff.Sdk.Data.Dto;

namespace Snapshot.Client.Bff.Sdk.Dao {
  public interface ILabelDao {
    /// <summary>
    /// すべてのラベルを読み込みます
    /// </summary>
    /// <returns></returns>
    ICollection<Label> LoadLabel ();

    /// <summary>
    /// ラベル情報を読み込みます
    /// </summary>
    /// <param name="labelId"></param>
    /// <returns></returns>
    Label LoadLabel (long labelId);

    /// <summary>
    /// ルート階層のラベル情報を読み込みます。
    /// </summary>
    /// <returns></returns>
    ICollection<Label> LoadRoot ();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="offset"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    ICollection<Category> LoadLabelLinkCategory (string query, int offset, int limit);
  }
}