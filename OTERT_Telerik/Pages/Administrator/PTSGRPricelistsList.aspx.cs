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

    public partial class PTSGRPricelistsList : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;
        protected int LineTypeID, CustomerID;
        protected UserB loggedUser;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Τιμοκαταλόγου ΠΤΣ προς Ελλάδα";
                gridMain.MasterTableView.Caption = "Τιμοκατάλογος ΠΤΣ προς Ελλάδα";
                CustomerID = -1;
                Session.Remove("CustomerID");
                LineTypeID = -1;
                Session.Remove("LineTypeID");
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.MasterTableView.CurrentPageIndex * gridMain.MasterTableView.PageSize;
            int recTake = gridMain.MasterTableView.PageSize;
            try {
                PTSGRPricelistController cont = new PTSGRPricelistController();
                gridMain.VirtualItemCount = cont.CountPTSGRPricelists("");
                gridMain.DataSource = cont.GetPTSGRPricelists(recSkip, recTake, "");
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
            if (e.Item is GridEditableItem && (e.Item.IsInEditMode)) {
                GridEditableItem item = e.Item as GridEditableItem;
                (item["Name"].Controls[0] as TextBox).Width = Unit.Pixel(400);
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
                var currPricelist = dbContext.PTSGRPricelist.Where(n => n.ID == ID).FirstOrDefault();
                if (currPricelist != null) {
                    editableItem.UpdateValues(currPricelist);
                    try { dbContext.SaveChanges(); }
                    catch (Exception) { ShowErrorMessage(-1); }
                }
            }
        }

        protected void gridMain_InsertCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            using (var dbContext = new OTERTConnStr()) {
                var currPricelist = new PTSGRPricelist();
                Hashtable values = new Hashtable();
                editableItem.ExtractValues(values);
                try {
                    currPricelist.Name = (string)values["Name"];
                    currPricelist.InstallationCost = decimal.Parse((string)values["InstallationCost"]);
                    currPricelist.ChargesPerMonth = decimal.Parse((string)values["ChargesPerMonth"]);
                    currPricelist.ChargesPerDay = decimal.Parse((string)values["ChargesPerDay"]);
                    currPricelist.MSNPerMonth = decimal.Parse((string)values["MSNPerMonth"]);
                    currPricelist.MSNPerDay = decimal.Parse((string)values["MSNPerDay"]);
                    currPricelist.IsChargePerMonth = (bool)values["IsChargePerMonth"];
                    dbContext.PTSGRPricelist.Add(currPricelist);
                    dbContext.SaveChanges();
                }
                catch (Exception) { ShowErrorMessage(-1); }
            }
        }

        protected void gridMain_DeleteCommand(object source, GridCommandEventArgs e) {
            var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var currPricelist = dbContext.PTSGRPricelist.Where(n => n.ID == ID).FirstOrDefault();
                if (currPricelist != null) {
                    dbContext.PTSGRPricelist.Remove(currPricelist);
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

    }

}