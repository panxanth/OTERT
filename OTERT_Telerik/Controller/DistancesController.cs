using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class DistancesController {

        public int CountDistances() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.Distances.Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public DistanceB GetDistance(int DistanceID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    DistanceB data = (from us in dbContext.Distances
                                         select new DistanceB {
                                             ID = us.ID,
                                             JobsMainID = us.JobsMainID,
                                             JobsMain = new JobMainDTO { ID = us.JobsMain.ID, PageID = us.JobsMain.PageID, Name = us.JobsMain.Name },
                                             Description = us.Position1 + " - " + us.Position2 + " (" + us.KM.ToString() + " km)",
                                             Position1 = us.Position1,
                                             Position2 = us.Position2,
                                             KM = us.KM
                                        }).Where(o => o.ID == DistanceID).FirstOrDefault();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<DistanceB> GetDistances(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<DistanceB> data = (from us in dbContext.Distances
                                            select new DistanceB {
                                                ID = us.ID,
                                                JobsMainID = us.JobsMainID,
                                                JobsMain = new JobMainDTO { ID = us.JobsMain.ID, PageID = us.JobsMain.PageID, Name = us.JobsMain.Name },
                                                Description = us.Position1 + " - " + us.Position2 + " (" + us.KM.ToString() + " km)",
                                                Position1 = us.Position1,
                                                Position2 = us.Position2,
                                                KM = us.KM
                                            }).OrderBy(o => o.Position1).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<DistanceB> GetDistancesForPageID(int PageID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<DistanceB> data = (from us in dbContext.Distances
                                            select new DistanceB {
                                                ID = us.ID,
                                                JobsMainID = us.JobsMainID,
                                                JobsMain = new JobMainDTO { ID = us.JobsMain.ID, PageID = us.JobsMain.PageID, Name = us.JobsMain.Name },
                                                Description = us.Position1 + " - " + us.Position2 + " (" + us.KM.ToString() + " km)",
                                                Position1 = us.Position1,
                                                Position2 = us.Position2,
                                                KM = us.KM
                                            }).Where(k => k.JobsMain.PageID == PageID).OrderBy(o => o.Position1).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}