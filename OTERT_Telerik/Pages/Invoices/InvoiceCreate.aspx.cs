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
        protected RadGrid gridJobs, gridTasks, gridSales;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected RadButton btnShow1, btnShow2, btnShowPrev2, btnShow3, btnShowPrev3, btnShow4, btnShowPrev4;
        protected PlaceHolder phStep1, phStep2, phStep3, phStep4;
        protected RadTextBox txtAccountNo;
        protected RadCheckBox chkIsLocked;
        protected string pageTitle, uploadedFilePath;
        protected UserB loggedUser;
        const string templatesFolder = "~/Templates/";
        const int PTSFromGreeceID = 14;
        const int PTSToGreeceID = 13;

        protected void Page_Load(object sender, EventArgs e) {
            wizardData wData;
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Τιμολόγια > Δημιουργία Νέου Τιμολογίου";
                dpDateFrom.SelectedDate = DateTime.Now.Date;
                dpDateTo.SelectedDate = DateTime.Now.Date;
                dpDateCreated.SelectedDate = DateTime.Now.Date;
                dpDatePay.SelectedDate = DateTime.Now.Date.AddDays(30);
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
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
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

        protected void gridTasks_ItemDataBound(object sender, GridItemEventArgs e) {
            if (e.Item is GridDataItem) {
                GridDataItem item = e.Item as GridDataItem;
                TaskB curItemData = item.DataItem as TaskB;
                if (curItemData.CostActual == null) {
                    CheckBox chk = (CheckBox)item.FindControl("chk");
                    chk.Enabled = false;
                    item.BackColor = System.Drawing.Color.LightCoral;
                }
            }
        }

        protected void gridSales_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            try {
                wizardData wData = readWizardSteps();
                TasksController tcont = new TasksController();
                SalesFormulasController sfcont = new SalesFormulasController();
                List<TaskB> selectedTasks = tcont.GetTasksForInvoice(wData.CustomerID, wData.DateFrom, wData.DateTo, wData.SelectedJobs, wData.SelectedTasks).OrderBy(o => o.DateTimeStartOrder).ToList();
                List<int> distinctJobsID = selectedTasks.Where(x => x.JobID != null).Select(x => x.JobID.Value).Distinct().ToList();
                List<tasksTotalsPerJob> tot = new List<tasksTotalsPerJob>();
                foreach (int curJobID in distinctJobsID) {
                    List<SalesFormulaB> curSaleFormulas = new List<SalesFormulaB>();
                    tasksTotalsPerJob curTotal = new tasksTotalsPerJob();
                    curTotal.JobID = curJobID;
                    List<TaskB> tasksForJobID = selectedTasks.Where(x => x.JobID == curJobID).ToList();
                    curTotal.JobName = tasksForJobID.First().Job.Name;
                    int? salesID = tasksForJobID.First().Job.SalesID;
                    if (salesID != null) {
                        curSaleFormulas = sfcont.GetSalesFormulas(salesID.Value);
                    }
                    curTotal.TasksCount = tasksForJobID.Count();
                    curTotal.TasksCost = 0;
                    curTotal.SalesCost = 0;
                    foreach (TaskB curTask in tasksForJobID) {
                        if (curTask.CostActual != null) {
                            curTotal.TasksCost += curTask.CostActual.Value;
                            if (curSaleFormulas.Count > 0) {
                                if (curSaleFormulas.First().Sale.Type == 1) {
                                    decimal tmpKM = curTask.Distance.KM;
                                    decimal tmpKmToChk = 0;
                                    foreach (SalesFormulaB sf in curSaleFormulas) {
                                        decimal distanceValue = (sf.Distance != null ? sf.Distance.Value : 0);
                                        if (tmpKM >= distanceValue - tmpKmToChk) {
                                            curTotal.SalesCost += curTotal.TasksCost * sf.SalePercent / 100;
                                            tmpKM -= distanceValue;
                                            tmpKmToChk += distanceValue;
                                        } else {
                                            curTotal.SalesCost += curTotal.TasksCost * sf.SalePercent / 100;
                                            break;
                                        }
                                    }
                                } else if (curSaleFormulas.First().Sale.Type == 2) {
                                    decimal salePercent = 0;
                                    if (curTask.Distance.KM > curSaleFormulas.Last().Distance) {
                                        salePercent = curSaleFormulas.Last().SalePercent;
                                    } else {
                                        foreach (SalesFormulaB sf in curSaleFormulas) {
                                            if (curTask.Distance.KM <= sf.Distance) {
                                                salePercent = sf.SalePercent;
                                                break;
                                            }
                                        }
                                    }
                                    curTotal.SalesCost += curTotal.TasksCost * salePercent / 100;
                                }
                            }
                        }
                    }
                    tot.Add(curTotal);
                }
                gridSales.DataSource = tot;
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
            //Session["wizardStep"] = wData;
            //showWizardSteps(wData);
            //gridSales.Rebind();
            //wizardData wData = readWizardSteps();
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    OTERT_Entity.Invoices curInvoice;
                    curInvoice = new OTERT_Entity.Invoices();
                    curInvoice.CustomerID = wData.CustomerID;
                    curInvoice.DateFrom = wData.DateFrom;
                    curInvoice.DateTo = wData.DateTo;
                    curInvoice.RegNo = wData.Code;
                    curInvoice.IsLocked = wData.locked;
                    curInvoice.DatePaid = wData.DatePayed;
                    curInvoice.DateCreated = wData.DateCreated;
                    TasksController tcont = new TasksController();
                    List<TaskB> invTasks = tcont.GetTasksForInvoice(curInvoice.CustomerID, wData.DateFrom, wData.DateTo, wData.SelectedJobs, wData.SelectedTasks);
                    decimal totalTasksCost = 0;
                    foreach (TaskB curTask in invTasks) { totalTasksCost += curTask.CostActual.GetValueOrDefault(); }
                    curInvoice.TasksLineAmount = totalTasksCost;
                    dbContext.Invoices.Add(curInvoice);
                    dbContext.SaveChanges();
                    foreach (TaskB curTask in invTasks) {
                        TasksLine newTaskLine = new TasksLine();
                        newTaskLine.InvoiceID = curInvoice.ID;
                        newTaskLine.TaskID = curTask.ID;
                        newTaskLine.JobID = curTask.JobID.GetValueOrDefault();
                        Tasks curTaskEntity = dbContext.Tasks.Where(s => s.ID == curTask.ID).First();
                        curTaskEntity.IsLocked = true;
                        DateTime dnow = DateTime.Now;
                        //curTaskEntity.PaymentDateActual = dnow;
                        curTaskEntity.PaymentDateCalculated = wData.DatePayed;
                        curTaskEntity.PaymentDateOrder = wData.DateCreated;
                        dbContext.TasksLine.Add(newTaskLine);
                    }
                    dbContext.SaveChanges();
                }
                catch (Exception) { }
                Response.Redirect("~/Pages/Invoices/InvoiceShow.aspx", false);
            }
        }

        protected void btnShowPrev3_Click(object sender, EventArgs e) {
            wizardData wData = readWizardSteps();
            wData.Step = 2;
            Session["wizardStep"] = wData;
            showWizardSteps(wData);
        }

        protected void btnShow4_Click(object sender, EventArgs e) {
            wizardData wData = readWizardSteps();
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    OTERT_Entity.Invoices curInvoice;
                    curInvoice = new OTERT_Entity.Invoices();
                    curInvoice.CustomerID = wData.CustomerID;
                    curInvoice.DateFrom = wData.DateFrom;
                    curInvoice.DateTo = wData.DateTo;
                    curInvoice.RegNo = wData.Code;
                    curInvoice.IsLocked = wData.locked;
                    curInvoice.DatePaid = wData.DatePayed;
                    curInvoice.DateCreated = wData.DateCreated;
                    TasksController tcont = new TasksController();
                    List<TaskB> invTasks = tcont.GetTasksForInvoice(curInvoice.CustomerID, wData.DateFrom, wData.DateTo, wData.SelectedJobs, wData.SelectedTasks);
                    decimal totalTasksCost = 0;
                    foreach (TaskB curTask in invTasks){ totalTasksCost += curTask.CostActual.GetValueOrDefault(); }
                    curInvoice.TasksLineAmount = totalTasksCost;
                    /*
                    foreach (GridDataItem item in gridSales.MasterTableView.Items) {
                        CheckBox chk = (CheckBox)item.FindControl("chk");
                        decimal? tasksValue = decimal.Parse(item["TasksCost"].Text);
                        decimal? salesValue = decimal.Parse(item["SalesCost"].Text);
                        curInvoice.TasksLineAmount = tasksValue;
                        if (chk.Checked) {
                            curInvoice.DiscountLineAmount = salesValue;
                        }
                        dbContext.Invoices.Add(curInvoice);
                    }
                    */
                    dbContext.Invoices.Add(curInvoice);
                    dbContext.SaveChanges();
                    foreach (TaskB curTask in invTasks) {
                        TasksLine newTaskLine = new TasksLine();
                        newTaskLine.InvoiceID = curInvoice.ID;
                        newTaskLine.TaskID = curTask.ID;
                        newTaskLine.JobID = curTask.JobID.GetValueOrDefault();
                        dbContext.TasksLine.Add(newTaskLine);
                    }
                    dbContext.SaveChanges();
                }
                catch (Exception) { }
                Response.Redirect("/Pages/Invoices/InvoiceShow.aspx", false);
            }
            //Session["wizardStep"] = wData;
            //showWizardSteps(wData);
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
                if (chk.Enabled == true) { chk.Checked = true; }
            }
        }

        protected void btnDeSelectAllTasks_Click(object sender, EventArgs e) {
            foreach (GridDataItem item in gridTasks.MasterTableView.Items) {
                CheckBox chk = (CheckBox)item.FindControl("chk");
                chk.Checked = false;
            }
        }

        protected void btnSelectAllSales_Click(object sender, EventArgs e) {
            foreach (GridDataItem item in gridSales.MasterTableView.Items) {
                CheckBox chk = (CheckBox)item.FindControl("chk");
                chk.Checked = true;
            }
        }

        protected void btnDeSelectAllSales_Click(object sender, EventArgs e) {
            foreach (GridDataItem item in gridSales.MasterTableView.Items) {
                CheckBox chk = (CheckBox)item.FindControl("chk");
                chk.Checked = false;
            }
        }

    }

}