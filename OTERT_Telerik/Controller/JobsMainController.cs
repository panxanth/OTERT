using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class JobsMainController {

        public int CountJobsMain() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.JobsMain.Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public List<JobMainB> GetJobsMain() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<JobMainB> data = (from us in dbContext.JobsMain
                                           select new JobMainB {
                                               ID = us.ID,
                                               PageID = us.PageID,
                                               Name = us.Name
                                           }).OrderBy(o => o.ID).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<JobMainB> GetJobsMain(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<JobMainB> data = (from us in dbContext.JobsMain
                                           select new JobMainB {
                                               ID = us.ID,
                                               PageID = us.PageID,
                                               Name = us.Name
                                           }).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}