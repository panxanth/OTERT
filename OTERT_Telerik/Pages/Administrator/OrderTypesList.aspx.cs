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

    public partial class OrderTypesList : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Ειδών Παραγγελίας";
                gridMain.MasterTableView.Caption = "Είδη Παραγγελιών";
            }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            try {
                OrderTypesController cont = new OrderTypesController();
                gridMain.VirtualItemCount = cont.CountOrderTypes();
                gridMain.DataSource = cont.GetOrderTypes(recSkip, recTake);
            }
            catch (Exception) { }

        }

        private void ShowErrorMessage() {
            RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
        }

        protected void gridMain_UpdateCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            var ID = (int)editableItem.GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var orderType = dbContext.OrderTypes.Where(n => n.ID == ID).FirstOrDefault();
                if (orderType != null) {
                    editableItem.UpdateValues(orderType);
                    try { dbContext.SaveChanges(); }
                    catch (Exception) { ShowErrorMessage(); }
                }
            }
        }

    }

}