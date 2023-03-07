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
using Telerik.Web.Data.Extensions;

namespace OTERT.Pages.Administrator {

    public partial class UsersList : System.Web.UI.Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;
        protected int newID;
        protected UserB loggedUser;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Χρηστών";
                gridMain.MasterTableView.Caption = "Χρήστες Εφαρμογής";
                newID = -1;
                Session.Remove("UserGroupID");
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            string recFilter = gridMain.MasterTableView.FilterExpression;
            GridSortExpressionCollection gridSortExxpressions = gridMain.MasterTableView.SortExpressions;
            try {
                UsersController cont = new UsersController();
                gridMain.VirtualItemCount = cont.CountUsers(recFilter);
                gridMain.DataSource = cont.GetUsers(recSkip, recTake, recFilter, gridSortExxpressions);
            }
            catch (Exception) { }

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
                catch (Exception) { }
            }
            if (e.Item is GridFilteringItem) {
                GridFilteringItem filterItem = (GridFilteringItem)e.Item;
                RadDropDownList flist = (RadDropDownList)filterItem.FindControl("ddlUserGroupsFilter");
                try {
                    UserGroupsController cont = new UserGroupsController();
                    flist.DataSource = cont.GetUserGroups();
                    flist.DataTextField = "Name";
                    flist.DataValueField = "ID";
                    flist.DataBind();
                    flist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                }
                catch (Exception) { }                                                                        //combo.Items.Add(new RadComboBoxItem("New"));
            }
        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) { }

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
                    try { 
                        dbContext.SaveChanges();
                        string message = "Modified User: " + user.UserName + " (" + user.ID.ToString() + ")";
                        Utilities.logSomething(loggedUser.UserName, Utilities.GetIPAddress(), Utilities.LogEventTypes.UserModified, message);
                    }
                    catch (Exception) { ShowErrorMessage(-1); }
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
                        string plainPassword = "EnterOTE-RT123!";
                        string salt = Utilities.GetRandomSalt(10);
                        //string hashedSalt = Utilities.ComputeHash(salt);
                        string hashedPassword = Utilities.ComputeHash(plainPassword, salt);
                        user.UserGroupID = newID;
                        user.NameGR = (string)values["NameGR"];
                        user.NameEN = (string)values["NameEN"];
                        user.Telephone = (string)values["Telephone"];
                        user.FAX = (string)values["FAX"];
                        user.Email = (string)values["Email"];
                        user.UserName = (string)values["UserName"];
                        user.Password = hashedPassword;
                        user.PasswordSalt = salt;
                        user.PasswordReset = true;
                        user.PasswordWrongTimes = 0;
                        user.PasswordLockedDatetime = new DateTime(1900, 1, 1);
                        user.PasswordIsHashed = true;
                        dbContext.Users.Add(user);
                        dbContext.SaveChanges();
                        string message = "New User: " + user.UserName ;
                        Utilities.logSomething(loggedUser.UserName, Utilities.GetIPAddress(), Utilities.LogEventTypes.UserCreated, message);
                    }
                    catch (Exception) { ShowErrorMessage(-1); }
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
                    List<UserPasswords> upl = dbContext.UserPasswords.Where(k => k.UserID == ID).ToList();
                    if (upl.Count > 0) {
                        foreach (UserPasswords up in upl) { dbContext.UserPasswords.Remove(up); }
                    }
                    List<UserPasswordReset> uprl = dbContext.UserPasswordReset.Where(k => k.UserID == ID).ToList();
                    if (uprl.Count > 0) {
                        foreach (UserPasswordReset upr in uprl) { dbContext.UserPasswordReset.Remove(upr); }
                    }
                    dbContext.Users.Remove(user);
                    try { 
                        dbContext.SaveChanges();
                        string message = "Deleted User: " + user.UserName + " (" + user.ID.ToString() + ")";
                        Utilities.logSomething(loggedUser.UserName, Utilities.GetIPAddress(), Utilities.LogEventTypes.UserDeleted, message);
                    }
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
            catch (Exception) { }
        }

        protected void ddlUserGroupsFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("UserGroupID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(UserGroupID = " + e.Value + ")";
                gridMain.MasterTableView.GetColumn("UserGroupID").CurrentFilterFunction = GridKnownFunction.EqualTo;
                gridMain.MasterTableView.GetColumn("UserGroupID").CurrentFilterValue = e.Value;
            } else {
                gridMain.MasterTableView.GetColumn("UserGroupID").CurrentFilterFunction = GridKnownFunction.NoFilter;
                gridMain.MasterTableView.GetColumn("UserGroupID").CurrentFilterValue = null;
            }
            gridMain.MasterTableView.FilterExpression = finalExpression;
            ViewState[list.ClientID] = e.Value;
            gridMain.MasterTableView.Rebind();
        }

        protected void ddlUserGroupsFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }


    }

}