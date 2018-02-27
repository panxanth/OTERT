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

    public partial class SatelitesList : System.Web.UI.Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Δορυφόρων";
                gridMain.MasterTableView.Caption = "Δορυφόροι";
            }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            try {
                SatelitesController cont = new SatelitesController();
                gridMain.VirtualItemCount = cont.CountSatelites();
                gridMain.DataSource = cont.GetSatelites(recSkip, recTake);
            }
            catch (Exception) { }

        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) {

        }

        private void ShowErrorMessage(int errCode) {
            if (errCode == 1) {
                RadWindowManager1.RadAlert("Ο συγκεκριμένος Δορυφόρος σχετίζεται με κάποια Παραγγελία και δεν μπορεί να διαγραφεί!", 400, 200, "Σφάλμα", "");
            } else {
                RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
            }
        }

        protected void gridMain_UpdateCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            var ID = (int)editableItem.GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var curSat = dbContext.Satelites.Where(n => n.ID == ID).FirstOrDefault();
                if (curSat != null) {
                    editableItem.UpdateValues(curSat);
                    try { dbContext.SaveChanges(); }
                    catch (Exception) { ShowErrorMessage(-1); }
                }
            }
        }

        protected void gridMain_InsertCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            using (var dbContext = new OTERTConnStr()) {
                var curSat = new Satelites();
                Hashtable values = new Hashtable();
                editableItem.ExtractValues(values);
                curSat.Name = (string)values["Name"];
                curSat.Frequency = (string)values["Frequency"];
                dbContext.Satelites.Add(curSat);
                try { dbContext.SaveChanges(); }
                catch (Exception) { ShowErrorMessage(-1); }
            }
        }

        protected void gridMain_DeleteCommand(object source, GridCommandEventArgs e) {
            var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var curSat = dbContext.Satelites.Where(n => n.ID == ID).FirstOrDefault();
                if (curSat != null) {
                    dbContext.Satelites.Remove(curSat);
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