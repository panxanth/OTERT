using System;
using System.Collections;
using System.Configuration;
using System.Web.UI;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using OTERT.Model;
using OTERT.Controller;
using OTERT_Entity;

namespace OTERT.Pages.Administrator {

    public partial class PTStoGR : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle, uploadedFilePath;
        protected int Customer1ID, EventID, CountryID;
        const string fileUploadFolder = "~/UploadedFiles/";
        const int OrderTypeID = 1;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Έργα > ΠΤΣ προς Ελλάδα";
                gridMain.MasterTableView.Caption = "Έργα > ΠΤΣ προς Ελλάδα";
                CountryID = -1;
                Session.Remove("CountryID");
                Customer1ID = -1;
                Session.Remove("Customer1ID");
                EventID = -1;
                Session.Remove("EventID");
            }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.MasterTableView.CurrentPageIndex * gridMain.MasterTableView.PageSize;
            int recTake = gridMain.MasterTableView.PageSize;
            try {
                OrdersController cont = new OrdersController();
                gridMain.VirtualItemCount = cont.CountOrders();
                gridMain.DataSource = cont.GetOrders(recSkip, recTake);
            }
            catch (Exception) { }
        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                if (e.Item is GridDataItem) {
                    GridDataItem item = (GridDataItem)e.Item;
                    ElasticButton img = (ElasticButton)item["btnDelete"].Controls[0];
                    img.ToolTip = "Διαγραφή";
                }
            } else if (e.Item.OwnerTableView.Name == "TasksDetails") {
                if (e.Item is GridDataItem) {
                    GridDataItem item = (GridDataItem)e.Item;
                    ElasticButton img = (ElasticButton)item["btnDelete2"].Controls[0];
                    img.ToolTip = "Διαγραφή";
                }
            } else if (e.Item.OwnerTableView.Name == "AttachedFiles") {
                if (e.Item is GridDataItem) {
                    GridDataItem item = (GridDataItem)e.Item;
                    ElasticButton img = (ElasticButton)item["btnDeleteFile"].Controls[0];
                    img.ToolTip = "Διαγραφή";
                }
            }
        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                    CountryID = -1;
                    Session.Remove("CountryID");
                    Customer1ID = -1;
                    Session.Remove("Customer1ID");
                    EventID = -1;
                    Session.Remove("EventID");
                    GridEditableItem item = e.Item as GridEditableItem;
                    RadDropDownList ddlCountry = item.FindControl("ddlCountry") as RadDropDownList;
                    RadDropDownList ddlCustomer1 = item.FindControl("ddlCustomer1") as RadDropDownList;
                    RadDropDownList ddlEvent = item.FindControl("ddlEvent") as RadDropDownList;
                    try {
                        OrderB currOrder = e.Item.DataItem as OrderB;
                        CountriesController ccont = new CountriesController();
                        ddlCountry.DataSource = ccont.GetCountries();
                        ddlCountry.DataTextField = "NameGR";
                        ddlCountry.DataValueField = "ID";
                        ddlCountry.DataBind();
                        ddlCountry.Items.Insert(0, new DropDownListItem("Όλες οι Χώρες ...", "0"));
                        CustomersController custcont = new CustomersController();
                        ddlCustomer1.DataSource = custcont.GetProviders();
                        ddlCustomer1.DataTextField = "NameGR";
                        ddlCustomer1.DataValueField = "ID";
                        ddlCustomer1.DataBind();
                        EventsController econt = new EventsController();
                        ddlEvent.DataSource = econt.GetEvents();
                        ddlEvent.DataTextField = "NameGR";
                        ddlEvent.DataValueField = "ID";
                        ddlEvent.DataBind();
                        if (currOrder != null) {
                            if (currOrder.Event != null) {
                                ddlCountry.SelectedIndex = ddlCountry.FindItemByValue(currOrder.Event.Place.CountryID.ToString()).Index;
                                Session["CountryID"] = currOrder.Event.Place.CountryID;
                            } else {
                                ddlCountry.SelectedIndex = 0;
                                Session["CountryID"] = ddlCountry.SelectedItem.Value;
                            }
                            ddlCustomer1.SelectedIndex = ddlCustomer1.FindItemByValue(currOrder.Customer1ID.ToString()).Index;
                            Session["Customer1ID"] = currOrder.Customer1ID;
                            ddlEvent.SelectedIndex = ddlEvent.FindItemByValue(currOrder.EventID.ToString()).Index;
                            Session["EventID"] = currOrder.EventID;
                        } else {
                            ddlCountry.SelectedIndex = 0;
                            Session["CountryID"] = ddlCountry.SelectedItem.Value;
                            ddlCustomer1.SelectedIndex = 0;
                            Session["Customer1ID"] = ddlCustomer1.SelectedItem.Value;
                            ddlEvent.SelectedIndex = 0;
                            Session["EventID"] = ddlEvent.SelectedItem.Value;
                        }
                    }
                    catch (Exception) { }
                }
            }
        }

        protected void gridMain_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e) {
            if (e.DetailTableView.Name == "TasksDetails") {
                GridTableView detailtabl = (GridTableView)e.DetailTableView;
                int recSkip = detailtabl.CurrentPageIndex * gridMain.PageSize;
                int recTake = detailtabl.PageSize;
                GridDataItem parentItem = (GridDataItem)detailtabl.ParentItem;
                int orderID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
                TasksController cont = new TasksController();
                detailtabl.DataSource = cont.GetTasksForOrder(orderID);
            } else if (e.DetailTableView.Name == "AttachedFiles") {
                GridTableView detailtabl = e.DetailTableView;
                int recSkip = detailtabl.CurrentPageIndex * gridMain.PageSize;
                int recTake = detailtabl.PageSize;
                GridDataItem parentItem = detailtabl.ParentItem;
                int taskID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
                FilesController cont = new FilesController();
                detailtabl.VirtualItemCount = cont.CountFiles(taskID);
                detailtabl.DataSource = cont.GetFilesByTaskID(taskID, recSkip, recTake);
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
                default:
                    RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
                    break;
            }
        }

        protected void gridMain_UpdateCommand(object source, GridCommandEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                var editableItem = ((GridEditableItem)e.Item);
                var ID = (int)editableItem.GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curOrder = dbContext.Orders.Where(n => n.ID == ID).FirstOrDefault();
                    if (curOrder != null) {
                        editableItem.UpdateValues(curOrder);
                        if (Session["Customer1ID"] != null) { Customer1ID = int.Parse(Session["Customer1ID"].ToString()); }
                        if (Customer1ID > 0) {
                            curOrder.Customer1ID = Customer1ID;
                            Customer1ID = -1;
                            Session.Remove("Customer1ID");
                        }
                        if (Session["EventID"] != null) { EventID = int.Parse(Session["EventID"].ToString()); }
                        if (EventID > 0) {
                            curOrder.EventID = EventID;
                            EventID = -1;
                            Session.Remove("EventID");
                        }
                        CountryID = -1;
                        Session.Remove("CountryID");
                        try { dbContext.SaveChanges(); }
                        catch (Exception) { ShowErrorMessage(-1); }
                    }
                }
            } else if (e.Item.OwnerTableView.Name == "TasksDetails") {
                var editableItem = ((GridEditableItem)e.Item);
                var ID = (int)editableItem.GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curJobFurmula = dbContext.JobFormulas.Where(n => n.ID == ID).FirstOrDefault();
                    if (curJobFurmula != null) {
                        editableItem.UpdateValues(curJobFurmula);
                        try { dbContext.SaveChanges(); }
                        catch (Exception) { ShowErrorMessage(-1); }
                    }
                }
            }
        }

        protected void gridMain_InsertCommand(object source, GridCommandEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                var editableItem = ((GridEditableItem)e.Item);
                using (var dbContext = new OTERTConnStr()) {
                    var curOrder = new Orders();
                    Hashtable values = new Hashtable();
                    editableItem.ExtractValues(values);
                    if (Session["Customer1ID"] != null) { Customer1ID = int.Parse(Session["Customer1ID"].ToString()); }
                    if (Session["EventID"] != null) { EventID = int.Parse(Session["EventID"].ToString()); }
                    if (Customer1ID > 0 && EventID > 0) {
                        try {
                            curOrder.RegNo = (string)values["RegNo"];
                            curOrder.OrderTypeID = OrderTypeID;
                            curOrder.Customer1ID = Customer1ID;
                            curOrder.EventID = EventID;
                            curOrder.IsLocked = (bool)values["IsLocked"];
                            dbContext.Orders.Add(curOrder);
                            dbContext.SaveChanges();
                        }
                        catch (Exception) { ShowErrorMessage(-1); }
                        finally {
                            CountryID = -1;
                            Session.Remove("CountryID");
                            Customer1ID = -1;
                            Session.Remove("Customer1ID");
                            EventID = -1;
                            Session.Remove("EventID");
                        }
                    } else { ShowErrorMessage(-1); }
                }
            } else if (e.Item.OwnerTableView.Name == "TasksDetails") {
                GridTableView detailtabl = (GridTableView)e.Item.OwnerTableView;
                GridDataItem parentItem = (GridDataItem)detailtabl.ParentItem;
                int jobsID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
                var editableItem = ((GridEditableItem)e.Item);
                using (var dbContext = new OTERTConnStr()) {
                    var curJobFurmula = new JobFormulas();
                    Hashtable values = new Hashtable();
                    editableItem.ExtractValues(values);
                    curJobFurmula.JobsID = jobsID;
                    curJobFurmula.Formula = (string)values["Formula"];
                    curJobFurmula.Condition = (string)values["Condition"];
                    dbContext.JobFormulas.Add(curJobFurmula);
                    try { dbContext.SaveChanges(); }
                    catch (Exception) { ShowErrorMessage(-1); }
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
            /*
            if (e.Item.OwnerTableView.Name == "Master") {
                var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curJob = dbContext.Jobs.Where(n => n.ID == ID).FirstOrDefault();
                    if (curJob != null) {
                        dbContext.Jobs.Remove(curJob);
                        try { dbContext.SaveChanges(); }
                        catch (Exception ex) {
                            string err = ex.InnerException.InnerException.Message;
                            int errCode = -1;
                            if (err.StartsWith("The DELETE statement conflicted with the REFERENCE constraint")) { errCode = 1; }
                            ShowErrorMessage(errCode);
                        }
                    }
                }
            } else if (e.Item.OwnerTableView.Name == "TasksDetails") {
                var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curJobFurmula = dbContext.JobFormulas.Where(n => n.ID == ID).FirstOrDefault();
                    if (curJobFurmula != null) {
                        dbContext.JobFormulas.Remove(curJobFurmula);
                        try { dbContext.SaveChanges(); }
                        catch (Exception) { ShowErrorMessage(-1); }
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
            */
        }

        protected void ddlCustomer1_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                Customer1ID = int.Parse(e.Value);
                Session["Customer1ID"] = Customer1ID;
            }
            catch (Exception) { }
        }

        protected void ddlEvent_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                EventID = int.Parse(e.Value);
                Session["EventID"] = EventID;
            }
            catch (Exception) { }
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                CountryID = int.Parse(e.Value);
                Session["CountryID"] = CountryID;
                if (CountryID > 0) {
                    RadDropDownList ddlCountries = (RadDropDownList)sender;
                    GridEditableItem item = (GridEditableItem)ddlCountries.NamingContainer;
                    RadDropDownList ddlCustomer1 = (RadDropDownList)item.FindControl("ddlCustomer1");
                    ddlCustomer1.ClearSelection();
                    CustomersController custcont = new CustomersController();
                    ddlCustomer1.DataSource = custcont.GetProvidersForCountry(CountryID);
                    ddlCustomer1.DataTextField = "NameGR";
                    ddlCustomer1.DataValueField = "ID";
                    ddlCustomer1.DataBind();
                    ddlCustomer1.SelectedIndex = 0;
                    if (ddlCustomer1.Items.Count > 0) { Session["Customer1ID"] = ddlCustomer1.SelectedItem.Value; } else { Session.Remove("Customer1ID"); }
                    RadDropDownList ddlEvent = (RadDropDownList)item.FindControl("ddlEvent");
                    ddlEvent.ClearSelection();
                    EventsController econt = new EventsController();
                    ddlEvent.DataSource = econt.GetEventsForCountry(CountryID);
                    ddlEvent.DataTextField = "NameGR";
                    ddlEvent.DataValueField = "ID";
                    ddlEvent.DataBind();
                    ddlEvent.SelectedIndex = 0;
                    if (ddlEvent.Items.Count > 0) { Session["EventID"] = ddlEvent.SelectedItem.Value; } else { Session.Remove("EventID"); }
                } else {
                    RadDropDownList ddlCountries = (RadDropDownList)sender;
                    GridEditableItem item = (GridEditableItem)ddlCountries.NamingContainer;
                    RadDropDownList ddlCustomer1 = (RadDropDownList)item.FindControl("ddlCustomer1");
                    ddlCustomer1.ClearSelection();
                    CustomersController custcont = new CustomersController();
                    ddlCustomer1.DataSource = custcont.GetProviders();
                    ddlCustomer1.DataTextField = "NameGR";
                    ddlCustomer1.DataValueField = "ID";
                    ddlCustomer1.DataBind();
                    ddlCustomer1.SelectedIndex = 0;
                    Session["Customer1ID"] = ddlCustomer1.SelectedItem.Value;
                    RadDropDownList ddlEvent = (RadDropDownList)item.FindControl("ddlEvent");
                    ddlEvent.ClearSelection();
                    EventsController econt = new EventsController();
                    ddlEvent.DataSource = econt.GetEvents();
                    ddlEvent.DataTextField = "NameGR";
                    ddlEvent.DataValueField = "ID";
                    ddlEvent.DataBind();
                    ddlEvent.SelectedIndex = 0;
                    Session["EventID"] = ddlEvent.SelectedItem.Value;
                }
            }
            catch (Exception) { }
        }




        protected void ddlCustomers_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                //CustomersID = int.Parse(e.Value);
                //Session["CustomersID"] = CustomersID;
            }
            catch (Exception) { }
        }

        protected void ddlRequestedPosition_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                //DistancesID = int.Parse(e.Value);
                //Session["DistancesID"] = DistancesID;
            }
            catch (Exception) { }
        }

        protected void txtAddedCharges_TextChanged(object sender, EventArgs e) {
            //TextBox txtAddedCharges = ((TextBox)(sender));
            //GridEditableItem eitem = (GridEditableItem)txtAddedCharges.NamingContainer;
            //calculateCosts(eitem);
        }

        protected void calculateCosts(GridEditableItem eitem) {
            /*
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
            RadDropDownList ddlSatelites = (RadDropDownList)eitem.FindControl("ddlSatelites");
            int sateliteID = int.Parse(ddlSatelites.SelectedItem.Value);
            SatelitesController contD = new SatelitesController();
            SateliteB selectedSatelite = contD.GetSatelite(sateliteID);
            int jobID = -1;
            if (Session["JobsID"] != null) {
                jobID = int.Parse(Session["JobsID"].ToString());
                JobFormulasController cont = new JobFormulasController();
                List<JobFormulaB> curJobFormulas = cont.GetJobFormulas(jobID);
                string formula = "";
                if (orderStartDate > nullDate && orderEndDate > nullDate && orderEndDate > orderStartDate) {
                    TimeSpan orderSpan = orderEndDate.Subtract(orderStartDate);
                    txtOrderDurationOrder.Text = ((int)Math.Ceiling(orderSpan.TotalMinutes)).ToString();
                    formula = findFormula(curJobFormulas, (int)Math.Ceiling(orderSpan.TotalMinutes), double.Parse(selectedSatelite.Frequency.Replace(".", ",")), -1);
                    formula = formula.Replace("#TIME#", ((int)Math.Ceiling(orderSpan.TotalMinutes)).ToString());
                    formula = formula.Replace("#BANDWIDTH#", selectedSatelite.Frequency);
                    //formula = formula.Replace("#DISTANCE#", selectedDistance.KM.ToString());
                    formula = formula.Replace(",", ".");
                    double calculatedCost = Evaluator.EvalToDouble(formula);
                    if (!string.IsNullOrEmpty(txtAddedCharges.Text)) { calculatedCost += double.Parse(txtAddedCharges.Text.Replace(".", ",")); }
                    txtCostCalculated.Text = calculatedCost.ToString();
                }
                if (actualStartDate > nullDate && actualEndDate > nullDate && actualEndDate > actualStartDate) {
                    TimeSpan actualSpan = actualEndDate.Subtract(actualStartDate);
                    txtActualDuration.Text = ((int)Math.Ceiling(actualSpan.TotalMinutes)).ToString();
                    formula = findFormula(curJobFormulas, (int)Math.Ceiling(actualSpan.TotalMinutes), double.Parse(selectedSatelite.Frequency), -1);
                    formula = formula.Replace("#TIME#", ((int)Math.Ceiling(actualSpan.TotalMinutes)).ToString());
                    formula = formula.Replace("#BANDWIDTH#", selectedSatelite.Frequency);
                    //formula = formula.Replace("#DISTANCE#", selectedDistance.KM.ToString());
                    formula = formula.Replace(",", ".");
                    double calculatedCost = Evaluator.EvalToDouble(formula);
                    if (!string.IsNullOrEmpty(txtAddedCharges.Text)) { calculatedCost += double.Parse(txtAddedCharges.Text.Replace(".", ",")); }
                    txtCostActual.Text = calculatedCost.ToString();
                }
            }
            */
        }

        protected void uplFile_FileUploaded(object sender, FileUploadedEventArgs e) {
            string fullPath = Server.MapPath(fileUploadFolder);
            bool exists = System.IO.Directory.Exists(fullPath);
            if (!exists) { System.IO.Directory.CreateDirectory(fullPath); }
            string newfilename = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + "_" + e.File.GetNameWithoutExtension().Replace(" ", "_") + e.File.GetExtension();
            uploadedFilePath = fileUploadFolder + newfilename;
            e.File.SaveAs(System.IO.Path.Combine(fullPath, newfilename));
        }

    }

}