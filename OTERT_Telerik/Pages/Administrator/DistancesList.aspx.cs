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

namespace OTERT.Pages.Administrator {

    public partial class DistancesList : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;
        protected int JobsMainID;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Αποστάσεων";
                gridMain.MasterTableView.Caption = "Αποστάσεις";
                JobsMainID = -1;
                Session.Remove("JobsMainID");
            }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            try {
                DistancesController cont = new DistancesController();
                gridMain.VirtualItemCount = cont.CountDistances();
                gridMain.DataSource = cont.GetDistances(recSkip, recTake);
            }
            catch (Exception) { }
        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) { }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {
            if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                JobsMainID = -1;
                Session.Remove("JobsMainID");
                GridEditableItem item = e.Item as GridEditableItem;
                RadDropDownList ddlJobsMain = item.FindControl("ddlJobsMain") as RadDropDownList;
                try {
                    DistanceB currDistance = e.Item.DataItem as DistanceB;
                    JobsMainController cont = new JobsMainController();
                    ddlJobsMain.DataSource = cont.GetJobsMain();
                    ddlJobsMain.DataTextField = "Name";
                    ddlJobsMain.DataValueField = "ID";
                    ddlJobsMain.DataBind();
                    if (currDistance != null) {
                        ddlJobsMain.SelectedIndex = ddlJobsMain.FindItemByValue(currDistance.JobsMainID.ToString()).Index;
                        Session["JobsMainID"] = currDistance.JobsMainID;
                    } else {
                        ddlJobsMain.SelectedIndex = 0;
                        Session["JobsMainID"] = ddlJobsMain.SelectedItem.Value;
                    }
                }
                catch (Exception) { }
            }
        }

        private void ShowErrorMessage(int errCode) {
            if (errCode == 1) {
                RadWindowManager1.RadAlert("Το συγκεκριμένο Ζεύγος σχετίζεται με κάποια Παραγγελία και δεν μπορεί να διαγραφεί!", 400, 200, "Σφάλμα", "");
            } else {
                RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
            }
        }

        protected void gridMain_UpdateCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            var ID = (int)editableItem.GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var curDistance = dbContext.Distances.Where(n => n.ID == ID).FirstOrDefault();
                if (curDistance != null) {
                    editableItem.UpdateValues(curDistance);
                    if (Session["JobsMainID"] != null) { JobsMainID = int.Parse(Session["JobsMainID"].ToString()); }
                    if (JobsMainID > 0) {
                        curDistance.JobsMainID = JobsMainID;
                        JobsMainID = -1;
                        Session.Remove("JobsMainID");
                    }
                    try { dbContext.SaveChanges(); }
                    catch (Exception) { ShowErrorMessage(-1); }
                }
            }
        }

        protected void gridMain_InsertCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            using (var dbContext = new OTERTConnStr()) {
                var newDistance = new Distances();
                Hashtable values = new Hashtable();
                editableItem.ExtractValues(values);
                if (Session["JobsMainID"] != null) { JobsMainID = int.Parse(Session["JobsMainID"].ToString()); }
                if (JobsMainID > 0) {
                    try {
                        newDistance.JobsMainID = JobsMainID;
                        newDistance.Position1 = (string)values["Position1"];
                        newDistance.Position2 = (string)values["Position2"];
                        newDistance.KM = int.Parse((string)values["KM"]);
                        dbContext.Distances.Add(newDistance);
                        dbContext.SaveChanges();
                    }
                    catch (Exception) { ShowErrorMessage(-1); }
                    finally {
                        JobsMainID = -1;
                        Session.Remove("JobsMainID");
                    }
                } else { ShowErrorMessage(-1); }
            }
        }

        protected void gridMain_DeleteCommand(object source, GridCommandEventArgs e) {
            var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var curDistance = dbContext.Distances.Where(n => n.ID == ID).FirstOrDefault();
                if (curDistance != null) {
                    dbContext.Distances.Remove(curDistance);
                    try { dbContext.SaveChanges(); }
                    catch (Exception ex) {
                        string err = ex.InnerException.InnerException.Message;
                        int errCode = -1;
                        if (err.StartsWith("The DELETE statement conflicted with the REFERENCE constraint")) { errCode = 1; }
                        ShowErrorMessage(errCode);
                    }
                }
            }
        }

        protected void ddlJobsMain_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                JobsMainID = int.Parse(e.Value);
                Session["JobsMainID"] = JobsMainID;
            }
            catch (Exception) { }
        }

    }

}