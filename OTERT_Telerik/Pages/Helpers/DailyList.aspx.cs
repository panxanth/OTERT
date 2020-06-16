using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Telerik.Web.UI.Calendar;
using Telerik.Windows.Documents.Flow.Model;
using Telerik.Windows.Documents.Flow.Model.Editing;
using Telerik.Windows.Documents.Flow.Model.Styles;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Documents.Common.FormatProviders;
using Telerik.Windows.Documents.Flow.FormatProviders.Docx;
using ExpressionParser;
using OTERT.Model;
using OTERT.Controller;
using OTERT_Entity;

namespace OTERT.Pages.Helpers {

    public partial class DailyList : Page {

        protected RadDatePicker dpDate;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected Button btnShow;
        protected string pageTitle, uploadedFilePath;
        protected UserB loggedUser;
        const string templatesFolder = "~/Templates/";

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Τμήμα Υποστήριξης (ΚΕΤ) - Λίστα Ημερ. Μεταδόσεων";
                dpDate.SelectedDate = DateTime.Now.Date;
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected void btnShow_Click(object sender, EventArgs e) {
            try {
                if (dpDate.SelectedDate != null) {
                    Response.Redirect("DailyListInside.aspx?date="+ dpDate.SelectedDate.ToString(), false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception) { }
        }

    }

}