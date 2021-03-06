﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Telerik.Web.UI.Calendar;
using ExpressionParser;
using OTERT.Model;
using OTERT.Controller;
using OTERT_Entity;

namespace OTERT.Pages.UserPages {

    public partial class Downlink : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected int pageID = 11;
        protected string pageTitle, uploadedFilePath;
        protected int JobsID, CustomersID, DistancesID;
        protected UserB loggedUser;
        const string fileUploadFolder = "~/UploadedFiles/";

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Έργα > Έκτακτα > Downlink > Σταθεροί Δορυφορικοί Σταθμοί";
                gridMain.MasterTableView.Caption = "<div id='MTVCaption'>Έργα > Έκτακτα > Downlink > Σταθεροί Δορυφορικοί Σταθμοί</div>";
                JobsID = -1;
                Session.Remove("JobsID");
                CustomersID = -1;
                Session.Remove("CustomersID");
                DistancesID = -1;
                Session.Remove("DistancesID");
                uploadedFilePath = "";
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int taskID = -1;
            if (Request.QueryString["ID"] != null && Request.QueryString["ID"] != string.Empty) {
                int.TryParse(Request.QueryString["ID"], out taskID);
            }
            int recSkip = gridMain.MasterTableView.CurrentPageIndex * gridMain.MasterTableView.PageSize;
            int recTake = gridMain.MasterTableView.PageSize;
            string recFilter = gridMain.MasterTableView.FilterExpression;
            GridSortExpressionCollection gridSortExxpressions = gridMain.MasterTableView.SortExpressions;
            if (taskID < 0) {
                try {
                    TasksController cont = new TasksController();
                    gridMain.VirtualItemCount = cont.CountTasksForPageID(pageID, recFilter);
                    gridMain.DataSource = cont.GetTasksForPage(pageID, recSkip, recTake, recFilter, gridSortExxpressions);
                }
                catch (Exception) { }
            } else {
                try {
                    TasksController cont = new TasksController();
                    gridMain.VirtualItemCount = 1;
                    List<TaskB> ds = new List<TaskB>();
                    TaskB singleTask = cont.GetTask(taskID);
                    if (singleTask != null) { ds.Add(singleTask); }
                    gridMain.DataSource = ds;
                }
                catch (Exception) { }
            }
        }

        protected void gridMain_PreRender(object sender, EventArgs e) {
            gridMain.MasterTableView.GetColumn("ExpandColumn").Display = false;
        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                if (e.Item is GridDataItem) {
                    GridDataItem item = (GridDataItem)e.Item;
                    ElasticButton img = (ElasticButton)item["btnDelete"].Controls[0];
                    img.ToolTip = "Διαγραφή";
                    ImageButton btnUnlock = (ImageButton)item["btnUnlock"].Controls[0];
                    btnUnlock.ToolTip = "Ξεκλείδωμα";
                }
                if (e.Item is GridFilteringItem) {
                    GridFilteringItem filterItem = (GridFilteringItem)e.Item;
                    (filterItem["OrderDate"].Controls[0] as LiteralControl).Text = "Από: ";
                    (filterItem["OrderDate"].Controls[3] as LiteralControl).Text = "<br />Έως: ";
                    (filterItem["DateTimeStartActual"].Controls[0] as LiteralControl).Text = "Από: ";
                    (filterItem["DateTimeStartActual"].Controls[3] as LiteralControl).Text = "<br />Έως: ";
                    (filterItem["DateTimeStartOrder"].Controls[0] as LiteralControl).Text = "Από: ";
                    (filterItem["DateTimeStartOrder"].Controls[3] as LiteralControl).Text = "<br />Έως: ";
                    RadDateTimePicker OrderDateFrom = filterItem["OrderDate"].Controls[1] as RadDateTimePicker;
                    OrderDateFrom.TimePopupButton.Visible = false;
                    RadDateTimePicker OrderDateTo = filterItem["OrderDate"].Controls[4] as RadDateTimePicker;
                    OrderDateTo.TimePopupButton.Visible = false;
                    OrderDateFrom.DateInput.Attributes.Add("onchange", "javascript:UpdateTo('" + OrderDateFrom.ClientID + "', '" + OrderDateTo.ClientID + "');");
                    RadDateTimePicker startActualFrom = filterItem["DateTimeStartActual"].Controls[1] as RadDateTimePicker;
                    startActualFrom.TimePopupButton.Visible = false;
                    RadDateTimePicker startActualTo = filterItem["DateTimeStartActual"].Controls[4] as RadDateTimePicker;
                    startActualTo.TimePopupButton.Visible = false;
                    OrderDateFrom.DateInput.Attributes.Add("onchange", "javascript:UpdateTo('" + startActualFrom.ClientID + "', '" + startActualTo.ClientID + "');");
                    RadDateTimePicker StartOrderFrom = filterItem["DateTimeStartOrder"].Controls[1] as RadDateTimePicker;
                    StartOrderFrom.TimePopupButton.Visible = false;
                    RadDateTimePicker StartOrderΤο = filterItem["DateTimeStartOrder"].Controls[4] as RadDateTimePicker;
                    StartOrderΤο.TimePopupButton.Visible = false;
                    StartOrderFrom.DateInput.Attributes.Add("onchange", "javascript:UpdateTo('" + StartOrderFrom.ClientID + "', '" + StartOrderΤο.ClientID + "');");
                }
                if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                    GridEditableItem item = e.Item as GridEditableItem;
                    RadDateTimePicker dpOrderDate = (RadDateTimePicker)item["OrderDate"].Controls[0];
                    dpOrderDate.DateInput.Width = Unit.Pixel(250);
                    dpOrderDate.DatePopupButton.ToolTip = "Επιλογή ημερομηνίας παραγγελίας";
                    dpOrderDate.TimePopupButton.ToolTip = "Επιλογή ώρας παραγγελίας";
                    dpOrderDate.DateInput.DateFormat = "dd/MM/yyyy  HH:mm";
                    dpOrderDate.DateInput.DisplayDateFormat = "dd/MM/yyyy  HH:mm";
                    dpOrderDate.SharedTimeView.HeaderText = "Επιλέξτε Ώρα";
                    dpOrderDate.SharedTimeView.TimeFormat = "HH:mm";
                    RadDatePicker dpPaymentDateOrder = (RadDatePicker)item["PaymentDateOrder"].Controls[0];
                    dpPaymentDateOrder.DateInput.Width = Unit.Pixel(250);
                    RadDatePicker dpPaymentDateCalculated = (RadDatePicker)item["PaymentDateCalculated"].Controls[0];
                    dpPaymentDateCalculated.DateInput.Width = Unit.Pixel(250);
                    RadDatePicker dpPaymentDateActual = (RadDatePicker)item["PaymentDateActual"].Controls[0];
                    dpPaymentDateActual.DateInput.Width = Unit.Pixel(250);
                    RadDateTimePicker dpDateTimeStartOrder = (RadDateTimePicker)item["DateTimeStartOrder"].Controls[0];
                    dpDateTimeStartOrder.DateInput.Width = Unit.Pixel(250);
                    dpDateTimeStartOrder.DatePopupButton.ToolTip = "Επιλογή προγραμματισμένης ημερομηνίας έναρξης";
                    dpDateTimeStartOrder.TimePopupButton.ToolTip = "Επιλογή προγραμματισμένης ώρας έναρξης";
                    dpDateTimeStartOrder.DateInput.DateFormat = "dd/MM/yyyy  HH:mm";
                    dpDateTimeStartOrder.DateInput.DisplayDateFormat = "dd/MM/yyyy  HH:mm";
                    dpDateTimeStartOrder.SharedTimeView.HeaderText = "Επιλέξτε Ώρα";
                    dpDateTimeStartOrder.SharedTimeView.TimeFormat = "HH:mm";
                    //dpDateTimeStartOrder.SharedTimeView.Interval = new TimeSpan(0, 30, 0);
                    dpDateTimeStartOrder.AutoPostBackControl = Telerik.Web.UI.Calendar.AutoPostBackControl.Both;
                    dpDateTimeStartOrder.SelectedDateChanged += new SelectedDateChangedEventHandler(dpDate_SelectedIndexChanged);
                    RadDateTimePicker dpDateTimeEndOrder = (RadDateTimePicker)item["DateTimeEndOrder"].Controls[0];
                    dpDateTimeEndOrder.DateInput.Width = Unit.Pixel(250);
                    dpDateTimeEndOrder.DatePopupButton.ToolTip = "Επιλογή προγραμματισμένης ημερομηνίας λήξης";
                    dpDateTimeEndOrder.TimePopupButton.ToolTip = "Επιλογή προγραμματισμένης ώρας λήξης";
                    dpDateTimeEndOrder.DateInput.DateFormat = "dd/MM/yyyy  HH:mm";
                    dpDateTimeEndOrder.DateInput.DisplayDateFormat = "dd/MM/yyyy  HH:mm";
                    dpDateTimeEndOrder.SharedTimeView.HeaderText = "Επιλέξτε Ώρα";
                    dpDateTimeEndOrder.SharedTimeView.TimeFormat = "HH:mm";
                    //dpDateTimeEndOrder.SharedTimeView.Interval = new TimeSpan(0, 30, 0);
                    dpDateTimeEndOrder.AutoPostBackControl = Telerik.Web.UI.Calendar.AutoPostBackControl.Both;
                    dpDateTimeEndOrder.SelectedDateChanged += new SelectedDateChangedEventHandler(dpDate_SelectedIndexChanged);
                    RadDateTimePicker dpDateTimeStartActual = (RadDateTimePicker)item["DateTimeStartActual"].Controls[0];
                    dpDateTimeStartActual.DateInput.Width = Unit.Pixel(250);
                    dpDateTimeStartActual.DatePopupButton.ToolTip = "Επιλογή πραγματικής ημερομηνίας έναρξης";
                    dpDateTimeStartActual.TimePopupButton.ToolTip = "Επιλογή πραγματικής ώρας έναρξης";
                    dpDateTimeStartActual.DateInput.DateFormat = "dd/MM/yyyy  HH:mm";
                    dpDateTimeStartActual.DateInput.DisplayDateFormat = "dd/MM/yyyy  HH:mm";
                    dpDateTimeStartActual.SharedTimeView.HeaderText = "Επιλέξτε Ώρα";
                    dpDateTimeStartActual.SharedTimeView.TimeFormat = "HH:mm";
                    //dpDateTimeStartActual.SharedTimeView.Interval = new TimeSpan(0, 30, 0);
                    dpDateTimeStartActual.AutoPostBackControl = Telerik.Web.UI.Calendar.AutoPostBackControl.Both;
                    dpDateTimeStartActual.SelectedDateChanged += new SelectedDateChangedEventHandler(dpDate_SelectedIndexChanged);
                    RadDateTimePicker dpDateTimeEndActual = (RadDateTimePicker)item["DateTimeEndActual"].Controls[0];
                    dpDateTimeEndActual.DateInput.Width = Unit.Pixel(250);
                    dpDateTimeEndActual.DatePopupButton.ToolTip = "Επιλογή πραγματικής ημερομηνίας λήξης";
                    dpDateTimeEndActual.TimePopupButton.ToolTip = "Επιλογή πραγματικής ώρας λήξης";
                    dpDateTimeEndActual.DateInput.DateFormat = "dd/MM/yyyy  HH:mm";
                    dpDateTimeEndActual.DateInput.DisplayDateFormat = "dd/MM/yyyy  HH:mm";
                    dpDateTimeEndActual.SharedTimeView.HeaderText = "Επιλέξτε Ώρα";
                    dpDateTimeEndActual.SharedTimeView.TimeFormat = "HH:mm";
                    //dpDateTimeEndActual.SharedTimeView.Interval = new TimeSpan(0, 30, 0);
                    dpDateTimeEndActual.AutoPostBackControl = Telerik.Web.UI.Calendar.AutoPostBackControl.Both;
                    dpDateTimeEndActual.SelectedDateChanged += new SelectedDateChangedEventHandler(dpDate_SelectedIndexChanged);
                    CheckBox chkIsCanceled = (CheckBox)item.FindControl("chkIsCanceled");
                    chkIsCanceled.Enabled = true;
                    chkIsCanceled.AutoPostBack = true;
                    chkIsCanceled.CheckedChanged += new EventHandler(chkIsCanceled_CheckedChanged);
                }
            } else if (e.Item.OwnerTableView.Name == "AttachedFiles") {
                if (e.Item is GridDataItem) {
                    GridDataItem item = (GridDataItem)e.Item;
                    ElasticButton img = (ElasticButton)item["btnDeleteFile"].Controls[0];
                    img.ToolTip = "Διαγραφή";
                }
            }
        }

        protected void txtAddedCharges_TextChanged(object sender, EventArgs e) {
            TextBox txtAddedCharges = ((TextBox)(sender));
            GridEditableItem eitem = (GridEditableItem)txtAddedCharges.NamingContainer;
            calculateCosts(eitem);
        }

        protected void calculateCosts(GridEditableItem eitem) {
            DateTime nullDate = new DateTime(1900, 1, 1);
            RadDatePicker dpOrderStartDate = (RadDatePicker)eitem["DateTimeStartOrder"].Controls[0]; ;
            DateTime orderStartDate = dpOrderStartDate.SelectedDate ?? nullDate;
            RadDatePicker dpOrderEndDate = (RadDatePicker)eitem["DateTimeEndOrder"].Controls[0];
            DateTime orderEndDate = dpOrderEndDate.SelectedDate ?? nullDate;
            RadDatePicker dpActualStartDate = (RadDatePicker)eitem["DateTimeStartActual"].Controls[0]; ;
            DateTime actualStartDate = dpActualStartDate.SelectedDate ?? nullDate;
            RadDatePicker dpActualEndDate = (RadDatePicker)eitem["DateTimeEndActual"].Controls[0];
            DateTime actualEndDate = dpActualEndDate.SelectedDate ?? nullDate;
            TextBox txtOrderDurationOrder = (TextBox)eitem["DateTimeDurationOrder"].Controls[0];
            TextBox txtActualDuration = (TextBox)eitem["DateTimeDurationActual"].Controls[0];
            TextBox txtAddedCharges = (TextBox)eitem.FindControl("txtAddedCharges");
            TextBox txtCostCalculated = (TextBox)eitem["CostCalculated"].Controls[0];
            TextBox txtCostActual = (TextBox)eitem["CostActual"].Controls[0];
            int jobID = -1;
            if (Session["JobsID"] != null) {
                jobID = int.Parse(Session["JobsID"].ToString());
                JobsController jc = new JobsController();
                JobB currentJob = jc.GetJob(jobID);
                int minDuration = 0;
                if (currentJob != null) { minDuration = currentJob.MinimumTime.GetValueOrDefault(); }
                JobFormulasController cont = new JobFormulasController();
                List<JobFormulaB> curJobFormulas = cont.GetJobFormulas(jobID);
                string formula = "";
                if (actualStartDate > nullDate && actualEndDate > nullDate && actualEndDate > actualStartDate) {
                    TimeSpan actualSpan = actualEndDate.Subtract(actualStartDate);
                    int askedDuration = (int)Math.Ceiling(actualSpan.TotalMinutes);
                    if (askedDuration < minDuration) { askedDuration = minDuration; }
                    txtActualDuration.Text = askedDuration.ToString();
                    formula = findFormula(curJobFormulas, askedDuration, -1, -1);
                    formula = formula.Replace("#TIME#", askedDuration.ToString());
                    //formula = formula.Replace("#BANDWIDTH#", bandwidth.ToString());
                    //formula = formula.Replace("#DISTANCE#", selectedDistance.KM.ToString());
                    formula = formula.Replace(",", ".");
                    double calculatedCost = Evaluator.EvalToDouble(formula);
                    calculatedCost = Math.Round(calculatedCost, 2, MidpointRounding.AwayFromZero);
                    txtCostCalculated.Text = calculatedCost.ToString();
                    if (!string.IsNullOrEmpty(txtAddedCharges.Text)) { calculatedCost += double.Parse(txtAddedCharges.Text); }
                    txtCostActual.Text = calculatedCost.ToString();
                } else if (orderStartDate > nullDate && orderEndDate > nullDate && orderEndDate > orderStartDate) {
                    TimeSpan orderSpan = orderEndDate.Subtract(orderStartDate);
                    int askedDuration = (int)Math.Ceiling(orderSpan.TotalMinutes);
                    if (askedDuration < minDuration) { askedDuration = minDuration; }
                    txtOrderDurationOrder.Text = askedDuration.ToString();
                    formula = findFormula(curJobFormulas, askedDuration, -1, -1);
                    formula = formula.Replace("#TIME#", askedDuration.ToString());
                    //formula = formula.Replace("#BANDWIDTH#", bandwidth.ToString());
                    //formula = formula.Replace("#DISTANCE#", selectedDistance.KM.ToString());
                    formula = formula.Replace(",", ".");
                    double calculatedCost = Evaluator.EvalToDouble(formula);
                    calculatedCost = Math.Round(calculatedCost, 2, MidpointRounding.AwayFromZero);
                    //if (!string.IsNullOrEmpty(txtAddedCharges.Text)) { calculatedCost += double.Parse(txtAddedCharges.Text); }
                    txtCostCalculated.Text = calculatedCost.ToString();
                }
                if ((actualStartDate == nullDate || actualEndDate == nullDate || actualEndDate <= actualStartDate) && (orderStartDate == nullDate || orderEndDate == nullDate || orderEndDate <= orderStartDate)) {
                    txtActualDuration.Text = "";
                    txtCostActual.Text = "";
                    txtOrderDurationOrder.Text = "";
                    txtCostCalculated.Text = "";
                } else if ((actualStartDate == nullDate || actualEndDate == nullDate || actualEndDate <= actualStartDate) && (orderStartDate != nullDate && orderEndDate != nullDate && orderEndDate > orderStartDate)) {
                    txtActualDuration.Text = "";
                    txtCostActual.Text = "";
                } else if ((actualStartDate != nullDate && actualEndDate != nullDate && actualEndDate > actualStartDate) && (orderStartDate == nullDate || orderEndDate == nullDate || orderEndDate <= orderStartDate)) {
                    txtOrderDurationOrder.Text = "";
                }
            }
        }

        protected string findFormula(List<JobFormulaB> lstFormulas, int span, double bandwidth, int distance) {
            string formula = "";
            if (lstFormulas.Count > 1) {
                foreach (JobFormulaB jobFormula in lstFormulas) {
                    string currCondition = jobFormula.Condition;
                    if (span > 0) { currCondition = currCondition.Replace("#TIME#", span.ToString()); }
                    if (bandwidth > 0) { currCondition = currCondition.Replace("#BANDWIDTH#", bandwidth.ToString()); }
                    if (distance > 0) { currCondition = currCondition.Replace("#DISTANCE#", distance.ToString()); }
                    currCondition = currCondition.Replace(",", ".");
                    string valueVar = "";
                    string valueConst = "";
                    string formulaEval = "";
                    if (currCondition.IndexOfAny(new char[] { '>' }) != -1) {
                        valueVar = currCondition.Split(new char[] { '>' })[0];
                        valueConst = currCondition.Split(new char[] { '>' })[1];
                        formulaEval = ">";
                    } else if (currCondition.IndexOfAny(new char[] { '<' }) != -1) {
                        valueVar = currCondition.Split(new char[] { '<' })[0];
                        valueConst = currCondition.Split(new char[] { '<' })[1];
                        formulaEval = "<";
                    } else {
                        valueVar = currCondition.Split(new char[] { '=' })[0];
                        valueConst = currCondition.Split(new char[] { '=' })[1];
                        formulaEval = "=";
                    }
                    if (formulaEval == "=") {
                        if (valueVar == valueConst) {
                            formula = jobFormula.Formula;
                            break;
                        }
                    } else if (formulaEval == "<") {
                        if (float.Parse(valueVar) < float.Parse(valueConst)) {
                            formula = jobFormula.Formula;
                            break;
                        }
                    } else {
                        if (float.Parse(valueVar) > float.Parse(valueConst)) {
                            formula = jobFormula.Formula;
                            break;
                        }
                    }
                }
            } else { formula = lstFormulas[0].Formula; }
            return formula;
        }

        private void dpDate_SelectedIndexChanged(object sender, SelectedDateChangedEventArgs e) {
            RadDatePicker dpStartDate = (RadDatePicker)sender;
            GridEditableItem eitem = (GridEditableItem)dpStartDate.NamingContainer;
            calculateCosts(eitem);
        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {
            /*
            if (e.Item.OwnerTableView.Name == "Master") {
                if (e.Item is GridDataItem) {
                    GridDataItem item = (GridDataItem)e.Item;
                    if (item.OwnerTableView.DataSource != null) {
                        TaskB curTask = (item.OwnerTableView.DataSource as List<TaskB>)[item.DataSetIndex];
                        TableCell curCell = item["RegNo"];
                        string curComments = curTask.Comments;
                        string curTooltip = "<span><span class=\"tooltip tooltip-effect-4\"><span class=\"tooltip-item\">";
                        curTooltip += curCell.Text;
                        curTooltip += "</span><span class=\"tooltip-content clearfix\"><span class=\"tooltip-text\"><strong>Παρατηρήσεις:</strong><br/>";
                        curTooltip += curComments;
                        curTooltip += "</span></span></span></span>";
                        if (!string.IsNullOrWhiteSpace(curComments)) { curCell.Text = curTooltip; }
                        System.Drawing.Color hColor = System.Drawing.Color.FromArgb(0, 0, 0);
                        if (curTask.IsLocked == true) { hColor = System.Drawing.Color.FromArgb(200, 0, 0); }
                        item["ID"].ForeColor = hColor;
                        item["RegNo"].ForeColor = hColor;
                        item["OrderDate"].ForeColor = hColor;
                        item["CustomerID"].ForeColor = hColor;
                        item["JobsID"].ForeColor = hColor;
                        item["DateTimeStartActual"].ForeColor = hColor;
                        if (curTask.IsLocked == true && loggedUser.UserGroupID != 1) {
                            item["EditCommandColumn"].Controls[0].Visible = false;
                            item["btnDelete"].Controls[0].Visible = false;
                            item["ExapandColumn"].Controls[0].Visible = false;
                        }
                    }
                }
            }
            */
            if (e.Item.OwnerTableView.Name == "Master") {
                if (e.Item is GridDataItem) {
                    GridDataItem item = e.Item as GridDataItem;
                    TaskB curItemData = item.DataItem as TaskB;
                    if (curItemData.IsLocked == true) {
                        //item.BackColor = System.Drawing.Color.LightCyan;
                        ElasticButton btnEdit = (ElasticButton)item.FindControl("EditButton");
                        if (btnEdit != null) { btnEdit.Visible = false; }
                        ElasticButton btnDelete = (ElasticButton)item["btnDelete"].Controls[0];
                        if (btnDelete != null) { btnDelete.Visible = false; }
                    } else {
                        ImageButton btnUnlock = (ImageButton)item["btnUnlock"].Controls[0];
                        if (btnUnlock != null) { btnUnlock.Visible = false; }
                    }
                }
            }
            if (e.Item is GridFilteringItem) {
                GridFilteringItem filterItem = (GridFilteringItem)e.Item;
                RadDropDownList clist = (RadDropDownList)filterItem.FindControl("ddlCustomersFilter");
                RadDropDownList jlist = (RadDropDownList)filterItem.FindControl("ddlJobsFilter");
                RadDropDownList plist = (RadDropDownList)filterItem.FindControl("ddlPositionsFilter");
                try {
                    CustomersController ccont = new CustomersController();
                    clist.DataSource = ccont.GetCustomers();
                    clist.DataTextField = "NameGR";
                    clist.DataValueField = "ID";
                    clist.DataBind();
                    clist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                    JobsController jcont = new JobsController();
                    jlist.DataSource = jcont.GetJobsForPageID(pageID);
                    jlist.DataTextField = "Name";
                    jlist.DataValueField = "ID";
                    jlist.DataBind();
                    jlist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                    DistancesController dcont = new DistancesController();
                    plist.DataSource = dcont.GetDistancesForPageID(pageID);
                    plist.DataTextField = "Description";
                    plist.DataValueField = "ID";
                    plist.DataBind();
                    plist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                }
                catch (Exception) { }
            }
            if (e.Item is GridEditableItem && e.Item.IsInEditMode && e.Item.OwnerTableView.Name == "Master") {
                JobsID = -1;
                Session.Remove("JobsID");
                CustomersID = -1;
                Session.Remove("CustomersID");
                DistancesID = -1;
                Session.Remove("DistancesID");
                GridEditableItem item = e.Item as GridEditableItem;
                RadDropDownList ddlJobs = item.FindControl("ddlJobs") as RadDropDownList;
                RadDropDownList ddlCustomers = item.FindControl("ddlCustomers") as RadDropDownList;
                RadDropDownList ddlDistances = item.FindControl("ddlDistances") as RadDropDownList;
                CheckBox chkIsCanceled = (CheckBox)item.FindControl("chkIsCanceled");
                RadDateTimePicker dpOrderDate = (RadDateTimePicker)item["OrderDate"].Controls[0];
                try {
                    TaskB currTask = e.Item.DataItem as TaskB;
                    JobsController cont1 = new JobsController();
                    ddlJobs.DataSource = cont1.GetJobsForPageID(pageID);
                    ddlJobs.DataTextField = "Name";
                    ddlJobs.DataValueField = "ID";
                    ddlJobs.DataBind();
                    CustomersController cont2 = new CustomersController();
                    ddlCustomers.DataSource = cont2.GetCustomers();
                    ddlCustomers.DataTextField = "NameGR";
                    ddlCustomers.DataValueField = "ID";
                    ddlCustomers.DataBind();
                    DistancesController cont3 = new DistancesController();
                    ddlDistances.DataSource = cont3.GetDistancesForPageID(pageID);
                    ddlDistances.DataTextField = "Description";
                    ddlDistances.DataValueField = "ID";
                    ddlDistances.DataBind();
                    if (currTask != null) {
                        ddlJobs.SelectedIndex = ddlJobs.FindItemByValue(currTask.JobID.ToString()).Index;
                        Session["JobsID"] = currTask.JobID;
                        ddlCustomers.SelectedIndex = ddlCustomers.FindItemByValue(currTask.CustomerID.ToString()).Index;
                        Session["CustomersID"] = currTask.CustomerID;
                        ddlDistances.SelectedIndex = ddlDistances.FindItemByValue(currTask.DistanceID.ToString()).Index;
                        Session["DistancesID"] = currTask.DistanceID;
                        chkIsCanceled.Checked = currTask.IsCanceled;
                    } else {
                        ddlJobs.SelectedIndex = 0;
                        Session["JobsID"] = ddlJobs.SelectedItem.Value;
                        ddlCustomers.SelectedIndex = 0;
                        Session["CustomersID"] = ddlCustomers.SelectedItem.Value;
                        ddlDistances.SelectedIndex = 0;
                        Session["DistancesID"] = ddlDistances.SelectedItem.Value;
                        chkIsCanceled.Checked = false;
                        dpOrderDate.SelectedDate = DateTime.Now.Date;
                    }
                }
                catch (Exception) { }
            }
        }

        private void ShowErrorMessage(int errCode) {
            switch (errCode) {
                case 1:
                    RadWindowManager1.RadAlert("Η συγκεκριμένη Παραγγελία σχετίζεται με κάποιο Έργο και δεν μπορεί να διαγραφεί!", 400, 200, "Σφάλμα", "");
                    break;
                case 2:
                    RadWindowManager1.RadAlert("Η συγκεκριμένη Παραγγελία είναι κλειδωμένη και δεν μπορεί να διαγραφεί!", 400, 200, "Σφάλμα", "");
                    break;
                case 3:
                    RadWindowManager1.RadAlert("Μόνο χρήστης με ρόλο Administrator μπορεί να ξεκλειδώσει ένα έργο!", 400, 200, "Σφάλμα", "");
                    break;
                case 4:
                    RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ελέγξτε τον τύπο ακύρωσης.", 400, 200, "Σφάλμα", "");
                    break;
                default:
                    RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
                    break;
            }
        }

        protected void gridMain_UpdateCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            var ID = (int)editableItem.GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var curTask = dbContext.Tasks.Where(n => n.ID == ID).FirstOrDefault();
                if (curTask != null) {
                    try {
                        editableItem.UpdateValues(curTask);
                        if (Session["JobsID"] != null) { JobsID = int.Parse(Session["JobsID"].ToString()); }
                        if (JobsID > 0) {
                            curTask.JobID = JobsID;
                            JobsID = -1;
                            Session.Remove("JobsID");
                        }
                        if (Session["CustomersID"] != null) { CustomersID = int.Parse(Session["CustomersID"].ToString()); }
                        if (CustomersID > 0) {
                            curTask.CustomerID = CustomersID;
                            CustomersID = -1;
                            Session.Remove("CustomersID");
                        }
                        if (Session["DistancesID"] != null) { DistancesID = int.Parse(Session["DistancesID"].ToString()); }
                        if (DistancesID > 0) {
                            curTask.DistancesID = DistancesID;
                            DistancesID = -1;
                            Session.Remove("DistancesID");
                        }
                        CheckBox chkIsCanceled = (CheckBox)editableItem.FindControl("chkIsCanceled");
                        curTask.IsCanceled = chkIsCanceled.Checked;
                        if (curTask.IsCanceled == true) { curTask.IsForHelpers = false; }
                        dbContext.SaveChanges();
                    }
                    catch (Exception) { ShowErrorMessage(-1); }
                }
            }
        }

        protected void gridMain_InsertCommand(object source, GridCommandEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                var editableItem = ((GridEditableItem)e.Item);
                using (var dbContext = new OTERTConnStr()) {
                    var curTask = new Tasks();
                    Hashtable values = new Hashtable();
                    editableItem.ExtractValues(values);
                    if (Session["JobsID"] != null) { JobsID = int.Parse(Session["JobsID"].ToString()); }
                    if (Session["CustomersID"] != null) { CustomersID = int.Parse(Session["CustomersID"].ToString()); }
                    if (Session["DistancesID"] != null) { DistancesID = int.Parse(Session["DistancesID"].ToString()); }
                    if (JobsID > 0 && CustomersID > 0 && DistancesID > 0) {
                        try {
                            curTask.OrderID = null;
                            curTask.RegNo = (string)values["RegNo"];
                            curTask.OrderDate = DateTime.Parse((string)values["OrderDate"]);
                            curTask.CustomerID = CustomersID;
                            curTask.RequestedPositionID = null;
                            curTask.JobID = JobsID;
                            curTask.DistancesID = DistancesID;
                            curTask.DateTimeStartOrder = DateTime.Parse((string)values["DateTimeStartOrder"]);
                            curTask.DateTimeEndOrder = DateTime.Parse((string)values["DateTimeEndOrder"]);
                            curTask.DateTimeDurationOrder = int.Parse((string)values["DateTimeDurationOrder"]);
                            if (values["DateTimeStartActual"] != null) { curTask.DateTimeStartActual = DateTime.Parse((string)values["DateTimeStartActual"]); } else { curTask.DateTimeStartActual = null; }
                            if (values["DateTimeEndActual"] != null) { curTask.DateTimeEndActual = DateTime.Parse((string)values["DateTimeEndActual"]); } else { curTask.DateTimeEndActual = null; }
                            if (values["DateTimeDurationActual"] != null) { curTask.DateTimeDurationActual = int.Parse((string)values["DateTimeDurationActual"]); } else { curTask.DateTimeDurationActual = null; }
                            if (values["CostCalculated"] != null) { curTask.CostCalculated = decimal.Parse((string)values["CostCalculated"]); } else { curTask.CostCalculated = null; }
                            curTask.InstallationCharges = false;
                            curTask.MonthlyCharges = false;
                            curTask.CallCharges = 0;
                            curTask.TelephoneNumber = "";
                            curTask.TechnicalSupport = 0;
                            if (values["AddedCharges"] != null) { curTask.AddedCharges = decimal.Parse((string)values["AddedCharges"]); } else { curTask.AddedCharges = null; }
                            if (values["CostActual"] != null) { curTask.CostActual = decimal.Parse((string)values["CostActual"]); } else { curTask.CostActual = null; }
                            if (values["PaymentDateOrder"] != null) { curTask.PaymentDateOrder = DateTime.Parse((string)values["PaymentDateOrder"]); } else { curTask.PaymentDateOrder = null; }
                            if (values["PaymentDateCalculated"] != null) { curTask.PaymentDateCalculated = DateTime.Parse((string)values["PaymentDateCalculated"]); } else { curTask.PaymentDateCalculated = null; }
                            if (values["PaymentDateActual"] != null) { curTask.PaymentDateActual = DateTime.Parse((string)values["PaymentDateActual"]); } else { curTask.PaymentDateActual = null; }
                            curTask.IsForHelpers = (bool)values["IsForHelpers"];
                            curTask.IsLocked = (bool)values["IsLocked"];
                            CheckBox chkIsCanceled = (CheckBox)editableItem.FindControl("chkIsCanceled");
                            curTask.IsCanceled = chkIsCanceled.Checked;
                            if (curTask.IsCanceled == true) { curTask.IsForHelpers = false; }
                            curTask.CancelPrice = 0;
                            curTask.Comments = (string)values["Comments"];
                            curTask.InvoceComments = (string)values["InvoceComments"];
                            curTask.SateliteID = null;
                            curTask.DateStamp = DateTime.Now;
                            curTask.EnteredByUser = loggedUser.NameGR;
                            dbContext.Tasks.Add(curTask);
                            dbContext.SaveChanges();
                        }
                        catch (Exception) { ShowErrorMessage(-1); }
                        finally {
                            JobsID = -1;
                            Session.Remove("JobsID");
                            CustomersID = -1;
                            Session.Remove("CustomersID");
                            DistancesID = -1;
                            Session.Remove("DistancesID");
                        }
                    } else { ShowErrorMessage(-1); }
                }
            } else if (e.Item.OwnerTableView.Name == "AttachedFiles") {
                GridTableView detailtabl = e.Item.OwnerTableView;
                GridDataItem parentItem = detailtabl.ParentItem;
                int taskID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
                var editableItem = ((GridEditableItem)e.Item);
                using (var dbContext = new OTERTConnStr()) {
                    var curFile = new Files();
                    Hashtable values = new Hashtable();
                    editableItem.ExtractValues(values);
                    curFile.TaskID = taskID;
                    curFile.FileName = (string)values["FileName"];
                    curFile.FilePath = uploadedFilePath;
                    curFile.DateStamp = DateTime.Now;
                    dbContext.Files.Add(curFile);
                    try { dbContext.SaveChanges(); }
                    catch (Exception) { ShowErrorMessage(-1); }
                }
            }
        }

        protected void gridMain_DeleteCommand(object source, GridCommandEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curTask = dbContext.Tasks.Where(n => n.ID == ID).FirstOrDefault();
                    if (curTask != null) {
                        if (curTask.IsLocked != true) {
                            List<Files> curFiles = dbContext.Files.Where(k => k.TaskID == ID).ToList();
                            foreach (Files curFile in curFiles) {
                                string FileToDelete = Server.MapPath(curFile.FilePath);
                                if (System.IO.File.Exists(FileToDelete)) { System.IO.File.Delete(FileToDelete); }
                                dbContext.Files.Remove(curFile);
                                try { dbContext.SaveChanges(); }
                                catch (Exception) { ShowErrorMessage(-1); }
                            }
                            dbContext.Tasks.Remove(curTask);
                            try { dbContext.SaveChanges(); }
                            catch (Exception ex) {
                                string err = ex.InnerException.InnerException.Message;
                                int errCode = -1;
                                if (err.StartsWith("The DELETE statement conflicted with the REFERENCE constraint")) { errCode = 1; }
                                ShowErrorMessage(errCode);
                            }
                        } else { ShowErrorMessage(2); }
                    }
                }
            } else if (e.Item.OwnerTableView.Name == "AttachedFiles") {
                var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curFile = dbContext.Files.Where(n => n.ID == ID).FirstOrDefault();
                    if (curFile != null) {
                        string FileToDelete = Server.MapPath(curFile.FilePath);
                        if (System.IO.File.Exists(FileToDelete)) { System.IO.File.Delete(FileToDelete); }
                        dbContext.Files.Remove(curFile);
                        try { dbContext.SaveChanges(); }
                        catch (Exception) { ShowErrorMessage(-1); }
                    }
                }
            }
        }

        protected void gridMain_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e) {
            if (e.CommandName == "Unlock") {
                if (loggedUser.UserGroupID == 1) {
                    var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                    using (var dbContext = new OTERTConnStr()) {
                        Tasks curTask = dbContext.Tasks.Where(n => n.ID == ID).FirstOrDefault();
                        if (curTask != null) {
                            curTask.IsLocked = false;
                            try {
                                dbContext.SaveChanges();
                                gridMain.Rebind();
                            }
                            catch (Exception) { ShowErrorMessage(-1); }
                        } else { ShowErrorMessage(-1); }
                    }
                } else { ShowErrorMessage(3); }
            }
        }

        protected void ddlJobs_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                JobsID = int.Parse(e.Value);
                Session["JobsID"] = JobsID;
                RadDropDownList ddlJobs = (RadDropDownList)sender;
                GridEditableItem eitem = (GridEditableItem)ddlJobs.NamingContainer;
                calculateCosts(eitem);
            }
            catch (Exception) { }
        }

        protected void ddlCustomers_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                CustomersID = int.Parse(e.Value);
                Session["CustomersID"] = CustomersID;
            }
            catch (Exception) { }
        }

        protected void ddlDistances_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                DistancesID = int.Parse(e.Value);
                Session["DistancesID"] = DistancesID;
            }
            catch (Exception) { }
        }

        protected void gridMain_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e) {
            GridTableView detailtabl = e.DetailTableView;
            int recSkip = detailtabl.CurrentPageIndex * gridMain.PageSize;
            int recTake = detailtabl.PageSize;
            GridDataItem parentItem = detailtabl.ParentItem;
            int taskID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
            FilesController cont = new FilesController();
            detailtabl.VirtualItemCount = cont.CountFiles(taskID);
            detailtabl.DataSource = cont.GetFilesByTaskID(taskID, recSkip, recTake);
        }

        protected void uplFile_FileUploaded(object sender, FileUploadedEventArgs e) {
            string fullPath = Server.MapPath(fileUploadFolder);
            string newFolder = DateTime.Now.ToString("yyyy-MM");
            fullPath += newFolder.Split('-')[0] + "\\" + newFolder.Split('-')[1];
            bool exists = System.IO.Directory.Exists(fullPath);
            if (!exists) { System.IO.Directory.CreateDirectory(fullPath); }
            string newfilename = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + "_" + e.File.GetNameWithoutExtension().Replace(" ", "_") + e.File.GetExtension();
            uploadedFilePath = fileUploadFolder + newFolder.Split('-')[0] + "/" + newFolder.Split('-')[1] + "/" + newfilename;
            e.File.SaveAs(System.IO.Path.Combine(fullPath, newfilename));
        }

        protected void btnCancelationCancel_Click(object sender, EventArgs e) {
            Button btnCancelationCancel = (Button)sender;
            GridEditableItem eitem = (GridEditableItem)btnCancelationCancel.NamingContainer;
            CheckBox chkIsCanceled = (CheckBox)eitem.FindControl("chkIsCanceled"); ;
            Label lblCancelationMsg = (Label)eitem.FindControl("lblCancelationMsg");
            RadDropDownList ddlCancelationPrices = (RadDropDownList)eitem.FindControl("ddlCancelationPrices");
            Button btnCancelationOK = (Button)eitem.FindControl("btnCancelationOK");
            lblCancelationMsg.Visible = false;
            ddlCancelationPrices.Visible = false;
            btnCancelationOK.Visible = false;
            btnCancelationCancel.Visible = false;
            chkIsCanceled.Checked = false;
            calculateCosts(eitem);
        }

        protected void btnCancelationOK_Click(object sender, EventArgs e) {
            Button btnCancelationOK = (Button)sender;
            GridEditableItem eitem = (GridEditableItem)btnCancelationOK.NamingContainer;
            CheckBox chkIsCanceled = (CheckBox)eitem.FindControl("chkIsCanceled"); ;
            Label lblCancelationMsg = (Label)eitem.FindControl("lblCancelationMsg");
            RadDropDownList ddlCancelationPrices = (RadDropDownList)eitem.FindControl("ddlCancelationPrices");
            Button btnCancelationCancel = (Button)eitem.FindControl("btnCancelationCancel");
            lblCancelationMsg.Visible = false;
            ddlCancelationPrices.Visible = false;
            btnCancelationOK.Visible = false;
            btnCancelationCancel.Visible = false;
            chkIsCanceled.Checked = true;
            TextBox txtCostActual = (TextBox)eitem["CostActual"].Controls[0];
            txtCostActual.Text = "0";
        }

        protected void chkIsCanceled_CheckedChanged(object sender, EventArgs e) {
            CheckBox chkIsCanceled = (CheckBox)sender;
            GridEditableItem eitem = (GridEditableItem)chkIsCanceled.NamingContainer;
            Label lblCancelationMsg = (Label)eitem.FindControl("lblCancelationMsg");
            RadDropDownList ddlCancelationPrices = (RadDropDownList)eitem.FindControl("ddlCancelationPrices");
            Button btnCancelationOK = (Button)eitem.FindControl("btnCancelationOK");
            Button btnCancelationCancel = (Button)eitem.FindControl("btnCancelationCancel");
            if (chkIsCanceled.Checked) {
                if (Session["JobsID"] != null) {
                    int jobID = int.Parse(Session["JobsID"].ToString());
                    JobCancelPricesController cont = new JobCancelPricesController();
                    List<JobCancelPriceB> curJobCancelPrices = cont.GetJobCancelPrices(jobID);
                    if (curJobCancelPrices.Count>0) {
                        ddlCancelationPrices.DataSource = curJobCancelPrices;
                        ddlCancelationPrices.DataTextField = "Name";
                        ddlCancelationPrices.DataValueField = "Price";
                        ddlCancelationPrices.DataBind();
                        ddlCancelationPrices.Items.Insert(0, new DropDownListItem("Επιλέξτε Ποσό Ακύρωσης", "-1"));
                        lblCancelationMsg.Visible = false;
                        ddlCancelationPrices.Visible = true;
                        btnCancelationOK.Visible = false;
                        btnCancelationCancel.Visible = false;
                    } else {
                        lblCancelationMsg.Text = "Δεν υπάρχουν Ποσά Ακύρωσης για τη συγκεκριμένη Κατηγορία Έργου. Το συνολικό κόστος θα μηδενιστεί!";
                        lblCancelationMsg.Visible = true;
                        ddlCancelationPrices.Visible = false;
                        btnCancelationOK.Visible = true;
                        btnCancelationCancel.Visible = true;
                    }
                }
            } else {
                lblCancelationMsg.Visible = false;
                ddlCancelationPrices.Visible = false;
                btnCancelationOK.Visible = false;
                btnCancelationCancel.Visible = false;
                calculateCosts(eitem);
            }
        }

        protected void ddlCancelationPrices_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                RadDropDownList ddlCancelationPrices = (RadDropDownList)sender;
                if (ddlCancelationPrices.SelectedIndex != 0) {
                    GridEditableItem eitem = (GridEditableItem)ddlCancelationPrices.NamingContainer;
                    TextBox txtCostActual = (TextBox)eitem["CostActual"].Controls[0];
                    double cancelationCost = 0;
                    bool successParce = double.TryParse(ddlCancelationPrices.SelectedItem.Value.Replace(".", ","), out cancelationCost);
                    if (successParce == false) {
                        string cancelationFormula = ddlCancelationPrices.SelectedItem.Value.Replace(".", ",");
                        if (cancelationFormula.StartsWith("#EUTELSAT#")) {
                            cancelationFormula = cancelationFormula.Replace("#EUTELSAT#", "");
                            string[] subs = cancelationFormula.Split('#');
                            int timePercentDivide = int.Parse(subs[0]);
                            double percent1 = double.Parse(subs[1]);
                            double percent2 = double.Parse(subs[2]);
                            double timeFormulaDivide = double.Parse(subs[3]);
                            double pricePerMin1 = double.Parse(subs[4]);
                            double pricePerMin2 = double.Parse(subs[5]);
                            TextBox txtOrderDurationOrder = (TextBox)eitem["DateTimeDurationOrder"].Controls[0];
                            int orderDuration = int.Parse(txtOrderDurationOrder.Text);
                            double pricePerMin = pricePerMin1;
                            if (orderDuration > timeFormulaDivide) { pricePerMin = pricePerMin2; }
                            if (orderDuration < timePercentDivide) {
                                cancelationCost = orderDuration * pricePerMin * percent1 / 100;
                            } else {
                                cancelationCost = timePercentDivide * pricePerMin * percent1 / 100 + (orderDuration - timePercentDivide) * pricePerMin * percent2 / 100;
                            }
                        } else {
                            TextBox txtCostCalculated = (TextBox)eitem["CostCalculated"].Controls[0];
                            double costCalculated = 0;
                            bool successParceCC = double.TryParse(txtCostCalculated.Text.Replace(".", ","), out costCalculated);
                            if (successParceCC == true) {
                                cancelationFormula = cancelationFormula.Replace("#CALCPRICE#", costCalculated.ToString()).Replace(",", ".");
                                cancelationCost = Evaluator.EvalToDouble(cancelationFormula);
                            }
                        }
                    }
                    cancelationCost = Math.Round(cancelationCost, 2, MidpointRounding.AwayFromZero);
                    txtCostActual.Text = cancelationCost.ToString();
                    CheckBox chkIsForHelpers = (CheckBox)eitem["chkIsForHelpers"].Controls[0];
                    chkIsForHelpers.Checked = false;
                }
            }
            catch (Exception) { ShowErrorMessage(4); }
        }

        protected void ddlCancelationPrices_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

        protected void ddlCustomersFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("CustomerID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(CustomerID = " + e.Value + ")";
                gridMain.MasterTableView.GetColumn("CustomerID").CurrentFilterFunction = GridKnownFunction.EqualTo;
                gridMain.MasterTableView.GetColumn("CustomerID").CurrentFilterValue = e.Value;
            } else {
                gridMain.MasterTableView.GetColumn("CustomerID").CurrentFilterFunction = GridKnownFunction.NoFilter;
                gridMain.MasterTableView.GetColumn("CustomerID").CurrentFilterValue = null;
            }
            gridMain.MasterTableView.FilterExpression = finalExpression;
            ViewState[list.ClientID] = e.Value;
            gridMain.MasterTableView.Rebind();
        }

        protected void ddlCustomersFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

        protected void ddlJobsFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("JobID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(JobID = " + e.Value + ")";
                gridMain.MasterTableView.GetColumn("JobID").CurrentFilterFunction = GridKnownFunction.EqualTo;
                gridMain.MasterTableView.GetColumn("JobID").CurrentFilterValue = e.Value;
            } else {
                gridMain.MasterTableView.GetColumn("JobID").CurrentFilterFunction = GridKnownFunction.NoFilter;
                gridMain.MasterTableView.GetColumn("JobID").CurrentFilterValue = null;
            }
            gridMain.MasterTableView.FilterExpression = finalExpression;
            ViewState[list.ClientID] = e.Value;
            gridMain.MasterTableView.Rebind();
        }

        protected void ddlJobsFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

        protected void ddlPositionsFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("DistanceID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(DistanceID = " + e.Value + ")";
                gridMain.MasterTableView.GetColumn("DistanceID").CurrentFilterFunction = GridKnownFunction.EqualTo;
                gridMain.MasterTableView.GetColumn("DistanceID").CurrentFilterValue = e.Value;
            } else {
                gridMain.MasterTableView.GetColumn("DistanceID").CurrentFilterFunction = GridKnownFunction.NoFilter;
                gridMain.MasterTableView.GetColumn("DistanceID").CurrentFilterValue = null;
            }
            gridMain.MasterTableView.FilterExpression = finalExpression;
            ViewState[list.ClientID] = e.Value;
            gridMain.MasterTableView.Rebind();
        }

        protected void ddlPositionsFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

    }

}