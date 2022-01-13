using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using xi = Telerik.Web.UI.ExportInfrastructure;
using Telerik.Web.UI.Calendar;
using ExpressionParser;
using OTERT.Model;
using OTERT.Controller;
using OTERT_Entity;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.TextBased.Csv;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.TextBased.Txt;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Documents.Spreadsheet.Utilities;
using System.IO;

namespace OTERT.Pages.UserPages {

    public partial class TasksList : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;
        protected UserB loggedUser;

        protected readonly ThemableColor cellBackground = ThemableColor.FromArgb(0, 255, 255, 255);
        protected readonly ThemableColor tcBlack = ThemableColor.FromArgb(255, 0, 0, 0);
        protected readonly ThemableColor tcWhite = ThemableColor.FromArgb(255, 255, 255, 255);
        protected readonly string dateFormat = "dd/MM/yyyy HH:mm";
        protected readonly string currencyFormat = "#.##0,00 €";

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Λίστες > Έργα";
                gridMain.MasterTableView.Caption = "Λίστες > Έργα";
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.MasterTableView.CurrentPageIndex * gridMain.MasterTableView.PageSize;
            int recTake = gridMain.MasterTableView.PageSize;
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
                    (filterItem["DateTimeStartOrder"].Controls[0] as LiteralControl).Text = "Από: ";
                    (filterItem["DateTimeStartOrder"].Controls[3] as LiteralControl).Text = "<br />Έως: ";
                    (filterItem["DateTimeStartActual"].Controls[0] as LiteralControl).Text = "Από: ";
                    (filterItem["DateTimeStartActual"].Controls[3] as LiteralControl).Text = "<br />Έως: ";
                    RadDateTimePicker OrderDateFrom = filterItem["OrderDate"].Controls[1] as RadDateTimePicker;
                    OrderDateFrom.TimePopupButton.Visible = false;
                    OrderDateFrom.DateInput.DisplayDateFormat = "d/M/yyyy";
                    OrderDateFrom.DateInput.DateFormat = "d/M/yyyy";
                    RadDateTimePicker OrderDateTo = filterItem["OrderDate"].Controls[4] as RadDateTimePicker;
                    OrderDateTo.TimePopupButton.Visible = false;
                    OrderDateTo.DateInput.DisplayDateFormat = "d/M/yyyy";
                    OrderDateTo.DateInput.DateFormat = "d/M/yyyy";
                    OrderDateFrom.DateInput.Attributes.Add("onchange", "javascript:UpdateTo('" + OrderDateFrom.ClientID + "', '" + OrderDateTo.ClientID + "');");
                    RadDateTimePicker startOrderFrom = filterItem["DateTimeStartOrder"].Controls[1] as RadDateTimePicker;
                    startOrderFrom.TimePopupButton.Visible = false;
                    startOrderFrom.DateInput.DisplayDateFormat = "d/M/yyyy";
                    startOrderFrom.DateInput.DateFormat = "d/M/yyyy";
                    RadDateTimePicker startOrderTo = filterItem["DateTimeStartOrder"].Controls[4] as RadDateTimePicker;
                    startOrderTo.TimePopupButton.Visible = false;
                    startOrderTo.DateInput.DisplayDateFormat = "d/M/yyyy";
                    startOrderTo.DateInput.DateFormat = "d/M/yyyy";
                    startOrderFrom.DateInput.Attributes.Add("onchange", "javascript:UpdateTo('" + startOrderFrom.ClientID + "', '" + startOrderTo.ClientID + "');");
                    RadDateTimePicker startActualFrom = filterItem["DateTimeStartActual"].Controls[1] as RadDateTimePicker;
                    startActualFrom.TimePopupButton.Visible = false;
                    startActualFrom.DateInput.DisplayDateFormat = "d/M/yyyy";
                    startActualFrom.DateInput.DateFormat = "d/M/yyyy";
                    RadDateTimePicker startActualTo = filterItem["DateTimeStartActual"].Controls[4] as RadDateTimePicker;
                    startActualTo.TimePopupButton.Visible = false;
                    startActualTo.DateInput.DisplayDateFormat = "d/M/yyyy";
                    startActualTo.DateInput.DateFormat = "d/M/yyyy";
                    startActualFrom.DateInput.Attributes.Add("onchange", "javascript:UpdateTo('" + startActualFrom.ClientID + "', '" + startActualTo.ClientID + "');");
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
                    RadDropDownList plist = (RadDropDownList)filterItem.FindControl("ddlPositionsFilter");
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
                        DistancesController dcont = new DistancesController();
                        plist.DataSource = dcont.GetDistances();
                        plist.DataTextField = "Description";
                        plist.DataValueField = "ID";
                        plist.DataBind();
                        plist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
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

        protected void ddlPositionsFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("DistanceID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(DistanceID = " + e.Value + ")";
                gridMain.MasterTableView.GetColumn("DistanceID").CurrentFilterFunction = GridKnownFunction.EqualTo;
                gridMain.MasterTableView.GetColumn("DistanceID").CurrentFilterValue = e.Value;
            } else {
                gridMain.MasterTableView.GetColumn("DistanceID").CurrentFilterFunction = GridKnownFunction.NoFilter;
                gridMain.MasterTableView.GetColumn("DistanceID").CurrentFilterValue = null;
            }
            gridMain.MasterTableView.FilterExpression = finalExpression;
            ViewState[list.ClientID] = e.Value;
            gridMain.MasterTableView.Rebind();
        }

        protected void ddlPositionsFilter_PreRender(object sender, EventArgs e) {
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

        protected void btnExportXLSX_Click(object sender, EventArgs e) {
            IWorkbookFormatProvider formatProvider = new XlsxFormatProvider();
            Workbook workbook = createWorkbook();
            byte[] renderedBytes = null;
            using (MemoryStream ms = new MemoryStream()) {
                formatProvider.Export(workbook, ms);
                renderedBytes = ms.ToArray();
            }
            Response.ClearHeaders();
            Response.ClearContent();
            Response.AppendHeader("content-disposition", "attachment; filename=DYEP-RT-Report-" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(renderedBytes);
            Response.End();
        }

        protected Workbook createWorkbook() {
            Workbook workbook = new Workbook();
            workbook.Sheets.Add(SheetType.Worksheet);
            Worksheet worksheet = workbook.ActiveWorksheet;
            worksheet.Name = "OTE RT";
            List<TaskB> tasks = new List<TaskB>();
            try {
                string recFilter = gridMain.MasterTableView.FilterExpression;
                GridSortExpressionCollection gridSortExxpressions = gridMain.MasterTableView.SortExpressions;
                TasksController cont = new TasksController();
                int tasksCount = cont.CountAllTasks(recFilter);
                tasks = cont.GetAllTasks(0, tasksCount, recFilter, gridSortExxpressions);
            }
            catch (Exception) { }
            prepareDocument(worksheet);
            int currentRow = 1;
            CellBorder border = new CellBorder(CellBorderStyle.Thin, tcBlack);
            CellBorders borders = new CellBorders(border, border, border, border, null, null, null, null);
            double fontSize = 12;
            foreach (TaskB curTask in tasks) {
                worksheet.Cells[currentRow,0].SetValue(curTask.ID.ToString());
                worksheet.Cells[currentRow,0].SetFontSize(fontSize);
                worksheet.Cells[currentRow,0].SetBorders(borders);
                worksheet.Cells[currentRow,1].SetValue(curTask.RegNo);
                worksheet.Cells[currentRow,1].SetFormat(new CellValueFormat("@"));
                worksheet.Cells[currentRow,1].SetFontSize(fontSize);
                worksheet.Cells[currentRow,1].SetBorders(borders);
                worksheet.Cells[currentRow,2].SetValue(curTask.OrderDate);
                worksheet.Cells[currentRow,2].SetFormat(new CellValueFormat(dateFormat));
                worksheet.Cells[currentRow,2].SetFontSize(fontSize);
                worksheet.Cells[currentRow,2].SetBorders(borders);
                worksheet.Cells[currentRow,3].SetValue(curTask.Customer.NameGR);
                worksheet.Cells[currentRow,3].SetFontSize(fontSize);
                worksheet.Cells[currentRow,3].SetBorders(borders);
                worksheet.Cells[currentRow,4].SetValue(curTask.Job.Name);
                worksheet.Cells[currentRow,4].SetFontSize(fontSize);
                worksheet.Cells[currentRow,4].SetBorders(borders);
                string distance = curTask.Distance.Position1 + " - " + curTask.Distance.Position2 + " (" + curTask.Distance.KM.ToString() + ")";
                worksheet.Cells[currentRow,5].SetValue(distance);
                worksheet.Cells[currentRow,5].SetFontSize(fontSize);
                worksheet.Cells[currentRow,5].SetBorders(borders);
                worksheet.Cells[currentRow,6].SetValue(curTask.DateTimeStartOrder.GetValueOrDefault());
                worksheet.Cells[currentRow,6].SetFormat(new CellValueFormat(dateFormat));
                worksheet.Cells[currentRow,6].SetFontSize(fontSize);
                worksheet.Cells[currentRow,6].SetBorders(borders);
                worksheet.Cells[currentRow,7].SetValue(curTask.DateTimeEndOrder.GetValueOrDefault());
                worksheet.Cells[currentRow,7].SetFormat(new CellValueFormat(dateFormat));
                worksheet.Cells[currentRow,7].SetFontSize(fontSize);
                worksheet.Cells[currentRow,7].SetBorders(borders);
                worksheet.Cells[currentRow,8].SetValue(curTask.DateTimeDurationOrder);
                worksheet.Cells[currentRow,8].SetFontSize(fontSize);
                worksheet.Cells[currentRow,8].SetBorders(borders);
                worksheet.Cells[currentRow,9].SetValue(double.Parse(curTask.AddedCharges.GetValueOrDefault().ToString()));
                worksheet.Cells[currentRow,9].SetFormat(new CellValueFormat(currencyFormat));
                worksheet.Cells[currentRow,9].SetFontSize(fontSize);
                worksheet.Cells[currentRow,9].SetBorders(borders);
                worksheet.Cells[currentRow,10].SetValue(double.Parse(curTask.CostCalculated.GetValueOrDefault().ToString()));
                worksheet.Cells[currentRow,10].SetFormat(new CellValueFormat(currencyFormat));
                worksheet.Cells[currentRow,10].SetFontSize(fontSize);
                worksheet.Cells[currentRow,10].SetBorders(borders);
                if (curTask.DateTimeStartActual.GetValueOrDefault().Year > 2000) {
                    worksheet.Cells[currentRow, 11].SetValue(curTask.DateTimeStartActual.GetValueOrDefault());
                } else {
                    worksheet.Cells[currentRow, 11].SetValue("");
                }
                worksheet.Cells[currentRow,11].SetFormat(new CellValueFormat(dateFormat));
                worksheet.Cells[currentRow,11].SetFontSize(fontSize);
                worksheet.Cells[currentRow,11].SetBorders(borders);
                if (curTask.DateTimeEndActual.GetValueOrDefault().Year > 2000) {
                    worksheet.Cells[currentRow, 12].SetValue(curTask.DateTimeEndActual.GetValueOrDefault());
                } else {
                    worksheet.Cells[currentRow, 12].SetValue("");
                }
                worksheet.Cells[currentRow,12].SetFormat(new CellValueFormat(dateFormat));
                worksheet.Cells[currentRow,12].SetFontSize(fontSize);
                worksheet.Cells[currentRow,12].SetBorders(borders);
                if (curTask.IsCanceled == true) {
                    worksheet.Cells[currentRow, 13].SetValue(0);
                } else {
                    worksheet.Cells[currentRow, 13].SetValue(curTask.DateTimeDurationActual.GetValueOrDefault());
                }
                worksheet.Cells[currentRow,13].SetFontSize(fontSize);
                worksheet.Cells[currentRow,13].SetBorders(borders);
                if (curTask.PaymentDateOrder.GetValueOrDefault().Year > 2000) {
                    worksheet.Cells[currentRow, 14].SetValue(curTask.PaymentDateOrder.GetValueOrDefault());
                } else {
                    worksheet.Cells[currentRow, 14].SetValue("");
                }
                worksheet.Cells[currentRow,14].SetFormat(new CellValueFormat(dateFormat));
                worksheet.Cells[currentRow,14].SetFontSize(fontSize);
                worksheet.Cells[currentRow,14].SetBorders(borders);
                worksheet.Cells[currentRow,15].SetValue(double.Parse(curTask.CostActual.GetValueOrDefault().ToString()));
                worksheet.Cells[currentRow,15].SetFormat(new CellValueFormat(currencyFormat));
                worksheet.Cells[currentRow,15].SetFontSize(fontSize);
                worksheet.Cells[currentRow,15].SetBorders(borders);
                if (curTask.PaymentDateCalculated.GetValueOrDefault().Year > 2000) {
                    worksheet.Cells[currentRow, 16].SetValue(curTask.PaymentDateCalculated.GetValueOrDefault());
                } else {
                    worksheet.Cells[currentRow, 16].SetValue("");
                }
                worksheet.Cells[currentRow,16].SetFormat(new CellValueFormat(dateFormat));
                worksheet.Cells[currentRow,16].SetFontSize(fontSize);
                worksheet.Cells[currentRow,16].SetBorders(borders);
                //if (curTask.PaymentDateActual.GetValueOrDefault().Year > 2000) {
                //    worksheet.Cells[currentRow, 17].SetValue(curTask.PaymentDateActual.GetValueOrDefault());
                //} else {
                //    worksheet.Cells[currentRow, 17].SetValue("");
                //}
                //worksheet.Cells[currentRow,17].SetFormat(new CellValueFormat(dateFormat));
                //worksheet.Cells[currentRow,17].SetFontSize(fontSize);
                //worksheet.Cells[currentRow,17].SetBorders(borders);
                string isCanc = "ΟΧΙ";
                if (curTask.IsCanceled == true) { isCanc = "ΝΑΙ"; }
                worksheet.Cells[currentRow,17].SetValue(isCanc);
                worksheet.Cells[currentRow,17].SetFontSize(fontSize);
                worksheet.Cells[currentRow,17].SetBorders(borders);
                worksheet.Cells[currentRow,18].SetValue(curTask.Comments);
                worksheet.Cells[currentRow,18].SetIsWrapped(true);
                worksheet.Cells[currentRow,18].SetFontSize(fontSize);
                worksheet.Cells[currentRow,18].SetBorders(borders);
                worksheet.Cells[currentRow,19].SetValue(curTask.InvoceComments);
                worksheet.Cells[currentRow,19].SetIsWrapped(true);
                worksheet.Cells[currentRow,19].SetFontSize(fontSize);
                worksheet.Cells[currentRow,19].SetBorders(borders);
                worksheet.Cells[currentRow, 20].SetValue(curTask.EnteredByUser);
                worksheet.Cells[currentRow, 20].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 20].SetBorders(borders);
                worksheet.Cells[currentRow, 21].SetValue(curTask.DateStamp);
                worksheet.Cells[currentRow, 21].SetFormat(new CellValueFormat(dateFormat));
                worksheet.Cells[currentRow, 21].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 21].SetBorders(borders);
                currentRow++;
            }
            for (int i = 0; i < worksheet.Columns.Count; i++) { worksheet.Columns[i].AutoFitWidth(); }
            for (int i = 0; i < worksheet.Columns.Count; i++) {
                if (i==18 || i==19) { worksheet.Columns[i].SetWidth(new ColumnWidth(300, true)); }
                ColumnSelection columnSelection = worksheet.Columns[i];
                ColumnWidth columnWidth = columnSelection.GetWidth().Value;
                double curColWidth = columnWidth.Value + 10;
                if (curColWidth > 2000) { curColWidth = 2000; }
                columnSelection.SetWidth(new ColumnWidth(curColWidth, columnWidth.IsCustom));
            }
            ColumnSelection columnSelection4 = worksheet.Columns[4];
            ColumnWidth columnWidth4 = columnSelection4.GetWidth().Value;
            double curColWidth4 = columnWidth4.Value + 10;
            if (curColWidth4 > 2000) { curColWidth4 = 2000; }
            columnSelection4.SetWidth(new ColumnWidth(curColWidth4, columnWidth4.IsCustom));
            return workbook;
        }

        protected void prepareDocument(Worksheet worksheet) {
            PatternFill pfGreen = new PatternFill(PatternType.Solid, System.Windows.Media.Color.FromArgb(255, 153, 204, 0), System.Windows.Media.Colors.Transparent);
            PatternFill pfOrange = new PatternFill(PatternType.Solid, System.Windows.Media.Color.FromArgb(255, 255, 204, 0), System.Windows.Media.Colors.Transparent);
            PatternFill pfRed = new PatternFill(PatternType.Solid, System.Windows.Media.Color.FromArgb(255, 255, 0, 0), System.Windows.Media.Colors.Transparent);
            PatternFill pfBlue = new PatternFill(PatternType.Solid, System.Windows.Media.Color.FromArgb(255, 0, 0, 255), System.Windows.Media.Colors.Transparent);
            CellBorder border = new CellBorder(CellBorderStyle.Thin, tcBlack);
            CellBorders borders = new CellBorders(border, border, border, border, null, null, null, null);
            double fontSize = 12;
            worksheet.Cells[0, 0].SetValue("A/A");
            worksheet.Cells[0, 0].SetFontSize(fontSize);
            worksheet.Cells[0, 0].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 0].SetIsBold(true);
            worksheet.Cells[0, 0].SetFill(pfGreen);
            worksheet.Cells[0, 0].SetBorders(borders);
            worksheet.Cells[0, 1].SetValue("Αριθμός Πρωτοκόλλου");
            worksheet.Cells[0, 1].SetFontSize(fontSize);
            worksheet.Cells[0, 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 1].SetIsBold(true);
            //worksheet.Cells[0, 1].SetIsWrapped(true);
            worksheet.Cells[0, 1].SetFill(pfGreen);
            worksheet.Cells[0, 1].SetBorders(borders);
            worksheet.Cells[0, 2].SetValue("Ημερομηνία Παραγγελίας");
            worksheet.Cells[0, 2].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 2].SetFontSize(fontSize);
            worksheet.Cells[0, 2].SetIsBold(true);
            //worksheet.Cells[0, 2].SetIsWrapped(true);
            worksheet.Cells[0, 2].SetFill(pfOrange);
            worksheet.Cells[0, 2].SetBorders(borders);
            worksheet.Cells[0, 3].SetValue("Πελάτης");
            worksheet.Cells[0, 3].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 3].SetFontSize(fontSize);
            worksheet.Cells[0, 3].SetIsBold(true);
            worksheet.Cells[0, 3].SetFill(pfGreen);
            worksheet.Cells[0, 3].SetBorders(borders);
            worksheet.Cells[0, 4].SetValue("Κατηγορία Έργου");
            worksheet.Cells[0, 4].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 4].SetFontSize(fontSize);
            worksheet.Cells[0, 4].SetIsBold(true);
            worksheet.Cells[0, 4].SetFill(pfGreen);
            worksheet.Cells[0, 4].SetBorders(borders);
            worksheet.Cells[0, 5].SetValue("Διαδρομή (km)");
            worksheet.Cells[0, 5].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 5].SetFontSize(fontSize);
            worksheet.Cells[0, 5].SetIsBold(true);
            worksheet.Cells[0, 5].SetFill(pfGreen);
            worksheet.Cells[0, 5].SetBorders(borders);
            worksheet.Cells[0, 6].SetValue("Προγραμματισμένη Ημερομηνία Έναρξης");
            worksheet.Cells[0, 6].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 6].SetFontSize(fontSize);
            worksheet.Cells[0, 6].SetIsBold(true);
            //worksheet.Cells[0, 6].SetIsWrapped(true);
            worksheet.Cells[0, 6].SetFill(pfOrange);
            worksheet.Cells[0, 6].SetBorders(borders);
            worksheet.Cells[0, 7].SetValue("Προγραμματισμένη Ημερομηνία Λήξης");
            worksheet.Cells[0, 7].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 7].SetFontSize(fontSize);
            worksheet.Cells[0, 7].SetIsBold(true);
            //worksheet.Cells[0, 7].SetIsWrapped(true);
            worksheet.Cells[0, 7].SetFill(pfOrange);
            worksheet.Cells[0, 7].SetBorders(borders);
            worksheet.Cells[0, 8].SetValue("Προγραμματισμένη Διάρκεια");
            worksheet.Cells[0, 8].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 8].SetFontSize(fontSize);
            worksheet.Cells[0, 8].SetIsBold(true);
            //worksheet.Cells[0, 8].SetIsWrapped(true);
            worksheet.Cells[0, 8].SetFill(pfGreen);
            worksheet.Cells[0, 8].SetBorders(borders);
            worksheet.Cells[0, 9].SetValue("Πρόσθετα Τέλη");
            worksheet.Cells[0, 9].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 9].SetFontSize(fontSize);
            worksheet.Cells[0, 9].SetIsBold(true);
            //worksheet.Cells[0, 9].SetIsWrapped(true);
            worksheet.Cells[0, 9].SetForeColor(tcWhite);
            worksheet.Cells[0, 9].SetFill(pfRed);
            worksheet.Cells[0, 9].SetBorders(borders);
            worksheet.Cells[0, 10].SetValue("Προϋπολογιζόμενο Κόστος");
            worksheet.Cells[0, 10].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 10].SetFontSize(fontSize);
            worksheet.Cells[0, 10].SetIsBold(true);
            //worksheet.Cells[0, 10].SetIsWrapped(true);
            worksheet.Cells[0, 10].SetForeColor(tcWhite);
            worksheet.Cells[0, 10].SetFill(pfRed);
            worksheet.Cells[0, 10].SetBorders(borders);
            worksheet.Cells[0, 11].SetValue("Ημερομηνία Υλοποίησης (Έναρξη)");
            worksheet.Cells[0, 11].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 11].SetFontSize(fontSize);
            worksheet.Cells[0, 11].SetIsBold(true);
            //worksheet.Cells[0, 11].SetIsWrapped(true);
            worksheet.Cells[0, 11].SetFill(pfOrange);
            worksheet.Cells[0, 11].SetBorders(borders);
            worksheet.Cells[0, 12].SetValue("Ημερομηνία Υλοποίησης (Λήξη)");
            worksheet.Cells[0, 12].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 12].SetFontSize(fontSize);
            worksheet.Cells[0, 12].SetIsBold(true);
            //worksheet.Cells[0, 12].SetIsWrapped(true);
            worksheet.Cells[0, 12].SetFill(pfOrange);
            worksheet.Cells[0, 12].SetBorders(borders);
            worksheet.Cells[0, 13].SetValue("Τελική Διάρκεια");
            worksheet.Cells[0, 13].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 13].SetFontSize(fontSize);
            worksheet.Cells[0, 13].SetIsBold(true);
            //worksheet.Cells[0, 13].SetIsWrapped(true);
            worksheet.Cells[0, 13].SetFill(pfGreen);
            worksheet.Cells[0, 13].SetBorders(borders);
            worksheet.Cells[0, 14].SetValue("Ημερομηνία Εντολής Τιμολόγησης");
            worksheet.Cells[0, 14].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 14].SetFontSize(fontSize);
            worksheet.Cells[0, 14].SetIsBold(true);
            //worksheet.Cells[0, 14].SetIsWrapped(true);
            worksheet.Cells[0, 14].SetFill(pfOrange);
            worksheet.Cells[0, 14].SetBorders(borders);
            worksheet.Cells[0, 15].SetValue("Ποσό Είσπραξης");
            worksheet.Cells[0, 15].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 15].SetFontSize(fontSize);
            worksheet.Cells[0, 15].SetIsBold(true);
            //worksheet.Cells[0, 15].SetIsWrapped(true);
            worksheet.Cells[0, 15].SetForeColor(tcWhite);
            worksheet.Cells[0, 15].SetFill(pfRed);
            worksheet.Cells[0, 15].SetBorders(borders);
            worksheet.Cells[0, 16].SetValue("Προγραμματισμένη Ημερομηνία Είσπραξης");
            worksheet.Cells[0, 16].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 16].SetFontSize(fontSize);
            worksheet.Cells[0, 16].SetIsBold(true);
            //worksheet.Cells[0, 16].SetIsWrapped(true);
            worksheet.Cells[0, 16].SetFill(pfOrange);
            worksheet.Cells[0, 16].SetBorders(borders);
            worksheet.Cells[0, 17].SetValue("Ακυρώθηκε;");
            worksheet.Cells[0, 17].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 17].SetFontSize(fontSize);
            worksheet.Cells[0, 17].SetIsBold(true);
            worksheet.Cells[0, 17].SetFill(pfGreen);
            worksheet.Cells[0, 17].SetBorders(borders);
            worksheet.Cells[0, 18].SetValue("Παρατηρήσεις");
            worksheet.Cells[0, 18].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 18].SetFontSize(fontSize);
            worksheet.Cells[0, 18].SetIsBold(true);
            worksheet.Cells[0, 18].SetFill(pfGreen);
            worksheet.Cells[0, 18].SetBorders(borders);
            worksheet.Cells[0, 19].SetValue("Παρατηρήσεις Τιμολογίου");
            worksheet.Cells[0, 19].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 19].SetFontSize(fontSize);
            worksheet.Cells[0, 19].SetIsBold(true);
            worksheet.Cells[0, 19].SetFill(pfGreen);
            worksheet.Cells[0, 19].SetBorders(borders);
            worksheet.Cells[0, 20].SetValue("Χρήστης");
            worksheet.Cells[0, 20].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 20].SetFontSize(fontSize);
            worksheet.Cells[0, 20].SetIsBold(true);
            worksheet.Cells[0, 20].SetForeColor(tcWhite);
            worksheet.Cells[0, 20].SetFill(pfBlue);
            worksheet.Cells[0, 20].SetBorders(borders);
            worksheet.Cells[0, 21].SetValue("Ημερομηνία Καταχώρησης");
            worksheet.Cells[0, 21].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 21].SetFontSize(fontSize);
            worksheet.Cells[0, 21].SetIsBold(true);
            worksheet.Cells[0, 21].SetForeColor(tcWhite);
            worksheet.Cells[0, 21].SetFill(pfBlue);
            worksheet.Cells[0, 21].SetBorders(borders);
        }

    }

}