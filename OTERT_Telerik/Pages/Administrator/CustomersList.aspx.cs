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
        protected string pageTitle;
        protected int countryID, customerTypeID, languageID, userID;

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
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            try {
                CustomersController cont = new CustomersController();
                gridMain.VirtualItemCount = cont.CountCustomers();
                gridMain.DataSource = cont.GetCustomers(recSkip, recTake);
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
                        ddlCountries.SelectedIndex = 0;
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
                        ddlLanguages.SelectedIndex = 0;
                        Session["LanguageID"] = ddlLanguages.SelectedItem.Value;
                    }
                    UsersController usersCont = new UsersController();
                    ddlUsers.DataSource = usersCont.GetUsers();
                    ddlUsers.DataTextField = "NameGR";
                    ddlUsers.DataValueField = "ID";
                    ddlUsers.DataBind();
                    if (currCustomer != null) {
                        ddlUsers.SelectedIndex = ddlUsers.FindItemByValue(currCustomer.UserID.ToString()).Index;
                        Session["UserID"] = currCustomer.UserID;
                    } else {
                        ddlUsers.SelectedIndex = 0;
                        Session["UserID"] = ddlUsers.SelectedItem.Value;
                    }
                }
                catch (Exception) { }
            }
        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) {

        }

        protected void gridMain_ItemCommand(object source, GridCommandEventArgs e) {
            if (e.CommandName == RadGrid.FilterCommandName) {

            }
        }

        protected void gridMain_DataBound(object sender, EventArgs e) {

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
                        selCustomer.CustomerTypeID = customerTypeID;
                        selCustomer.LanguageID = languageID;
                        selCustomer.Email = (string)values["Email"];
                        selCustomer.URL = (string)values["URL"];
                        selCustomer.AFM = (string)values["AFM"];
                        selCustomer.DOY = (string)values["DOY"];
                        selCustomer.UserID = userID;
                        selCustomer.Comments = (string)values["Comments"];
                        selCustomer.IsProvider = (bool)values["IsProvider"];
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
        }

        protected void gridMain_DeleteCommand(object source, GridCommandEventArgs e) {
            var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var selCustomer = dbContext.Customers.Where(n => n.ID == ID).FirstOrDefault();
                if (selCustomer != null) {
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

    }

}