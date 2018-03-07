using System;
using System.Collections;
using System.Configuration;
using System.Web.UI;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using OTERT.Model;
using OTERT.Controller;
using OTERT_Entity;

namespace OTERT.Pages.UserPages {

    public partial class SateliteUplink : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected int pageID = 1;
        protected string pageTitle;
        protected int JobsID, CustomersID, SatelitesID;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Έργα > Έκτακτα > Δορυφορικά > Uplink";
                gridMain.MasterTableView.Caption = "Έργα > Έκτακτα > Δορυφορικά > Uplink";
                JobsID = -1;
                Session.Remove("JobsID");
                CustomersID = -1;
                Session.Remove("CustomersID");
                SatelitesID = -1;
                Session.Remove("SatelitesID");
            }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            try {
                TasksController cont = new TasksController();
                gridMain.VirtualItemCount = cont.CountTasksForPageID(pageID);
                gridMain.DataSource = cont.GetTasksForPage(pageID, recSkip, recTake);
            }
            catch (Exception) { }
        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) {
            if (e.Item is GridDataItem) {
                GridDataItem item = (GridDataItem)e.Item;
                ElasticButton img = (ElasticButton)item["btnDelete"].Controls[0];
                img.ToolTip = "Διαγραφή";
            }
        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                    JobsID = -1;
                    Session.Remove("JobsID");
                    CustomersID = -1;
                    Session.Remove("CustomersID");
                    GridEditableItem item = e.Item as GridEditableItem;
                    RadDropDownList ddlJobs = item.FindControl("ddlJobs") as RadDropDownList;
                    RadDropDownList ddlCustomers = item.FindControl("ddlCustomers") as RadDropDownList;
                    RadDropDownList ddlSatelites = item.FindControl("ddlSatelites") as RadDropDownList;
                    try {
                        TaskB currTask = e.Item.DataItem as TaskB;
                        JobsController cont1 = new JobsController();
                        ddlJobs.DataSource = cont1.GetJobsForPageID(pageID);
                        ddlJobs.DataTextField = "Name";
                        ddlJobs.DataValueField = "ID";
                        ddlJobs.DataBind();
                        CustomersController cont2 = new CustomersController();
                        ddlCustomers.DataSource = cont2.GetCustomers();
                        ddlCustomers.DataTextField = "NameGR";
                        ddlCustomers.DataValueField = "ID";
                        ddlCustomers.DataBind();
                        SatelitesController cont3 = new SatelitesController();
                        ddlSatelites.DataSource = cont3.GetSatelites();
                        ddlSatelites.DataTextField = "Name";
                        ddlSatelites.DataValueField = "ID";
                        ddlSatelites.DataBind();
                        if (currTask != null) {
                            ddlJobs.SelectedIndex = ddlJobs.FindItemByValue(currTask.JobID.ToString()).Index;
                            Session["JobsID"] = currTask.JobID;
                            ddlCustomers.SelectedIndex = ddlCustomers.FindItemByValue(currTask.CustomerID.ToString()).Index;
                            Session["CustomersID"] = currTask.CustomerID;
                            ddlSatelites.SelectedIndex = ddlSatelites.FindItemByValue(currTask.SateliteID.ToString()).Index;
                            Session["SatelitesID"] = currTask.SateliteID;
                        } else {
                            ddlJobs.SelectedIndex = 0;
                            Session["JobsID"] = ddlJobs.SelectedItem.Value;
                            ddlCustomers.SelectedIndex = 0;
                            Session["CustomersID"] = ddlCustomers.SelectedItem.Value;
                            ddlSatelites.SelectedIndex = 0;
                            Session["SatelitesID"] = ddlSatelites.SelectedItem.Value;
                        }
                    }
                    catch (Exception) { }
                }
            }
        }

        private void ShowErrorMessage(int errCode) {
            if (errCode == 1) {
                RadWindowManager1.RadAlert("Η συγκεκριμένη Παραγγελία σχετίζεται με κάποιο Έργο και δεν μπορεί να διαγραφεί!", 400, 200, "Σφάλμα", "");
            } else {
                RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
            }
        }

        protected void gridMain_UpdateCommand(object source, GridCommandEventArgs e) {
            /*
            var editableItem = ((GridEditableItem)e.Item);
            var ID = (int)editableItem.GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var curJob = dbContext.Jobs.Where(n => n.ID == ID).FirstOrDefault();
                if (curJob != null) {
                    editableItem.UpdateValues(curJob);
                    if (Session["SalesID"] != null) { SalesID = int.Parse(Session["SalesID"].ToString()); }
                    if (SalesID > -1) {
                        if (SalesID == 0) { curJob.SalesID = null; } else { curJob.SalesID = SalesID; }
                        SalesID = -1;
                        Session.Remove("SalesID");
                    }
                    if (Session["JobsMainID"] != null) { JobsMainID = int.Parse(Session["JobsMainID"].ToString()); }
                    if (JobsMainID > 0) {
                        curJob.JobsMainID = JobsMainID;
                        JobsMainID = -1;
                        Session.Remove("JobsMainID");
                    }
                    try { dbContext.SaveChanges(); }
                    catch (Exception) { ShowErrorMessage(-1); }
                }
            }
            */
        }

        protected void gridMain_InsertCommand(object source, GridCommandEventArgs e) {
            /*
            var editableItem = ((GridEditableItem)e.Item);
            using (var dbContext = new OTERTConnStr()) {
                var curJob = new Jobs();
                Hashtable values = new Hashtable();
                editableItem.ExtractValues(values);
                if (Session["SalesID"] != null) { SalesID = int.Parse(Session["SalesID"].ToString()); }
                if (Session["JobsMainID"] != null) { JobsMainID = int.Parse(Session["JobsMainID"].ToString()); }
                if (SalesID > -1 && JobsMainID > 0) {
                    try {
                        curJob.Name = (string)values["Name"];
                        if (SalesID == 0) { curJob.SalesID = null; } else { curJob.SalesID = SalesID; }
                        curJob.JobsMainID = JobsMainID;
                        curJob.MinimumTime = int.Parse((string)values["MinimumTime"]);
                        curJob.InvoiceCode = (string)values["InvoiceCode"];
                        dbContext.Jobs.Add(curJob);
                        dbContext.SaveChanges();
                    }
                    catch (Exception) { ShowErrorMessage(-1); }
                    finally {
                        SalesID = -1;
                        Session.Remove("SalesID");
                        JobsMainID = -1;
                        Session.Remove("JobsMainID");
                    }
                } else { ShowErrorMessage(-1); }
            }
            */
        }

        protected void gridMain_DeleteCommand(object source, GridCommandEventArgs e) {
            /*
            var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var curTask = dbContext.Tasks.Where(n => n.ID == ID).FirstOrDefault();
                if (curTask != null) {
                    dbContext.Tasks.Remove(curTask);
                    try { dbContext.SaveChanges(); }
                    catch (Exception ex) {
                        string err = ex.InnerException.InnerException.Message;
                        int errCode = -1;
                        if (err.StartsWith("The DELETE statement conflicted with the REFERENCE constraint")) { errCode = 1; }
                        ShowErrorMessage(errCode);
                    }
                }
            }
            */
        }

        protected void ddlJobs_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                JobsID = int.Parse(e.Value);
                Session["JobsID"] = JobsID;
            }
            catch (Exception) { }
        }

        protected void ddlCustomers_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                CustomersID = int.Parse(e.Value);
                Session["CustomersID"] = CustomersID;
            }
            catch (Exception) { }
        }

        protected void ddlSatelites_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                SatelitesID = int.Parse(e.Value);
                Session["SatelitesID"] = SatelitesID;
            }
            catch (Exception) { }
        }

    }

}