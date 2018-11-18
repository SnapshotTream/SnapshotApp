using Newtonsoft.Json;
using NLog;
using Snapshot.Server.Service.Infra.Core;
using Snapshot.Server.Service.Infra.Exception;
using Snapshot.Server.Service.Infra.Model;
using Snapshot.Server.Service.Infra.Repository;

namespace Snapshot.Server.Service.Apo.Builder {
  /// <summary>
  /// レスポンスデータモデル
  /// </summary>
  public class ApiResponseBuilder {
    private static Logger _logger = LogManager.GetCurrentClassLogger ();

    readonly ICategoryRepository mCategoryRepository;

    readonly IContentRepository mContentRepository;

    readonly ILabelRepository mLabelRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="categoryRepository"></param>
    /// <param name="contentRepository"></param>
    /// <param name="labelRepository"></param>
    public ApiResponseBuilder (
      ICategoryRepository categoryRepository,
      IContentRepository contentRepository,
      ILabelRepository labelRepository) {
      this.mCategoryRepository = categoryRepository;
      this.mContentRepository = contentRepository;
      this.mLabelRepository = labelRepository;
    }

    /// <summary>
    /// 適切な関連データを設定したAPIレスポンスデータを返します。
    /// </summary>
    /// <param name="category"></param>
    /// <param name="out_response"></param>
    public void AttachCategoryEntity (ICategory category, ResponseAapi<ICategory> out_response) {
      out_response.Value = category;

      // 関連データ設定
      out_response.Rel.Add ("labels", JsonConvert.SerializeObject (category.GetLabelList ()));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="out_respponse"></param>
    public void AttachContentEntity (long id, ResponseAapi<IContent> out_respponse) {
      // TODO: idパラメータの型をContentエンティティに変更するべき
      var content = mContentRepository.Load (id);
      if (content != null) {
        out_respponse.Value = content;
      } else {
        throw new InterfaceOperationException ("コンテント情報が見つかりません");
      }
    }

    /// <summary>
    ///
    /// /// </summary>
    /// <param name="id"></param>
    /// <param name="out_response"></param>
    public void AttachLabelEntity (long id, ResponseAapi<ILabel> out_response) {
      // TODO: idパラメータの型をLabelエンティティに変更するべき
      var label = mLabelRepository.Load (id);
      if (label != null) {
        out_response.Value = label;
      } else {
        throw new InterfaceOperationException ("ラベル情報が見つかりません");
      }
    }
  }
}
