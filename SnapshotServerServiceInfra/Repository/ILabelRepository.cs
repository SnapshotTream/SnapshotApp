using System.Collections.Generic;
using System.Linq;
using Foxpict.Service.Infra.Model;

namespace Foxpict.Service.Infra.Repository {
  public interface ILabelRepository : IRepositoryBase {
    ILabel New ();

    ILabel Load (long id);

    /// <summary>
    /// ラベル名に一致するラベル情報を取得する
    /// </summary>
    /// <param name="name">正規化ラベル名</param>
    /// <returns></returns>
    ILabel LoadByName (string name);

    /// <summary>
    /// ラベル名に一致するラベル情報を取得する
    /// </summary>
    /// <param name="name">正規化ラベル名</param>
    /// <param name="ownerType">一致するオーナータイプの検索文字列</param>
    /// <returns></returns>
    ILabel LoadByName (string name, string ownerType);

    /// <summary>
    ///
    /// </summary>
    /// <param name="parentLabelId"></param>
    /// <returns></returns>
    IQueryable<ILabel> FindChildren (long parentLabelId);

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    IQueryable<ILabel> FindRoot ();

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    IEnumerable<ILabel> GetAll ();

    /// <summary>
    /// 正規化ラベル名を更新する
    /// </summary>
    /// <param name="label"></param>
    void UpdateNormalizeName (ILabel label);
  }
}
