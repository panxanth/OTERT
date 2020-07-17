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

    public partial class TasksList : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;
        protected UserB loggedUser;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Λίστες > Έργα";
                gridMain.MasterTableView.Caption = "Λίστες > Έργα";
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            string recFilter = gridMain.MasterTableView.FilterExpression;
            GridSortExpressionCollection gridSortExxpressions = gridMain.MasterTableView.SortExpressions;
            try {
                TasksController cont = new TasksController();
                gridMain.VirtualItemCount = cont.CountAllTasks(recFilter);
                gridMain.DataSource = cont.GetAllTasks(recSkip, recTake, recFilter, gridSortExxpressions);
            }
            catch (Exception) { }
        }

        protected void gridMain_PreRender(object sender, EventArgs e) {
            gridMain.MasterTableView.GetColumn("ExpandColumn").Display = false;
        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                if (e.Item is GridFilteringItem) {
                    GridFilteringItem filterItem = (GridFilteringItem)e.Item;
                    (filterItem["OrderDate"].Controls[0] as LiteralControl).Text = "Από: ";
                    (filterItem["OrderDate"].Controls[3] as LiteralControl).Text = "<br />Έως: ";
                    (filterItem["DateTimeStartActual"].Controls[0] as LiteralControl).Text = "Από: ";
                    (filterItem["DateTimeStartActual"].Controls[3] as LiteralControl).Text = "<br />Έως: ";
                }
            }
        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {
            /*
            if (e.Item.OwnerTableView.Name == "Master") {
                if (e.Item is GridDataItem) {
                    GridDataItem item = (GridDataItem)e.Item;
                    if (item.OwnerTableView.DataSource != null) {
                        TaskB curTask = (item.OwnerTableView.DataSource as List<TaskB>)[item.DataSetIndex];
                        TableCell curCell = item["RegNo"];
                        string curComments = curTask.Comments;
                        string curTooltip = "<span><span class=\"tooltip tooltip-effect-4\"><span class=\"tooltip-item\">";
                        curTooltip += curCell.Text;
                        curTooltip += "</span><span class=\"tooltip-content clearfix\"><span class=\"tooltip-text\"><strong>Παρατηρήσεις:</strong><br/>";
                        curTooltip += curComments;
                        curTooltip += "</span></span></span></span>";
                        if (!string.IsNullOrWhiteSpace(curComments)) { curCell.Text = curTooltip; }
                        System.Drawing.Color hColor = System.Drawing.Color.FromArgb(0, 0, 0);
                        if (curTask.IsLocked == true) { hColor = System.Drawing.Color.FromArgb(200, 0, 0); }
                        item["ID"].ForeColor = hColor;
                        item["RegNo"].ForeColor = hColor;
                        item["OrderDate"].ForeColor = hColor;
                        item["CustomerID"].ForeColor = hColor;
                        item["JobsID"].ForeColor = hColor;
                        item["DateTimeStartActual"].ForeColor = hColor;
                        if (curTask.IsLocked == true && loggedUser.UserGroupID != 1) {
                            item["EditCommandColumn"].Controls[0].Visible = false;
                            item["btnDelete"].Controls[0].Visible = false;
                            item["ExapandColumn"].Controls[0].Visible = false;
                        }
                    }
                }
            }
            */
            if (e.Item.OwnerTableView.Name == "Master") {
                if (e.Item is GridFilteringItem) {
                    GridFilteringItem filterItem = (GridFilteringItem)e.Item;
                    RadDropDownList clist = (RadDropDownList)filterItem.FindControl("ddlCustomersFilter");
                    RadDropDownList jlist = (RadDropDownList)filterItem.FindControl("ddlJobsFilter");
                    try {
                        CustomersController ccont = new CustomersController();
                        clist.DataSource = ccont.GetCustomers();
                        clist.DataTextField = "NameGR";
                        clist.DataValueField = "ID";
                        clist.DataBind();
                        clist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                        JobsController jcont = new JobsController();
                        jlist.DataSource = jcont.GetJobs();
                        jlist.DataTextField = "Name";
                        jlist.DataValueField = "ID";
                        jlist.DataBind();
                        jlist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                    }
                    catch (Exception) { }
                }
                if (e.Item is GridDataItem) {
                    GridDataItem item = e.Item as GridDataItem;
                    ImageButton editButton = item["EditCommandColumn"].Controls[0] as ImageButton;
                    editButton.ImageUrl = "~/Images/mag.png";
                }
            }
        }

        protected void gridMain_EditCommand(object source, GridCommandEventArgs e) {
            GridEditableItem editableItem = ((GridEditableItem)e.Item);
            int ID = (int)editableItem.GetDataKeyValue("ID");
            try {
                TasksController cont = new TasksController();
                TaskB curTask = cont.GetTask(ID);
                if (curTask != null) {
                    switch (curTask.Job.JobsMain.PageID) {
                        case 1:
                            Response.Redirect("UrbanOnePoint.aspx?id=" + ID.ToString(), false);
                            break;
                        case 2:
                            Response.Redirect("UrbanTwoPoints.aspx?id=" + ID.ToString(), false);
                            break;
                        case 3:
                            Response.Redirect("LongDistanceOnePoint.aspx?id=" + ID.ToString(), false);
                            break;
                        case 4:
                            Response.Redirect("LongDistanceTwoPoints.aspx?id=" + ID.ToString(), false);
                            break;
                        case 5:
                            Response.Redirect("UrbanCalls.aspx?id=" + ID.ToString(), false);
                            break;
                        case 6:
                            Response.Redirect("LongDistanceCalls.aspx?id=" + ID.ToString(), false);
                            break;
                        case 7:
                            Response.Redirect("SateliteHS.aspx?id=" + ID.ToString(), false);
                            break;
                        case 8:
                            Response.Redirect("SateliteEU.aspx?id=" + ID.ToString(), false);
                            break;
                        case 9:
                            Response.Redirect("Uplink.aspx?id=" + ID.ToString(), false);
                            break;
                        case 10:
                            Response.Redirect("UplinkSNG.aspx?id=" + ID.ToString(), false);
                            break;
                        case 11:
                            Response.Redirect("Downlink.aspx?id=" + ID.ToString(), false);
                            break;
                        case 12:
                            Response.Redirect("DownlinkSNG.aspx?id=" + ID.ToString(), false);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception) { }
        }

        protected void ddlCustomersFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
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

        protected void ddlCustomersFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

        protected void ddlJobsFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("JobID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(JobID = " + e.Value + ")";
                gridMain.MasterTableView.GetColumn("JobID").CurrentFilterFunction = GridKnownFunction.EqualTo;
                gridMain.MasterTableView.GetColumn("JobID").CurrentFilterValue = e.Value;
            } else {
                gridMain.MasterTableView.GetColumn("JobID").CurrentFilterFunction = GridKnownFunction.NoFilter;
                gridMain.MasterTableView.GetColumn("JobID").CurrentFilterValue = null;
            }
            gridMain.MasterTableView.FilterExpression = finalExpression;
            ViewState[list.ClientID] = e.Value;
            gridMain.MasterTableView.Rebind();
        }

        protected void ddlJobsFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

        protected void gridMain_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e) {
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

}