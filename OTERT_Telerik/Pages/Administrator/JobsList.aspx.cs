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

    public partial class JobsList : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;
        protected int SalesID;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Κατηγοριών Έργων";
                gridMain.MasterTableView.Caption = "Κατηγορίες Έργων";
                SalesID = -1;
                Session.Remove("SalesID");
            }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            try {
                JobsController cont = new JobsController();
                gridMain.VirtualItemCount = cont.CountJobs();
                gridMain.DataSource = cont.GetJobs(recSkip, recTake);
            }
            catch (Exception) { }
        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                if (e.Item is GridDataItem) {
                    GridDataItem item = (GridDataItem)e.Item;
                    ElasticButton img = (ElasticButton)item["btnDelete"].Controls[0];
                    img.ToolTip = "Διαγραφή";
                }
            } else if (e.Item.OwnerTableView.Name == "FormulaDetails") {
                if (e.Item is GridDataItem) {
                    GridDataItem item = (GridDataItem)e.Item;
                    ElasticButton img = (ElasticButton)item["btnDelete2"].Controls[0];
                    img.ToolTip = "Διαγραφή";
                }
            } else if (e.Item.OwnerTableView.Name == "CancelDetails") {
                if (e.Item is GridDataItem) {
                    GridDataItem item = (GridDataItem)e.Item;
                    ElasticButton img = (ElasticButton)item["btnDelete3"].Controls[0];
                    img.ToolTip = "Διαγραφή";
                }
            }
        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                    SalesID = -1;
                    Session.Remove("SalesID");
                    GridEditableItem item = e.Item as GridEditableItem;
                    RadDropDownList list = item.FindControl("ddlSale") as RadDropDownList;
                    try {
                        JobB currJob = e.Item.DataItem as JobB;
                        SalesController cont = new SalesController();
                        list.DataSource = cont.GetSales();
                        list.DataTextField = "Name";
                        list.DataValueField = "ID";
                        list.DataBind();
                        list.Items.Insert(0, new DropDownListItem("Χωρίς Έκπτωση", "0"));
                        if (currJob != null) {
                            list.SelectedIndex = list.FindItemByValue(currJob.SalesID.ToString()).Index;
                            Session["SalesID"] = currJob.SalesID;
                        } else {
                            list.SelectedIndex = 0;
                            Session["SalesID"] = list.SelectedItem.Value;
                        }
                    }
                    catch (Exception) { }
                } else if (e.Item is GridDataItem) {
                    GridDataItem item = e.Item as GridDataItem;
                    Label lblST = item.FindControl("lblSale") as Label;
                    JobB currJob = e.Item.DataItem as JobB;
                    if (currJob.SalesID == null) { lblST.Text = "Χωρίς Έκπτωση"; } else { lblST.Text = currJob.Sale.Name; }
                }
            }
        }

        protected void gridMain_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e) {
            if (e.DetailTableView.Name == "FormulaDetails") {
                GridTableView detailtabl = (GridTableView)e.DetailTableView;
                int recSkip = detailtabl.CurrentPageIndex * gridMain.PageSize;
                int recTake = detailtabl.PageSize;
                GridDataItem parentItem = (GridDataItem)detailtabl.ParentItem;
                int salesID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
                JobFormulasController cont = new JobFormulasController();
                detailtabl.VirtualItemCount = cont.CountJobFormulas(salesID);
                detailtabl.DataSource = cont.GetJobFormulas(salesID, recSkip, recTake);
            } else if (e.DetailTableView.Name == "CancelDetails") {
                GridTableView detailtabl = (GridTableView)e.DetailTableView;
                int recSkip = detailtabl.CurrentPageIndex * gridMain.PageSize;
                int recTake = detailtabl.PageSize;
                GridDataItem parentItem = (GridDataItem)detailtabl.ParentItem;
                int salesID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
                JobCancelPricesController cont = new JobCancelPricesController();
                detailtabl.VirtualItemCount = cont.CountJobCancelPrices(salesID);
                detailtabl.DataSource = cont.GetJobCancelPrices(salesID, recSkip, recTake);
            }
        }

        private void ShowErrorMessage(int errCode) {
            if (errCode == 1) {
                RadWindowManager1.RadAlert("Η συγκεκριμένη Κατηγορία Έργου σχετίζεται με κάποιο Έργο και δεν μπορεί να διαγραφεί!", 400, 200, "Σφάλμα", "");
            } else {
                RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
            }
        }

        protected void gridMain_UpdateCommand(object source, GridCommandEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
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
                        try { dbContext.SaveChanges(); }
                        catch (Exception ex) { ShowErrorMessage(-1); }
                    }
                }
            } else if (e.Item.OwnerTableView.Name == "FormulaDetails") {
                var editableItem = ((GridEditableItem)e.Item);
                var ID = (int)editableItem.GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curJobFurmula = dbContext.JobFormulas.Where(n => n.ID == ID).FirstOrDefault();
                    if (curJobFurmula != null) {
                        editableItem.UpdateValues(curJobFurmula);
                        try { dbContext.SaveChanges(); }
                        catch (Exception) { ShowErrorMessage(-1); }
                    }
                }
            } else if (e.Item.OwnerTableView.Name == "CancelDetails") {
                var editableItem = ((GridEditableItem)e.Item);
                var ID = (int)editableItem.GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curCancel = dbContext.JobCancelPrices.Where(n => n.ID == ID).FirstOrDefault();
                    if (curCancel != null) {
                        editableItem.UpdateValues(curCancel);
                        try { dbContext.SaveChanges(); }
                        catch (Exception) { ShowErrorMessage(-1); }
                    }
                }
            }
        }

        protected void gridMain_InsertCommand(object source, GridCommandEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                var editableItem = ((GridEditableItem)e.Item);
                using (var dbContext = new OTERTConnStr()) {
                    var curJob = new Jobs();
                    Hashtable values = new Hashtable();
                    editableItem.ExtractValues(values);
                    if (Session["SalesID"] != null) { SalesID = int.Parse(Session["SalesID"].ToString()); }
                    if (SalesID > -1) {
                        try {
                            curJob.Name = (string)values["Name"];
                            if (SalesID==0) { curJob.SalesID = null; } else { curJob.SalesID = SalesID; }
                            curJob.MinimumTime = int.Parse((string)values["MinimumTime"]);
                            curJob.InvoiceCode = (string)values["InvoiceCode"];
                            dbContext.Jobs.Add(curJob);
                            dbContext.SaveChanges();
                        }
                        catch (Exception) { ShowErrorMessage(-1); }
                        finally {
                            SalesID = -1;
                            Session.Remove("SalesID");
                        }
                    } else { ShowErrorMessage(-1); }
                }
            } else if (e.Item.OwnerTableView.Name == "FormulaDetails") {
                GridTableView detailtabl = (GridTableView)e.Item.OwnerTableView;
                GridDataItem parentItem = (GridDataItem)detailtabl.ParentItem;
                int jobsID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
                var editableItem = ((GridEditableItem)e.Item);
                using (var dbContext = new OTERTConnStr()) {
                    var curJobFurmula = new JobFormulas();
                    Hashtable values = new Hashtable();
                    editableItem.ExtractValues(values);
                    curJobFurmula.JobsID = jobsID;
                    curJobFurmula.Formula = (string)values["Formula"];
                    curJobFurmula.Condition = (string)values["Condition"];
                    dbContext.JobFormulas.Add(curJobFurmula);
                    try { dbContext.SaveChanges(); }
                    catch (Exception) { ShowErrorMessage(-1); }
                }
            } else if (e.Item.OwnerTableView.Name == "CancelDetails") {
                GridTableView detailtabl = (GridTableView)e.Item.OwnerTableView;
                GridDataItem parentItem = (GridDataItem)detailtabl.ParentItem;
                int jobsID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
                var editableItem = ((GridEditableItem)e.Item);
                using (var dbContext = new OTERTConnStr()) {
                    var curCancel = new JobCancelPrices();
                    Hashtable values = new Hashtable();
                    editableItem.ExtractValues(values);
                    curCancel.JobsID = jobsID;
                    curCancel.Price = decimal.Parse((string)values["Price"]);
                    dbContext.JobCancelPrices.Add(curCancel);
                    try { dbContext.SaveChanges(); }
                    catch (Exception) { ShowErrorMessage(-1); }
                }
            }
        }

        protected void gridMain_DeleteCommand(object source, GridCommandEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curJob = dbContext.Jobs.Where(n => n.ID == ID).FirstOrDefault();
                    if (curJob != null) {
                        dbContext.Jobs.Remove(curJob);
                        try { dbContext.SaveChanges(); }
                        catch (Exception ex) {
                            string err = ex.InnerException.InnerException.Message;
                            int errCode = -1;
                            if (err.StartsWith("The DELETE statement conflicted with the REFERENCE constraint")) { errCode = 1; }
                            ShowErrorMessage(errCode);
                        }
                    }
                }
            } else if (e.Item.OwnerTableView.Name == "FormulaDetails") {
                var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curJobFurmula = dbContext.JobFormulas.Where(n => n.ID == ID).FirstOrDefault();
                    if (curJobFurmula != null) {
                        dbContext.JobFormulas.Remove(curJobFurmula);
                        try { dbContext.SaveChanges(); }
                        catch (Exception) { ShowErrorMessage(-1); }
                    }
                }
            } else if (e.Item.OwnerTableView.Name == "CancelDetails") {
                var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curCancel = dbContext.JobCancelPrices.Where(n => n.ID == ID).FirstOrDefault();
                    if (curCancel != null) {
                        dbContext.JobCancelPrices.Remove(curCancel);
                        try { dbContext.SaveChanges(); }
                        catch (Exception) { ShowErrorMessage(-1); }
                    }
                }
            }
        }

        protected void ddlSale_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                SalesID = int.Parse(e.Value);
                Session["SalesID"] = SalesID;
            }
            catch (Exception) { }
        }

    }

}