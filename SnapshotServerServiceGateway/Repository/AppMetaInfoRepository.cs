using System.Linq;
using Hyperion.Pf.Entity.Repository;
using Microsoft.EntityFrameworkCore;
using Foxpict.Service.Infra;
using Foxpict.Service.Infra.Model;
using Foxpict.Service.Infra.Repository;
using Foxpict.Service.Model;

namespace Foxpict.Service.Gateway.Repository
{
    public class ThumbnailAppMetaInfoRepository : AppMetaInfoRepository, IThumbnailAppMetaInfoRepository
    {
        public ThumbnailAppMetaInfoRepository(IThumbnailDbContext context) : base((DbContext)context)
        {
        }
    }

    public class AppAppMetaInfoRepository : AppMetaInfoRepository, IAppAppMetaInfoRepository
    {
        public AppAppMetaInfoRepository(IAppDbContext context) : base((DbContext)context)
        {
        }
    }

    public abstract class AppMetaInfoRepository : GenericRepository<AppMetaInfo>
    {
        public AppMetaInfoRepository(DbContext context) : base((DbContext)context)
        {

        }

        public IAppMetaInfo Load(long id)
        {
            return _dbset.Where(x => x.Id == id).FirstOrDefault();
        }

        public IAppMetaInfo LoadByKey(string keyName)
        {
            return _dbset.Where(x => x.Key == keyName).FirstOrDefault();
        }

        public IAppMetaInfo New()
        {
            var entity = new AppMetaInfo();
            return this.Add(entity);
        }
    }
}