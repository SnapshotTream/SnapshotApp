namespace Snapshot.Server.Service.Infra.Model
{
    public interface IContent : Snapshot.Share.Common.Infra.Model.IContent
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="category"></param>
        void SetCategory(ICategory category);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        ICategory GetCategory();

        void SetFileMappingInfo(IFileMappingInfo fileMappingInfo);

        IFileMappingInfo GetFileMappingInfo();
    }
}
