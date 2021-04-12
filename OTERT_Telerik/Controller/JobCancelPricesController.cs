using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class JobCancelPricesController {

        public int CountJobCancelPrices(int jobsID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.JobCancelPrices.Where(k => k.JobsID == jobsID).Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public List<JobCancelPriceB> GetJobCancelPrices(int jobsID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<JobCancelPriceB> data = (from us in dbContext.JobCancelPrices
                                                  select new JobCancelPriceB {
                                                        ID = us.ID,
                                                        JobsID = us.JobsID,
                                                        Name = us.Name,
                                                        Price = us.Price
                                                  }).Where(k => k.JobsID == jobsID).OrderBy(o => o.ID).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<JobCancelPriceB> GetJobCancelPrices(int jobsID, int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<JobCancelPriceB> data = (from us in dbContext.JobCancelPrices
                                                  select new JobCancelPriceB {
                                                      ID = us.ID,
                                                      JobsID = us.JobsID,
                                                      Name = us.Name,
                                                      Price = us.Price
                                                  }).Where(k => k.JobsID == jobsID).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}