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
using OTERT.Model;
using OTERT.Controller;
using OTERT_Entity;

namespace OTERT.Pages.Invoices {

    public partial class InvoiceEdit : Page {

        protected RadDatePicker dpDateFrom, dpDateTo, dpDateCreated, dpDatePay;
        protected RadDropDownList ddlCustomers;
        protected RadGrid gridJobs, gridTasks, gridSales;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected RadButton btnShow1, btnShow2, btnShowPrev2, btnShow3, btnShowPrev3, btnShow4, btnShowPrev4;
        protected PlaceHolder phStep1, phStep2, phStep3, phStep4;
        protected RadTextBox txtAccountNo;
        protected RadCheckBox chkIsLocked;
        protected Label lblCustomer, lblDateFrom, lblDateTo, lblDateCreated;
        protected string pageTitle, uploadedFilePath;
        protected UserB loggedUser;
        const string templatesFolder = "~/Templates/";
        const int PTSFromGreeceID = 14;
        const int PTSToGreeceID = 13;

        protected void Page_Load(object sender, EventArgs e) {
            wizardData wData;
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Τιμολόγια > Επεξεργασία Τιμολογίου";
                int invoiceID = -1;
                if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != string.Empty) {
                    int.TryParse(Request.QueryString["ID"], out invoiceID);
                }
                wData = new wizardData();
                if (invoiceID > 0) {
                    try {
                        InvoicesController cont = new InvoicesController();
                        InvoiceB singleInvoice = cont.GetInvoice(invoiceID);
                        if (singleInvoice != null) {
                            lblCustomer.Text = singleInvoice.Customer.NameGR;
                            lblDateFrom.Text = singleInvoice.DateFrom.GetValueOrDefault().ToString("dd/MM/yyyy");
                            lblDateTo.Text = singleInvoice.DateTo.GetValueOrDefault().ToString("dd/MM/yyyy");
                            lblDateCreated.Text = singleInvoice.DateCreated.GetValueOrDefault().ToString("dd/MM/yyyy");
                            txtAccountNo.Text = singleInvoice.RegNo;
                            dpDatePay.SelectedDate = singleInvoice.DatePaid;
                            chkIsLocked.Checked = singleInvoice.IsLocked;
                            wData.CustomerID = invoiceID;
                        }
                    }
                    catch (Exception) { }
                }
                Session["wizardStep"] = wData;
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected wizardData readWData() {
            wizardData wData = (Session["wizardStep"] != null ? (wizardData)Session["wizardStep"] : new wizardData());
            return (wData);
        }

        protected void btnUpdate_Click(object sender, EventArgs e) {
            wizardData wData = readWData();
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    OTERT_Entity.Invoices curInvoice = dbContext.Invoices.Where(o => o.ID == wData.CustomerID).FirstOrDefault();
                    curInvoice.RegNo = txtAccountNo.Text.Trim();
                    curInvoice.IsLocked = (chkIsLocked.Checked != null ? (bool)chkIsLocked.Checked : false);
                    curInvoice.DatePaid = (dpDatePay.SelectedDate != null ? (DateTime)dpDatePay.SelectedDate : DateTime.Now);
                    dbContext.SaveChanges();
                }
                catch (Exception ex) { }
                Response.Redirect("/Pages/Invoices/InvoiceShow.aspx", false);
            }
        }

    }

}