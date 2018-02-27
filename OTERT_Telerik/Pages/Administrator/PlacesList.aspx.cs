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

    public partial class PlacesList : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;
        protected int newID;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Τοποθεσιών";
                gridMain.MasterTableView.Caption = "Τοποθεσίες";
                newID = -1;
                Session.Remove("CountryID");
            }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            try {
                PlacesController cont = new PlacesController();
                gridMain.VirtualItemCount = cont.CountPlaces();
                gridMain.DataSource = cont.GetPlaces(recSkip, recTake);
            }
            catch (Exception) { }

        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {
            if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                newID = -1;
                Session.Remove("CountryID");
                GridEditableItem item = e.Item as GridEditableItem;
                RadDropDownList list = item.FindControl("ddlCountries") as RadDropDownList;
                try {
                    PlaceB currPlace = e.Item.DataItem as PlaceB;
                    CountriesController cont = new CountriesController();
                    list.DataSource = cont.GetCountries();
                    list.DataTextField = "NameGR";
                    list.DataValueField = "ID";                    
                    list.DataBind();
                    if (currPlace != null) {
                        list.SelectedIndex = list.FindItemByValue(currPlace.CountryID.ToString()).Index;
                        Session["CountryID"] = currPlace.CountryID;
                    } else {
                        list.SelectedIndex = 0;
                        Session["CountryID"] = list.SelectedItem.Value;
                    }
                }
                catch (Exception) { }
            }
        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) {

        }

        private void ShowErrorMessage(int errCode) {
            if (errCode == 1) {
                RadWindowManager1.RadAlert("Η συγκεκριμένη Τοποθεσία σχετίζεται με κάποιο Γεγονός και δεν μπορεί να διαγραφεί!", 400, 200, "Σφάλμα", "");
            } else {
                RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
            }
        }

        protected void gridMain_UpdateCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            var ID = (int)editableItem.GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var place = dbContext.Places.Where(n => n.ID == ID).FirstOrDefault();
                if (place != null) {
                    editableItem.UpdateValues(place);
                    if (Session["CountryID"] != null) { newID = int.Parse(Session["CountryID"].ToString()); }
                    if (newID > 0) {
                        place.CountryID = newID;
                        newID = -1;
                        Session.Remove("CountryID");
                    }
                    try { dbContext.SaveChanges(); }
                    catch (Exception) { ShowErrorMessage(-1); }
                }
            }
        }

        protected void gridMain_InsertCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            using (var dbContext = new OTERTConnStr()) {
                var place = new Places();
                Hashtable values = new Hashtable();
                editableItem.ExtractValues(values);
                if (Session["CountryID"] != null) { newID = int.Parse(Session["CountryID"].ToString()); }
                if (newID > 0) {
                    try {
                        place.CountryID = newID;
                        place.NameGR = (string)values["NameGR"];
                        place.NameEN = (string)values["NameEN"];
                        dbContext.Places.Add(place);
                        dbContext.SaveChanges();
                    }
                    catch (Exception) { ShowErrorMessage(-1); }
                    finally {
                        newID = -1;
                        Session.Remove("CountryID");
                    }
                } else { ShowErrorMessage(-1); }
            }
        }

        protected void gridMain_DeleteCommand(object source, GridCommandEventArgs e) {
            var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var place = dbContext.Places.Where(n => n.ID == ID).FirstOrDefault();
                if (place != null) {
                    dbContext.Places.Remove(place);
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
                newID = int.Parse(e.Value);
                Session["CountryID"] = newID;
            }
            catch (Exception) { }
        }

    }

}