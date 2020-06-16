using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;
using Telerik.Web.Data.Extensions;

namespace OTERT.Controller {

    public class TaskLinesController {

        public List<TasksLineB> GetTaskLinesForInvoice(int invoiceID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<TasksLineB> data = (from us in dbContext.TasksLine
                                             select new TasksLineB {
                                                 ID = us.ID,
                                                 InvoiceID = us.InvoiceID,
                                                 Invoice = new InvoiceDTO { },
                                                 TaskID = us.TaskID,
                                                 Task = new TaskDTO {
                                                     ID = us.Tasks.ID,
                                                     RequestedPositionID = us.Tasks.RequestedPositionID,
                                                     DistanceID = us.Tasks.DistancesID,
                                                     Distance = new DistanceDTO { 
                                                         ID = us.Tasks.Distances.ID,
                                                         Position1 = us.Tasks.Distances.Position1,
                                                         Position2 = us.Tasks.Distances.Position2,
                                                         KM = us.Tasks.Distances.KM
                                                     },
                                                     DateTimeStartActual = us.Tasks.DateTimeStartActual,
                                                     DateTimeEndActual = us.Tasks.DateTimeEndActual,
                                                     DateTimeDurationActual = us.Tasks.DateTimeDurationActual,
                                                     CostCalculated = us.Tasks.CostCalculated,
                                                     AddedCharges = us.Tasks.AddedCharges,
                                                     CostActual = us.Tasks.CostActual,
                                                     InvoceComments = us.Tasks.InvoceComments
                                                 },
                                                 JobID = us.JobID,
                                                 Job = new JobDTO {
                                                     ID = us.Tasks.Jobs.ID,
                                                     Name = us.Tasks.Jobs.Name,
                                                     InvoiceCode = us.Tasks.Jobs.InvoiceCode
                                                 }
                                             }).Where(o => o.InvoiceID == invoiceID).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<JobB> GetDistinctJobsForInvoice(int invoiceID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<TasksLineB> tmp = GetTaskLinesForInvoice(invoiceID);
                    List<int> DistinctJobID = tmp.Select(x => x.JobID).Distinct().ToList();
                    tmp.Clear();
                    List<JobB> DistinctJobs = new List<JobB>();
                    JobsController jCont = new JobsController();
                    foreach (int jid in DistinctJobID) {
                        JobB curjob = jCont.GetJob(jid);
                        if (curjob != null) { DistinctJobs.Add(curjob); }
                    }
                    DistinctJobID.Clear();
                    List<JobB> SortedList = DistinctJobs.OrderBy(o => o.Name).ToList();
                    DistinctJobs.Clear();
                    return SortedList;
                }
                catch (Exception) { return null; }
            }
        }

    }

}