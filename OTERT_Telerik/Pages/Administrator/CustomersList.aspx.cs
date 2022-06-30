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
using System.Collections.Generic;

namespace OTERT.Pages.Administrator {

    public partial class CustomersList : System.Web.UI.Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle, uploadedFilePath;
        protected int countryID, customerTypeID, languageID, userID;
        protected UserB loggedUser;
        const string fileUploadFolder = "~/UploadedFiles/";

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Πελατών";
                gridMain.MasterTableView.Caption = "Πελάτες";
                countryID = -1;
                Session.Remove("CountryID");
                customerTypeID = -1;
                Session.Remove("CustomerTypeID");
                languageID = -1;
                Session.Remove("LanguageID");
                userID = -1;
                Session.Remove("UserID");
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            string recFilter = gridMain.MasterTableView.FilterExpression;
            try {
                CustomersController cont = new CustomersController();
                gridMain.VirtualItemCount = cont.CountCustomers(recFilter);
                gridMain.DataSource = cont.GetCustomers(recSkip, recTake, recFilter);
            }
            catch (Exception) { }

        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {
            if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                countryID = -1;
                Session.Remove("CountryID");
                customerTypeID = -1;
                Session.Remove("CustomerTypeID");
                languageID = -1;
                Session.Remove("LanguageID");
                userID = -1;
                Session.Remove("UserID");
                GridEditableItem item = e.Item as GridEditableItem;
                RadDropDownList ddlCountries = item.FindControl("ddlCountries") as RadDropDownList;
                RadDropDownList ddlCustomerTypes = item.FindControl("ddlCustomerTypes") as RadDropDownList;
                RadDropDownList ddlLanguages = item.FindControl("ddlLanguages") as RadDropDownList;
                RadDropDownList ddlUsers = item.FindControl("ddlUsers") as RadDropDownList;
                try {
                    CustomerB currCustomer = e.Item.DataItem as CustomerB;
                    CountriesController countriesCont = new CountriesController();
                    ddlCountries.DataSource = countriesCont.GetCountries();
                    ddlCountries.DataTextField = "NameGR";
                    ddlCountries.DataValueField = "ID";
                    ddlCountries.DataBind();
                    if (currCustomer != null) {
                        ddlCountries.SelectedIndex = ddlCountries.FindItemByValue(currCustomer.CountryID.ToString()).Index;
                        Session["CountryID"] = currCustomer.CountryID;
                    } else {
                        ddlCountries.SelectedIndex = ddlCountries.FindItemByValue("1").Index; ;
                        Session["CountryID"] = ddlCountries.SelectedItem.Value;
                    }
                    CustomerTypesController customerTypesCont = new CustomerTypesController();
                    ddlCustomerTypes.DataSource = customerTypesCont.GetCustomerTypes();
                    ddlCustomerTypes.DataTextField = "NameGR";
                    ddlCustomerTypes.DataValueField = "ID";
                    ddlCustomerTypes.DataBind();
                    if (currCustomer != null) {
                        ddlCustomerTypes.SelectedIndex = ddlCustomerTypes.FindItemByValue(currCustomer.CustomerTypeID.ToString()).Index;
                        Session["CustomerTypeID"] = currCustomer.CustomerTypeID;
                    } else {
                        ddlCustomerTypes.SelectedIndex = 0;
                        Session["CustomerTypeID"] = ddlCustomerTypes.SelectedItem.Value;
                    }
                    LanguagesController languagesCont = new LanguagesController();
                    ddlLanguages.DataSource = languagesCont.GetLanguages();
                    ddlLanguages.DataTextField = "Name";
                    ddlLanguages.DataValueField = "ID";
                    ddlLanguages.DataBind();
                    if (currCustomer != null) {
                        ddlLanguages.SelectedIndex = ddlLanguages.FindItemByValue(currCustomer.LanguageID.ToString()).Index;
                        Session["LanguageID"] = currCustomer.LanguageID;
                    } else {
                        ddlLanguages.SelectedIndex = ddlLanguages.FindItemByValue("1").Index;
                        Session["LanguageID"] = ddlLanguages.SelectedItem.Value;
                    }
                    UsersController usersCont = new UsersController();
                    ddlUsers.DataSource = usersCont.GetUsers();
                    ddlUsers.DataTextField = "NameGR";
                    ddlUsers.DataValueField = "ID";
                    ddlUsers.DataBind();
                    if (currCustomer != null) {
                        if (currCustomer.UserID != null) {
                            ddlUsers.SelectedIndex = ddlUsers.FindItemByValue(currCustomer.UserID.ToString()).Index;
                        }
                        Session["UserID"] = currCustomer.UserID;
                    } else {
                        ddlUsers.SelectedIndex = 0;
                        Session["UserID"] = ddlUsers.SelectedItem.Value;
                    }
                }
                catch (Exception) { }
            }
            if (e.Item is GridFilteringItem) {
                GridFilteringItem filterItem = (GridFilteringItem)e.Item;
                RadDropDownList cflist = (RadDropDownList)filterItem.FindControl("ddlCountryFilter");
                RadDropDownList ctflist = (RadDropDownList)filterItem.FindControl("ddlCustomerTypeFilter");
                RadDropDownList lflist = (RadDropDownList)filterItem.FindControl("ddlLanguageFilter");
                RadDropDownList uflist = (RadDropDownList)filterItem.FindControl("ddlUserFilter");
                try {
                    CountriesController ccont = new CountriesController();
                    cflist.DataSource = ccont.GetCountries();
                    cflist.DataTextField = "NameGR";
                    cflist.DataValueField = "ID";
                    cflist.DataBind();
                    cflist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                    CustomerTypesController ctcont = new CustomerTypesController();
                    ctflist.DataSource = ctcont.GetCustomerTypes();
                    ctflist.DataTextField = "NameGR";
                    ctflist.DataValueField = "ID";
                    ctflist.DataBind();
                    ctflist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                    LanguagesController lcont = new LanguagesController();
                    lflist.DataSource = lcont.GetLanguages();
                    lflist.DataTextField = "Name";
                    lflist.DataValueField = "ID";
                    lflist.DataBind();
                    lflist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                    UsersController ucont = new UsersController();
                    uflist.DataSource = ucont.GetUsers();
                    uflist.DataTextField = "NameGR";
                    uflist.DataValueField = "ID";
                    uflist.DataBind();
                    uflist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                }
                catch (Exception) { }
            }
        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) {
            if (e.Item.OwnerTableView.Name == "AttachedFiles") {
                if (e.Item is GridDataItem) {
                    GridDataItem item = (GridDataItem)e.Item;
                    ElasticButton img = (ElasticButton)item["btnDeleteFile"].Controls[0];
                    img.ToolTip = "Διαγραφή";
                }
            }
        }

        protected void gridMain_ItemCommand(object source, GridCommandEventArgs e) {
            if (e.CommandName == RadGrid.FilterCommandName) { }
        }

        private void ShowErrorMessage(int errCode) {
            if (errCode == 1) {
                RadWindowManager1.RadAlert("O συγκεκριμένος Πελάτης σχετίζεται με κάποια Παραγγελία ή/και με κάποιο Τιμοκατάλογο Χώρας και δεν μπορεί να διαγραφεί!", 400, 200, "Σφάλμα", "");
            } else {
                RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
            }
        }

        protected void gridMain_UpdateCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            var ID = (int)editableItem.GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var selCustomer = dbContext.Customers.Where(n => n.ID == ID).FirstOrDefault();
                if (selCustomer != null) {
                    editableItem.UpdateValues(selCustomer);
                    if (Session["CountryID"] != null) { countryID = int.Parse(Session["CountryID"].ToString()); }
                    if (countryID > 0) {
                        selCustomer.CountryID = countryID;
                        countryID = -1;
                        Session.Remove("CountryID");
                    }
                    if (Session["CustomerTypeID"] != null) { customerTypeID = int.Parse(Session["CustomerTypeID"].ToString()); }
                    if (customerTypeID > 0) {
                        selCustomer.CustomerTypeID = customerTypeID;
                        customerTypeID = -1;
                        Session.Remove("CustomerTypeID");
                    }
                    if (Session["LanguageID"] != null) { languageID = int.Parse(Session["LanguageID"].ToString()); }
                    if (languageID > 0) {
                        selCustomer.LanguageID = languageID;
                        languageID = -1;
                        Session.Remove("LanguageID");
                    }
                    if (Session["UserID"] != null) { userID = int.Parse(Session["UserID"].ToString()); }
                    if (userID > 0) {
                        selCustomer.UserID = userID;
                        userID = -1;
                        Session.Remove("UserID");
                    }
                    try { dbContext.SaveChanges(); }
                    catch (Exception) { ShowErrorMessage(-1); }
                }
            }
        }

        protected void gridMain_InsertCommand(object source, GridCommandEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                var editableItem = ((GridEditableItem)e.Item);
                using (var dbContext = new OTERTConnStr()) {
                    var selCustomer = new Customers();
                    Hashtable values = new Hashtable();
                    editableItem.ExtractValues(values);
                    if (Session["CountryID"] != null) { countryID = int.Parse(Session["CountryID"].ToString()); }
                    if (Session["CustomerTypeID"] != null) { customerTypeID = int.Parse(Session["CustomerTypeID"].ToString()); }
                    if (Session["LanguageID"] != null) { languageID = int.Parse(Session["LanguageID"].ToString()); }
                    if (Session["UserID"] != null) { userID = int.Parse(Session["UserID"].ToString()); }
                    if (countryID > 0 && customerTypeID > 0 && languageID > 0 && userID > 0) {
                        try {
                            selCustomer.CountryID = countryID;
                            selCustomer.NameGR = (string)values["NameGR"];
                            selCustomer.NameEN = (string)values["NameEN"];
                            selCustomer.NamedInvoiceGR = (string)values["NamedInvoiceGR"];
                            selCustomer.NamedInvoiceEN = (string)values["NamedInvoiceEN"];
                            selCustomer.ZIPCode = (string)values["ZIPCode"];
                            selCustomer.CityGR = (string)values["CityGR"];
                            selCustomer.CityEN = (string)values["CityEN"];
                            selCustomer.ChargeTelephone = (string)values["ChargeTelephone"];
                            selCustomer.Telephone1 = (string)values["Telephone1"];
                            selCustomer.Telephone2 = (string)values["Telephone2"];
                            selCustomer.FAX1 = (string)values["FAX1"];
                            selCustomer.FAX2 = (string)values["FAX2"];
                            selCustomer.Address1GR = (string)values["Address1GR"];
                            selCustomer.Address1EN = (string)values["Address1EN"];
                            selCustomer.Address2GR = (string)values["Address2GR"];
                            selCustomer.Address2EN = (string)values["Address2EN"];
                            selCustomer.ContactPersonGR = (string)values["ContactPersonGR"];
                            selCustomer.ContactPersonEN = (string)values["ContactPersonEN"];
                            selCustomer.ContactPersonInvoice = (string)values["ContactPersonInvoice"];
                            selCustomer.CustomerTypeID = customerTypeID;
                            selCustomer.LanguageID = languageID;
                            selCustomer.Email = (string)values["Email"];
                            selCustomer.URL = (string)values["URL"];
                            selCustomer.AFM = (string)values["AFM"];
                            selCustomer.DOY = (string)values["DOY"];
                            selCustomer.SAPCode = (string)values["SAPCode"];
                            selCustomer.UserID = userID;
                            selCustomer.Comments = (string)values["Comments"];
                            selCustomer.IsProvider = (bool)values["IsProvider"];
                            selCustomer.IsPTS = (bool)values["IsPTS"];
                            selCustomer.IsTemporary = (bool)values["IsTemporary"];
                            dbContext.Customers.Add(selCustomer);
                            dbContext.SaveChanges();
                        }
                        catch (Exception) { ShowErrorMessage(-1); }
                        finally {
                            countryID = -1;
                            Session.Remove("CountryID");
                            customerTypeID = -1;
                            Session.Remove("CustomerTypeID");
                            languageID = -1;
                            Session.Remove("LanguageID");
                            userID = -1;
                            Session.Remove("UserID");
                        }
                    } else { ShowErrorMessage(-1); }
                }
            } else if (e.Item.OwnerTableView.Name == "AttachedFiles") {
                GridTableView detailtabl = e.Item.OwnerTableView;
                GridDataItem parentItem = detailtabl.ParentItem;
                int customerID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
                var editableItem = ((GridEditableItem)e.Item);
                using (var dbContext = new OTERTConnStr()) {
                    var curFile = new Files();
                    Hashtable values = new Hashtable();
                    editableItem.ExtractValues(values);
                    curFile.CustomerID = customerID;
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
                    var selCustomer = dbContext.Customers.Where(n => n.ID == ID).FirstOrDefault();
                    if (selCustomer != null) {
                        List<Files> curFiles = dbContext.Files.Where(k => k.CustomerID == ID).ToList();
                        foreach (Files curFile in curFiles) {
                            string FileToDelete = Server.MapPath(curFile.FilePath);
                            if (System.IO.File.Exists(FileToDelete)) { System.IO.File.Delete(FileToDelete); }
                            dbContext.Files.Remove(curFile);
                            try { dbContext.SaveChanges(); }
                            catch (Exception) { ShowErrorMessage(-1); }
                        }
                        dbContext.Customers.Remove(selCustomer);
                        try { dbContext.SaveChanges(); }
                        catch (Exception ex) {
                            string err = ex.InnerException.InnerException.Message;
                            int errCode = -1;
                            if (err.StartsWith("The DELETE statement conflicted with the REFERENCE constraint")) { errCode = 1; }
                            ShowErrorMessage(errCode);
                        }
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

        protected void ddlCountries_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                countryID = int.Parse(e.Value);
                Session["CountryID"] = countryID;
            }
            catch (Exception) { }
        }

        protected void ddlCustomerTypes_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                customerTypeID = int.Parse(e.Value);
                Session["CustomerTypeID"] = customerTypeID;
            }
            catch (Exception) { }
        }

        protected void ddlLanguages_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                languageID = int.Parse(e.Value);
                Session["LanguageID"] = languageID;
            }
            catch (Exception) { }
        }

        protected void ddlUsers_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                userID = int.Parse(e.Value);
                Session["UserID"] = userID;
            }
            catch (Exception) { }
        }

        protected void gridMain_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e) {
            GridTableView detailtabl = e.DetailTableView;
            int recSkip = detailtabl.CurrentPageIndex * gridMain.PageSize;
            int recTake = detailtabl.PageSize;
            GridDataItem parentItem = detailtabl.ParentItem;
            int customerID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
            FilesController cont = new FilesController();
            detailtabl.VirtualItemCount = cont.CountFilesByCustomerID(customerID);
            detailtabl.DataSource = cont.GetFilesByCustomerID(customerID, recSkip, recTake);
        }

        protected void uplFile_FileUploaded(object sender, FileUploadedEventArgs e) {
            string fullPath = Server.MapPath(fileUploadFolder);
            bool exists = System.IO.Directory.Exists(fullPath);
            if (!exists) { System.IO.Directory.CreateDirectory(fullPath); }
            string newfilename = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + "_" + e.File.GetNameWithoutExtension().Replace(" ", "_") + e.File.GetExtension();
            uploadedFilePath = fileUploadFolder + newfilename;
            e.File.SaveAs(System.IO.Path.Combine(fullPath, newfilename));
        }

        protected void ddlCountryFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("CountryID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(CountryID = " + e.Value + ")";
                gridMain.MasterTableView.GetColumn("CountryID").CurrentFilterFunction = GridKnownFunction.EqualTo;
                gridMain.MasterTableView.GetColumn("CountryID").CurrentFilterValue = e.Value;
            } else {
                gridMain.MasterTableView.GetColumn("CountryID").CurrentFilterFunction = GridKnownFunction.NoFilter;
                gridMain.MasterTableView.GetColumn("CountryID").CurrentFilterValue = null;
            }
            gridMain.MasterTableView.FilterExpression = finalExpression;
            ViewState[list.ClientID] = e.Value;
            gridMain.MasterTableView.Rebind();
        }

        protected void ddlCountryFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

        protected void ddlCustomerTypeFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("CustomerTypeID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(CustomerTypeID = " + e.Value + ")";
                gridMain.MasterTableView.GetColumn("CustomerTypeID").CurrentFilterFunction = GridKnownFunction.EqualTo;
                gridMain.MasterTableView.GetColumn("CustomerTypeID").CurrentFilterValue = e.Value;
            } else {
                gridMain.MasterTableView.GetColumn("CustomerTypeID").CurrentFilterFunction = GridKnownFunction.NoFilter;
                gridMain.MasterTableView.GetColumn("CustomerTypeID").CurrentFilterValue = null;
            }
            gridMain.MasterTableView.FilterExpression = finalExpression;
            ViewState[list.ClientID] = e.Value;
            gridMain.MasterTableView.Rebind();
        }

        protected void ddlCustomerTypeFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

        protected void ddlLanguageFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("LanguageID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(LanguageID = " + e.Value + ")";
                gridMain.MasterTableView.GetColumn("LanguageID").CurrentFilterFunction = GridKnownFunction.EqualTo;
                gridMain.MasterTableView.GetColumn("LanguageID").CurrentFilterValue = e.Value;
            } else {
                gridMain.MasterTableView.GetColumn("LanguageID").CurrentFilterFunction = GridKnownFunction.NoFilter;
                gridMain.MasterTableView.GetColumn("LanguageID").CurrentFilterValue = null;
            }
            gridMain.MasterTableView.FilterExpression = finalExpression;
            ViewState[list.ClientID] = e.Value;
            gridMain.MasterTableView.Rebind();
        }

        protected void ddlLanguageFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

        protected void ddlUserFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("UserID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(UserID = " + e.Value + ")";
                gridMain.MasterTableView.GetColumn("UserID").CurrentFilterFunction = GridKnownFunction.EqualTo;
                gridMain.MasterTableView.GetColumn("UserID").CurrentFilterValue = e.Value;
            } else {
                gridMain.MasterTableView.GetColumn("UserID").CurrentFilterFunction = GridKnownFunction.NoFilter;
                gridMain.MasterTableView.GetColumn("UserID").CurrentFilterValue = null;
            }
            gridMain.MasterTableView.FilterExpression = finalExpression;
            ViewState[list.ClientID] = e.Value;
            gridMain.MasterTableView.Rebind();
        }

        protected void ddlUserFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

    }

}