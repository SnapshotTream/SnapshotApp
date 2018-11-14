using System.Linq;
using Hyperion.Pf.Entity.Repository;
using Microsoft.EntityFrameworkCore;
using Foxpict.Service.Infra;
using Foxpict.Service.Infra.Model;
using Foxpict.Service.Infra.Repository;
using Foxpict.Service.Model;

namespace Foxpict.Service.Gateway.Repository
{
    public class ThumbnailRepository : GenericRepository<Thumbnail>, IThumbnailRepository
    {
        public ThumbnailRepository(IThumbnailDbContext context) : base((DbContext)context)
        {
        }

        public void Delete(IThumbnail entity)
        {
            this.Delete((Thumbnail)entity);
        }

        public IQueryable<IThumbnail> FindByKey(string key)
        {
            return _dbset.Where(x => x.ThumbnailKey == key);
        }

        public IThumbnail Load(long id)
        {
            return _dbset.Where(x => x.Id == id).FirstOrDefault();
        }

        public IThumbnail New()
        {
            var entity = new Thumbnail();
            return this.Add(entity);
        }
    }
}