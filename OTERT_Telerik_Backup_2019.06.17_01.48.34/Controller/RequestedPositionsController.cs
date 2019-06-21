using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class RequestedPositionsController {

        public int CountRequestedPositions() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.RequestedPositions.Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public List<RequestedPositionB> GetRequestedPositions() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<RequestedPositionB> data = (from us in dbContext.RequestedPositions
                                            select new RequestedPositionB {
                                                ID = us.ID,
                                                NameGR = us.NameGR,
                                                NameEN = us.NameEN,
                                            }).OrderBy(o => o.NameGR).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<RequestedPositionB> GetRequestedPositions(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<RequestedPositionB> data = (from us in dbContext.RequestedPositions
                                                     select new RequestedPositionB {
                                                         ID = us.ID,
                                                         NameGR = us.NameGR,
                                                         NameEN = us.NameEN,
                                                     }).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}