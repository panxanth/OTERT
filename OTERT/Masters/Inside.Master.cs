using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OTERT.Masters {

    public partial class Inside : System.Web.UI.MasterPage {


        protected void Page_Load(object sender, EventArgs e) {
            if (Session["LogedInUsername"] == null) { Response.Redirect("/Default.aspx", true); }
        }

    }

}