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
    static readonly Logger LOGGER = LogManager.GetCurrentClassLogger ();

    public EventLogDao (DaoContext context) : base (context) { }

    public EventLog Load (long eventLogId) {
      var request = new RestRequest ("event/{id}", Method.GET);
      request.AddUrlSegment ("id", eventLogId);

      var response = mClient.Execute<ServerResponseApi<EventLog>> (request);
      OutputResponseErrorMessage (response);
      return response.Data.Value;
    }

    public List<EventLog> Find (DateTime startDate, DateTime endDate, string eventType) {
      var eventLogList = new List<EventLog> ();
      var request = new RestRequest ("event/s/{begin}-{end}/{eventType}", Method.GET);
      request.AddUrlSegment ("begin", DatetimeUtil.ToUnixTime (startDate));
      request.AddUrlSegment ("end", DatetimeUtil.ToUnixTime (endDate));
      request.AddUrlSegment ("eventType", eventType);

      var response = mClient.Execute<ServerResponseApi<List<EventLog>>> (request);
      OutputResponseErrorMessage (response);
      return response.Data.Value;
    }

    public void UpdateValue (EventLog eventLog) {
      var request = new RestRequest ("event/{id}", Method.PATCH);
      request.RequestFormat = DataFormat.Json;
      request.AddUrlSegment ("id", eventLog.Id);
      request.AddBody (eventLog);

      var response = mClient.Execute (request);
      OutputResponseErrorMessage (response);
    }

    public EventLog Create (EventLog eventLog) {
      var request = new RestRequest ("event", Method.POST);
      request.RequestFormat = DataFormat.Json;
      request.AddBody (eventLog);

      var response = mClient.Execute<ServerResponseApi<EventLog>> (request);
      OutputResponseErrorMessage (response);
      return response.Data.Value;
    }
  }
}
