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

namespace OTERT.Pages.Invoices {

    public partial class InvoiceCreate : Page {

        protected RadDatePicker dpDate;
        protected RadGrid gridJobs;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected Button btnShow;
        protected string pageTitle, uploadedFilePath;
        const string templatesFolder = "~/Templates/";
        const int PTSFromGreeceID = 14;
        const int PTSToGreeceID = 13;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Τμήμα Υποστήριξης (ΚΕΤ) - Λίστα Ημερ. Μεταδόσεων";
                dpDate.SelectedDate = DateTime.Now.Date;
            }
        }

        protected void gridJobs_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            try {
                JobsController jcont = new JobsController();
                gridJobs.DataSource = jcont.GetJobs().Where(o => o.JobsMainID != PTSFromGreeceID && o.JobsMainID != PTSToGreeceID).OrderBy(o => o.Name);
            }
            catch (Exception) { }
        }

        protected void btnShow_Click(object sender, EventArgs e) {
            try {
                /*
                if (dpDate.SelectedDate != null) {
                    Response.Redirect("DailyListInside.aspx?date="+ dpDate.SelectedDate.ToString(), false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                */
                string items = string.Empty;
                foreach (GridDataItem item in gridJobs.MasterTableView.Items) {
                    CheckBox chk = (CheckBox)item.FindControl("chk");
                    string value = item.GetDataKeyValue("ID").ToString();
                    if (chk.Checked) { items += value + ","; }
                }
                items = items.TrimEnd(',');
            }
            catch (Exception) { }
        }

        protected void btnSelectAll_Click(object sender, EventArgs e) {
            foreach (GridDataItem item in gridJobs.MasterTableView.Items) {
                CheckBox chk = (CheckBox)item.FindControl("chk");
                chk.Checked = true;
            }
        }

        protected void btnDeSelectAll_Click(object sender, EventArgs e) {
            foreach (GridDataItem item in gridJobs.MasterTableView.Items) {
                CheckBox chk = (CheckBox)item.FindControl("chk");
                chk.Checked = false;
            }
        }


    }

}