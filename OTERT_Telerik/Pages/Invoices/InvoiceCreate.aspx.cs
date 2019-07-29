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

        protected RadDatePicker dpDateFrom, dpDateTo, dpDateCreated, dpDatePay;
        protected RadDropDownList ddlCustomers;
        protected RadGrid gridJobs, gridTasks, gridJobsTotal;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected RadButton btnShow1, btnShow2, btnShowPrev2, btnShow3, btnShowPrev3, btnShow4, btnShowPrev4;
        protected PlaceHolder phStep1, phStep2, phStep3, phStep4;
        protected RadTextBox txtAccountNo;
        protected RadCheckBox chkIsLocked;
        protected string pageTitle, uploadedFilePath;
        const string templatesFolder = "~/Templates/";
        const int PTSFromGreeceID = 14;
        const int PTSToGreeceID = 13;

        protected void Page_Load(object sender, EventArgs e) {
            wizardData wData;
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Τμήμα Υποστήριξης (ΚΕΤ) - Λίστα Ημερ. Μεταδόσεων";
                dpDateFrom.SelectedDate = DateTime.Now.Date;
                dpDateTo.SelectedDate = DateTime.Now.Date;
                dpDateCreated.SelectedDate = DateTime.Now.Date;
                dpDatePay.SelectedDate = DateTime.Now.Date;
                wData = new wizardData();
                wData.Step = 1;
                Session["wizardStep"] = wData;
                showWizardSteps(wData);
                try {
                    CustomersController cont = new CustomersController();
                    ddlCustomers.DataSource = cont.GetCustomersForCountry(1);
                    ddlCustomers.DataTextField = "NameGR";
                    ddlCustomers.DataValueField = "ID";
                    ddlCustomers.DataBind();
                }
                catch (Exception) { }
            }
        }

        protected void showWizardSteps(wizardData wData) {
            switch (wData.Step) {
                case 1:
                    phStep1.Visible = true;
                    phStep2.Visible = false;
                    phStep3.Visible = false;
                    phStep4.Visible = false;
                    break;
                case 2:
                    phStep1.Visible = false;
                    phStep2.Visible = true;
                    phStep3.Visible = false;
                    phStep4.Visible = false;
                    break;
                case 3:
                    phStep1.Visible = false;
                    phStep2.Visible = false;
                    phStep3.Visible = true;
                    phStep4.Visible = false;
                    break;
                case 4:
                    phStep1.Visible = false;
                    phStep2.Visible = false;
                    phStep3.Visible = false;
                    phStep4.Visible = true;
                    break;
                default:
                    phStep1.Visible = false;
                    phStep2.Visible = false;
                    phStep3.Visible = false;
                    phStep4.Visible = false;
                    break;
            }
        } 

        protected wizardData readWizardSteps() {
            wizardData wData = (Session["wizardStep"] != null ? (wizardData)Session["wizardStep"] : new wizardData());
            return (wData);
        }

        protected void gridJobs_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            try {
                JobsController jcont = new JobsController();
                gridJobs.DataSource = jcont.GetJobs().Where(o => o.JobsMainID != PTSFromGreeceID && o.JobsMainID != PTSToGreeceID).OrderBy(o => o.Name);
            }
            catch (Exception) { }
        }

        protected void gridTasks_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            try {
                wizardData wData = readWizardSteps();
                TasksController tcont = new TasksController();
                gridTasks.DataSource = tcont.GetTasksForInvoice(wData.CustomerID, wData.DateFrom, wData.DateTo, wData.SelectedJobs).OrderBy(o => o.DateTimeStartOrder);
            }
            catch (Exception) { }
        }

        protected void gridJobsTotal_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            try {
                wizardData wData = readWizardSteps();
                TasksController tcont = new TasksController();
                List<TaskB> selectedTasks = tcont.GetTasksForInvoice(wData.CustomerID, wData.DateFrom, wData.DateTo, wData.SelectedJobs, wData.SelectedTasks).OrderBy(o => o.DateTimeStartOrder).ToList();
                List<int> distinctJobsID = selectedTasks.Where(x => x.JobID != null).Select(x => x.JobID.Value).Distinct().ToList();
                List<tasksTotalsPerJob> tot = new List<tasksTotalsPerJob>();
                foreach (int curJobID in distinctJobsID) {
                    tasksTotalsPerJob curTotal = new tasksTotalsPerJob();
                    curTotal.JobID = curJobID;
                    List<TaskB> tasksForJobID = selectedTasks.Where(x => x.JobID == curJobID).ToList();
                    curTotal.JobName = tasksForJobID.First().Job.Name;
                    curTotal.TasksCount = tasksForJobID.Count();
                    curTotal.TasksCost = 0;
                    foreach (TaskB curTask in tasksForJobID) {
                        if (curTask.CostActual != null) { curTotal.TasksCost += curTask.CostActual.Value; }
                    }
                    tot.Add(curTotal);
                }
                gridJobsTotal.DataSource = tot;
            }
            catch (Exception) { }
        }

        protected void btnShow1_Click(object sender, EventArgs e) {
            wizardData wData = readWizardSteps();
            wData.Step = 2;
            wData.CustomerID = int.Parse(ddlCustomers.SelectedItem.Value);
            wData.DateFrom = (dpDateFrom.SelectedDate != null ? (DateTime)dpDateFrom.SelectedDate : DateTime.Now);
            wData.DateTo = (dpDateTo.SelectedDate != null ? (DateTime)dpDateTo.SelectedDate : DateTime.Now);
            wData.DateCreated = (dpDateCreated.SelectedDate != null ? (DateTime)dpDateCreated.SelectedDate : DateTime.Now);
            wData.Code = txtAccountNo.Text.Trim();
            wData.DatePayed = (dpDatePay.SelectedDate != null ? (DateTime)dpDatePay.SelectedDate : DateTime.Now);
            wData.locked = (chkIsLocked.Checked != null ? (bool)chkIsLocked.Checked : false);
            Session["wizardStep"] = wData;
            showWizardSteps(wData);
            gridJobs.Rebind();
        }

        protected void btnShow2_Click(object sender, EventArgs e) {
            wizardData wData = readWizardSteps();
            wData.Step = 3;
            wData.SelectedJobs = new List<string>();
            foreach (GridDataItem item in gridJobs.MasterTableView.Items) {
                CheckBox chk = (CheckBox)item.FindControl("chk");
                string value = item.GetDataKeyValue("ID").ToString();
                if (chk.Checked) { wData.SelectedJobs.Add(value); }
            }
            Session["wizardStep"] = wData;
            showWizardSteps(wData);
            gridTasks.Rebind();
        }

        protected void btnShowPrev2_Click(object sender, EventArgs e) {
            wizardData wData = readWizardSteps();
            wData.Step = 1;
            Session["wizardStep"] = wData;
            showWizardSteps(wData);
        }

        protected void btnShow3_Click(object sender, EventArgs e) {
            wizardData wData = readWizardSteps();
            wData.Step = 4;
            wData.SelectedTasks = new List<string>();
            foreach (GridDataItem item in gridTasks.MasterTableView.Items) {
                CheckBox chk = (CheckBox)item.FindControl("chk");
                string value = item.GetDataKeyValue("ID").ToString();
                if (chk.Checked) { wData.SelectedTasks.Add(value); }
            }
            Session["wizardStep"] = wData;
            showWizardSteps(wData);
            gridJobsTotal.Rebind();
        }

        protected void btnShowPrev3_Click(object sender, EventArgs e) {
            wizardData wData = readWizardSteps();
            wData.Step = 2;
            Session["wizardStep"] = wData;
            showWizardSteps(wData);
        }

        protected void btnShow4_Click(object sender, EventArgs e) {
            wizardData wData = readWizardSteps();
            wData.Step = 4;
            Session["wizardStep"] = wData;
            showWizardSteps(wData);
        }

        protected void btnShowPrev4_Click(object sender, EventArgs e) {
            wizardData wData = readWizardSteps();
            wData.Step = 3;
            Session["wizardStep"] = wData;
            showWizardSteps(wData);
        }

        protected void btnSelectAllJobs_Click(object sender, EventArgs e) {
            foreach (GridDataItem item in gridJobs.MasterTableView.Items) {
                CheckBox chk = (CheckBox)item.FindControl("chk");
                chk.Checked = true;
            }
        }

        protected void btnDeSelectAllJobs_Click(object sender, EventArgs e) {
            foreach (GridDataItem item in gridJobs.MasterTableView.Items) {
                CheckBox chk = (CheckBox)item.FindControl("chk");
                chk.Checked = false;
            }
        }

        protected void btnSelectAllTasks_Click(object sender, EventArgs e) {
            foreach (GridDataItem item in gridTasks.MasterTableView.Items) {
                CheckBox chk = (CheckBox)item.FindControl("chk");
                chk.Checked = true;
            }
        }

        protected void btnDeSelectAllTasks_Click(object sender, EventArgs e) {
            foreach (GridDataItem item in gridTasks.MasterTableView.Items) {
                CheckBox chk = (CheckBox)item.FindControl("chk");
                chk.Checked = false;
            }
        }

    }

}