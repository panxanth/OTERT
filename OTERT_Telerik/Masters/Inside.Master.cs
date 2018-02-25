using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OTERT_Telerik.Masters {

    public partial class Inside : System.Web.UI.MasterPage {

        public string groupID = "";
        public string userName = "";

        protected void Page_Load(object sender, EventArgs e) {
            if (Session["LogedInUsername"] == null) { Response.Redirect("/Default.aspx", true); }
            if (Session["LogedInUsergroupID"] != null) { groupID = Session["LogedInUsergroupID"].ToString(); }
            if (Session["LogedInUserDisplayName"] != null) { userName = Session["LogedInUserDisplayName"].ToString(); }
        }

    }

}