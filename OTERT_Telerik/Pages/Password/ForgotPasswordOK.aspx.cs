using OTERT.Controller;
using OTERT.Model;
using OTERT_Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace OTERT.Pages.Password {

    public partial class ForgotPasswordOK : Page {

        protected Literal litText;
        protected string pageTitle;
        protected int newID;
        

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Αλλαγή Κωδικού Χρήστη";
                newID = -1;
                Session.Remove("UserGroupID");
            }
            string result = Request.QueryString["result"];
            if (result == "True") {
                litText.Text = "Σας έχει αποσταλεί ένα μήνυμα στο εταιρικό σας email για να μπορέσετε να αλλάξετε τον Κωδικό σας.";
            } else {
                litText.Text = "Λόγω τεχνικού προβλήματος δεν είναι δυνατή η αλλαγή του κωδικού σας.<br/><br/>Παρακαλούμε ξαναπροσπαθήστε σε λίγα λεπτά ή επικοινωνήστε με τον διαχειριστή της εφαρμογής.";
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e) {
            Response.Redirect("~/Default.aspx");
        }

    }

}