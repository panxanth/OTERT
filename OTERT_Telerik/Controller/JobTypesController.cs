using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class JobTypesController {

        public int CountJobTypes() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.JobTypes.Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public List<JobTypeB> GetJobTypes() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<JobTypeB> data = (from us in dbContext.JobTypes
                                           select new JobTypeB {
                                               ID = us.ID,
                                               Name = us.Name
                                           }).OrderBy(o => o.ID).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<JobTypeB> GetJobTypes(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<JobTypeB> data = (from us in dbContext.JobTypes
                                           select new JobTypeB {
                                               ID = us.ID,
                                               Name = us.Name
                                           }).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}