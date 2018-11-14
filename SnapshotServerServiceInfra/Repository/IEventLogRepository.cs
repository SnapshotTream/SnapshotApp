using Foxpict.Service.Infra.Model;

namespace Foxpict.Service.Infra.Repository
{
    public interface IEventLogRepository : IRepositoryBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEventLog New();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEventLog Load(long id);
    }
}