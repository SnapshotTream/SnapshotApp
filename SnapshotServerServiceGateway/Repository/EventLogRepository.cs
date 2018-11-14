using System.Linq;
using Hyperion.Pf.Entity.Repository;
using Microsoft.EntityFrameworkCore;
using Foxpict.Service.Infra;
using Foxpict.Service.Infra.Model;
using Foxpict.Service.Infra.Repository;
using Foxpict.Service.Model;

namespace Foxpict.Service.Gateway.Repository
{
    public class EventLogRepository : PixstockAppRepositoryBase<EventLog, IEventLog>, IEventLogRepository
    {
        public EventLogRepository(IAppDbContext context)
            : base((DbContext)context, "EventLog")
        {

        }

        public IEventLog Load(long id)
        {
            return _dbset.Where(x => x.Id == id).FirstOrDefault();
        }

        public IEventLog New()
        {
            var entity = new EventLog();
            return this.Add(entity);
        }
    }
}