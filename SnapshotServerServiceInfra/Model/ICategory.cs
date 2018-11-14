using System.Collections.Generic;

namespace Snapshot.Server.Service.Infra.Model
{
    /// <summary>
    ///
    /// </summary>
    public interface ICategory : Snapshot.Share.Common.Infra.Model.ICategory
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="category"></param>
        void SetParentCategory(ICategory category);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        ICategory GetParentCategory();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        List<IContent> GetContentList();

        /// <summary>
        /// 関連付けられているラベル情報の一覧を取得する
        /// </summary>
        /// <returns></returns>
        List<ILabel> GetLabelList();

        /// <summary>
        /// ラベル情報との関連付けを追加する
        /// </summary>
        /// <param name="label"></param>
        void AddLabelRelation(ILabel label, LabelCauseType cause, string causeDetail = "");
    }
}
