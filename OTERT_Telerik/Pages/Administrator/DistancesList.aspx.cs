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
using System.Collections.Generic;

namespace OTERT.Pages.Administrator {

    public partial class DistancesList : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;
        protected int JobsMainID;
        protected UserB loggedUser;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Αποστάσεων";
                gridMain.MasterTableView.Caption = "Αποστάσεις";
                JobsMainID = -1;
                Session.Remove("JobsMainID");
                Session.Remove("Position1");
                Session.Remove("Position2");
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            string recFilter = gridMain.MasterTableView.FilterExpression;
            GridSortExpressionCollection gridSortExxpressions = gridMain.MasterTableView.SortExpressions;
            try {
                DistancesController cont = new DistancesController();
                gridMain.VirtualItemCount = cont.CountDistances(recFilter);
                gridMain.DataSource = cont.GetDistances(recSkip, recTake, recFilter, gridSortExxpressions);
            }
            catch (Exception) { }
        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {
            if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                JobsMainID = -1;
                Session.Remove("JobsMainID");
                Session.Remove("Position1");
                Session.Remove("Position2");
                GridEditableItem item = e.Item as GridEditableItem;
                RadAutoCompleteBox txtPosition1 = item.FindControl("txtPosition1") as RadAutoCompleteBox;
                RadAutoCompleteBox txtPosition2 = item.FindControl("txtPosition2") as RadAutoCompleteBox;
                RadDropDownList ddlJobsMain = item.FindControl("ddlJobsMain") as RadDropDownList;
                RadDropDownList ddlPosition1 = item.FindControl("ddlPosition1") as RadDropDownList;
                RadDropDownList ddlPosition2 = item.FindControl("ddlPosition2") as RadDropDownList;
                try {
                    DistanceB currDistance = e.Item.DataItem as DistanceB;
                    JobsMainController cont = new JobsMainController();
                    ddlJobsMain.DataSource = cont.GetJobsMain();
                    ddlJobsMain.DataTextField = "Name";
                    ddlJobsMain.DataValueField = "ID";
                    ddlJobsMain.DataBind();
                    DistancesController dcont = new DistancesController();
                    List<string> distPos = dcont.GetDistinctPositions();
                    ddlPosition1.DataSource = distPos;
                    ddlPosition1.DataBind();
                    ddlPosition2.DataSource = distPos;
                    ddlPosition2.DataBind();
                    if (currDistance != null) {
                        txtPosition1.Entries.Add(new AutoCompleteBoxEntry(currDistance.Position1, currDistance.Position1));
                        txtPosition2.Entries.Add(new AutoCompleteBoxEntry(currDistance.Position2, currDistance.Position2));
                        ddlJobsMain.SelectedIndex = ddlJobsMain.FindItemByValue(currDistance.JobsMainID.ToString()).Index;
                        Session["JobsMainID"] = currDistance.JobsMainID;
                        Session["Position1"] = currDistance.Position1;
                        Session["Position2"] = currDistance.Position2;
                    } else {
                        ddlJobsMain.SelectedIndex = 0;
                        Session["JobsMainID"] = ddlJobsMain.SelectedItem.Value;
                    }
                }
                catch (Exception) { }
            }
            if (e.Item is GridFilteringItem) {
                GridFilteringItem filterItem = (GridFilteringItem)e.Item;
                RadDropDownList cflist = (RadDropDownList)filterItem.FindControl("ddlJobsMainFilter");
                try {
                    JobsMainController ccont = new JobsMainController();
                    cflist.DataSource = ccont.GetJobsMain();
                    cflist.DataTextField = "Name";
                    cflist.DataValueField = "ID";
                    cflist.DataBind();
                    cflist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
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
                    if (JobsMainID > 0 && Session["Position1"] != null && Session["Position1"] != null) {
                        curDistance.Position1 = Session["Position1"].ToString();
                        curDistance.Position2 = Session["Position2"].ToString();
                        curDistance.JobsMainID = JobsMainID;
                        JobsMainID = -1;
                        Session.Remove("JobsMainID");
                        Session.Remove("Position1");
                        Session.Remove("Position2");
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
                if (JobsMainID > 0 && Session["Position1"] != null && Session["Position1"] != null) {
                    try {
                        newDistance.JobsMainID = JobsMainID;
                        newDistance.Position1 = Session["Position1"].ToString();
                        newDistance.Position2 = Session["Position2"].ToString();
                        newDistance.KM = int.Parse((string)values["KM"]);
                        dbContext.Distances.Add(newDistance);
                        dbContext.SaveChanges();
                    }
                    catch (Exception) { ShowErrorMessage(-1); }
                    finally {
                        JobsMainID = -1;
                        Session.Remove("JobsMainID");
                        Session.Remove("Position1");
                        Session.Remove("Position2");
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

        protected void txtPosition1_TextChanged(object sender, AutoCompleteTextEventArgs e) {
            RadAutoCompleteBox autoComplete = sender as RadAutoCompleteBox;
            Session["Position1"] = autoComplete.Entries[0].Text;
        }

        protected void txtPosition2_TextChanged(object sender, AutoCompleteTextEventArgs e) {
            RadAutoCompleteBox autoComplete = sender as RadAutoCompleteBox;
            Session["Position2"] = autoComplete.Entries[0].Text;
        }

        protected void ddlPosition1_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                RadAutoCompleteBox txtPosition = FindControlRecursive(Page, "txtPosition1") as RadAutoCompleteBox;
                txtPosition.Entries.Add(new AutoCompleteBoxEntry(e.Text, e.Text));
                Session["Position1"] = e.Text;
            }
            catch (Exception) { }
        }

        protected void ddlPosition2_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                RadAutoCompleteBox txtPosition = FindControlRecursive(Page, "txtPosition2") as RadAutoCompleteBox;
                txtPosition.Entries.Add(new AutoCompleteBoxEntry(e.Text, e.Text));
                Session["Position2"] = e.Text;
            }
            catch (Exception) { }
        }

        protected void ddlJobsMainFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("JobsMainID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(JobsMainID = " + e.Value + ")";
                gridMain.MasterTableView.GetColumn("JobsMainID").CurrentFilterFunction = GridKnownFunction.EqualTo;
                gridMain.MasterTableView.GetColumn("JobsMainID").CurrentFilterValue = e.Value;
            } else {
                gridMain.MasterTableView.GetColumn("JobsMainID").CurrentFilterFunction = GridKnownFunction.NoFilter;
                gridMain.MasterTableView.GetColumn("JobsMainID").CurrentFilterValue = null;
            }
            gridMain.MasterTableView.FilterExpression = finalExpression;
            ViewState[list.ClientID] = e.Value;
            gridMain.MasterTableView.Rebind();
        }

        protected void ddlJobsMainFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

        protected Control FindControlRecursive(Control container, string name) {
            if ((container.ID != null) && (container.ID.Equals(name))) { return container; }
            foreach (Control ctrl in container.Controls) {
                Control foundCtrl = FindControlRecursive(ctrl, name);
                if (foundCtrl != null) { return foundCtrl; }
            }
            return null;
        }

    }

}