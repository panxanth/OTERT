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

    public partial class EventsList : System.Web.UI.Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;
        protected int placeID, countryID;
        protected UserB loggedUser;
        private FilterData filterData;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Γεγονότων";
                gridMain.MasterTableView.Caption = "Γεγονότα";
                placeID = -1;
                Session.Remove("PlaceID");
                countryID = -1;
                Session.Remove("CountryID");
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            string recFilter = gridMain.MasterTableView.FilterExpression;
            GridSortExpressionCollection gridSortExxpressions = gridMain.MasterTableView.SortExpressions;
            try {
                EventsController cont = new EventsController();
                gridMain.VirtualItemCount = cont.CountEvents();
                gridMain.DataSource = cont.GetEvents(recSkip, recTake, recFilter, gridSortExxpressions);
            }
            catch (Exception) { }
        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {
            if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                placeID = -1;
                Session.Remove("PlaceID");
                countryID = -1;
                Session.Remove("CountryID");
                GridEditableItem item = e.Item as GridEditableItem;
                RadDropDownList ddlPlaces = item.FindControl("ddlPlaces") as RadDropDownList;
                RadDropDownList ddlCountries = item.FindControl("ddlCountries") as RadDropDownList;
                TextBox txtNameGR = (TextBox)item["NameGR"].Controls[0];
                txtNameGR.Width = Unit.Pixel(300);
                TextBox txtNameEN = (TextBox)item["NameEN"].Controls[0];
                txtNameEN.Width = Unit.Pixel(300);
                try {
                    EventB currEvent = e.Item.DataItem as EventB;
                    CountriesController ccont = new CountriesController();
                    PlacesController cont = new PlacesController();
                    ddlCountries.DataSource = ccont.GetCountries();
                    ddlCountries.DataTextField = "NameGR";
                    ddlCountries.DataValueField = "ID";
                    ddlCountries.DataBind();
                    if (currEvent != null) {
                        ddlCountries.SelectedIndex = ddlCountries.FindItemByValue(currEvent.Place.CountryID.ToString()).Index;
                        Session["CountryID"] = currEvent.Place.CountryID;
                        ddlPlaces.DataSource = cont.GetPlaces().Where(k => k.CountryID == currEvent.Place.CountryID);
                        ddlPlaces.DataTextField = "NameGR";
                        ddlPlaces.DataValueField = "ID";
                        ddlPlaces.DataBind();
                        ddlPlaces.SelectedIndex = ddlPlaces.FindItemByValue(currEvent.PlaceID.ToString()).Index;
                        Session["PlaceID"] = currEvent.PlaceID;
                    } else {
                        ddlCountries.SelectedIndex = 0;
                        Session["CountryID"] = ddlCountries.SelectedItem.Value;
                        ddlPlaces.DataSource = cont.GetPlaces().Where(k => k.CountryID == int.Parse(ddlCountries.SelectedItem.Value));
                        ddlPlaces.DataTextField = "NameGR";
                        ddlPlaces.DataValueField = "ID";
                        ddlPlaces.DataBind();
                        ddlPlaces.SelectedIndex = 0;
                        Session["PlaceID"] = ddlPlaces.SelectedItem.Value;
                    }
                }
                catch (Exception) { }
            }
        }

        protected void gridMain_ItemCommand(object source, GridCommandEventArgs e) {
            if (e.CommandName == RadGrid.FilterCommandName) {
                try {
                    filterData = new FilterData();
                    if (Session["FilterRecords"] != null) { filterData = (FilterData)Session["FilterRecords"]; }
                    Pair filterPair = (Pair)e.CommandArgument;
                    TextBox filterBox = (e.Item as GridFilteringItem)[filterPair.Second.ToString()].Controls[0] as TextBox;
                    string valuetxt = filterBox.Text;
                    if (!string.IsNullOrWhiteSpace(valuetxt)) {
                        filterData.AddRecord(filterPair.First.ToString(), filterPair.Second.ToString(), valuetxt);
                    } else {
                        filterData.RemoveRecord(filterPair.First.ToString(), filterPair.Second.ToString());
                    }
                }
                catch (Exception) { }
                /*
                TextBox filterBox = (e.Item as GridFilteringItem)[filterPair.Second.ToString()].Controls[0] as TextBox;
                SqlConnection conn = new SqlConnection(connStr);
                dt = new DataTable();
                com.Connection = conn;
                com.CommandText = "SELECT * FROM tblEmployees where EmployeeID='" + filterBox.Text + "' OR FirstName='" + filterBox.Text + "' OR LastName='" + filterBox.Text + "' OR Address='" + filterBox.Text + "'";
                sqlda = new SqlDataAdapter(com);
                sqlda.Fill(dt);
                RadGrid1.DataSource = dt;
                RadGrid1.DataBind();
                */
            }
        }

        protected void gridMain_DataBound(object sender, EventArgs e) {
            if (filterData != null) {
                int test1 = filterData.Records.Count;
            }
        }

        private void ShowErrorMessage(int errCode) {
            if (errCode == 1) {
                RadWindowManager1.RadAlert("Το συγκεκριμένο Γεγονός σχετίζεται με κάποια Παραγγελία και δεν μπορεί να διαγραφεί!", 400, 200, "Σφάλμα", "");
            } else {
                RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
            }
        }

        protected void gridMain_UpdateCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            var ID = (int)editableItem.GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var selEvent = dbContext.Events.Where(n => n.ID == ID).FirstOrDefault();
                if (selEvent != null) {
                    editableItem.UpdateValues(selEvent);
                    if (Session["PlaceID"] != null) { placeID = int.Parse(Session["PlaceID"].ToString()); }
                    if (placeID > 0) {
                        selEvent.PlaceID = placeID;
                        placeID = -1;
                        Session.Remove("PlaceID");
                    }
                    try { dbContext.SaveChanges(); }
                    catch (Exception) { ShowErrorMessage(-1); }
                }
            }
        }

        protected void gridMain_InsertCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            using (var dbContext = new OTERTConnStr()) {
                var selEvent = new Events();
                Hashtable values = new Hashtable();
                editableItem.ExtractValues(values);
                if (Session["PlaceID"] != null) { placeID = int.Parse(Session["PlaceID"].ToString()); }
                if (placeID > 0) {
                    try {
                        selEvent.PlaceID = placeID;
                        selEvent.NameGR = (string)values["NameGR"];
                        selEvent.NameEN = (string)values["NameEN"];
                        dbContext.Events.Add(selEvent);
                        dbContext.SaveChanges();   
                    }
                    catch (Exception) { ShowErrorMessage(-1); }
                    finally {
                        placeID = -1;
                        Session.Remove("PlaceID");
                    }
                } else { ShowErrorMessage(-1); }
            }
        }

        protected void gridMain_DeleteCommand(object source, GridCommandEventArgs e) {
            var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var selEvent = dbContext.Events.Where(n => n.ID == ID).FirstOrDefault();
                if (selEvent != null) {
                    dbContext.Events.Remove(selEvent);
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

        protected void ddlPlaces_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                placeID = int.Parse(e.Value);
                Session["PlaceID"] = placeID;
            }
            catch (Exception) { }
        }

        protected void ddlCountries_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                countryID = int.Parse(e.Value);
                Session["CountryID"] = countryID;
                RadDropDownList ddlCountries = (RadDropDownList)sender;
                GridEditableItem item = (GridEditableItem)ddlCountries.NamingContainer;
                RadDropDownList ddlPlaces = (RadDropDownList)item.FindControl("ddlPlaces");
                ddlPlaces.ClearSelection();
                PlacesController cont = new PlacesController();
                ddlPlaces.DataSource = cont.GetPlaces().Where(k => k.CountryID == countryID);
                ddlPlaces.DataTextField = "NameGR";
                ddlPlaces.DataValueField = "ID";
                ddlPlaces.DataBind();
                ddlPlaces.SelectedIndex = 0;
                if (ddlPlaces.Items.Count > 0) { Session["PlaceID"] = ddlPlaces.SelectedItem.Value; } else { Session.Remove("PlaceID"); }
            }
            catch (Exception) { }
        }

    }

}