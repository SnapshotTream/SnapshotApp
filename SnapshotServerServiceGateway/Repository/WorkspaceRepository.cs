using System.Linq;
using Hyperion.Pf.Entity.Repository;
using Microsoft.EntityFrameworkCore;
using Snapshot.Server.Service.Infra;
using Snapshot.Server.Service.Infra.Model;
using Snapshot.Server.Service.Infra.Repository;
using Snapshot.Server.Service.Model;

namespace Snapshot.Server.Service.Gateway.Repository
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