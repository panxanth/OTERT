using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class DistancesController {

        public int CountDistances(string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    int count = 0;
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    if (!string.IsNullOrEmpty(recFilter)) {
                        count = dbContext.Distances.Where(recFilter).Count();
                    } else {
                        count = dbContext.Distances.Count();
                    }
                    return count;
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

        public List<DistanceB> GetDistances() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    IQueryable<DistanceB> datatmp = (from us in dbContext.Distances
                                                     select new DistanceB {
                                                         ID = us.ID,
                                                         JobsMainID = us.JobsMainID,
                                                         JobsMain = new JobMainDTO { ID = us.JobsMain.ID, PageID = us.JobsMain.PageID, Name = us.JobsMain.Name },
                                                         Description = us.Position1 + " - " + us.Position2 + " (" + us.KM.ToString() + " km)",
                                                         Position1 = us.Position1,
                                                         Position2 = us.Position2,
                                                         KM = us.KM
                                                     });
                    List<DistanceB> data = datatmp.OrderBy(o => o.Position1).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<DistanceB> GetDistances(int recSkip, int recTake, string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    IQueryable<DistanceB> datatmp = (from us in dbContext.Distances
                                                    select new DistanceB {
                                                        ID = us.ID,
                                                        JobsMainID = us.JobsMainID,
                                                        JobsMain = new JobMainDTO { ID = us.JobsMain.ID, PageID = us.JobsMain.PageID, Name = us.JobsMain.Name },
                                                        Description = us.Position1 + " - " + us.Position2 + " (" + us.KM.ToString() + " km)",
                                                        Position1 = us.Position1,
                                                        Position2 = us.Position2,
                                                        KM = us.KM
                                                    });
                    if (!string.IsNullOrEmpty(recFilter)) { datatmp = datatmp.Where(recFilter); }
                    List<DistanceB> data = datatmp.OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
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