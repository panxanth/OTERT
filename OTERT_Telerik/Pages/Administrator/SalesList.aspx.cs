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

    public partial class SalesList : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;
        protected int SalesType;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Εκπτώσεων";
                gridMain.MasterTableView.Caption = "Εκπτώσεις";
                SalesType = -1;
                Session.Remove("SalesType");
            }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            try {
                SalesController cont = new SalesController();
                gridMain.VirtualItemCount = cont.CountSales();
                gridMain.DataSource = cont.GetSales(recSkip, recTake);
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
            } else if (e.Item.OwnerTableView.Name == "Details") {
                if (e.Item is GridDataItem) {
                    GridDataItem item = (GridDataItem)e.Item;
                    ElasticButton img = (ElasticButton)item["btnDelete2"].Controls[0];
                    img.ToolTip = "Διαγραφή";
                }
            }
        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                    SalesType = -1;
                    Session.Remove("SalesType");
                    GridEditableItem item = e.Item as GridEditableItem;
                    try {
                        SaleB currSale = e.Item.DataItem as SaleB;
                        RadDropDownList list = item.FindControl("ddlSaleType") as RadDropDownList;
                        list.Items.Clear();
                        list.Items.Add(new DropDownListItem("Κλιμακωτά", "1"));
                        list.Items.Add(new DropDownListItem("Σε όλο το ποσό", "2"));
                        list.DataBind();
                        if (currSale != null) {
                            list.SelectedIndex = list.FindItemByValue(currSale.Type.ToString()).Index;
                            Session["SalesType"] = currSale.Type;
                        } else {
                            list.SelectedIndex = 0;
                            Session["SalesType"] = list.SelectedItem.Value;
                        }
                    }
                    catch (Exception) { }
                } else if (e.Item is GridDataItem) {
                    GridDataItem item = e.Item as GridDataItem;
                    Label lblST = item.FindControl("lblSaleType") as Label;
                    SaleB currSale = e.Item.DataItem as SaleB;
                    if (currSale.Type == 1) { lblST.Text = "Κλιμακωτά"; } else if (currSale.Type == 2) { lblST.Text = "Σε όλο το ποσό"; }
                }
            }
        }

        protected void gridMain_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e) {
            if (e.DetailTableView.Name == "Details") {
                GridTableView detailtabl = (GridTableView)e.DetailTableView;
                int recSkip = detailtabl.CurrentPageIndex * gridMain.PageSize;
                int recTake = detailtabl.PageSize;
                GridDataItem parentItem = (GridDataItem)detailtabl.ParentItem;
                int salesID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
                SalesFormulasController cont = new SalesFormulasController();
                detailtabl.VirtualItemCount = cont.CountSalesFormulas(salesID);
                detailtabl.DataSource = cont.GetSalesFormulas(salesID, recSkip, recTake);
            }
        }

        private void ShowErrorMessage(int errCode) {
            if (errCode == 1) {
                RadWindowManager1.RadAlert("Η συγκεκριμένη Έκπτωση σχετίζεται με κάποια Κατηγορία Έργου και δεν μπορεί να διαγραφεί!", 400, 200, "Σφάλμα", "");
            } else {
                RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
            }
        }

        protected void gridMain_UpdateCommand(object source, GridCommandEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                var editableItem = ((GridEditableItem)e.Item);
                var ID = (int)editableItem.GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curSale = dbContext.Sales.Where(n => n.ID == ID).FirstOrDefault();
                    if (curSale != null) {
                        editableItem.UpdateValues(curSale);
                        if (Session["SalesType"] != null) { SalesType = int.Parse(Session["SalesType"].ToString()); }
                        if (SalesType > 0) {
                            curSale.Type = SalesType;
                            SalesType = -1;
                            Session.Remove("SalesType");
                        }
                        try { dbContext.SaveChanges(); }
                        catch (Exception) { ShowErrorMessage(-1); }
                    }
                }
            } else if (e.Item.OwnerTableView.Name == "Details") {
                var editableItem = ((GridEditableItem)e.Item);
                var ID = (int)editableItem.GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curSaleFurmula = dbContext.SalesFormulas.Where(n => n.ID == ID).FirstOrDefault();
                    if (curSaleFurmula != null) {
                        editableItem.UpdateValues(curSaleFurmula);
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
                    var curSale = new Sales();
                    Hashtable values = new Hashtable();
                    editableItem.ExtractValues(values);
                    if (Session["SalesType"] != null) { SalesType = int.Parse(Session["SalesType"].ToString()); }
                    if (SalesType > 0) {
                        try {
                            curSale.Name = (string)values["Name"];
                            curSale.Type = SalesType;
                            dbContext.Sales.Add(curSale);
                            dbContext.SaveChanges();
                        }
                        catch (Exception) { ShowErrorMessage(-1); }
                        finally {
                            SalesType = -1;
                            Session.Remove("SalesType");
                        }
                    } else { ShowErrorMessage(-1); }
                }
            } else if (e.Item.OwnerTableView.Name == "Details") {
                GridTableView detailtabl = (GridTableView)e.Item.OwnerTableView;
                GridDataItem parentItem = (GridDataItem)detailtabl.ParentItem;
                int salesID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
                var editableItem = ((GridEditableItem)e.Item);
                using (var dbContext = new OTERTConnStr()) {
                    var curSaleFurmula = new SalesFormulas();
                    Hashtable values = new Hashtable();
                    editableItem.ExtractValues(values);
                    curSaleFurmula.SalesID = salesID;
                    curSaleFurmula.Distance = decimal.Parse((string)values["Distance"]);
                    curSaleFurmula.SalePercent = decimal.Parse((string)values["SalePercent"]);
                    dbContext.SalesFormulas.Add(curSaleFurmula);
                    try { dbContext.SaveChanges(); }
                    catch (Exception) { ShowErrorMessage(-1); }
                }
            }
        }

        protected void gridMain_DeleteCommand(object source, GridCommandEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curSale = dbContext.Sales.Where(n => n.ID == ID).FirstOrDefault();
                    if (curSale != null) {
                        dbContext.Sales.Remove(curSale);
                        try { dbContext.SaveChanges(); }
                        catch (Exception ex) {
                            string err = ex.InnerException.InnerException.Message;
                            int errCode = -1;
                            if (err.StartsWith("The DELETE statement conflicted with the REFERENCE constraint")) { errCode = 1; }
                            ShowErrorMessage(errCode);
                        }
                    }
                }
            } else if (e.Item.OwnerTableView.Name == "Details") {
                var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curSaleFurmula = dbContext.SalesFormulas.Where(n => n.ID == ID).FirstOrDefault();
                    if (curSaleFurmula != null) {
                        dbContext.SalesFormulas.Remove(curSaleFurmula);
                        try { dbContext.SaveChanges(); }
                        catch (Exception) { ShowErrorMessage(-1); }
                    }
                }
            } 
        }

        protected void ddlSaleType_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                SalesType = int.Parse(e.Value);
                Session["SalesType"] = SalesType;
            }
            catch (Exception) { }
        }

    }

}