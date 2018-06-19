using System;
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

    public partial class SateliteUplink : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected int pageID = 1;
        protected string pageTitle;
        protected int JobsID, CustomersID, SatelitesID, DistancesID;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Έργα > Έκτακτα > Δορυφορικά > Uplink";
                gridMain.MasterTableView.Caption = "Έργα > Έκτακτα > Δορυφορικά > Uplink";
                JobsID = -1;
                Session.Remove("JobsID");
                CustomersID = -1;
                Session.Remove("CustomersID");
                SatelitesID = -1;
                Session.Remove("SatelitesID");
                DistancesID = -1;
                Session.Remove("DistancesID");
            }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            try {
                TasksController cont = new TasksController();
                gridMain.VirtualItemCount = cont.CountTasksForPageID(pageID);
                gridMain.DataSource = cont.GetTasksForPage(pageID, recSkip, recTake);
            }
            catch (Exception) { }
        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) {
            if (e.Item is GridDataItem) {
                GridDataItem item = (GridDataItem)e.Item;
                ElasticButton img = (ElasticButton)item["btnDelete"].Controls[0];
                img.ToolTip = "Διαγραφή";
            }
            if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                GridEditableItem item = e.Item as GridEditableItem;
                RadDateTimePicker dpDateTimeStartOrder = (RadDateTimePicker)item["DateTimeStartOrder"].Controls[0];
                dpDateTimeStartOrder.AutoPostBackControl = Telerik.Web.UI.Calendar.AutoPostBackControl.Both;
                dpDateTimeStartOrder.SelectedDateChanged += new SelectedDateChangedEventHandler(dpDateTimeStartOrder_SelectedIndexChanged);
                RadDateTimePicker dpDateTimeEndOrder = (RadDateTimePicker)item["DateTimeEndOrder"].Controls[0];
                dpDateTimeEndOrder.AutoPostBackControl = Telerik.Web.UI.Calendar.AutoPostBackControl.Both;
                dpDateTimeEndOrder.SelectedDateChanged += new SelectedDateChangedEventHandler(dpDateTimeEndOrder_SelectedIndexChanged);
                RadDateTimePicker dpDateTimeStartActual = (RadDateTimePicker)item["DateTimeStartActual"].Controls[0];
                dpDateTimeStartActual.AutoPostBackControl = Telerik.Web.UI.Calendar.AutoPostBackControl.Both;
                dpDateTimeStartActual.SelectedDateChanged += new SelectedDateChangedEventHandler(dpDateTimeStartActual_SelectedIndexChanged);
                RadDateTimePicker dpDateTimeEndActual = (RadDateTimePicker)item["DateTimeEndActual"].Controls[0];
                dpDateTimeEndActual.AutoPostBackControl = Telerik.Web.UI.Calendar.AutoPostBackControl.Both;
                dpDateTimeEndActual.SelectedDateChanged += new SelectedDateChangedEventHandler(dpDateTimeEndActual_SelectedIndexChanged);
            }
        }

        private void dpDateTimeStartOrder_SelectedIndexChanged(object sender, SelectedDateChangedEventArgs e) {
            DateTime startDate = e.NewDate ?? DateTime.Now;
            RadDateTimePicker dpStartDate = (RadDateTimePicker)sender;
            GridEditableItem eitem = (GridEditableItem)dpStartDate.NamingContainer;
            RadDateTimePicker dpEndDate = (RadDateTimePicker)eitem["DateTimeEndOrder"].Controls[0];
            DateTime endDate = dpEndDate.SelectedDate ?? DateTime.Now;
            if (endDate > startDate) {
                TimeSpan span = endDate.Subtract(startDate);
                TextBox txtDateTimeDurationOrder = (TextBox)eitem["DateTimeDurationOrder"].Controls[0];
                txtDateTimeDurationOrder.Text = ((int)Math.Ceiling(span.TotalMinutes)).ToString();
                int jobID = -1;
                int sateliteID = -1;
                if (Session["JobsID"] != null && Session["SatelitesID"] != null) {
                    jobID = int.Parse(Session["JobsID"].ToString());
                    sateliteID = int.Parse(Session["SatelitesID"].ToString());
                    JobFormulasController cont = new JobFormulasController();
                    List<JobFormulaB> curJobFormulas = cont.GetJobFormulas(jobID);
                    SatelitesController cont2 = new SatelitesController();
                    SateliteB curSat = cont2.GetSatelite(sateliteID);

                    string formula = "";
                    if (curJobFormulas.Count > 1) {
                        foreach (JobFormulaB jobFormula in curJobFormulas) {
                            string currCondition = jobFormula.Condition;
                            currCondition = currCondition.Replace("#BANDWIDTH#", curSat.Frequency);
                            currCondition = currCondition.Replace("#TIME#", ((int)Math.Ceiling(span.TotalMinutes)).ToString());
                            currCondition = currCondition.Replace(",", ".");
                            string valueVar = "";
                            string valueConst = "";
                            string formulaEval = "";
                            if (currCondition.IndexOfAny(new char[] { '>' }) != -1) {
                                valueVar = currCondition.Split(new Char[] { '>' })[0];
                                valueConst = currCondition.Split(new Char[] { '>' })[1];
                                formulaEval = ">";
                            } else if (currCondition.IndexOfAny(new char[] { '<' }) != -1) {
                                valueVar = currCondition.Split(new Char[] { '<' })[0];
                                valueConst = currCondition.Split(new Char[] { '<' })[1];
                                formulaEval = "<";
                            } else {
                                valueVar = currCondition.Split(new Char[] { '=' })[0];
                                valueConst = currCondition.Split(new Char[] { '=' })[1];
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
                    } else { formula = curJobFormulas[0].Formula; }
                    formula = formula.Replace("#BANDWIDTH#", curSat.Frequency);
                    formula = formula.Replace("#TIME#", ((int)Math.Ceiling(span.TotalMinutes)).ToString());
                    formula = formula.Replace(",", ".");
                    double calculatedCost = Evaluator.EvalToDouble(formula);
                    TextBox txtAddedCharges = (TextBox)eitem["AddedCharges"].Controls[0];
                    if (!string.IsNullOrEmpty(txtAddedCharges.Text)) { calculatedCost += double.Parse(txtAddedCharges.Text); }
                    TextBox txtCostCalculated = (TextBox)eitem["CostCalculated"].Controls[0];
                    txtCostCalculated.Text = calculatedCost.ToString();
                }
            }
        }

        private void dpDateTimeEndOrder_SelectedIndexChanged(object sender, SelectedDateChangedEventArgs e) {
            DateTime endDate = e.NewDate ?? DateTime.Now;
            RadDateTimePicker dpEndDate = (RadDateTimePicker)sender;
            GridEditableItem eitem = (GridEditableItem)dpEndDate.NamingContainer;
            RadDateTimePicker dpStartDate = (RadDateTimePicker)eitem["DateTimeStartOrder"].Controls[0];
            DateTime startDate = dpStartDate.SelectedDate ?? DateTime.Now;
            if (endDate > startDate) {
                TimeSpan span = endDate.Subtract(startDate);
                TextBox txtDateTimeDurationOrder = (TextBox)eitem["DateTimeDurationOrder"].Controls[0];
                txtDateTimeDurationOrder.Text = ((int)Math.Ceiling(span.TotalMinutes)).ToString();
                int jobID = -1;
                int sateliteID = -1;
                if (Session["JobsID"] != null && Session["SatelitesID"] != null) {
                    jobID = int.Parse(Session["JobsID"].ToString());
                    sateliteID = int.Parse(Session["SatelitesID"].ToString());
                    JobFormulasController cont = new JobFormulasController();
                    List<JobFormulaB> curJobFormulas = cont.GetJobFormulas(jobID);
                    SatelitesController cont2 = new SatelitesController();
                    SateliteB curSat = cont2.GetSatelite(sateliteID);

                    string formula = "";
                    if (curJobFormulas.Count > 1) {
                        foreach (JobFormulaB jobFormula in curJobFormulas) {
                            string currCondition = jobFormula.Condition;
                            currCondition = currCondition.Replace("#BANDWIDTH#", curSat.Frequency);
                            currCondition = currCondition.Replace("#TIME#", ((int)Math.Ceiling(span.TotalMinutes)).ToString());
                            currCondition = currCondition.Replace(",", ".");
                            string valueVar = "";
                            string valueConst = "";
                            string formulaEval = "";
                            if (currCondition.IndexOfAny(new char[] { '>' }) != -1) {
                                valueVar = currCondition.Split(new Char[] { '>' })[0];
                                valueConst = currCondition.Split(new Char[] { '>' })[1];
                                formulaEval = ">";
                            } else if (currCondition.IndexOfAny(new char[] { '<' }) != -1) {
                                valueVar = currCondition.Split(new Char[] { '<' })[0];
                                valueConst = currCondition.Split(new Char[] { '<' })[1];
                                formulaEval = "<";
                            } else {
                                valueVar = currCondition.Split(new Char[] { '=' })[0];
                                valueConst = currCondition.Split(new Char[] { '=' })[1];
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
                    } else { formula = curJobFormulas[0].Formula; }
                    formula = formula.Replace("#BANDWIDTH#", curSat.Frequency);
                    formula = formula.Replace("#TIME#", ((int)Math.Ceiling(span.TotalMinutes)).ToString());
                    formula = formula.Replace(",", ".");
                    double calculatedCost = Evaluator.EvalToDouble(formula);
                    TextBox txtAddedCharges = (TextBox)eitem["AddedCharges"].Controls[0];
                    if (!string.IsNullOrEmpty(txtAddedCharges.Text)) { calculatedCost += double.Parse(txtAddedCharges.Text); }
                    TextBox txtCostCalculated = (TextBox)eitem["CostCalculated"].Controls[0];
                    txtCostCalculated.Text = calculatedCost.ToString();
                }
            }
        }

        private void dpDateTimeStartActual_SelectedIndexChanged(object sender, SelectedDateChangedEventArgs e) {
            DateTime startDate = e.NewDate ?? DateTime.Now;
            RadDateTimePicker dpStartDate = (RadDateTimePicker)sender;
            GridEditableItem eitem = (GridEditableItem)dpStartDate.NamingContainer;
            RadDateTimePicker dpEndDate = (RadDateTimePicker)eitem["DateTimeEndActual"].Controls[0];
            DateTime endDate = dpEndDate.SelectedDate ?? DateTime.Now;
            if (endDate > startDate) {
                TimeSpan span = endDate.Subtract(startDate);
                TextBox txtDateTimeDurationOrder = (TextBox)eitem["DateTimeDurationActual"].Controls[0];
                txtDateTimeDurationOrder.Text = ((int)Math.Ceiling(span.TotalMinutes)).ToString();
            }
        }

        private void dpDateTimeEndActual_SelectedIndexChanged(object sender, SelectedDateChangedEventArgs e) {
            DateTime endDate = e.NewDate ?? DateTime.Now;
            RadDateTimePicker dpEndDate = (RadDateTimePicker)sender;
            GridEditableItem eitem = (GridEditableItem)dpEndDate.NamingContainer;
            RadDateTimePicker dpStartDate = (RadDateTimePicker)eitem["DateTimeStartActual"].Controls[0];
            DateTime startDate = dpStartDate.SelectedDate ?? DateTime.Now;
            if (endDate > startDate) {
                TimeSpan span = endDate.Subtract(startDate);
                TextBox txtDateTimeDurationOrder = (TextBox)eitem["DateTimeDurationActual"].Controls[0];
                txtDateTimeDurationOrder.Text = ((int)Math.Ceiling(span.TotalMinutes)).ToString();
            }
        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {
            if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                JobsID = -1;
                Session.Remove("JobsID");
                CustomersID = -1;
                Session.Remove("CustomersID");
                SatelitesID = -1;
                Session.Remove("SatelitesID");
                DistancesID = -1;
                Session.Remove("DistancesID");
                GridEditableItem item = e.Item as GridEditableItem;
                RadDropDownList ddlJobs = item.FindControl("ddlJobs") as RadDropDownList;
                RadDropDownList ddlCustomers = item.FindControl("ddlCustomers") as RadDropDownList;
                RadDropDownList ddlSatelites = item.FindControl("ddlSatelites") as RadDropDownList;
                RadDropDownList ddlDistances = item.FindControl("ddlDistances") as RadDropDownList;
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
                    SatelitesController cont3 = new SatelitesController();
                    ddlSatelites.DataSource = cont3.GetSatelites();
                    ddlSatelites.DataTextField = "Name";
                    ddlSatelites.DataValueField = "ID";
                    ddlSatelites.DataBind();
                    DistancesController cont4 = new DistancesController();
                    ddlDistances.DataSource = cont4.GetDistancesForPageID(pageID);
                    ddlDistances.DataTextField = "Description";
                    ddlDistances.DataValueField = "ID";
                    ddlDistances.DataBind();
                    if (currTask != null) {
                        ddlJobs.SelectedIndex = ddlJobs.FindItemByValue(currTask.JobID.ToString()).Index;
                        Session["JobsID"] = currTask.JobID;
                        ddlCustomers.SelectedIndex = ddlCustomers.FindItemByValue(currTask.CustomerID.ToString()).Index;
                        Session["CustomersID"] = currTask.CustomerID;
                        ddlSatelites.SelectedIndex = ddlSatelites.FindItemByValue(currTask.SateliteID.ToString()).Index;
                        Session["SatelitesID"] = currTask.SateliteID;
                        ddlDistances.SelectedIndex = ddlDistances.FindItemByValue(currTask.DistanceID.ToString()).Index;
                        Session["DistancesID"] = currTask.DistanceID;
                    } else {
                        ddlJobs.SelectedIndex = 0;
                        Session["JobsID"] = ddlJobs.SelectedItem.Value;
                        ddlCustomers.SelectedIndex = 0;
                        Session["CustomersID"] = ddlCustomers.SelectedItem.Value;
                        ddlSatelites.SelectedIndex = 0;
                        Session["SatelitesID"] = ddlSatelites.SelectedItem.Value;
                        ddlDistances.SelectedIndex = 0;
                        Session["DistancesID"] = ddlDistances.SelectedItem.Value;
                    }
                }
                catch (Exception) { }
            }
        }

        private void ShowErrorMessage(int errCode) {
            if (errCode == 1) {
                RadWindowManager1.RadAlert("Η συγκεκριμένη Παραγγελία σχετίζεται με κάποιο Έργο και δεν μπορεί να διαγραφεί!", 400, 200, "Σφάλμα", "");
            } else {
                RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
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
                        if (Session["SatelitesID"] != null) { SatelitesID = int.Parse(Session["SatelitesID"].ToString()); }
                        if (SatelitesID > 0) {
                            curTask.SateliteID = SatelitesID;
                            SatelitesID = -1;
                            Session.Remove("SatelitesID");
                        }
                        if (Session["DistancesID"] != null) { DistancesID = int.Parse(Session["DistancesID"].ToString()); }
                        if (DistancesID > 0) {
                            curTask.DistancesID = DistancesID;
                            DistancesID = -1;
                            Session.Remove("DistancesID");
                        }
                        dbContext.SaveChanges();
                    }
                    catch (Exception) { ShowErrorMessage(-1); }
                }
            }
        }

        protected void gridMain_InsertCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            using (var dbContext = new OTERTConnStr()) {
                var curTask = new Tasks();
                Hashtable values = new Hashtable();
                editableItem.ExtractValues(values);
                if (Session["JobsID"] != null) { JobsID = int.Parse(Session["JobsID"].ToString()); }
                if (Session["CustomersID"] != null) { CustomersID = int.Parse(Session["CustomersID"].ToString()); }
                if (Session["SatelitesID"] != null) { SatelitesID = int.Parse(Session["SatelitesID"].ToString()); }
                if (Session["DistancesID"] != null) { DistancesID = int.Parse(Session["DistancesID"].ToString()); }
                if (JobsID > 0 && CustomersID > 0 && SatelitesID > 0 && DistancesID > 0) {
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
                        curTask.IsDurationInDays = (bool)values["IsDurationInDays"];
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
                        curTask.IsCanceled = (bool)values["IsCanceled"];
                        curTask.CancelPrice = 0;
                        curTask.Comments = (string)values["Comments"];
                        curTask.InvoceComments = (string)values["InvoceComments"];
                        curTask.SateliteID = SatelitesID;
                        dbContext.Tasks.Add(curTask);
                        dbContext.SaveChanges();
                    }
                    catch (Exception) { ShowErrorMessage(-1); }
                    finally {
                        JobsID = -1;
                        Session.Remove("JobsID");
                        CustomersID = -1;
                        Session.Remove("CustomersID");
                        SatelitesID = -1;
                        Session.Remove("SatelitesID");
                        DistancesID = -1;
                        Session.Remove("DistancesID");
                    }
                } else { ShowErrorMessage(-1); }
            }
        }

        protected void gridMain_DeleteCommand(object source, GridCommandEventArgs e) {
            var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var curTask = dbContext.Tasks.Where(n => n.ID == ID).FirstOrDefault();
                if (curTask != null) {
                    dbContext.Tasks.Remove(curTask);
                    try { dbContext.SaveChanges(); }
                    catch (Exception ex) {
                        string err = ex.InnerException.InnerException.Message;
                        int errCode = -1;
                        if (err.StartsWith("The DELETE statement conflicted with the REFERENCE constraint")) { errCode = 1; }
                        ShowErrorMessage(errCode);
                    }
                }
            }
        }

        protected void ddlJobs_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                JobsID = int.Parse(e.Value);
                Session["JobsID"] = JobsID;
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

        protected void ddlSatelites_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                SatelitesID = int.Parse(e.Value);
                Session["SatelitesID"] = SatelitesID;
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

    }

}