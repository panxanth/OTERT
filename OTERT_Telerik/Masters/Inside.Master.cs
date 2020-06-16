using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OTERT.Model;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;

namespace OTERT.Masters {

    public partial class Inside : System.Web.UI.MasterPage {

        public int groupID = -1;
        public string userName = "";

        protected void Page_Load(object sender, EventArgs e) {
            if (Session["LoggedUser"] == null) {
                Response.Redirect("/Default.aspx", true);
            } else {
                UserB loggedUser = Session["LoggedUser"] as UserB;
                groupID = loggedUser.UserGroupID;
                userName = loggedUser.NameGR;
            } 
        }

    }

}