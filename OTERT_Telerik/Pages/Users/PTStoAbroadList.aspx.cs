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

    public partial class PTStoAbroadList : Page {

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
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Λίστες > ΠΤΣ προς ΕΞωτερικό";
                gridMain.MasterTableView.Caption = "Λίστες > ΠΤΣ προς ΕΞωτερικό";
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
                gridMain.VirtualItemCount = cont.CountAllTasksforPTStoAbroad(recFilter);
                gridMain.DataSource = cont.GetAllTasksforPTStoAbroad(recSkip, recTake, recFilter, gridSortExxpressions);
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
                    (filterItem["PaymentDateOrder"].Controls[0] as LiteralControl).Text = "Από: ";
                    (filterItem["PaymentDateOrder"].Controls[3] as LiteralControl).Text = "<br />Έως: ";
                    (filterItem["PaymentDateCalculated"].Controls[0] as LiteralControl).Text = "Από: ";
                    (filterItem["PaymentDateCalculated"].Controls[3] as LiteralControl).Text = "<br />Έως: ";
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
                    RadDateTimePicker PaymentDateOrderFrom = filterItem["PaymentDateOrder"].Controls[1] as RadDateTimePicker;
                    PaymentDateOrderFrom.TimePopupButton.Visible = false;
                    PaymentDateOrderFrom.DateInput.DisplayDateFormat = "d/M/yyyy";
                    PaymentDateOrderFrom.DateInput.DateFormat = "d/M/yyyy";
                    RadDateTimePicker PaymentDateOrderTo = filterItem["PaymentDateOrder"].Controls[4] as RadDateTimePicker;
                    PaymentDateOrderTo.TimePopupButton.Visible = false;
                    PaymentDateOrderTo.DateInput.DisplayDateFormat = "d/M/yyyy";
                    PaymentDateOrderTo.DateInput.DateFormat = "d/M/yyyy";
                    PaymentDateOrderFrom.DateInput.Attributes.Add("onchange", "javascript:UpdateTo('" + PaymentDateOrderFrom.ClientID + "', '" + PaymentDateOrderTo.ClientID + "');");
                    RadDateTimePicker PaymentDateCalculatedFrom = filterItem["PaymentDateCalculated"].Controls[1] as RadDateTimePicker;
                    PaymentDateCalculatedFrom.TimePopupButton.Visible = false;
                    PaymentDateCalculatedFrom.DateInput.DisplayDateFormat = "d/M/yyyy";
                    PaymentDateCalculatedFrom.DateInput.DateFormat = "d/M/yyyy";
                    RadDateTimePicker PaymentDateCalculatedTo = filterItem["PaymentDateCalculated"].Controls[4] as RadDateTimePicker;
                    PaymentDateCalculatedTo.TimePopupButton.Visible = false;
                    PaymentDateCalculatedTo.DateInput.DisplayDateFormat = "d/M/yyyy";
                    PaymentDateCalculatedTo.DateInput.DateFormat = "d/M/yyyy";
                    PaymentDateCalculatedFrom.DateInput.Attributes.Add("onchange", "javascript:UpdateTo('" + PaymentDateCalculatedFrom.ClientID + "', '" + PaymentDateCalculatedTo.ClientID + "');");
                }
            }
        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                if (e.Item is GridFilteringItem) {
                    GridFilteringItem filterItem = (GridFilteringItem)e.Item;
                    RadDropDownList c1list = (RadDropDownList)filterItem.FindControl("ddlCustomer1Filter");
                    RadDropDownList clist = (RadDropDownList)filterItem.FindControl("ddlCustomersFilter");
                    RadDropDownList elist = (RadDropDownList)filterItem.FindControl("ddlEventFilter");
                    RadDropDownList countrylist = (RadDropDownList)filterItem.FindControl("ddlCuntryFilter");
                    try {
                        CustomersController ccont = new CustomersController();
                        c1list.DataSource = ccont.GetForeignProviders();
                        c1list.DataTextField = "NameGR";
                        c1list.DataValueField = "ID";
                        c1list.DataBind();
                        c1list.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                        clist.DataSource = ccont.GetPTSCustomers();
                        clist.DataTextField = "NameGR";
                        clist.DataValueField = "ID";
                        clist.DataBind();
                        clist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                        EventsController econt = new EventsController();
                        elist.DataSource = econt.GetEvents();
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
            int ID = (int)editableItem.GetDataKeyValue("OrderID");
            try {
                Response.Redirect("PTStoAbroad.aspx?id=" + ID.ToString(), false);
            }
            catch (Exception) { }
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
            Response.AppendHeader("content-disposition", "attachment; filename=PTSToAbroad-Report-" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx");
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
                int tasksCount = cont.CountAllTasksforPTStoAbroad(recFilter);
                tasks = cont.GetAllTasksforPTStoAbroad(0, tasksCount, recFilter, gridSortExxpressions);
            }
            catch (Exception) { }
            prepareDocument(worksheet);
            int currentRow = 1;
            CellBorder border = new CellBorder(CellBorderStyle.Thin, tcBlack);
            CellBorders borders = new CellBorders(border, border, border, border, null, null, null, null);
            double fontSize = 12;
            foreach (TaskB curTask in tasks) {
                worksheet.Cells[currentRow,0].SetValue(curTask.OrderID.ToString());
                worksheet.Cells[currentRow,0].SetFontSize(fontSize);
                worksheet.Cells[currentRow,0].SetBorders(borders);
                worksheet.Cells[currentRow,1].SetValue(curTask.Order.RegNo);
                worksheet.Cells[currentRow,1].SetFontSize(fontSize);
                worksheet.Cells[currentRow,1].SetBorders(borders);
                worksheet.Cells[currentRow,2].SetValue(curTask.Order.Event.Place.Country.NameGR);
                worksheet.Cells[currentRow,2].SetFontSize(fontSize);
                worksheet.Cells[currentRow,2].SetBorders(borders);
                worksheet.Cells[currentRow,3].SetValue(curTask.Order.Customer1.NameGR);
                worksheet.Cells[currentRow,3].SetFontSize(fontSize);
                worksheet.Cells[currentRow,3].SetBorders(borders);
                worksheet.Cells[currentRow,4].SetValue(curTask.Order.Event.NameGR);
                worksheet.Cells[currentRow,4].SetFontSize(fontSize);
                worksheet.Cells[currentRow,4].SetBorders(borders);
                worksheet.Cells[currentRow,5].SetValue(curTask.ID);
                worksheet.Cells[currentRow,5].SetFontSize(fontSize);
                worksheet.Cells[currentRow,5].SetBorders(borders);
                worksheet.Cells[currentRow,6].SetValue(curTask.OrderDate);
                worksheet.Cells[currentRow,6].SetFormat(new CellValueFormat(dateFormat));
                worksheet.Cells[currentRow,6].SetFontSize(fontSize);
                worksheet.Cells[currentRow,6].SetBorders(borders);
                worksheet.Cells[currentRow,7].SetValue(curTask.Customer.NameGR);
                worksheet.Cells[currentRow,7].SetFontSize(fontSize);
                worksheet.Cells[currentRow,7].SetBorders(borders);
                worksheet.Cells[currentRow,8].SetValue(curTask.TelephoneNumber);
                worksheet.Cells[currentRow,8].SetFontSize(fontSize);
                worksheet.Cells[currentRow,8].SetBorders(borders);
                worksheet.Cells[currentRow,9].SetValue(curTask.DateTimeStartActual.GetValueOrDefault());
                worksheet.Cells[currentRow,9].SetFormat(new CellValueFormat(dateFormat));
                worksheet.Cells[currentRow,9].SetFontSize(fontSize);
                worksheet.Cells[currentRow,9].SetBorders(borders);
                worksheet.Cells[currentRow,10].SetValue(curTask.CostActual.ToString());
                worksheet.Cells[currentRow,10].SetFormat(new CellValueFormat(currencyFormat));
                worksheet.Cells[currentRow,10].SetFontSize(fontSize);
                worksheet.Cells[currentRow,10].SetBorders(borders);
                if (curTask.PaymentDateOrder.GetValueOrDefault().Year > 2000) {
                    worksheet.Cells[currentRow, 11].SetValue(curTask.PaymentDateOrder.GetValueOrDefault());
                } else {
                    worksheet.Cells[currentRow, 11].SetValue("");
                }
                worksheet.Cells[currentRow,11].SetFormat(new CellValueFormat(dateFormat));
                worksheet.Cells[currentRow,11].SetFontSize(fontSize);
                worksheet.Cells[currentRow,11].SetBorders(borders);
                if (curTask.PaymentDateCalculated.GetValueOrDefault().Year > 2000) {
                    worksheet.Cells[currentRow, 12].SetValue(curTask.PaymentDateCalculated.GetValueOrDefault());
                } else {
                    worksheet.Cells[currentRow, 12].SetValue("");
                }
                worksheet.Cells[currentRow,12].SetFormat(new CellValueFormat(dateFormat));
                worksheet.Cells[currentRow,12].SetFontSize(fontSize);
                worksheet.Cells[currentRow,12].SetBorders(borders);
                worksheet.Cells[currentRow, 13].SetValue(curTask.EnteredByUser);
                worksheet.Cells[currentRow, 13].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 13].SetBorders(borders);
                worksheet.Cells[currentRow, 14].SetValue(curTask.DateStamp);
                worksheet.Cells[currentRow, 14].SetFormat(new CellValueFormat(dateWithHoursFormat));
                worksheet.Cells[currentRow, 14].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 14].SetBorders(borders);
                currentRow++;
            }

            //worksheet.Cells[currentRow, 0].SetValue("=SUM(K2:K4)");
            //worksheet.Cells[currentRow, 0].SetFontSize(fontSize);
            //worksheet.Cells[currentRow, 0].SetBorders(borders);

            //worksheet.Cells[currentRow, 0].SetValue("=SUBTOTAL(109,K2:K4)");
            //worksheet.GroupingProperties.SummaryRowIsBelow = true;
            //bool test = worksheet.Rows[1, 5].Group();

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
            worksheet.Cells[0, 2].SetValue("Χώρα");
            worksheet.Cells[0, 2].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 2].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 2].SetFontSize(fontSize);
            worksheet.Cells[0, 2].SetIsBold(true);
            worksheet.Cells[0, 2].SetFill(pfGreen);
            worksheet.Cells[0, 2].SetBorders(borders);
            worksheet.Cells[0, 3].SetValue("Πάροχος");
            worksheet.Cells[0, 3].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 3].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 3].SetFontSize(fontSize);
            worksheet.Cells[0, 3].SetIsBold(true);
            worksheet.Cells[0, 3].SetFill(pfGreen);
            worksheet.Cells[0, 3].SetBorders(borders);
            worksheet.Cells[0, 4].SetValue("Διοργάνωση");
            worksheet.Cells[0, 4].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 4].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 4].SetFontSize(fontSize);
            worksheet.Cells[0, 4].SetIsBold(true);
            worksheet.Cells[0, 4].SetFill(pfGreen);
            worksheet.Cells[0, 4].SetBorders(borders);
            worksheet.Cells[0, 5].SetValue("Παραγγελία Α/Α");
            worksheet.Cells[0, 5].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 5].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 5].SetFontSize(fontSize);
            worksheet.Cells[0, 5].SetIsBold(true);
            worksheet.Cells[0, 5].SetFill(pfOrange);
            worksheet.Cells[0, 5].SetBorders(borders);
            worksheet.Cells[0, 6].SetValue("Ημ/νία\nΠαραγγελίας");
            worksheet.Cells[0, 6].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 6].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 6].SetFontSize(fontSize);
            worksheet.Cells[0, 6].SetIsBold(true);
            worksheet.Cells[0, 6].SetIsWrapped(true);
            worksheet.Cells[0, 6].SetFill(pfOrange);
            worksheet.Cells[0, 6].SetBorders(borders);
            worksheet.Cells[0, 7].SetValue("Πελάτης");
            worksheet.Cells[0, 7].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 7].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 7].SetFontSize(fontSize);
            worksheet.Cells[0, 7].SetIsBold(true);
            worksheet.Cells[0, 7].SetFill(pfOrange);
            worksheet.Cells[0, 7].SetBorders(borders);
            worksheet.Cells[0, 8].SetValue("Τηλέφωνο Χρέωσης");
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
            worksheet.Cells[0, 11].SetValue("Ημ/νία Λήψης\nΞένου Τιμολογίου");
            worksheet.Cells[0, 11].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 11].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 11].SetFontSize(fontSize);
            worksheet.Cells[0, 11].SetIsBold(true);
            worksheet.Cells[0, 11].SetIsWrapped(true);
            worksheet.Cells[0, 11].SetForeColor(tcWhite);
            worksheet.Cells[0, 11].SetFill(pfRed);
            worksheet.Cells[0, 11].SetBorders(borders);
            worksheet.Cells[0, 12].SetValue("Ημ/νία Ελέγχου\nΤιμολογίου");
            worksheet.Cells[0, 12].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 12].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 12].SetFontSize(fontSize);
            worksheet.Cells[0, 12].SetIsBold(true);
            worksheet.Cells[0, 12].SetIsWrapped(true);
            worksheet.Cells[0, 12].SetForeColor(tcWhite);
            worksheet.Cells[0, 12].SetFill(pfRed);
            worksheet.Cells[0, 12].SetBorders(borders);
            worksheet.Cells[0, 13].SetValue("Χρήστης");
            worksheet.Cells[0, 13].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 13].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 13].SetFontSize(fontSize);
            worksheet.Cells[0, 13].SetIsBold(true);
            worksheet.Cells[0, 13].SetIsWrapped(true);
            worksheet.Cells[0, 13].SetForeColor(tcWhite);
            worksheet.Cells[0, 13].SetFill(pfBlue);
            worksheet.Cells[0, 13].SetBorders(borders);
            worksheet.Cells[0, 14].SetValue("Ημ/νία\nΚαταχώρησης");
            worksheet.Cells[0, 14].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 14].SetVerticalAlignment(RadVerticalAlignment.Center);
            worksheet.Cells[0, 14].SetFontSize(fontSize);
            worksheet.Cells[0, 14].SetIsBold(true);
            worksheet.Cells[0, 14].SetIsWrapped(true);
            worksheet.Cells[0, 14].SetForeColor(tcWhite);
            worksheet.Cells[0, 14].SetFill(pfBlue);
            worksheet.Cells[0, 14].SetBorders(borders);
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
                if (expression.Contains("Order.Event.Place.CountryID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(Order.Event.Place.CountryID = " + e.Value + ")";
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

        protected void ddlCustomer1Filter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("Order.Customer1ID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(Order.Customer1ID = " + e.Value + ")";
                gridMain.MasterTableView.GetColumn("Customer1ID").CurrentFilterFunction = GridKnownFunction.EqualTo;
                gridMain.MasterTableView.GetColumn("Customer1ID").CurrentFilterValue = e.Value;
            } else {
                gridMain.MasterTableView.GetColumn("Customer1ID").CurrentFilterFunction = GridKnownFunction.NoFilter;
                gridMain.MasterTableView.GetColumn("Customer1ID").CurrentFilterValue = null;
            }
            gridMain.MasterTableView.FilterExpression = finalExpression;
            ViewState[list.ClientID] = e.Value;
            gridMain.MasterTableView.Rebind();
        }

        protected void ddlCustomer1Filter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

        protected void ddlEventFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("Order.EventID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(Order.EventID = " + e.Value + ")";
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