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

    public partial class UsersList : System.Web.UI.Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;
        protected int newID;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Χρηστών";
                //gridMain.PageSize = 10;
                newID = -1;
                Session.Remove("UserGroupID");
            }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            try {
                UsersController cont = new UsersController();
                gridMain.VirtualItemCount = cont.CountUsers();
                gridMain.DataSource = cont.GetUsers(recSkip, recTake);
            }
            catch (Exception ex) { }

        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {
            if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                newID = -1;
                Session.Remove("UserGroupID");
                GridEditableItem item = e.Item as GridEditableItem;
                RadDropDownList list = item.FindControl("ddlUserGroups") as RadDropDownList;
                try {
                    UserB currUser = e.Item.DataItem as UserB;
                    UserGroupsController cont = new UserGroupsController();
                    list.DataSource = cont.GetUserGroups();
                    list.DataTextField = "Name";
                    list.DataValueField = "ID";                    
                    list.DataBind();
                    if (currUser != null) {
                        list.SelectedIndex = list.FindItemByValue(currUser.UserGroupID.ToString()).Index;
                        Session["UserGroupID"] = currUser.UserGroupID;
                    } else {
                        list.SelectedIndex = 0;
                        Session["UserGroupID"] = list.SelectedItem.Value;
                    }
                }
                catch (Exception ex) { }
            }
        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) {

        }

        private void ShowErrorMessage(int errCode) {
            if (errCode == 1) {
                RadWindowManager1.RadAlert("Ο συγκεκριμένος Χρήστης σχετίζεται με κάποιον Πελάτη και δεν μπορεί να διαγραφεί!", 400, 200, "Σφάλμα", "");
            } else {
                RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
            }
        }

        protected void gridMain_UpdateCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            var ID = (int)editableItem.GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var user = dbContext.Users.Where(n => n.ID == ID).FirstOrDefault();
                if (user != null) {
                    editableItem.UpdateValues(user);
                    if (Session["UserGroupID"] != null) { newID = int.Parse(Session["UserGroupID"].ToString()); }
                    if (newID > 0) {
                        user.UserGroupID = newID;
                        newID = -1;
                        Session.Remove("UserGroupID");
                    }
                    try { dbContext.SaveChanges(); }
                    catch (Exception ex) { ShowErrorMessage(-1); }
                }
            }
        }

        protected void gridMain_InsertCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            using (var dbContext = new OTERTConnStr()) {
                var user = new Users();
                Hashtable values = new Hashtable();
                editableItem.ExtractValues(values);
                if (Session["UserGroupID"] != null) { newID = int.Parse(Session["UserGroupID"].ToString()); }
                if (newID > 0) {
                    try {
                        user.UserGroupID = newID;
                        user.NameGR = (string)values["NameGR"];
                        user.NameEN = (string)values["NameEN"];
                        user.Telephone = (string)values["Telephone"];
                        user.FAX = (string)values["FAX"];
                        user.Email = (string)values["Email"];
                        user.UserName = (string)values["UserName"];
                        user.Password = (string)values["Password"];
                        dbContext.Users.Add(user);
                        dbContext.SaveChanges();
                    }
                    catch (Exception ex) { ShowErrorMessage(-1); }
                    finally {
                        newID = -1;
                        Session.Remove("UserGroupID");
                    }
                } else { ShowErrorMessage(-1); }
            }
        }

        protected void gridMain_DeleteCommand(object source, GridCommandEventArgs e) {
            var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var user = dbContext.Users.Where(n => n.ID == ID).FirstOrDefault();
                if (user != null) {
                    dbContext.Users.Remove(user);
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

        protected void ddlUserGroups_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                newID = int.Parse(e.Value);
                Session["UserGroupID"] = newID;
            }
            catch (Exception ex) { }
        }

    }

}