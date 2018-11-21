using System;
using System.Collections.Generic;
using NLog;
using RestSharp;
using Snapshot.Client.Bff.Dao;
using Snapshot.Client.Bff.Mock.Data;
using Snapshot.Client.Bff.Sdk;
using Snapshot.Client.Bff.Sdk.Dao;
using Snapshot.Client.Bff.Sdk.Data.Dto;
using Snapshot.Share.Common.Utils;

namespace Snapshot.Client.Bff.Core.Dao {
  public class EventLogDao : DaoBase, IEventLogDao {
    readonly Logger mLogger = LogManager.GetCurrentClassLogger ();

    public EventLogDao (DaoContext context) : base (context) { }

    public EventLog Load (long eventLogId) {
      var request = new RestRequest ("event/{id}", Method.GET);
      request.AddUrlSegment ("id", eventLogId);

      var response = mClient.Execute<ServerResponseApi<EventLog>> (request);
      return response.Data.Value;
    }

    public List<EventLog> Find (DateTime startDate, DateTime endDate) {
      var eventLogList = new List<EventLog> ();
      var request = new RestRequest ("event/s/{begin}-{end}", Method.GET);
      request.AddUrlSegment ("begin", DatetimeUtil.ToUnixTime (startDate));
      request.AddUrlSegment ("end", DatetimeUtil.ToUnixTime (endDate));

      var response = mClient.Execute<ServerResponseApi<List<EventLog>>> (request);
      return response.Data.Value;
    }
  }
}
