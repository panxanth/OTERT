using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class PTSGRPricelistController {

        public int CountPTSGRPricelists(string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    int count = 0;
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    if (!string.IsNullOrEmpty(recFilter)) {
                        count = dbContext.PTSGRPricelist.Where(recFilter).Count();
                    } else {
                        count = dbContext.PTSGRPricelist.Count();
                    }
                    return count;
                }
                catch (Exception) { return -1; }
            }
        }

        public List<PTSGRPricelistB> GetPTSGRPricelists() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<PTSGRPricelistB> data = (from us in dbContext.PTSGRPricelist
                                                  select new PTSGRPricelistB {
                                                      ID = us.ID,
                                                      Name = us.Name,
                                                      InstallationCost = us.InstallationCost,
                                                      ChargesPerMonth = us.ChargesPerMonth,
                                                      ChargesPerDay = us.ChargesPerDay,
                                                      MSNPerMonth = us.MSNPerMonth == null ? 0 : us.MSNPerMonth,
                                                      MSNPerDay = us.MSNPerDay == null ? 0 : us.MSNPerDay,
                                                      HasRouter = us.HasRouter,
                                                      SupportsMSN = us.SupportsMSN,
                                                      IsChargePerMonth = us.IsChargePerMonth
                                                  }).OrderBy(o => o.Name).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<PTSGRPricelistB> GetPTSGRPricelists(int recSkip, int recTake, string recFilter) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    IQueryable<PTSGRPricelistB> datatmp =  (from us in dbContext.PTSGRPricelist
                                                            select new PTSGRPricelistB {
                                                                ID = us.ID,
                                                                Name = us.Name,
                                                                InstallationCost = us.InstallationCost,
                                                                ChargesPerMonth = us.ChargesPerMonth,
                                                                ChargesPerDay = us.ChargesPerDay,
                                                                MSNPerMonth = us.MSNPerMonth == null ? 0 : us.MSNPerMonth,
                                                                MSNPerDay = us.MSNPerDay == null ? 0 : us.MSNPerDay,
                                                                HasRouter = us.HasRouter,
                                                                SupportsMSN = us.SupportsMSN,
                                                                IsChargePerMonth = us.IsChargePerMonth
                                                            });
                    if (!string.IsNullOrEmpty(recFilter)) { datatmp = datatmp.Where(recFilter); }
                    List<PTSGRPricelistB> data = datatmp.OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}