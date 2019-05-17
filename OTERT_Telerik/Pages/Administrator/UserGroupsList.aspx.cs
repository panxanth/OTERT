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

    public partial class UserGroupsList : System.Web.UI.Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Ομάδων Χρηστών";
                gridMain.MasterTableView.Caption = "Ομάδες Χρηστών";
            }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            string recFilter = gridMain.MasterTableView.FilterExpression;
            try {
                UserGroupsController cont = new UserGroupsController();
                gridMain.VirtualItemCount = cont.CountUserGroups(recFilter);
                gridMain.DataSource = cont.GetUserGroups(recSkip, recTake, recFilter);
            }
            catch (Exception) { }

        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) {

        }

        private void ShowErrorMessage(int errCode) {
            if (errCode == 1) {
                RadWindowManager1.RadAlert("Ο συγκεκριμένος Ρόλος σχετίζεται με κάποιον Χρήστη και δεν μπορεί να διαγραφεί!", 400, 200, "Σφάλμα", "");
            } else {
                RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
            }
        }

        protected void gridMain_UpdateCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            var ID = (int)editableItem.GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var ugroup = dbContext.UserGroups.Where(n => n.ID == ID).FirstOrDefault();
                if (ugroup != null) {
                    editableItem.UpdateValues(ugroup);
                    try { dbContext.SaveChanges(); }
                    catch (Exception) { ShowErrorMessage(-1); }
                }
            }
        }

        protected void gridMain_InsertCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            using (var dbContext = new OTERTConnStr()) {
                var ugroup = new UserGroups();
                Hashtable values = new Hashtable();
                editableItem.ExtractValues(values);
                ugroup.Name = (string)values["Name"];
                dbContext.UserGroups.Add(ugroup);
                try { dbContext.SaveChanges(); }
                catch (Exception) { ShowErrorMessage(-1); }
            }
        }

        protected void gridMain_DeleteCommand(object source, GridCommandEventArgs e) {
            var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var ugroup = dbContext.UserGroups.Where(n => n.ID == ID).FirstOrDefault();
                if (ugroup != null) {
                    dbContext.UserGroups.Remove(ugroup);
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