
using Foxpict.Service.Infra.Model;

namespace Foxpict.Service.Infra.Repository
{
    public interface IContentRepository : IRepositoryBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IContent New();

        /// <summary>
        /// コンテント情報を読み込む
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IContent Load(long id);

        /// <summary>
        /// コンテント情報を読み込む
        /// </summary>
        /// <param name="fileMappingInfo"></param>
        /// <returns></returns>
        IContent Load(IFileMappingInfo fileMappingInfo);
    }
}