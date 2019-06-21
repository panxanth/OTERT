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

    public partial class CountryPricelistsList : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;
        protected int LineTypeID, CustomerID;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Τιμοκαταλόγων Παρόχων Εξωτερικού";
                gridMain.MasterTableView.Caption = "Τιμοκατάλογοι Παρόχων Εξωτερικού";
                CustomerID = -1;
                Session.Remove("CustomerID");
                LineTypeID = -1;
                Session.Remove("LineTypeID");
            }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.MasterTableView.CurrentPageIndex * gridMain.MasterTableView.PageSize;
            int recTake = gridMain.MasterTableView.PageSize;
            try {
                CountryPricelistsController cont = new CountryPricelistsController();
                gridMain.VirtualItemCount = cont.CountCountryPricelists();
                gridMain.DataSource = cont.GetCountryPricelists(recSkip, recTake);
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
            if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                CustomerID = -1;
                Session.Remove("CustomerID");
                LineTypeID = -1;
                Session.Remove("LineTypeID");
                GridEditableItem item = e.Item as GridEditableItem;
                RadDropDownList ddlCustomer = item.FindControl("ddlCustomer") as RadDropDownList;
                RadDropDownList ddlLineType = item.FindControl("ddlLineType") as RadDropDownList;
                try {
                    CountryPricelistB currPricelist = e.Item.DataItem as CountryPricelistB;
                    CustomersController cont = new CustomersController();
                    ddlCustomer.DataSource = cont.GetProviders();
                    ddlCustomer.DataTextField = "NameGR";
                    ddlCustomer.DataValueField = "ID";
                    ddlCustomer.DataBind();
                    LineTypesController cont2 = new LineTypesController();
                    ddlLineType.DataSource = cont2.GetLineTypes();
                    ddlLineType.DataTextField = "Name";
                    ddlLineType.DataValueField = "ID";
                    ddlLineType.DataBind();
                    if (currPricelist != null) {
                        ddlCustomer.SelectedIndex = ddlCustomer.FindItemByValue(currPricelist.CustomerID.ToString()).Index;
                        Session["CustomerID"] = currPricelist.CustomerID;
                        ddlLineType.SelectedIndex = ddlLineType.FindItemByValue(currPricelist.LineTypeID.ToString()).Index;
                        Session["LineTypeID"] = currPricelist.LineTypeID;
                    } else {
                        ddlCustomer.SelectedIndex = 0;
                        Session["CustomerID"] = ddlCustomer.SelectedItem.Value;
                        ddlLineType.SelectedIndex = 0;
                        Session["LineTypeID"] = ddlLineType.SelectedItem.Value;
                    }
                }
                catch (Exception) { }
            }
        }

        private void ShowErrorMessage(int errCode) {
            switch (errCode) {
                case 1:
                    RadWindowManager1.RadAlert("O συγκεκριμένος Τιμοκατάλογος Παρόχου Εξωτερικού σχετίζεται με κάποια Παραγγελία και δεν μπορεί να διαγραφεί!", 400, 200, "Σφάλμα", "");
                    break;
                case 2:
                    RadWindowManager1.RadAlert("Ο συγκεκριμένος συνδυασμός Πελάτη και Είδους Γραμμής υπάρχει ήδη!", 400, 200, "Σφάλμα", "");
                    break;
                default:
                    RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
                    break;
            }
        }

        protected void gridMain_UpdateCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            var ID = (int)editableItem.GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var currPricelist = dbContext.CountryPricelist.Where(n => n.ID == ID).FirstOrDefault();
                if (currPricelist != null) {
                    editableItem.UpdateValues(currPricelist);
                    if (Session["CustomerID"] != null) { CustomerID = int.Parse(Session["CustomerID"].ToString()); }
                    if (CustomerID > 0) {
                        currPricelist.CustomerID = CustomerID;
                        CustomerID = -1;
                        Session.Remove("CustomerID");
                    }
                    if (Session["LineTypeID"] != null) { LineTypeID = int.Parse(Session["LineTypeID"].ToString()); }
                    if (LineTypeID > 0) {
                        currPricelist.LineTypeID = LineTypeID;
                        LineTypeID = -1;
                        Session.Remove("LineTypeID");
                    }
                    try { dbContext.SaveChanges(); }
                    catch (Exception ex) {
                        string err = ex.InnerException.InnerException.Message;
                        int errCode = -1;
                        if (err.StartsWith("Violation of UNIQUE KEY constraint 'UC_CustomerID_LineTypeID'")) { errCode = 2; }
                        ShowErrorMessage(errCode);
                    }
                }
            }
        }

        protected void gridMain_InsertCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            using (var dbContext = new OTERTConnStr()) {
                var currPricelist = new CountryPricelist();
                Hashtable values = new Hashtable();
                editableItem.ExtractValues(values);
                if (Session["CustomerID"] != null) { CustomerID = int.Parse(Session["CustomerID"].ToString()); }
                if (Session["LineTypeID"] != null) { LineTypeID = int.Parse(Session["LineTypeID"].ToString()); }
                if (CustomerID > 0 && LineTypeID > 0) {
                    try {
                        currPricelist.CustomerID = CustomerID;
                        currPricelist.LineTypeID = LineTypeID;
                        currPricelist.InstallationCost = decimal.Parse((string)values["InstallationCost"]);
                        currPricelist.MonthlyCharges = decimal.Parse((string)values["MonthlyCharges"]);
                        currPricelist.Internet = decimal.Parse((string)values["Internet"]);
                        currPricelist.MSN = decimal.Parse((string)values["MSN"]);
                        currPricelist.PaymentIsForWholeMonth = (bool)values["PaymentIsForWholeMonth"];
                        dbContext.CountryPricelist.Add(currPricelist);
                        dbContext.SaveChanges();
                    }
                    catch (Exception ex) {
                        string err = ex.InnerException.InnerException.Message;
                        int errCode = -1;
                        if (err.StartsWith("Violation of UNIQUE KEY constraint 'UC_CustomerID_LineTypeID'")) { errCode = 2; }
                        ShowErrorMessage(errCode);
                    }
                    finally {
                        CustomerID = -1;
                        Session.Remove("CustomerID");
                        LineTypeID = -1;
                        Session.Remove("LineTypeID");
                    }
                } else { ShowErrorMessage(-1); }
            }
        }

        protected void gridMain_DeleteCommand(object source, GridCommandEventArgs e) {
            var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var currPricelist = dbContext.CountryPricelist.Where(n => n.ID == ID).FirstOrDefault();
                if (currPricelist != null) {
                    dbContext.CountryPricelist.Remove(currPricelist);
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

        protected void ddlCustomer_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                CustomerID = int.Parse(e.Value);
                Session["CustomerID"] = CustomerID;
            }
            catch (Exception) { }
        }

        protected void ddlLineType_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                LineTypeID = int.Parse(e.Value);
                Session["LineTypeID"] = LineTypeID;
            }
            catch (Exception) { }
        }

    }

}