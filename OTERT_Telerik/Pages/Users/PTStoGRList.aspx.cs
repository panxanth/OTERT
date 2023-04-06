using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using OTERT.Model;
using OTERT.Controller;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;
using System.IO;
using Telerik.Windows.Documents.Spreadsheet.Utilities;

namespace OTERT.Pages.UserPages {

    public partial class PTStoGRList : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;
        protected UserB loggedUser;

        protected readonly ThemableColor cellBackground = ThemableColor.FromArgb(0, 255, 255, 255);
        protected readonly ThemableColor tcBlack = ThemableColor.FromArgb(255, 0, 0, 0);
        protected readonly ThemableColor tcWhite = ThemableColor.FromArgb(255, 255, 255, 255);
        protected readonly string dateFormat = "dd/MM/yyyy";
        protected readonly string dateWithHoursFormat = "dd/MM/yyyy HH:mm";
        protected readonly string currencyFormat = "#.##0,00 €";

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Λίστες > ΠΤΣ προς Ελλάδα";
                gridMain.MasterTableView.Caption = "Λίστες > ΠΤΣ προς Ελλάδα";
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.MasterTableView.CurrentPageIndex * gridMain.MasterTableView.PageSize;
            int recTake = gridMain.MasterTableView.PageSize;
            string recFilter = gridMain.MasterTableView.FilterExpression;
            GridSortExpressionCollection gridSortExxpressions = gridMain.MasterTableView.SortExpressions;
            try {
                TasksPTSGRController cont = new TasksPTSGRController();
                gridMain.VirtualItemCount = cont.CountAllTasksPTSGR(recFilter);
                gridMain.DataSource = cont.GetAllTasksPTSGR(recSkip, recTake, recFilter, gridSortExxpressions);
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
                    (filterItem["PaymentDateActual"].Controls[0] as LiteralControl).Text = "Από: ";
                    (filterItem["PaymentDateActual"].Controls[3] as LiteralControl).Text = "<br />Έως: ";
                    RadDateTimePicker OrderDateFrom = filterItem["OrderDate"].Controls[1] as RadDateTimePicker;
                    OrderDateFrom.TimePopupButton.Visible = false;
                    OrderDateFrom.DateInput.DisplayDateFormat = "d/M/yyyy";
                    OrderDateFrom.DateInput.DateFormat = "d/M/yyyy";
                    RadDateTimePicker OrderDateTo = filterItem["OrderDate"].Controls[4] as RadDateTimePicker;
                    OrderDateTo.TimePopupButton.Visible = false;
                    OrderDateTo.DateInput.DisplayDateFormat = "d/M/yyyy";
                    OrderDateTo.DateInput.DateFormat = "d/M/yyyy";
                    OrderDateFrom.DateInput.Attributes.Add("onchange", "javascript:UpdateTo('" + OrderDateFrom.ClientID + "', '" + OrderDateTo.ClientID + "');");
                    RadDateTimePicker startActualFrom = filterItem["DateTimeStartActual"].Controls[1] as RadDateTimePicker;
                    startActualFrom.TimePopupButton.Visible = false;
                    startActualFrom.DateInput.DisplayDateFormat = "d/M/yyyy";
                    startActualFrom.DateInput.DateFormat = "d/M/yyyy";
                    RadDateTimePicker startActualTo = filterItem["DateTimeStartActual"].Controls[4] as RadDateTimePicker;
                    startActualTo.TimePopupButton.Visible = false;
                    startActualTo.DateInput.DisplayDateFormat = "d/M/yyyy";
                    startActualTo.DateInput.DateFormat = "d/M/yyyy";
                    startActualFrom.DateInput.Attributes.Add("onchange", "javascript:UpdateTo('" + startActualFrom.ClientID + "', '" + startActualTo.ClientID + "');");
                    RadDateTimePicker PaymentDateActualFrom = filterItem["PaymentDateActual"].Controls[1] as RadDateTimePicker;
                    PaymentDateActualFrom.TimePopupButton.Visible = false;
                    PaymentDateActualFrom.DateInput.DisplayDateFormat = "d/M/yyyy";
                    PaymentDateActualFrom.DateInput.DateFormat = "d/M/yyyy";
                    RadDateTimePicker PaymentDateActualTo = filterItem["PaymentDateActual"].Controls[4] as RadDateTimePicker;
                    PaymentDateActualTo.TimePopupButton.Visible = false;
                    PaymentDateActualTo.DateInput.DisplayDateFormat = "d/M/yyyy";
                    PaymentDateActualTo.DateInput.DateFormat = "d/M/yyyy";
                    startActualFrom.DateInput.Attributes.Add("onchange", "javascript:UpdateTo('" + PaymentDateActualFrom.ClientID + "', '" + PaymentDateActualTo.ClientID + "');");
                }
            }
        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                if (e.Item is GridFilteringItem) {
                    GridFilteringItem filterItem = (GridFilteringItem)e.Item;
                    RadDropDownList plist = (RadDropDownList)filterItem.FindControl("ddlProvidersFilter");
                    RadDropDownList clist = (RadDropDownList)filterItem.FindControl("ddlCustomersFilter");
                    RadDropDownList elist = (RadDropDownList)filterItem.FindControl("ddlEventFilter");
                    RadDropDownList countrylist = (RadDropDownList)filterItem.FindControl("ddlCuntryFilter");
                    try {
                        CustomersController ccont = new CustomersController();
                        plist.DataSource = ccont.GetForeignProviders();
                        plist.DataTextField = "NameGR";
                        plist.DataValueField = "ID";
                        plist.DataBind();
                        plist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                        clist.DataSource = ccont.GetForeignPTSCustomers();
                        clist.DataTextField = "NameGR";
                        clist.DataValueField = "ID";
                        clist.DataBind();
                        clist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                        EventsController econt = new EventsController();
                        elist.DataSource = econt.GetGreekEvents();
                        elist.DataTextField = "NameGR";
                        elist.DataValueField = "ID";
                        elist.DataBind();
                        elist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                        CountriesController countrycont = new CountriesController();
                        countrylist.DataSource = countrycont.GetForeignCountries();
                        countrylist.DataTextField = "NameGR";
                        countrylist.DataValueField = "ID";
                        countrylist.DataBind();
                        countrylist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
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
            int ID = (int)editableItem.GetDataKeyValue("Order.OrdersPTSGRID");
            try {
                Response.Redirect("PTStoGR.aspx?id=" + ID.ToString(), false);
            }
            catch (Exception) { }
        }

        protected void gridMain_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e) {
            GridTableView detailtabl = e.DetailTableView;
            int recSkip = detailtabl.CurrentPageIndex * gridMain.PageSize;
            int recTake = detailtabl.PageSize;
            GridDataItem parentItem = detailtabl.ParentItem;
            int taskID = int.Parse(parentItem.GetDataKeyValue("Order.OrdersPTSGRID").ToString());
            FilesController cont = new FilesController();
            detailtabl.VirtualItemCount = cont.CountFiles(taskID);
            detailtabl.DataSource = cont.GetFilesByOrderPTSGRID(taskID, recSkip, recTake);
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
            Response.AppendHeader("content-disposition", "attachment; filename=PTSToGR-Report-" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(renderedBytes);
            Response.End();
        }

        protected Workbook createWorkbook() {
            Workbook workbook = new Workbook();
            workbook.Sheets.Add(SheetType.Worksheet);
            Worksheet worksheet = workbook.ActiveWorksheet;
            worksheet.Name = "OTE RT";
            List<TaskPTSGRB> tasks = new List<TaskPTSGRB>();
            try {
                string recFilter = gridMain.MasterTableView.FilterExpression;
                GridSortExpressionCollection gridSortExxpressions = gridMain.MasterTableView.SortExpressions;
                TasksPTSGRController cont = new TasksPTSGRController();
                int tasksCount = cont.CountAllTasksPTSGR(recFilter);
                tasks = cont.GetAllTasksPTSGR(0, tasksCount, recFilter, gridSortExxpressions);
            }
            catch (Exception) { }
            prepareDocument(worksheet);
            int currentRow = 1;
            CellBorder border = new CellBorder(CellBorderStyle.Thin, tcBlack);
            CellBorders borders = new CellBorders(border, border, border, border, null, null, null, null);
            double fontSize = 12;
            foreach (TaskPTSGRB curTask in tasks) {
                worksheet.Cells[currentRow, 0].SetValue(curTask.Order.OrdersPTSGRID.ToString());
                worksheet.Cells[currentRow, 0].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 0].SetBorders(borders);
                worksheet.Cells[currentRow, 1].SetValue(curTask.RegNo);
                worksheet.Cells[currentRow, 1].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 1].SetBorders(borders);
                worksheet.Cells[currentRow, 2].SetValue(curTask.Order.OrderPTSGR.Event.NameGR);
                worksheet.Cells[currentRow, 2].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 2].SetBorders(borders);
                worksheet.Cells[currentRow, 3].SetValue(curTask.OrderPTSGR2ID.ToString());
                worksheet.Cells[currentRow, 3].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 3].SetBorders(borders);
                worksheet.Cells[currentRow, 4].SetValue(curTask.Order.Country.NameGR);
                worksheet.Cells[currentRow, 4].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 4].SetBorders(borders);
                worksheet.Cells[currentRow, 5].SetValue(curTask.Order.Provider.NameGR);
                worksheet.Cells[currentRow, 5].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 5].SetBorders(borders);
                worksheet.Cells[currentRow, 6].SetValue(curTask.ID);
                worksheet.Cells[currentRow, 6].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 6].SetBorders(borders);
                worksheet.Cells[currentRow, 7].SetValue(curTask.OrderDate);
                worksheet.Cells[currentRow, 7].SetFormat(new CellValueFormat(dateFormat));
                worksheet.Cells[currentRow, 7].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 7].SetBorders(borders);
                worksheet.Cells[currentRow, 8].SetValue(curTask.Customer.NameGR);
                worksheet.Cells[currentRow, 8].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 8].SetBorders(borders);
                if (curTask.DateTimeStartActual.GetValueOrDefault().Year > 2000) {
                    worksheet.Cells[currentRow, 9].SetValue(curTask.DateTimeStartActual.GetValueOrDefault());
                } else {
                    worksheet.Cells[currentRow, 9].SetValue("");
                }
                worksheet.Cells[currentRow, 9].SetFormat(new CellValueFormat(dateFormat));
                worksheet.Cells[currentRow, 9].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 9].SetBorders(borders);
                worksheet.Cells[currentRow, 10].SetValue(curTask.CostActual.ToString());
                worksheet.Cells[currentRow, 10].SetFormat(new CellValueFormat(currencyFormat));
                worksheet.Cells[currentRow, 10].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 10].SetBorders(borders);

                if (curTask.PaymentDateActual.GetValueOrDefault().Year > 2000) {
                    worksheet.Cells[currentRow, 11].SetValue(curTask.PaymentDateActual.GetValueOrDefault());
                } else {
                    worksheet.Cells[currentRow, 11].SetValue("");
                }
                worksheet.Cells[currentRow, 11].SetFormat(new CellValueFormat(dateFormat));
                worksheet.Cells[currentRow, 11].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 11].SetBorders(borders);
                worksheet.Cells[currentRow, 12].SetValue(curTask.EnteredByUser);
                worksheet.Cells[currentRow, 12].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 12].SetBorders(borders);
                worksheet.Cells[currentRow, 13].SetValue(curTask.DateStamp);
                worksheet.Cells[currentRow, 13].SetFormat(new CellValueFormat(dateWithHoursFormat));
                worksheet.Cells[currentRow, 13].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 13].SetBorders(borders);
                currentRow++;
            }
            for (int i = 0; i < worksheet.Columns.Count; i++) { worksheet.Columns[i].AutoFitWidth(); }
            for (int i = 0; i < worksheet.Columns.Count; i++) {
                if (i==1) { worksheet.Columns[i].SetWidth(new ColumnWidth(70, true)); }
                if (i == 6 || i == 9 || i == 11 || i == 12) { worksheet.Columns[i].SetWidth(new ColumnWidth(100, true)); }
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
            PatternFill pfCyan = new PatternFill(PatternType.Solid, System.Windows.Media.Color.FromArgb(255, 137, 233, 245), System.Windows.Media.Colors.Transparent);
            PatternFill pfRed = new PatternFill(PatternType.Solid, System.Windows.Media.Color.FromArgb(255, 255, 0, 0), System.Windows.Media.Colors.Transparent);
            PatternFill pfBlue = new PatternFill(PatternType.Solid, System.Windows.Media.Color.FromArgb(255, 0, 0, 255), System.Windows.Media.Colors.Transparent);
            CellBorder border = new CellBorder(CellBorderStyle.Thin, tcBlack);
            CellBorders borders = new CellBorders(border, border, border, border, null, null, null, null);
            double fontSize = 12;
            worksheet.Cells[0, 0].SetValue("Διοργάνωση Α/Α");
            worksheet.Cells[0, 0].SetFontSize(fontSize);
            worksheet.Cells[0, 0].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 0].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 0].SetIsBold(true);
            worksheet.Cells[0, 0].SetFill(pfGreen);
            worksheet.Cells[0, 0].SetBorders(borders);
            worksheet.Cells[0, 1].SetValue("Αριθμός\nΠρωτοκόλλου");
            worksheet.Cells[0, 1].SetFontSize(fontSize);
            worksheet.Cells[0, 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 1].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 1].SetIsBold(true);
            worksheet.Cells[0, 1].SetIsWrapped(true);
            worksheet.Cells[0, 1].SetFill(pfGreen);
            worksheet.Cells[0, 1].SetBorders(borders);
            worksheet.Cells[0, 2].SetValue("Διοργάνωση");
            worksheet.Cells[0, 2].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 2].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 2].SetFontSize(fontSize);
            worksheet.Cells[0, 2].SetIsBold(true);
            worksheet.Cells[0, 2].SetFill(pfGreen);
            worksheet.Cells[0, 2].SetBorders(borders);
            worksheet.Cells[0, 3].SetValue("Πάροχος Α/Α");
            worksheet.Cells[0, 3].SetFontSize(fontSize);
            worksheet.Cells[0, 3].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 3].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 3].SetIsBold(true);
            worksheet.Cells[0, 3].SetFill(pfCyan);
            worksheet.Cells[0, 3].SetBorders(borders);
            worksheet.Cells[0, 4].SetValue("Από Χώρα");
            worksheet.Cells[0, 4].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 4].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 4].SetFontSize(fontSize);
            worksheet.Cells[0, 4].SetIsBold(true);
            worksheet.Cells[0, 4].SetFill(pfCyan);
            worksheet.Cells[0, 4].SetBorders(borders);
            worksheet.Cells[0, 5].SetValue("Πάροχος");
            worksheet.Cells[0, 5].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 5].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 5].SetFontSize(fontSize);
            worksheet.Cells[0, 5].SetIsBold(true);
            worksheet.Cells[0, 5].SetFill(pfCyan);
            worksheet.Cells[0, 5].SetBorders(borders);
            worksheet.Cells[0, 6].SetValue("Παραγγελία Α/Α");
            worksheet.Cells[0, 6].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 6].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 6].SetFontSize(fontSize);
            worksheet.Cells[0, 6].SetIsBold(true);
            worksheet.Cells[0, 6].SetFill(pfOrange);
            worksheet.Cells[0, 6].SetBorders(borders);
            worksheet.Cells[0, 7].SetValue("Ημ/νία\nΠαραγγελίας");
            worksheet.Cells[0, 7].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 7].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 7].SetFontSize(fontSize);
            worksheet.Cells[0, 7].SetIsBold(true);
            worksheet.Cells[0, 7].SetIsWrapped(true);
            worksheet.Cells[0, 7].SetFill(pfOrange);
            worksheet.Cells[0, 7].SetBorders(borders);
            worksheet.Cells[0, 8].SetValue("Πελάτης");
            worksheet.Cells[0, 8].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 8].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 8].SetFontSize(fontSize);
            worksheet.Cells[0, 8].SetIsBold(true);
            worksheet.Cells[0, 8].SetFill(pfOrange);
            worksheet.Cells[0, 8].SetBorders(borders);
            worksheet.Cells[0, 9].SetValue("Ημ/νία Υλοποίησης\n(Έναρξη)");
            worksheet.Cells[0, 9].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 9].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 9].SetFontSize(fontSize);
            worksheet.Cells[0, 9].SetIsBold(true);
            worksheet.Cells[0, 9].SetIsWrapped(true);
            worksheet.Cells[0, 9].SetFill(pfOrange);
            worksheet.Cells[0, 9].SetBorders(borders);
            worksheet.Cells[0, 10].SetValue("Ποσό Είσπραξης (€)");
            worksheet.Cells[0, 10].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 10].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 10].SetFontSize(fontSize);
            worksheet.Cells[0, 10].SetIsBold(true);
            worksheet.Cells[0, 10].SetFill(pfOrange);
            worksheet.Cells[0, 10].SetBorders(borders);
            worksheet.Cells[0, 11].SetValue("Ημ/νία Τιμολόγησης");
            worksheet.Cells[0, 11].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 11].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 11].SetFontSize(fontSize);
            worksheet.Cells[0, 11].SetIsBold(true);
            worksheet.Cells[0, 11].SetIsWrapped(true);
            worksheet.Cells[0, 11].SetForeColor(tcWhite);
            worksheet.Cells[0, 11].SetFill(pfRed);
            worksheet.Cells[0, 11].SetBorders(borders);
            worksheet.Cells[0, 12].SetValue("Χρήστης");
            worksheet.Cells[0, 12].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 12].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 12].SetFontSize(fontSize);
            worksheet.Cells[0, 12].SetIsBold(true);
            worksheet.Cells[0, 12].SetIsWrapped(true);
            worksheet.Cells[0, 12].SetForeColor(tcWhite);
            worksheet.Cells[0, 12].SetFill(pfBlue);
            worksheet.Cells[0, 12].SetBorders(borders);
            worksheet.Cells[0, 13].SetValue("Ημ/νία\nΚαταχώρησης");
            worksheet.Cells[0, 13].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 13].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 13].SetFontSize(fontSize);
            worksheet.Cells[0, 13].SetIsBold(true);
            worksheet.Cells[0, 13].SetIsWrapped(true);
            worksheet.Cells[0, 13].SetForeColor(tcWhite);
            worksheet.Cells[0, 13].SetFill(pfBlue);
            worksheet.Cells[0, 13].SetBorders(borders);
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

        protected void ddlCuntryFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("Order.CountryID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(Order.CountryID = " + e.Value + ")";
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

        protected void ddlCuntrysFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

        protected void ddlProvidersFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("Order.ProviderID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(Order.ProviderID = " + e.Value + ")";
                gridMain.MasterTableView.GetColumn("ProviderID").CurrentFilterFunction = GridKnownFunction.EqualTo;
                gridMain.MasterTableView.GetColumn("ProviderID").CurrentFilterValue = e.Value;
            } else {
                gridMain.MasterTableView.GetColumn("ProviderID").CurrentFilterFunction = GridKnownFunction.NoFilter;
                gridMain.MasterTableView.GetColumn("ProviderID").CurrentFilterValue = null;
            }
            gridMain.MasterTableView.FilterExpression = finalExpression;
            ViewState[list.ClientID] = e.Value;
            gridMain.MasterTableView.Rebind();
        }

        protected void ddlProvidersFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

        protected void ddlEventFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("Order.OrderPTSGR.EventID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(Order.OrderPTSGR.EventID = " + e.Value + ")";
                gridMain.MasterTableView.GetColumn("EventID").CurrentFilterFunction = GridKnownFunction.EqualTo;
                gridMain.MasterTableView.GetColumn("EventID").CurrentFilterValue = e.Value;
            } else {
                gridMain.MasterTableView.GetColumn("EventID").CurrentFilterFunction = GridKnownFunction.NoFilter;
                gridMain.MasterTableView.GetColumn("EventID").CurrentFilterValue = null;
            }
            gridMain.MasterTableView.FilterExpression = finalExpression;
            ViewState[list.ClientID] = e.Value;
            gridMain.MasterTableView.Rebind();
        }

        protected void ddlEventFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

    }

}