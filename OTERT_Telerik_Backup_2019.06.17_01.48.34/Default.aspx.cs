using System;
using System.Collections;
using System.Web.UI;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using OTERT.Model;
using OTERT_Entity;

public partial class Default : System.Web.UI.Page  {

    protected void Page_Load(object sender, EventArgs e) {
        Session.Remove("LogedInUsername");
        Session.Remove("LogedInUsergroupID");
        Session.Remove("LogedInUserDisplayName");
    }

}