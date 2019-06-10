using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class JobsController {

        public int CountJobs() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.Jobs.Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public JobB GetJob(int JobID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    JobB data = (from us in dbContext.Jobs
                                 select new JobB {
                                    ID = us.ID,
                                    JobsMainID = us.JobsMainID,
                                    JobsMain = new JobMainDTO { ID = us.JobsMain.ID, PageID = us.JobsMain.PageID, Name = us.JobsMain.Name },
                                    JobTypesID = us.JobTypesID,
                                    JobType = new JobTypeDTO { ID = us.JobTypes.ID, Name = us.JobTypes.Name },
                                    Name = us.Name,
                                    MinimumTime = us.MinimumTime,
                                    InvoiceCode = us.InvoiceCode,
                                    SalesID = us.SalesID,
                                    Sale = us.SalesID == null ? null : new SaleDTO { ID = us.Sales.ID, Name = us.Sales.Name, Type = us.Sales.Type }
                                }).Where(o => o.ID == JobID).FirstOrDefault();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<JobB> GetJobs() {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<JobB> data = (from us in dbContext.Jobs
                                       select new JobB {
                                           ID = us.ID,
                                           JobsMainID = us.JobsMainID,
                                           JobsMain = new JobMainDTO { ID = us.JobsMain.ID, PageID = us.JobsMain.PageID, Name = us.JobsMain.Name },
                                           JobTypesID = us.JobTypesID,
                                           JobType = new JobTypeDTO { ID = us.JobTypes.ID, Name = us.JobTypes.Name },
                                           Name = us.Name,
                                           MinimumTime = us.MinimumTime,
                                           InvoiceCode = us.InvoiceCode,
                                           SalesID = us.SalesID,
                                           Sale = us.SalesID == null ? null : new SaleDTO { ID = us.Sales.ID, Name = us.Sales.Name, Type = us.Sales.Type }
                                       }).OrderBy(o => o.ID).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<JobB> GetJobs(int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<JobB> data = (from us in dbContext.Jobs
                                        select new JobB {
                                            ID = us.ID,
                                            JobsMainID = us.JobsMainID,
                                            JobsMain = new JobMainDTO { ID = us.JobsMain.ID, PageID = us.JobsMain.PageID, Name = us.JobsMain.Name },
                                            JobTypesID = us.JobTypesID,
                                            JobType = new JobTypeDTO { ID = us.JobTypes.ID, Name = us.JobTypes.Name },
                                            Name = us.Name,
                                            MinimumTime = us.MinimumTime,
                                            InvoiceCode = us.InvoiceCode,
                                            SalesID = us.SalesID,
                                            Sale = us.SalesID == null ? null : new SaleDTO { ID = us.Sales.ID, Name = us.Sales.Name, Type = us.Sales.Type }
                                        }).OrderBy(o => o.ID).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<JobB> GetJobsForMainCategory(int JobsMainID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<JobB> data = (from us in dbContext.Jobs
                                       select new JobB {
                                           ID = us.ID,
                                           JobsMainID = us.JobsMainID,
                                           JobsMain = new JobMainDTO { ID = us.JobsMain.ID, PageID = us.JobsMain.PageID, Name = us.JobsMain.Name },
                                           JobTypesID = us.JobTypesID,
                                           JobType = new JobTypeDTO { ID = us.JobTypes.ID, Name = us.JobTypes.Name },
                                           Name = us.Name,
                                           MinimumTime = us.MinimumTime,
                                           InvoiceCode = us.InvoiceCode,
                                           SalesID = us.SalesID,
                                           Sale = us.SalesID == null ? null : new SaleDTO { ID = us.Sales.ID, Name = us.Sales.Name, Type = us.Sales.Type }
                                       }).Where(k => k.JobsMainID == JobsMainID).OrderBy(o => o.Name).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<JobB> GetJobsForPageID(int PageID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<JobB> data = (from us in dbContext.Jobs
                                       select new JobB {
                                           ID = us.ID,
                                           JobsMainID = us.JobsMainID,
                                           JobsMain = new JobMainDTO { ID = us.JobsMain.ID, PageID = us.JobsMain.PageID, Name = us.JobsMain.Name },
                                           JobTypesID = us.JobTypesID,
                                           JobType = new JobTypeDTO { ID = us.JobTypes.ID, Name = us.JobTypes.Name },
                                           Name = us.Name,
                                           MinimumTime = us.MinimumTime,
                                           InvoiceCode = us.InvoiceCode,
                                           SalesID = us.SalesID,
                                           Sale = us.SalesID == null ? null : new SaleDTO { ID = us.Sales.ID, Name = us.Sales.Name, Type = us.Sales.Type }
                                       }).Where(k => k.JobsMain.PageID == PageID).OrderBy(o => o.Name).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}