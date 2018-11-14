using Foxpict.Service.Infra;

namespace Foxpict.Service.Infra.Core.Model.Messaging {
  /// <summary>
  /// "Foxpict.MSG_NEWCATEGORY"メッセージのパラメータ
  /// /// </summary>
  public class NewCategoryMessageParameter : IMessagingParameter {
    public long CategoryId { get; set; }

    /// <summary>
    /// カテゴリパースを実施するかどうかのフラグ
    /// /// </summary>
    /// <value></value>
    public bool EnableCategoryParse { get; set; }
  }
}
