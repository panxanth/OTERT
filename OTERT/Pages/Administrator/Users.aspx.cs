using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syncfusion.JavaScript;
using OTERT_Entity;
using OTERT.WebServices;

namespace OTERT.Pages.Administrator {

    public partial class Users : System.Web.UI.Page {

        protected Syncfusion.JavaScript.Web.Grid UGGrid;
        protected Syncfusion.JavaScript.Web.Accordion mainAccordion;
        protected Label lblSessionTest;

        protected void Page_Load(object sender, EventArgs e) {
            //if (!Page.IsPostBack) {
            //UGGrid.DataManager = new DataSource();
            //UGGrid.DataManager.URL = "http://otert/WebServices/OTERTWS.asmx/GetUserGroups";
            //UGGrid.DataManager.Adaptor = "WebMethodAdaptor";
            //UGGrid.Query = ;
            //UGGrid.DataBind();
            //}
            //List<int> enabledItem = new List<int>() { 0 };
            //List<int> disabledItem = new List<int>() { 1 };
            //mainAccordion.EnabledItems = enabledItem;
            //mainAccordion.DisableItems = disabledItem;
            //for (int i=1; i<1101; i++) { AddUserGroup("Panos "+i.ToString()); }
        }

    }

}