using System.Collections.Generic;
using Snapshot.Client.Bff.Sdk.Data.Dto;

namespace Snapshot.Client.Bff.Sdk.Dao {
  public interface ICategoryDao {

    /// <summary>
    /// カテゴリ情報を読み込みます
    /// </summary>
    /// <param name="categoryId">カテゴリID</param>
    /// <param name="offsetSubCategory">子階層カテゴリリストの取得開始位置</param>
    /// <param name="limitSubCategory">子階層カテゴリリストの取得最大数</param>
    /// <returns></returns>
    Category LoadCategory (long categoryId, int offsetSubCategory = 0, int limitSubCategory = 100000, int offsetContent = 0);

    /// <summary>
    /// 親カテゴリ情報を取得する
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    Category LoadParentCategory (long categoryId);

    /// <summary>
    /// 条件に一致するカテゴリ一覧を取得します
    /// </summary>
    /// <param name="albumCategory">条件にアルバムカテゴリフラグの状態を含めます。nullの場合は、条件に含めません。</param>
    /// <param name="labelId">条件にラベル(AND)を含めます。nullの場合は、条件に含めません。</param>
    /// <returns></returns>
    List<Category> FindCategory (bool? albumCategory, long[] labelId);

    /// <summary>
    /// カテゴリ情報を永続化します。
    /// </summary>
    /// <param name="categoryId">カテゴリID</param>
    /// <param name="entity">永続化オブジェクト</param>
    void Update (long categoryId, Category entity);

    /// <summary>
    /// カテゴリのアートワークを自動設定します。
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    Category UpdateCategoryArtwork (long categoryId);

    /// <summary>
    /// カテゴリの既読状態を更新します。
    /// </summary>
    /// <param name="categoryId">カテゴリID</param>
    void UpdateRead(long categoryId);
  }
}
