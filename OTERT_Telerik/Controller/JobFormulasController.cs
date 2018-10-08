using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class JobFormulasController {

        public int CountJobFormulas(int jobsID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.JobFormulas.Where(k => k.JobsID == jobsID).Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public List<JobFormulaB> GetJobFormulas(int jobsID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<JobFormulaB> data = (from us in dbContext.JobFormulas
                                              select new JobFormulaB {
                                                    ID = us.ID,
                                                    JobsID = us.JobsID,
                                                    Condition = us.Condition,
                                                    Formula = us.Formula
                                              }).Where(k => k.JobsID == jobsID).OrderBy(o => o.ID).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<JobFormulaB> GetJobFormulas(int jobsID, int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<JobFormulaB> data = (from us in dbContext.JobFormulas
                                              select new JobFormulaB {
                                                  ID = us.ID,
                                                  JobsID = us.JobsID,
                                                  Condition = us.Condition,
                                                  Formula = us.Formula
                                              }).Where(k => k.JobsID == jobsID).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}