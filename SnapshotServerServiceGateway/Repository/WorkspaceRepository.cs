using System.Linq;
using Hyperion.Pf.Entity.Repository;
using Microsoft.EntityFrameworkCore;
using Foxpict.Service.Infra;
using Foxpict.Service.Infra.Model;
using Foxpict.Service.Infra.Repository;
using Foxpict.Service.Model;

namespace Foxpict.Service.Gateway.Repository
{
    public class WorkspaceRepository : PixstockAppRepositoryBase<Workspace, IWorkspace>, IWorkspaceRepository
    {
        public WorkspaceRepository(IAppDbContext context) : base((DbContext)context, "Workspace")
        {
        }

        /// <summary>
        /// Workpaceの読み込み
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IWorkspace Load(long id)
        {
            return _dbset.Where(x => x.Id == id).FirstOrDefault();
        }

        public IWorkspace New()
        {
            var entity = new Workspace();
            return this.Add(entity);
        }
    }
}