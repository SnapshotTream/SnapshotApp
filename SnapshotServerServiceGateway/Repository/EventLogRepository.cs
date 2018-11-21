using System;
using System.Linq;
using Hyperion.Pf.Entity.Repository;
using Microsoft.EntityFrameworkCore;
using Snapshot.Server.Service.Infra;
using Snapshot.Server.Service.Infra.Model;
using Snapshot.Server.Service.Infra.Repository;
using Snapshot.Server.Service.Model;

namespace Snapshot.Server.Service.Gateway.Repository {
  public class EventLogRepository : PixstockAppRepositoryBase<EventLog, IEventLog>, IEventLogRepository {
    public EventLogRepository (IAppDbContext context) : base ((DbContext) context, "EventLog") {

    }

    public IEventLog Load (long id) {
      return _dbset.Where (x => x.Id == id).FirstOrDefault ();
    }

    public IQueryable<IEventLog> FindEventLog (DateTime beginDate, DateTime endDate) {
      return _dbset.Where (x => x.Datetime > beginDate && x.Datetime < endDate);
    }

    public IQueryable<IEventLog> FindEventLog (DateTime beginDate, DateTime endDate, string eventType) {
      return _dbset.Where (x => x.Datetime > beginDate && x.Datetime < endDate).Where (x => x.EventType == eventType);
    }

    public IEventLog New () {
      var entity = new EventLog ();
      return this.Add (entity);
    }
  }
}
