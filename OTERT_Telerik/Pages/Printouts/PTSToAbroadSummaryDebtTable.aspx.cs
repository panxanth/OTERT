using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using OTERT.Model;
using OTERT.Controller;
using OTERT_Entity;
using Telerik.Windows.Documents.Flow.Model.Editing;
using Telerik.Windows.Documents.Flow.Model;
using Telerik.Windows.Documents.Common.FormatProviders;
using Telerik.Windows.Documents.Flow.FormatProviders.Docx;
using Microsoft.SqlServer.Server;
using Telerik.Windows.Documents.Flow.Model.Styles;
using Telerik.Windows.Documents.Spreadsheet.Model;
using System.Windows;

namespace OTERT.Pages.Printouts {

    public partial class PTSToAbroadSummaryDebtTable : Page {

        protected RadDatePicker dpDateFrom, dpDateTo;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected RadButton btnCreate;
        protected Label lblCustomer, lblDateFrom, lblDateTo, lblDateCreated;
        protected string pageTitle;
        protected UserB loggedUser;
        const string fileUploadFolder = "~/UploadedFiles/";
        const string templatesFolder = "~/Templates/";
        const string pageUniqueName = "PTStoAbroadSumDebts";

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Εκτυπωτικά > Συγκεντρωτικός Πίνακας Οφειλών σε Ξένες Τηλεπ/κες Υπηρεσίες";
                dpDateFrom.SelectedDate = DateTime.Today.AddMonths(-1).Date;
                dpDateTo.SelectedDate = DateTime.Today.Date;
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected void btnCreate_Click(object sender, EventArgs e) {
            try{
                DateTime DateFrom = (dpDateFrom.SelectedDate != null ? (DateTime)dpDateFrom.SelectedDate : DateTime.Now);
                DateTime DateTo = (dpDateTo.SelectedDate != null ? (DateTime)dpDateTo.SelectedDate : DateTime.Now);
                string dateSpan = getDatesSpan(DateFrom, DateTo);
                // Prepare Document
                DocumentReplacemetsController cont = new DocumentReplacemetsController();
                List<DocumentReplacemetB> reps = new List<DocumentReplacemetB>();
                reps = cont.GetDocumentReplacemets(pageUniqueName);
                string imgFolderPath = Server.MapPath(fileUploadFolder);
                DocumentReplacemetB curRep;
                BookmarkRangeStart bookmarkRangeStart;
                RadFlowDocument curDoc = LoadWordTemplate(pageUniqueName);
                RadFlowDocumentEditor editor = new RadFlowDocumentEditor(curDoc);
                List<BookmarkRangeStart> test = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().ToList();
                Telerik.Windows.Documents.Flow.Model.TableCell currCell;
                Run currRun;
                Header defaultHeader = editor.Document.Sections.First().Headers.Default;
                Footer defaultFooter = editor.Document.Sections.First().Footers.Default;
                Telerik.Windows.Documents.Flow.Model.Styles.Style tableStyle = new Telerik.Windows.Documents.Flow.Model.Styles.Style("TableStyle", StyleType.Table);
                tableStyle.Name = "Table Style";
                tableStyle.TableProperties.Borders.LocalValue = new TableBorders(new Border(0, Telerik.Windows.Documents.Flow.Model.Styles.BorderStyle.None, new ThemableColor(System.Windows.Media.Colors.Black)));
                tableStyle.TableProperties.Alignment.LocalValue = Alignment.Left;
                tableStyle.TableCellProperties.VerticalAlignment.LocalValue = VerticalAlignment.Center;
                tableStyle.TableCellProperties.PreferredWidth.LocalValue = new TableWidthUnit(TableWidthUnitType.Percent, 100);
                tableStyle.TableCellProperties.Padding.LocalValue = new Telerik.Windows.Documents.Primitives.Padding(8);
                editor.Document.StyleRepository.Add(tableStyle);
                // Document Replacements
                curRep = reps.Find(o => o.UniqueName == "PTStoAbroadSumDebts_Header_OTELogo");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                using (Stream firstImage = File.OpenRead(imgFolderPath + curRep.Text)) {
                    var inImage = ((Paragraph)currCell.Blocks.First()).Inlines.AddImageInline();
                    inImage.Image.ImageSource = new Telerik.Windows.Documents.Media.ImageSource(firstImage, curRep.Text.Split('.').Last());
                    if (curRep.ImageHeight != null && curRep.ImageWidth != null) {
                        inImage.Image.Height = curRep.ImageHeight.Value;
                        inImage.Image.Width = curRep.ImageWidth.Value;
                    }
                }
                curRep = reps.Find(o => o.UniqueName == "PTStoAbroadSumDebts_Header_OTEMoto");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                currRun.Text = curRep.Text;
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 13.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                curRep = reps.Find(o => o.UniqueName == "PTStoAbroadSumDebts_Header_Address");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                string[] arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                Paragraph newPar = (Paragraph)currCell.Blocks.First();
                newPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                for (int i = 0; i < arrText.Length; i++) {
                    currRun = editor.InsertLine(arrText[i]);
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                }
                currCell.Blocks.Remove(currCell.Blocks.Last());
                curRep = reps.Find(o => o.UniqueName == "PTStoAbroadSumDebts_Header_Info");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                newPar = (Paragraph)currCell.Blocks.First();
                newPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                for (int i = 0; i < arrText.Length; i++) {
                    currRun = editor.InsertLine(arrText[i]);
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                }
                currCell.Blocks.Remove(currCell.Blocks.Last());
                curRep = reps.Find(o => o.UniqueName == "PTStoAbroadSumDebts_Header_Title");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                currRun.Text = curRep.Text + " " + dateSpan;
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 13.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                curRep = reps.Find(o => o.UniqueName == "PTStoAbroadSumDebts_Footer_PageNo");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                editor.MoveToParagraphStart((Paragraph)currCell.Blocks.First());
                currRun = editor.InsertText("ΣΕΛΙΔΑ ");
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 12.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                editor.InsertField("PAGE", "3");
                if (curRep.Text == "Σελίδα Χ από Υ"){
                    currRun = editor.InsertText(" ΑΠΟ ");
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    editor.InsertField("NUMPAGES", "5");
                }
                curRep = reps.Find(o => o.UniqueName == "PTStoAbroadSumDebts_Check_Name");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                newPar = (Paragraph)currCell.Blocks.First();
                newPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                for (int i = 0; i < arrText.Length; i++) {
                    currRun = editor.InsertLine(arrText[i]);
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Center;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                }
                currCell.Blocks.Remove(currCell.Blocks.Last());
                curRep = reps.Find(o => o.UniqueName == "PTStoAbroadSumDebts_Chief_Name");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                newPar = (Paragraph)currCell.Blocks.First();
                newPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                for (int i = 0; i < arrText.Length; i++) {
                    currRun = editor.InsertLine(arrText[i]);
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Center;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                }
                currCell.Blocks.Remove(currCell.Blocks.Last());
                // Main Table
                bookmarkRangeStart = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().Where(rangeStart => rangeStart.Bookmark.Name == "Body_Main").FirstOrDefault();
                editor.MoveToInlineEnd(bookmarkRangeStart);
                decimal totalCost = 0.0M;
                decimal totalEuroCost = 0.0M;
                decimal totalAdministrativeCost = 0.0M;
                decimal totalCostPerOrder = 0.0M;
                decimal totalEuroCostPerOrder = 0.0M;
                decimal totalAdministrativeCostPerOrder = 0.0M;
                decimal totalCostPerCountry = 0.0M;
                decimal totalEuroCostPerCountry = 0.0M;
                decimal totalAdministrativeCostPerCountry = 0.0M;
                TasksController tc = new TasksController();
                EventsController ec = new EventsController();
                OrdersController oc = new OrdersController();
                List<TaskB> tasks = tc.GetAllTasksForOrdersBetweenDates(DateFrom, DateTo);
                List<int> distinctOrderIDs = tasks.Select(o => o.OrderID.Value).Distinct().ToList();
                List<OrderB> orders = new List<OrderB>();
                List<List<TaskB>> tasksForOrders = new List<List<TaskB>>();
                foreach (int orderID in distinctOrderIDs) {
                    OrderB newOrder = new OrderB();
                    newOrder.ID = orderID;
                    List<TaskB> tasksForOrder = tasks.Where(o => o.OrderID.Value == orderID).ToList();
                    EventB eventForOrder = ec.GetEventByID(tasksForOrder[0].Order.EventID);
                    newOrder.EventID = tasksForOrder[0].Order.EventID;
                    newOrder.Event = new EventDTO();
                    newOrder.Event.Place = new PlaceDTO();
                    newOrder.Event.Place.CountryID = eventForOrder.Place.Country.ID;
                    newOrder.Event.Place.Country = new CountryDTO();
                    newOrder.Event.Place.Country.ID = eventForOrder.Place.Country.ID;
                    newOrder.Event.Place.Country.NameGR = eventForOrder.Place.Country.NameGR;
                    newOrder.Event.Place.Country.NameEN = eventForOrder.Place.Country.NameEN;
                    orders.Add(newOrder);
                    tasksForOrders.Add(tasksForOrder);
                }
                List<int> distinctCountryIDs = orders.Select(o => o.Event.Place.CountryID).Distinct().ToList();
                foreach (int countryID in distinctCountryIDs) {
                    totalCostPerCountry = 0.0M;
                    totalEuroCostPerCountry = 0.0M;
                    totalAdministrativeCostPerCountry = 0.0M;
                    List<OrderB> ordersForCountryID = orders.Where(o => o.Event.Place.CountryID == countryID).ToList();
                    string countryName = ordersForCountryID[0].Event.Place.Country.NameGR;
                    editor.InsertLine(" ").FontSize = 12;
                    editor.InsertLine(toUpperGR(countryName)).FontSize = 14;
                    Telerik.Windows.Documents.Flow.Model.Table tblContent = editor.InsertTable();
                    tblContent.StyleId = "TableStyle";
                    tblContent.LayoutType = TableLayoutType.AutoFit;
                    ThemableColor cellBackground = new ThemableColor(System.Windows.Media.Colors.Beige);
                    List<int> orderIDsForCountryID = ordersForCountryID.Select(o => o.ID).ToList();
                    Telerik.Windows.Documents.Flow.Model.TableRow curRow = tblContent.Rows.AddTableRow();
                    Telerik.Windows.Documents.Flow.Model.TableCell curCell = curRow.Cells.AddTableCell();
                    currRun = curCell.Blocks.AddParagraph().Inlines.AddRun("ΤΙΜΟΛΟΓΙΟ");
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 14);
                    curCell = curRow.Cells.AddTableCell();
                    currRun = curCell.Blocks.AddParagraph().Inlines.AddRun("ΗΜΕΡ/ΝΙΑ ΕΚΔΟΣΗΣ");
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 14);
                    curCell = curRow.Cells.AddTableCell();
                    currRun = curCell.Blocks.AddParagraph().Inlines.AddRun("SDR");
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 14);
                    curCell = curRow.Cells.AddTableCell();
                    currRun = curCell.Blocks.AddParagraph().Inlines.AddRun("ΕΥΡΩ/SDR");
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 14);
                    curCell = curRow.Cells.AddTableCell();
                    currRun = curCell.Blocks.AddParagraph().Inlines.AddRun("ΕΥΡΩ");
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 14);
                    curCell = curRow.Cells.AddTableCell();
                    currRun = curCell.Blocks.AddParagraph().Inlines.AddRun("ΤΕΛΗ ΔΙΑΧ/ΣΗΣ");
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 15);
                    curCell = curRow.Cells.AddTableCell();
                    currRun = curCell.Blocks.AddParagraph().Inlines.AddRun("ΣΥΝΟΛΟ");
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 15);
                    foreach (int orderID in orderIDsForCountryID) {
                        List<TaskB> tasksForOrderID = tasks.Where(o => o.OrderID == orderID).ToList();
                        totalEuroCostPerOrder = 0.0M;
                        totalAdministrativeCostPerOrder = 0.0M;
                        totalCostPerOrder = 0.0M;
                        OrderB curOrder = oc.GetOrder(orderID);
                        string InoiceProtocol = curOrder.InoiceProtocol;
                        string PaymentDateOrder = tasksForOrderID[0].PaymentDateOrder.GetValueOrDefault().ToString("dd/MM/yyyy");
                        foreach (TaskB curTask in tasksForOrderID) {
                            decimal combinedCost = curTask.InvoiceCost.GetValueOrDefault() + curTask.AddedCharges.GetValueOrDefault();
                            totalEuroCostPerOrder += curTask.InvoiceCost.GetValueOrDefault();
                            totalAdministrativeCostPerOrder += curTask.AddedCharges.GetValueOrDefault();
                            totalCostPerOrder += combinedCost;
                            totalEuroCostPerCountry += curTask.InvoiceCost.GetValueOrDefault();
                            totalAdministrativeCostPerCountry += curTask.AddedCharges.GetValueOrDefault();
                            totalCostPerCountry += combinedCost;
                            totalEuroCost += curTask.InvoiceCost.GetValueOrDefault();
                            totalAdministrativeCost += curTask.AddedCharges.GetValueOrDefault();
                            totalCost += combinedCost;
                        }
                        curRow = tblContent.Rows.AddTableRow();
                        curCell = curRow.Cells.AddTableCell();
                        currRun = curCell.Blocks.AddParagraph().Inlines.AddRun(InoiceProtocol);
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 12.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                        currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 17);
                        curCell = curRow.Cells.AddTableCell();
                        currRun = curCell.Blocks.AddParagraph().Inlines.AddRun(PaymentDateOrder);
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 12.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                        currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 17);
                        curCell = curRow.Cells.AddTableCell();
                        currRun = curCell.Blocks.AddParagraph().Inlines.AddRun(" - ");
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 12.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                        currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 17);
                        curCell = curRow.Cells.AddTableCell();
                        currRun = curCell.Blocks.AddParagraph().Inlines.AddRun(" - ");
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 12.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                        currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 17);
                        curCell = curRow.Cells.AddTableCell();
                        currRun = curCell.Blocks.AddParagraph().Inlines.AddRun(totalEuroCostPerOrder.ToString());
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 12.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                        currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 17);
                        curCell = curRow.Cells.AddTableCell();
                        currRun = curCell.Blocks.AddParagraph().Inlines.AddRun(totalAdministrativeCostPerOrder.ToString());
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 12.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                        currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 17);
                        curCell = curRow.Cells.AddTableCell();
                        currRun = curCell.Blocks.AddParagraph().Inlines.AddRun(totalCostPerOrder.ToString());
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 12.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                        currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 17);
                    }
                    curRow = tblContent.Rows.AddTableRow();
                    curCell = curRow.Cells.AddTableCell();
                    curCell.ColumnSpan = 4;
                    string tim = "τιμολόγια";
                    if (orderIDsForCountryID.Count == 1) { tim= "τιμολόγιο"; }
                    currRun = curCell.Blocks.AddParagraph().Inlines.AddRun("Σύνολο για "+ toUpperGR(countryName) + " ("+ orderIDsForCountryID.Count.ToString() + " " + tim + ")");
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 17);
                    curCell = curRow.Cells.AddTableCell();
                    currRun = curCell.Blocks.AddParagraph().Inlines.AddRun(totalEuroCostPerCountry.ToString());
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 17);
                    curCell = curRow.Cells.AddTableCell();
                    currRun = curCell.Blocks.AddParagraph().Inlines.AddRun(totalAdministrativeCostPerCountry.ToString());
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 17);
                    curCell = curRow.Cells.AddTableCell();
                    currRun = curCell.Blocks.AddParagraph().Inlines.AddRun(totalCostPerCountry.ToString());
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    curCell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 17);
                    Telerik.Windows.Documents.Flow.Model.Table tblLine = editor.InsertTable();
                    tblLine.Borders = new TableBorders(null, new Border(Telerik.Windows.Documents.Flow.Model.Styles.BorderStyle.Thick), null, null);
                    tblLine.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 100);
                }
                editor.InsertLine(" ").FontSize = 12;
                editor.InsertLine(" ").FontSize = 12;
                editor.InsertLine(" ").FontSize = 12;
                Telerik.Windows.Documents.Flow.Model.Table tblContent2 = editor.InsertTable();
                tblContent2.StyleId = "TableStyle";
                tblContent2.LayoutType = TableLayoutType.AutoFit;
                Telerik.Windows.Documents.Flow.Model.TableRow curRow2 = tblContent2.Rows.AddTableRow();
                Telerik.Windows.Documents.Flow.Model.TableCell curCell2 = curRow2.Cells.AddTableCell();
                currRun = curCell2.Blocks.AddParagraph().Inlines.AddRun("ΣΥΝΟΛΟ ΟΦΕΙΛΗΣ ΣΕ SDR");
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 15.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                curCell2.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 45);
                curCell2 = curRow2.Cells.AddTableCell();
                currRun = curCell2.Blocks.AddParagraph().Inlines.AddRun(" - ");
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 18.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                curCell2.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 25);
                curCell2 = curRow2.Cells.AddTableCell();
                currRun = curCell2.Blocks.AddParagraph().Inlines.AddRun(" ");
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 14.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                curCell2.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 30);
                curRow2 = tblContent2.Rows.AddTableRow();
                curCell2 = curRow2.Cells.AddTableCell();
                currRun = curCell2.Blocks.AddParagraph().Inlines.AddRun("ΣΥΝΟΛΟ ΟΦΕΙΛΗΣ ΣΕ ΕΥΡΩ");
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 15.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                curCell2.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 45);
                curCell2 = curRow2.Cells.AddTableCell();
                currRun = curCell2.Blocks.AddParagraph().Inlines.AddRun(totalEuroCost.ToString());
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 18.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                curCell2.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 25);
                curCell2 = curRow2.Cells.AddTableCell();
                currRun = curCell2.Blocks.AddParagraph().Inlines.AddRun(" ");
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 14.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                curCell2.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 30);
                curRow2 = tblContent2.Rows.AddTableRow();
                curCell2 = curRow2.Cells.AddTableCell();
                currRun = curCell2.Blocks.AddParagraph().Inlines.AddRun("ΣΥΝΟΛΟ ΑΠΟ ΤΕΛΗ ΔΙΑΧΕΙΡΗΣΗΣ");
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 15.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                curCell2.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 45);
                curCell2 = curRow2.Cells.AddTableCell();
                currRun = curCell2.Blocks.AddParagraph().Inlines.AddRun(totalAdministrativeCost.ToString());
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 18.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                curCell2.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 25);
                curCell2 = curRow2.Cells.AddTableCell();
                currRun = curCell2.Blocks.AddParagraph().Inlines.AddRun(" ");
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 14.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                curCell2.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 30);
                curRow2 = tblContent2.Rows.AddTableRow();
                curCell2 = curRow2.Cells.AddTableCell();
                currRun = curCell2.Blocks.AddParagraph().Inlines.AddRun("ΣΥΝΟΛΟ ΧΡΕΩΣΗΣ ΣΤΟΥΣ ΠΕΛΑΤΕΣ");
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 15.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                curCell2.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 45);
                curCell2 = curRow2.Cells.AddTableCell();
                currRun = curCell2.Blocks.AddParagraph().Inlines.AddRun(totalCost.ToString());
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 18.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                curCell2.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 25);
                curCell2 = curRow2.Cells.AddTableCell();
                currRun = curCell2.Blocks.AddParagraph().Inlines.AddRun(" ");
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 14.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                curCell2.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 30);
                curDoc.UpdateFields();
                exportDOCX(curDoc, dateSpan);
            }
            catch (Exception ex) { }
        }

        protected RadFlowDocument LoadWordTemplate(string uniqueName) {
            RadFlowDocument document2;
            IFormatProvider<RadFlowDocument> fileFormatProvider = new DocxFormatProvider();
            string fileName = Server.MapPath(templatesFolder + pageUniqueName + ".docx");
            using (FileStream input = new FileStream(fileName, FileMode.Open)) {
                document2 = fileFormatProvider.Import(input);
            }
            return document2;
        }

        protected void exportDOCX(RadFlowDocument doc, string dateSpan) {
            IFormatProvider<RadFlowDocument> formatProvider = new DocxFormatProvider();
            byte[] renderedBytes = null;
            using (MemoryStream ms = new MemoryStream()) {
                formatProvider.Export(doc, ms);
                renderedBytes = ms.ToArray();
            }
            Response.ClearHeaders();
            Response.ClearContent();
            Response.AppendHeader("content-disposition", "attachment; filename=Summary_Debt_Table_" + dateSpan.Replace("/", "-").Replace(" ", "_") + ".docx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            Response.BinaryWrite(renderedBytes);
            Response.End();
        }

        protected string toUpperGR(string textToLower) {
            Dictionary<string, string> dicLower = new Dictionary<string, string>();
            dicLower.Add("άι", "αϊ");
            dicLower.Add("έι", "εϊ");
            dicLower.Add("όι", "οϊ");
            dicLower.Add("άυ", "αϋ");
            dicLower.Add("έυ", "εϋ");
            dicLower.Add("όυ", "οϋ");
            dicLower.Add("ά", "α");
            dicLower.Add("έ", "ε");
            dicLower.Add("ή", "η");
            dicLower.Add("ί", "ι");
            dicLower.Add("ό", "ο");
            dicLower.Add("ύ", "υ");
            dicLower.Add("ώ", "ω");
            dicLower.Add("ΐ", "ϊ");
            dicLower.Add("ΰ", "ϋ");
            string finalString = textToLower.ToLowerInvariant();
            foreach (KeyValuePair<string, string> entry in dicLower) {
                finalString = finalString.Replace(entry.Key, entry.Value);
            }
            return finalString.ToUpper();
        }

        protected string getDatesSpan(DateTime? dateStart, DateTime? dateEnd) {
            string date2print = "";
            if (dateStart.HasValue && dateEnd.HasValue) {
                int yearStart = dateStart.GetValueOrDefault().Year;
                int yearEnd = dateEnd.GetValueOrDefault().Year;
                int monthStart = dateStart.GetValueOrDefault().Month;
                int monthEnd = dateEnd.GetValueOrDefault().Month;
                if (yearStart == yearEnd && monthStart == monthEnd) {
                    date2print = dateStart.GetValueOrDefault().Day.ToString() + " - " + dateEnd.GetValueOrDefault().ToShortDateString();
                } else if (yearStart == yearEnd) {
                    date2print = dateStart.GetValueOrDefault().Day.ToString() + "/" + dateStart.GetValueOrDefault().Month.ToString() + " - " + dateEnd.GetValueOrDefault().ToShortDateString();
                } else {
                    date2print = dateStart.GetValueOrDefault().ToShortDateString() + " - " + dateEnd.GetValueOrDefault().ToShortDateString();
                }
            }
            return date2print;
        }

    }

}