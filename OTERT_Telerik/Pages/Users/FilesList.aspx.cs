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

namespace OTERT.Pages.UserPages {

    public partial class FilesList : System.Web.UI.Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;
        protected UserB loggedUser;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Αρχεία";
                gridMain.MasterTableView.Caption = "Αρχεία";
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            string recFilter = gridMain.MasterTableView.FilterExpression;
            GridSortExpressionCollection gridSortExxpressions = gridMain.MasterTableView.SortExpressions;
            try {
                FilesController cont = new FilesController();
                gridMain.VirtualItemCount = cont.CountFiles(recFilter);
                gridMain.DataSource = cont.GetFilesForList(recSkip, recTake, recFilter, gridSortExxpressions);
            }
            catch (Exception) { }
        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {   
            if (e.Item is GridFilteringItem) {
                GridFilteringItem filterItem = (GridFilteringItem)e.Item;
                RadDropDownList ctflist = (RadDropDownList)filterItem.FindControl("ddlCustomerFilter");
                try {
                    CustomersController ctcont = new CustomersController();
                    ctflist.DataSource = ctcont.GetAllCustomers();
                    ctflist.DataTextField = "NameGR";
                    ctflist.DataValueField = "ID";
                    ctflist.DataBind();
                    ctflist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                }
                catch (Exception) { }
            }
        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) {
            if (e.Item is GridFilteringItem) {
                GridFilteringItem filterItem = (GridFilteringItem)e.Item;
                (filterItem["DateStamp"].Controls[0] as LiteralControl).Text = "Από: ";
                (filterItem["DateStamp"].Controls[3] as LiteralControl).Text = "<br />Έως: ";
                RadDateTimePicker OrderDateFrom = filterItem["DateStamp"].Controls[1] as RadDateTimePicker;
                OrderDateFrom.TimePopupButton.Visible = false;
                RadDateTimePicker OrderDateTo = filterItem["DateStamp"].Controls[4] as RadDateTimePicker;
                OrderDateTo.TimePopupButton.Visible = false;
            }
        }

        protected void ddlCustomerFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
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

        protected void ddlCustomerFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

        protected void gridMain_SortCommand(object source, GridSortCommandEventArgs e) {
            /*
            if ("CustomerID".Equals(e.CommandArgument)) {
                switch (e.OldSortOrder) {
                    case GridSortOrder.None:
                        //e.Item.OwnerTableView.DataSource = GetDataTable("SELECT FirstName, LastName FROM Employees ORDER BY LEN(FirstName) ASC");
                        e.Item.OwnerTableView.Rebind();
                        break;
                    case GridSortOrder.Ascending:
                        //e.Item.OwnerTableView.DataSource = GetDataTable("SELECT FirstName, LastName FROM Employees ORDER BY LEN(FirstName) DESC");
                        e.Item.OwnerTableView.Rebind();
                        break;
                    case GridSortOrder.Descending:
                        //e.Item.OwnerTableView.DataSource = GetDataTable("SELECT FirstName, LastName FROM Employees");
                        e.Item.OwnerTableView.Rebind();
                        break;
                }
            } else if ("LastName".Equals(e.CommandArgument)) {
                switch (e.OldSortOrder) {
                    case GridSortOrder.None:
                        //e.Item.OwnerTableView.DataSource = GetDataTable("SELECT FirstName, LastName FROM Employees ORDER BY LastName DESC");
                        e.Item.OwnerTableView.Rebind();
                        break;
                    case GridSortOrder.Ascending:
                        //e.Item.OwnerTableView.DataSource = GetDataTable("SELECT FirstName, LastName FROM Employees ORDER BY LastName ASC");
                        e.Item.OwnerTableView.Rebind();
                        break;
                    case GridSortOrder.Descending:
                        //e.Item.OwnerTableView.DataSource = GetDataTable("SELECT FirstName, LastName FROM Employees");
                        e.Item.OwnerTableView.Rebind();
                        break;
                }
            }
            */
        }

    }

}