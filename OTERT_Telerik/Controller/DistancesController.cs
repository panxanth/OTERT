using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using OTERT.Model;
using OTERT_Entity;
using Telerik.Web.UI;

namespace OTERT.Controller {

    public class DistancesController {

        public int CountDistances(string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    int count = 0;
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    if (!string.IsNullOrEmpty(recFilter)) {
                        //count = dbContext.Distances.Where(recFilter).Count();

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
                        count = datatmp.Where(recFilter).Count();
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

        public List<DistanceB> GetDistances(int recSkip, int recTake, string recFilter, GridSortExpressionCollection gridSortExxpressions) {
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
                    if (gridSortExxpressions.Count > 0) {
                        datatmp = datatmp.OrderBy(gridSortExxpressions[0].FieldName + " " + gridSortExxpressions[0].SortOrder);
                    } else {
                        datatmp = datatmp.OrderByDescending(o => o.ID);
                    }
                    List<DistanceB> data = datatmp.Skip(recSkip).Take(recTake).ToList();
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
                                            }).Where(k => k.JobsMain.PageID == PageID).OrderBy(o => o.Description).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<string> GetDistinctPositions() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<string> Posotion1 = (from us in dbContext.Distances select us.Position1).ToList();
                    List<string> Posotion2 = (from us in dbContext.Distances select us.Position2).ToList();
                    Posotion1.AddRange(Posotion2);
                    List<string> Positions = new HashSet<string>(Posotion1).ToList();
                    Positions.Sort();
                    return Positions;
                }
                catch (Exception) { return null; }
            }
        }

    }

}