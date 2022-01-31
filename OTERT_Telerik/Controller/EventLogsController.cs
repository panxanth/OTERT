using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;
using Telerik.Web.UI;

namespace OTERT.Controller {

    public class EventLogsController {

        public int CountEventLogs() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.EventLogs.Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public List<EventLogB> GetEventLogs() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<EventLogB> data = (from us in dbContext.EventLogs
                                            select new EventLogB {
                                                ID = us.ID,
                                                EventDate = us.EventDate,
                                                EventID = us.EventID,
                                                EventDescription = us.EnevtDescription
                                             }).OrderBy(o => o.EventDate).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<EventLogB> GetEventLogs(int recSkip, int recTake, string recFilter, GridSortExpressionCollection gridSortExxpressions) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<EventLogB> data = (from us in dbContext.EventLogs
                                            select new EventLogB {
                                                ID = us.ID,
                                                EventDate = us.EventDate,
                                                EventID = us.EventID,
                                                EventDescription = us.EnevtDescription
                                            }).OrderBy(o => o.EventDate).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}