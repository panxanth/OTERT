using System;
using System.Collections;
using System.Configuration;
using System.Web.UI;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using OTERT_Telerik.Model;
using OTERT_Telerik.Controller;
using OTERT_Entity;
using System.Collections.Generic;

namespace OTERT_Telerik.Pages.Administrator {

    public partial class EventsList : System.Web.UI.Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;
        protected int newID;
        private FilterData filterData;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Γεγονότων";
                //gridMain.PageSize = 10;
                newID = -1;
                Session.Remove("PlaceID");
                //Session.Remove("FilterRecords");
            }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            try {
                EventsController cont = new EventsController();
                gridMain.VirtualItemCount = cont.CountEvents();
                gridMain.DataSource = cont.GetEvents(recSkip, recTake);
            }
            catch (Exception ex) { }

        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {
            if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                newID = -1;
                Session.Remove("PlaceID");
                GridEditableItem item = e.Item as GridEditableItem;
                RadDropDownList list = item.FindControl("ddlPlaces") as RadDropDownList;
                try {
                    EventB currEvent = e.Item.DataItem as EventB;
                    PlacesController cont = new PlacesController();
                    list.DataSource = cont.GetPlaces();
                    list.DataTextField = "NameGR";
                    list.DataValueField = "ID";                    
                    list.DataBind();
                    if (currEvent != null) {
                        list.SelectedIndex = list.FindItemByValue(currEvent.PlaceID.ToString()).Index;
                        Session["PlaceID"] = currEvent.PlaceID;
                    } else {
                        list.SelectedIndex = 0;
                        Session["PlaceID"] = list.SelectedItem.Value;
                    }
                }
                catch (Exception ex) { }
            }
        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) {

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
                catch (Exception ex) { }

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
                    if (Session["PlaceID"] != null) { newID = int.Parse(Session["PlaceID"].ToString()); }
                    if (newID > 0) {
                        selEvent.PlaceID = newID;
                        newID = -1;
                        Session.Remove("PlaceID");
                    }
                    try { dbContext.SaveChanges(); }
                    catch (Exception ex) { ShowErrorMessage(-1); }
                }
            }
        }

        protected void gridMain_InsertCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            using (var dbContext = new OTERTConnStr()) {
                var selEvent = new Events();
                Hashtable values = new Hashtable();
                editableItem.ExtractValues(values);
                if (Session["PlaceID"] != null) { newID = int.Parse(Session["PlaceID"].ToString()); }
                if (newID > 0) {
                    try {
                        selEvent.PlaceID = newID;
                        selEvent.NameGR = (string)values["NameGR"];
                        selEvent.NameEN = (string)values["NameEN"];
                        dbContext.Events.Add(selEvent);
                        dbContext.SaveChanges();   
                    }
                    catch (Exception ex) { ShowErrorMessage(-1); }
                    finally {
                        newID = -1;
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
                newID = int.Parse(e.Value);
                Session["PlaceID"] = newID;
            }
            catch (Exception ex) { }

        }

    }

}