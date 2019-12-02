using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI;
using Telerik.Web.UI.Calendar;
using System.Linq;
using System.Linq.Dynamic;
using System.IO;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Windows;
using Telerik.Windows.Documents.Flow.Model;
using Telerik.Windows.Documents.Flow.Model.Editing;
using Telerik.Windows.Documents.Flow.Model.Styles;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Documents.Common.FormatProviders;
using Telerik.Windows.Documents.Flow.FormatProviders.Docx;
using OTERT.Model;
using OTERT.Controller;
using OTERT_Entity;

namespace OTERT.Pages.Administrator {

    public partial class PTStoGR : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle, uploadedFilePath;
        protected int Customer1ID, EventID, CountryID;
        protected int CustomerID, PositionID, LineTypeID;
        const string fileUploadFolder = "~/UploadedFiles/";
        const string templatesFolder = "~/Templates/";
        const string pageUniqueName = "PTS";
        const int OrderTypeID = 1;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Έργα > ΠΤΣ προς Ελλάδα";
                gridMain.MasterTableView.Caption = "Έργα > ΠΤΣ προς Ελλάδα";
                CountryID = -1;
                Session.Remove("CountryID");
                Customer1ID = -1;
                Session.Remove("Customer1ID");
                EventID = -1;
                Session.Remove("EventID");
                CustomerID = -1;
                Session.Remove("CustomerID");
                PositionID = -1;
                Session.Remove("PositionID");
                LineTypeID = -1;
                Session.Remove("LineTypeID");
            }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.MasterTableView.CurrentPageIndex * gridMain.MasterTableView.PageSize;
            int recTake = gridMain.MasterTableView.PageSize;
            try {
                OrdersController cont = new OrdersController();
                gridMain.VirtualItemCount = cont.CountOrders(OrderTypeID);
                gridMain.DataSource = cont.GetOrders(OrderTypeID, recSkip, recTake);
            }
            catch (Exception) { }
        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                if (e.Item is GridDataItem) {
                    GridDataItem item = (GridDataItem)e.Item;
                    ElasticButton img = (ElasticButton)item["btnDelete"].Controls[0];
                    img.ToolTip = "Διαγραφή";
                }
            } else if (e.Item.OwnerTableView.Name == "TasksDetails") {
                if (e.Item is GridDataItem) {
                    GridDataItem item = (GridDataItem)e.Item;
                    ElasticButton img = (ElasticButton)item["btnDelete2"].Controls[0];
                    img.ToolTip = "Διαγραφή";
                }
                if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                    GridEditableItem item = e.Item as GridEditableItem;
                    RadDatePicker dpOrderDate = (RadDatePicker)item["OrderDate"].Controls[0];
                    dpOrderDate.DateInput.Width = Unit.Pixel(250);
                    RadDatePicker dpPaymentDateOrder = (RadDatePicker)item["PaymentDateOrder"].Controls[0];
                    dpPaymentDateOrder.DateInput.Width = Unit.Pixel(250);
                    RadDatePicker dpPaymentDateCalculated = (RadDatePicker)item["PaymentDateCalculated"].Controls[0];
                    dpPaymentDateCalculated.DateInput.Width = Unit.Pixel(250);
                    RadDatePicker dpPaymentDateActual = (RadDatePicker)item["PaymentDateActual"].Controls[0];
                    dpPaymentDateActual.DateInput.Width = Unit.Pixel(250);
                    RadDateTimePicker dpDateTimeStartActual = (RadDateTimePicker)item["DateTimeStartActual"].Controls[0];
                    dpDateTimeStartActual.DateInput.Width = Unit.Pixel(250);
                    dpDateTimeStartActual.DatePopupButton.ToolTip = "Επιλογή πραγματικής ημερομηνίας έναρξης";
                    dpDateTimeStartActual.TimePopupButton.ToolTip = "Επιλογή πραγματικής ώρας έναρξης";
                    dpDateTimeStartActual.DateInput.DateFormat = "dd/MM/yyyy  HH:mm";
                    dpDateTimeStartActual.DateInput.DisplayDateFormat = "dd/MM/yyyy  HH:mm";
                    dpDateTimeStartActual.SharedTimeView.HeaderText = "Επιλέξτε Ώρα";
                    dpDateTimeStartActual.SharedTimeView.TimeFormat = "HH:mm";
                    //dpDateTimeStartActual.SharedTimeView.Interval = new TimeSpan(0, 30, 0);
                    dpDateTimeStartActual.AutoPostBackControl = Telerik.Web.UI.Calendar.AutoPostBackControl.Both;
                    dpDateTimeStartActual.SelectedDateChanged += new SelectedDateChangedEventHandler(dpDate_SelectedIndexChanged);
                    RadDateTimePicker dpDateTimeEndActual = (RadDateTimePicker)item["DateTimeEndActual"].Controls[0];
                    dpDateTimeEndActual.DateInput.Width = Unit.Pixel(250);
                    dpDateTimeEndActual.DatePopupButton.ToolTip = "Επιλογή πραγματικής ημερομηνίας λήξης";
                    dpDateTimeEndActual.TimePopupButton.ToolTip = "Επιλογή πραγματικής ώρας λήξης";
                    dpDateTimeEndActual.DateInput.DateFormat = "dd/MM/yyyy  HH:mm";
                    dpDateTimeEndActual.DateInput.DisplayDateFormat = "dd/MM/yyyy  HH:mm";
                    dpDateTimeEndActual.SharedTimeView.HeaderText = "Επιλέξτε Ώρα";
                    dpDateTimeEndActual.SharedTimeView.TimeFormat = "HH:mm";
                    //dpDateTimeEndActual.SharedTimeView.Interval = new TimeSpan(0, 30, 0);
                    dpDateTimeEndActual.AutoPostBackControl = Telerik.Web.UI.Calendar.AutoPostBackControl.Both;
                    dpDateTimeEndActual.SelectedDateChanged += new SelectedDateChangedEventHandler(dpDate_SelectedIndexChanged);
                    CheckBox cbInternet = (CheckBox)item["Internet"].Controls[0];
                    cbInternet.AutoPostBack = true;
                    cbInternet.CheckedChanged += new EventHandler(cbInternet_CheckedChanged);
                    CheckBox cbMSN = (CheckBox)item["MSN"].Controls[0];
                    cbMSN.AutoPostBack = true;
                    cbMSN.CheckedChanged += new EventHandler(cbMSN_CheckedChanged);
                }
            } else if (e.Item.OwnerTableView.Name == "AttachedFiles") {
                if (e.Item is GridDataItem) {
                    GridDataItem item = (GridDataItem)e.Item;
                    ElasticButton img = (ElasticButton)item["btnDeleteFile"].Controls[0];
                    img.ToolTip = "Διαγραφή";
                }
            }
        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                    CountryID = -1;
                    Session.Remove("CountryID");
                    Customer1ID = -1;
                    Session.Remove("Customer1ID");
                    EventID = -1;
                    Session.Remove("EventID");
                    GridEditableItem item = e.Item as GridEditableItem;
                    RadDropDownList ddlCountry = item.FindControl("ddlCountry") as RadDropDownList;
                    RadDropDownList ddlCustomer1 = item.FindControl("ddlCustomer1") as RadDropDownList;
                    RadDropDownList ddlEvent = item.FindControl("ddlEvent") as RadDropDownList;
                    try {
                        OrderB currOrder = e.Item.DataItem as OrderB;
                        CountriesController ccont = new CountriesController();
                        ddlCountry.DataSource = ccont.GetCountries();
                        ddlCountry.DataTextField = "NameGR";
                        ddlCountry.DataValueField = "ID";
                        ddlCountry.DataBind();
                        ddlCountry.Items.Insert(0, new DropDownListItem("Όλες οι Χώρες ...", "0"));
                        CustomersController custcont = new CustomersController();
                        ddlCustomer1.DataSource = custcont.GetProviders();
                        ddlCustomer1.DataTextField = "NameGR";
                        ddlCustomer1.DataValueField = "ID";
                        ddlCustomer1.DataBind();
                        EventsController econt = new EventsController();
                        ddlEvent.DataSource = econt.GetEvents();
                        ddlEvent.DataTextField = "NameGR";
                        ddlEvent.DataValueField = "ID";
                        ddlEvent.DataBind();
                        if (currOrder != null) {
                            if (currOrder.Event != null) {
                                ddlCountry.SelectedIndex = ddlCountry.FindItemByValue(currOrder.Event.Place.CountryID.ToString()).Index;
                                Session["CountryID"] = currOrder.Event.Place.CountryID;
                            } else {
                                ddlCountry.SelectedIndex = 0;
                                Session["CountryID"] = ddlCountry.SelectedItem.Value;
                            }
                            ddlCustomer1.SelectedIndex = ddlCustomer1.FindItemByValue(currOrder.Customer1ID.ToString()).Index;
                            Session["Customer1ID"] = currOrder.Customer1ID;
                            ddlEvent.SelectedIndex = ddlEvent.FindItemByValue(currOrder.EventID.ToString()).Index;
                            Session["EventID"] = currOrder.EventID;
                        } else {
                            ddlCountry.SelectedIndex = 0;
                            Session["CountryID"] = ddlCountry.SelectedItem.Value;
                            ddlCustomer1.SelectedIndex = 0;
                            Session["Customer1ID"] = ddlCustomer1.SelectedItem.Value;
                            ddlEvent.SelectedIndex = 0;
                            Session["EventID"] = ddlEvent.SelectedItem.Value;
                        }
                    }
                    catch (Exception) { }
                }
            } else if (e.Item.OwnerTableView.Name == "TasksDetails") {
                if (e.Item is GridEditableItem && e.Item.IsInEditMode) {
                    CustomerID = -1;
                    Session.Remove("CustomerID");
                    PositionID = -1;
                    Session.Remove("PositionID");
                    LineTypeID = -1;
                    Session.Remove("LineTypeID");
                    GridDataItem parentItem = e.Item.OwnerTableView.ParentItem;
                    int orderID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
                    OrdersController ocon = new OrdersController();
                    OrderB curOrder = ocon.GetOrder(orderID);
                    GridEditableItem item = e.Item as GridEditableItem;
                    RadDropDownList ddlCustomers = item.FindControl("ddlCustomers") as RadDropDownList;
                    RadDropDownList ddlRequestedPosition = item.FindControl("ddlRequestedPosition") as RadDropDownList;
                    RadDropDownList ddlLineType = item.FindControl("ddlLineType") as RadDropDownList;
                    TextBox txtRegNo = item["RegNo"].Controls[0] as TextBox;
                    try {
                        txtRegNo.Text = curOrder.RegNo;
                        TaskB currTask = e.Item.DataItem as TaskB;
                        CustomersController ccont = new CustomersController();
                        ddlCustomers.DataSource = ccont.GetCustomers();
                        ddlCustomers.DataTextField = "NameGR";
                        ddlCustomers.DataValueField = "ID";
                        ddlCustomers.DataBind();
                        RequestedPositionsController rcont = new RequestedPositionsController();
                        ddlRequestedPosition.DataSource = rcont.GetRequestedPositions();
                        ddlRequestedPosition.DataTextField = "NameGR";
                        ddlRequestedPosition.DataValueField = "ID";
                        ddlRequestedPosition.DataBind();
                        LineTypesController ltcont = new LineTypesController();
                        ddlLineType.DataSource = ltcont.GetLineTypes();
                        ddlLineType.DataTextField = "Name";
                        ddlLineType.DataValueField = "ID";
                        ddlLineType.DataBind();
                        if (currTask != null) {
                            ddlCustomers.SelectedIndex = ddlCustomers.FindItemByValue(currTask.CustomerID.ToString()).Index;
                            Session["CustomerID"] = currTask.CustomerID;
                            if (currTask.RequestedPositionID != null) {
                                ddlRequestedPosition.SelectedIndex = ddlRequestedPosition.FindItemByValue(currTask.RequestedPositionID.ToString()).Index;
                                Session["PositionID"] = currTask.RequestedPositionID;
                            } else {
                                ddlRequestedPosition.SelectedIndex = 0;
                                Session["PositionID"] = ddlRequestedPosition.SelectedItem.Value;
                            }
                            if (currTask.LineTypeID != null) {
                                ddlLineType.SelectedIndex = ddlLineType.FindItemByValue(currTask.LineTypeID.ToString()).Index;
                                Session["LineTypeID"] = currTask.LineTypeID;
                            } else {
                                ddlRequestedPosition.SelectedIndex = 0;
                                Session["LineTypeID"] = ddlRequestedPosition.SelectedItem.Value;
                            }
                        } else {
                            ddlCustomers.SelectedIndex = 0;
                            Session["CustomerID"] = ddlCustomers.SelectedItem.Value;
                            ddlRequestedPosition.SelectedIndex = 0;
                            Session["PositionID"] = ddlRequestedPosition.SelectedItem.Value;
                            ddlLineType.SelectedIndex = 0;
                            Session["LineTypeID"] = ddlLineType.SelectedItem.Value;
                        }
                    }
                    catch (Exception) { }
                }
            }
        }

        protected void gridMain_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e) {
            if (e.DetailTableView.Name == "TasksDetails") {
                GridTableView detailtabl = (GridTableView)e.DetailTableView;
                int recSkip = detailtabl.CurrentPageIndex * gridMain.PageSize;
                int recTake = detailtabl.PageSize;
                GridDataItem parentItem = (GridDataItem)detailtabl.ParentItem;
                int orderID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
                TasksController cont = new TasksController();
                detailtabl.DataSource = cont.GetTasksForOrder(orderID);
            } else if (e.DetailTableView.Name == "AttachedFiles") {
                GridTableView detailtabl = e.DetailTableView;
                int recSkip = detailtabl.CurrentPageIndex * gridMain.PageSize;
                int recTake = detailtabl.PageSize;
                GridDataItem parentItem = detailtabl.ParentItem;
                int orderID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
                FilesController cont = new FilesController();
                detailtabl.VirtualItemCount = cont.CountFilesByOrderID(orderID);
                detailtabl.DataSource = cont.GetFilesByOrderID(orderID, recSkip, recTake);
            }
        }

        private void ShowErrorMessage(int errCode) {
            switch (errCode) {
                case 1:
                    RadWindowManager1.RadAlert("Η συγκεκριμένη Παραγγελία σχετίζεται με κάποιο Έργο και δεν μπορεί να διαγραφεί!", 400, 200, "Σφάλμα", "");
                    break;
                case 2:
                    RadWindowManager1.RadAlert("Η συγκεκριμένη Παραγγελία είναι κλειδωμένη και δεν μπορεί να διαγραφεί!", 400, 200, "Σφάλμα", "");
                    break;
                default:
                    RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
                    break;
            }
        }

        protected void gridMain_ItemCommand(object sender, GridCommandEventArgs e) {
            if (e.CommandName == "invPrint") {
                GridDataItem item = (GridDataItem)e.Item;
                int orderID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                OrdersController oCont = new OrdersController();
                OrderB curOrder = oCont.GetOrder(orderID);
                TasksController lCont = new TasksController();
                List<TaskB> tasksForOrder = lCont.GetTasksForOrder(orderID);
                try {
                    DocumentReplacemetsController cont = new DocumentReplacemetsController();
                    List<DocumentReplacemetB> reps = new List<DocumentReplacemetB>();
                    reps = cont.GetDocumentReplacemets(pageUniqueName);
                    string imgFolderPath = Server.MapPath(fileUploadFolder);
                    DocumentReplacemetB curRep;
                    BookmarkRangeStart bookmarkRangeStart;
                    RadFlowDocument curDoc = LoadSampleDocument(pageUniqueName);

                    RadFlowDocumentEditor editor = new RadFlowDocumentEditor(curDoc);
                    //System.Text.RegularExpressions.Regex textRegex = new System.Text.RegularExpressions.Regex("ΣΑΟΥΣΟΠΟΥΛΟΥ ΑΝΝΑ");
                    //editor.ReplaceText("ΣΑΟΥΣΟΠΟΥΛΟΥ ΑΝΝΑ", txtNew.Text, true, true);
                    List<BookmarkRangeStart> test = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().ToList();
                    Telerik.Windows.Documents.Flow.Model.TableCell currCell;
                    Run currRun;

                    Header defaultHeader = editor.Document.Sections.First().Headers.Default;
                    Footer defaultFooter = editor.Document.Sections.First().Footers.Default;
                    //Telerik.Windows.Documents.Flow.Model.Table headerTable = defaultHeader.Blocks.OfType<Telerik.Windows.Documents.Flow.Model.Table>().First();
                    //Telerik.Windows.Documents.Flow.Model.TableCell firstCell = headerTable.Rows[0].Cells[0];

                    Telerik.Windows.Documents.Flow.Model.Styles.Style tableStyle = new Telerik.Windows.Documents.Flow.Model.Styles.Style("TableStyle", StyleType.Table);
                    tableStyle.Name = "Table Style";
                    tableStyle.TableProperties.Borders.LocalValue = new TableBorders(new Border(1, Telerik.Windows.Documents.Flow.Model.Styles.BorderStyle.Single, new ThemableColor(System.Windows.Media.Colors.Black)));
                    tableStyle.TableProperties.Alignment.LocalValue = Alignment.Left;
                    tableStyle.TableCellProperties.VerticalAlignment.LocalValue = VerticalAlignment.Center;
                    tableStyle.TableCellProperties.PreferredWidth.LocalValue = new TableWidthUnit(TableWidthUnitType.Percent, 100);
                    tableStyle.TableCellProperties.Padding.LocalValue = new Telerik.Windows.Documents.Primitives.Padding(8);
                    editor.Document.StyleRepository.Add(tableStyle);
                    /*
                    curRep = reps.Find(o => o.UniqueName == "KET_Header_OTELogo");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    using (Stream firstImage = File.OpenRead(imgFolderPath + curRep.Text)) {
                        var inImage = ((Paragraph)currCell.Blocks.First()).Inlines.AddImageInline();
                        inImage.Image.ImageSource = new Telerik.Windows.Documents.Media.ImageSource(firstImage, curRep.Text.Split('.').Last());
                        if (curRep.ImageHeight != null && curRep.ImageWidth != null) {
                            inImage.Image.Height = curRep.ImageHeight.Value;
                            inImage.Image.Width = curRep.ImageWidth.Value;
                        }
                    }

                    curRep = reps.Find(o => o.UniqueName == "KET_Header_OTEMoto");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                    currRun.Text = curRep.Text;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 13.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    curRep = reps.Find(o => o.UniqueName == "KET_Header_Title");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                    currRun.Text = curRep.Text;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 15.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    curRep = reps.Find(o => o.UniqueName == "KET_Header_Department");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    string[] arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                    Paragraph newPar = (Paragraph)currCell.Blocks.First();
                    newPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText.Length; i++) {
                        currRun = editor.InsertLine(arrText[i]);
                        currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Center;
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Times New Roman");
                        currRun.Properties.FontSize.LocalValue = 15.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                        currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    }
                    currCell.Blocks.Remove(currCell.Blocks.Last());

                    curRep = reps.Find(o => o.UniqueName == "KET_Header_EDEPPOI");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                    currRun.Text = curRep.Text;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 15.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    curRep = reps.Find(o => o.UniqueName == "KET_Header_PageNo");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    editor.MoveToParagraphStart((Paragraph)currCell.Blocks.First());
                    editor.InsertText("ΣΕΛΙΔΑ ");
                    editor.InsertField("PAGE", "3");
                    if (curRep.Text == "Σελίδα Χ από Υ") {
                        editor.InsertText(" ΑΠΟ ");
                        editor.InsertField("NUMPAGES", "5");
                    }

                    curRep = reps.Find(o => o.UniqueName == "KET_Header_Date");
                    bookmarkRangeStart = defaultHeader.EnumerateChildrenOfType<BookmarkRangeStart>().Where(rangeStart => rangeStart.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault();
                    editor.MoveToInlineEnd(bookmarkRangeStart);
                    editor.InsertText(DateTime.Now.ToString(curRep.Text, new System.Globalization.CultureInfo("el-GR")));

                    curRep = reps.Find(o => o.UniqueName == "KET_Header_To");
                    bookmarkRangeStart = defaultHeader.EnumerateChildrenOfType<BookmarkRangeStart>().Where(rangeStart => rangeStart.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault();
                    editor.MoveToInlineEnd(bookmarkRangeStart);
                    editor.InsertText(curRep.Text);

                    curRep = reps.Find(o => o.UniqueName == "KET_Footer_OTELogo");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    using (Stream firstImage = File.OpenRead(imgFolderPath + curRep.Text)) {
                        var inImage = ((Paragraph)currCell.Blocks.First()).Inlines.AddImageInline();
                        inImage.Image.ImageSource = new Telerik.Windows.Documents.Media.ImageSource(firstImage, curRep.Text.Split('.').Last());
                        if (curRep.ImageHeight != null && curRep.ImageWidth != null) {
                            inImage.Image.Height = curRep.ImageHeight.Value;
                            inImage.Image.Width = curRep.ImageWidth.Value;
                        }
                    }

                    curRep = reps.Find(o => o.UniqueName == "KET_Footer_ELOT");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    using (Stream firstImage = File.OpenRead(imgFolderPath + curRep.Text)) {
                        var inImage = ((Paragraph)currCell.Blocks.First()).Inlines.AddImageInline();
                        inImage.Image.ImageSource = new Telerik.Windows.Documents.Media.ImageSource(firstImage, curRep.Text.Split('.').Last());
                        if (curRep.ImageHeight != null && curRep.ImageWidth != null) {
                            inImage.Image.Height = curRep.ImageHeight.Value;
                            inImage.Image.Width = curRep.ImageWidth.Value;
                        }
                    }
                    */

                    bookmarkRangeStart = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().Where(rangeStart => rangeStart.Bookmark.Name == "Body_Main").FirstOrDefault();
                    editor.MoveToInlineEnd(bookmarkRangeStart);
                    Telerik.Windows.Documents.Flow.Model.Table tblContent = editor.InsertTable();
                    tblContent.StyleId = "TableStyle";
                    tblContent.LayoutType = TableLayoutType.AutoFit;
                    ThemableColor cellBackground = new ThemableColor(System.Windows.Media.Colors.Beige);
                    for (int i = 0; i < tasksForOrder.Count + 1; i++) {
                        Telerik.Windows.Documents.Flow.Model.TableRow row1 = tblContent.Rows.AddTableRow();
                        for (int j = 0; j < 7; j++) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell = row1.Cells.AddTableCell();
                            if (i == 0) {
                                if (j == 0) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("Τ.Ε.Κ.");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 15.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                    //cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 5);
                                } else if (j == 1) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("ΤΛΦ ΧΡΕΩΣΗΣ");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 15.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                    //cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 20);
                                } else if (j == 2) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("ΕΠΩΝΥΜΙΑ ΠΕΛΑΤΗ");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 15.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                    //cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 20);
                                } else if (j == 3) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("ΗΜ/ΝΙΑ");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 15.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                    //cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 20);
                                } else if (j == 4) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("Κ.Τ. 6630(€)");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 15.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                    //cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 35);
                                } else if (j == 5) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("Κ.Τ. 6951(€)");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 15.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                    //cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 35);
                                } else if (j == 6) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("SDR");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 15.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                    //cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 35);
                                }
                            } else {
                                int k = i - 1;
                                if (j == 0) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("");
                                } else if (j == 1) {
                                    cell.Blocks.AddParagraph().Inlines.AddRun(tasksForOrder[k].Customer.ChargeTelephone);
                                } else if (j == 2) {
                                    cell.Blocks.AddParagraph().Inlines.AddRun(tasksForOrder[k].Customer.NameGR);
                                } else if (j == 3) {
                                    cell.Blocks.AddParagraph().Inlines.AddRun(tasksForOrder[k].DateTimeStartOrder != null ? tasksForOrder[k].DateTimeStartOrder.Value.ToShortDateString() : "");
                                } else if (j == 4) {
                                    cell.Blocks.AddParagraph().Inlines.AddRun(tasksForOrder[k].CostActual.ToString());
                                } else if (j == 5) {
                                    cell.Blocks.AddParagraph().Inlines.AddRun();
                                } else if (j == 6) {
                                    cell.Blocks.AddParagraph().Inlines.AddRun();
                                }
                            }
                        }
                    }
                    for (int i = 0; i < 2; i++) {
                        Telerik.Windows.Documents.Flow.Model.TableRow row2 = tblContent.Rows.AddTableRow();
                        for (int j = 0; j < 7; j++) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell = row2.Cells.AddTableCell();
                            currRun = cell.Blocks.AddParagraph().Inlines.AddRun("");
                        }
                    }
                    Telerik.Windows.Documents.Flow.Model.TableRow row = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 7; j++) {
                        Telerik.Windows.Documents.Flow.Model.TableCell cell = row.Cells.AddTableCell();
                        if (j == 0) {
                            currRun = cell.Blocks.AddParagraph().Inlines.AddRun("");
                        } else if (j == 1) {
                            currRun = cell.Blocks.AddParagraph().Inlines.AddRun("");
                        } else if (j == 2) {
                            currRun = cell.Blocks.AddParagraph().Inlines.AddRun("");
                        } else if (j == 3) {
                            currRun = cell.Blocks.AddParagraph().Inlines.AddRun("ΣΥΝΟΛΟ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 15.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 4) {
                            decimal totCost = 0;
                            for (int tr=0; tr< tasksForOrder.Count(); tr++) {
                                totCost += (tasksForOrder[tr].CostActual != null ? tasksForOrder[tr].CostActual.Value : 0); 
                            }
                            currRun = cell.Blocks.AddParagraph().Inlines.AddRun(totCost.ToString());
                        } else if (j == 5) {
                            currRun = cell.Blocks.AddParagraph().Inlines.AddRun("");
                        } else if (j == 6) {
                            currRun = cell.Blocks.AddParagraph().Inlines.AddRun("");
                        }
                    }

                    curDoc.UpdateFields();
                    exportDOCX(curDoc);
                }
                catch (Exception ex) { }
            }
        }

        protected void gridMain_UpdateCommand(object source, GridCommandEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                var editableItem = ((GridEditableItem)e.Item);
                var ID = (int)editableItem.GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curOrder = dbContext.Orders.Where(n => n.ID == ID).FirstOrDefault();
                    if (curOrder != null) {
                        editableItem.UpdateValues(curOrder);
                        if (Session["Customer1ID"] != null) { Customer1ID = int.Parse(Session["Customer1ID"].ToString()); }
                        if (Customer1ID > 0) {
                            curOrder.Customer1ID = Customer1ID;
                            Customer1ID = -1;
                            Session.Remove("Customer1ID");
                        }
                        if (Session["EventID"] != null) { EventID = int.Parse(Session["EventID"].ToString()); }
                        if (EventID > 0) {
                            curOrder.EventID = EventID;
                            EventID = -1;
                            Session.Remove("EventID");
                        }
                        CountryID = -1;
                        Session.Remove("CountryID");
                        try { dbContext.SaveChanges(); }
                        catch (Exception) { ShowErrorMessage(-1); }
                    }
                }
            } else if (e.Item.OwnerTableView.Name == "TasksDetails") {
                var editableItem = ((GridEditableItem)e.Item);
                var ID = (int)editableItem.GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curTask = dbContext.Tasks.Where(n => n.ID == ID).FirstOrDefault();
                    if (curTask != null) {
                        try {
                            editableItem.UpdateValues(curTask);
                            if (Session["CustomerID"] != null) { CustomerID = int.Parse(Session["CustomerID"].ToString()); }
                            if (CustomerID > 0) { curTask.CustomerID = CustomerID; }
                            if (Session["PositionID"] != null) { PositionID = int.Parse(Session["PositionID"].ToString()); }
                            if (PositionID > 0) { curTask.RequestedPositionID = PositionID; }
                            if (Session["LineTypeID"] != null) { LineTypeID = int.Parse(Session["LineTypeID"].ToString()); }
                            if (LineTypeID > 0) { curTask.LineTypeID = LineTypeID; }
                            dbContext.SaveChanges();
                        }
                        catch (Exception) { ShowErrorMessage(-1); }
                        finally {
                            CustomerID = -1;
                            Session.Remove("CustomerID");
                            PositionID = -1;
                            Session.Remove("PositionID");
                            LineTypeID = -1;
                            Session.Remove("LineTypeID");
                        }
                    }
                }
            }
        }

        protected void gridMain_InsertCommand(object source, GridCommandEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                var editableItem = ((GridEditableItem)e.Item);
                using (var dbContext = new OTERTConnStr()) {
                    var curOrder = new Orders();
                    Hashtable values = new Hashtable();
                    editableItem.ExtractValues(values);
                    if (Session["Customer1ID"] != null) { Customer1ID = int.Parse(Session["Customer1ID"].ToString()); }
                    if (Session["EventID"] != null) { EventID = int.Parse(Session["EventID"].ToString()); }
                    if (Customer1ID > 0 && EventID > 0) {
                        try {
                            curOrder.RegNo = (string)values["RegNo"];
                            curOrder.OrderTypeID = OrderTypeID;
                            curOrder.Customer1ID = Customer1ID;
                            curOrder.EventID = EventID;
                            curOrder.IsLocked = (bool)values["IsLocked"];
                            dbContext.Orders.Add(curOrder);
                            dbContext.SaveChanges();
                        }
                        catch (Exception) { ShowErrorMessage(-1); }
                        finally {
                            CountryID = -1;
                            Session.Remove("CountryID");
                            Customer1ID = -1;
                            Session.Remove("Customer1ID");
                            EventID = -1;
                            Session.Remove("EventID");
                        }
                    } else { ShowErrorMessage(-1); }
                }
            } else if (e.Item.OwnerTableView.Name == "TasksDetails") {
                JobsController jc = new JobsController();
                JobB tmpJob = jc.GetJobsForPageID(1)[0];
                DistancesController dc = new DistancesController();
                DistanceB tmpDistance = dc.GetDistancesForPageID(1)[0];
                GridTableView detailtabl = e.Item.OwnerTableView;
                GridDataItem parentItem = detailtabl.ParentItem;
                int orderID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
                var editableItem = ((GridEditableItem)e.Item);
                using (var dbContext = new OTERTConnStr()) {
                    var curTask = new Tasks();
                    Hashtable values = new Hashtable();
                    editableItem.ExtractValues(values);
                    if (Session["CustomerID"] != null) { CustomerID = int.Parse(Session["CustomerID"].ToString()); }
                    if (Session["PositionID"] != null) { PositionID = int.Parse(Session["PositionID"].ToString()); }
                    if (Session["LineTypeID"] != null) { LineTypeID = int.Parse(Session["LineTypeID"].ToString()); }
                    if (CustomerID > 0 && PositionID > 0 && LineTypeID > 0) {
                        try {
                            curTask.OrderID = orderID;
                            curTask.RegNo = (string)values["RegNo"];
                            curTask.OrderDate = DateTime.Parse((string)values["OrderDate"]);
                            curTask.CustomerID = CustomerID;
                            curTask.RequestedPositionID = PositionID;
                            curTask.JobID = tmpJob.ID;
                            curTask.DistancesID = tmpDistance.ID;
                            if (values["DateTimeStartActual"] != null) { curTask.DateTimeStartOrder = DateTime.Parse((string)values["DateTimeStartActual"]); } else { curTask.DateTimeStartOrder = null; }
                            if (values["DateTimeEndActual"] != null) { curTask.DateTimeEndOrder = DateTime.Parse((string)values["DateTimeEndActual"]); } else { curTask.DateTimeEndOrder = null; }
                            if (values["DateTimeDurationActual"] != null) { curTask.DateTimeDurationOrder = int.Parse((string)values["DateTimeDurationActual"]); } else { curTask.DateTimeDurationOrder = 0; }
                            if (values["DateTimeStartActual"] != null) { curTask.DateTimeStartActual = DateTime.Parse((string)values["DateTimeStartActual"]); } else { curTask.DateTimeStartActual = null; }
                            if (values["DateTimeEndActual"] != null) { curTask.DateTimeEndActual = DateTime.Parse((string)values["DateTimeEndActual"]); } else { curTask.DateTimeEndActual = null; }
                            if (values["DateTimeDurationActual"] != null) { curTask.DateTimeDurationActual = int.Parse((string)values["DateTimeDurationActual"]); } else { curTask.DateTimeDurationActual = null; }
                            if (values["CostCalculated"] != null) { curTask.CostCalculated = decimal.Parse((string)values["CostCalculated"]); } else { curTask.CostCalculated = null; }
                            curTask.InstallationCharges = false;
                            curTask.MonthlyCharges = false;
                            curTask.CallCharges = 0;
                            curTask.TelephoneNumber = (string)values["TelephoneNumber"];
                            curTask.TechnicalSupport = 0;
                            if (values["AddedCharges"] != null) { curTask.AddedCharges = decimal.Parse((string)values["AddedCharges"]); } else { curTask.AddedCharges = null; }
                            if (values["CostActual"] != null) { curTask.CostActual = decimal.Parse((string)values["CostActual"]); } else { curTask.CostActual = null; }
                            if (values["PaymentDateOrder"] != null) { curTask.PaymentDateOrder = DateTime.Parse((string)values["PaymentDateOrder"]); } else { curTask.PaymentDateOrder = null; }
                            if (values["PaymentDateCalculated"] != null) { curTask.PaymentDateCalculated = DateTime.Parse((string)values["PaymentDateCalculated"]); } else { curTask.PaymentDateCalculated = null; }
                            if (values["PaymentDateActual"] != null) { curTask.PaymentDateActual = DateTime.Parse((string)values["PaymentDateActual"]); } else { curTask.PaymentDateActual = null; }
                            curTask.IsForHelpers = null;
                            curTask.IsLocked = (bool)values["IsLocked"];
                            curTask.IsCanceled = (bool)values["IsCanceled"];
                            curTask.CancelPrice = 0;
                            curTask.Comments = (string)values["Comments"];
                            curTask.InvoceComments = (string)values["InvoceComments"];
                            curTask.SateliteID = null;
                            curTask.Internet = (bool)values["Internet"];
                            curTask.MSN = (bool)values["MSN"];
                            curTask.LineTypeID = LineTypeID;
                            dbContext.Tasks.Add(curTask);
                            dbContext.SaveChanges();
                        }
                        catch (Exception) { ShowErrorMessage(-1); }
                        finally {
                            CustomerID = -1;
                            Session.Remove("CustomerID");
                            PositionID = -1;
                            Session.Remove("PositionID");
                            LineTypeID = -1;
                            Session.Remove("LineTypeID");
                        }
                    } else { ShowErrorMessage(-1); }
                }
            } else if (e.Item.OwnerTableView.Name == "AttachedFiles") {
                GridTableView detailtabl = e.Item.OwnerTableView;
                GridDataItem parentItem = detailtabl.ParentItem;
                int orderID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
                var editableItem = ((GridEditableItem)e.Item);
                using (var dbContext = new OTERTConnStr()) {
                    var curFile = new Files();
                    Hashtable values = new Hashtable();
                    editableItem.ExtractValues(values);
                    curFile.OrderID = orderID;
                    curFile.FileName = (string)values["FileName"];
                    curFile.FilePath = uploadedFilePath;
                    curFile.DateStamp = DateTime.Now;
                    dbContext.Files.Add(curFile);
                    try { dbContext.SaveChanges(); }
                    catch (Exception) { ShowErrorMessage(-1); }
                }
            }
        }

        protected void gridMain_DeleteCommand(object source, GridCommandEventArgs e) {
            if (e.Item.OwnerTableView.Name == "Master") {
                var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curOrder = dbContext.Orders.Where(n => n.ID == ID).FirstOrDefault();
                    if (curOrder != null) {
                        if (curOrder.IsLocked != true) {
                            List<Files> curFiles = dbContext.Files.Where(k => k.OrderID == ID).ToList();
                            foreach (Files curFile in curFiles) {
                                string FileToDelete = Server.MapPath(curFile.FilePath);
                                if (System.IO.File.Exists(FileToDelete)) { System.IO.File.Delete(FileToDelete); }
                                dbContext.Files.Remove(curFile);
                                try { dbContext.SaveChanges(); }
                                catch (Exception) { ShowErrorMessage(-1); }
                            }
                            TasksController tc = new TasksController();
                            List<Tasks> tasksForOrder = dbContext.Tasks.Where(k => k.OrderID == curOrder.ID).ToList();
                            foreach (Tasks curTask in tasksForOrder) {
                                dbContext.Tasks.Remove(curTask);
                                try { dbContext.SaveChanges(); }
                                catch (Exception) { ShowErrorMessage(-1); }
                            }
                            dbContext.Orders.Remove(curOrder);
                            try { dbContext.SaveChanges(); }
                            catch (Exception ex) {
                                string err = ex.InnerException.InnerException.Message;
                                int errCode = -1;
                                if (err.StartsWith("The DELETE statement conflicted with the REFERENCE constraint")) { errCode = 1; }
                                ShowErrorMessage(errCode);
                            }
                        } else { ShowErrorMessage(2); }
                    }
                }
            } else if (e.Item.OwnerTableView.Name == "TasksDetails") {
                var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curTask = dbContext.Tasks.Where(n => n.ID == ID).FirstOrDefault();
                    if (curTask != null) {
                        if (curTask.IsLocked != true) {
                            dbContext.Tasks.Remove(curTask);
                            try { dbContext.SaveChanges(); }
                            catch (Exception ex) {
                                string err = ex.InnerException.InnerException.Message;
                                int errCode = -1;
                                if (err.StartsWith("The DELETE statement conflicted with the REFERENCE constraint")) { errCode = 1; }
                                ShowErrorMessage(errCode);
                            }
                        } else { ShowErrorMessage(2); }
                    }
                }
            } else if (e.Item.OwnerTableView.Name == "AttachedFiles") {
                var ID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var curFile = dbContext.Files.Where(n => n.ID == ID).FirstOrDefault();
                    if (curFile != null) {
                        string FileToDelete = Server.MapPath(curFile.FilePath);
                        if (System.IO.File.Exists(FileToDelete)) { System.IO.File.Delete(FileToDelete); }
                        dbContext.Files.Remove(curFile);
                        try { dbContext.SaveChanges(); }
                        catch (Exception) { ShowErrorMessage(-1); }
                    }
                }
            }
        }

        protected void ddlCustomer1_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                Customer1ID = int.Parse(e.Value);
                Session["Customer1ID"] = Customer1ID;
            }
            catch (Exception) { }
        }

        protected void ddlEvent_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                EventID = int.Parse(e.Value);
                Session["EventID"] = EventID;
            }
            catch (Exception) { }
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                CountryID = int.Parse(e.Value);
                Session["CountryID"] = CountryID;
                if (CountryID > 0) {
                    RadDropDownList ddlCountries = (RadDropDownList)sender;
                    GridEditableItem item = (GridEditableItem)ddlCountries.NamingContainer;
                    RadDropDownList ddlEvent = (RadDropDownList)item.FindControl("ddlEvent");
                    ddlEvent.ClearSelection();
                    EventsController econt = new EventsController();
                    ddlEvent.DataSource = econt.GetEventsForCountry(CountryID);
                    ddlEvent.DataTextField = "NameGR";
                    ddlEvent.DataValueField = "ID";
                    ddlEvent.DataBind();
                    ddlEvent.SelectedIndex = 0;
                    if (ddlEvent.Items.Count > 0) { Session["EventID"] = ddlEvent.SelectedItem.Value; } else { Session.Remove("EventID"); }
                } else {
                    RadDropDownList ddlCountries = (RadDropDownList)sender;
                    GridEditableItem item = (GridEditableItem)ddlCountries.NamingContainer;
                    RadDropDownList ddlEvent = (RadDropDownList)item.FindControl("ddlEvent");
                    ddlEvent.ClearSelection();
                    EventsController econt = new EventsController();
                    ddlEvent.DataSource = econt.GetEvents();
                    ddlEvent.DataTextField = "NameGR";
                    ddlEvent.DataValueField = "ID";
                    ddlEvent.DataBind();
                    ddlEvent.SelectedIndex = 0;
                    Session["EventID"] = ddlEvent.SelectedItem.Value;
                }
            }
            catch (Exception) { }
        }

        protected void ddlCustomers_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                CustomerID = int.Parse(e.Value);
                Session["CustomerID"] = CustomerID;
            }
            catch (Exception) { }
        }

        protected void ddlRequestedPosition_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                PositionID = int.Parse(e.Value);
                Session["PositionID"] = PositionID;
            }
            catch (Exception) { }
        }

        protected void ddlLineType_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                LineTypeID = int.Parse(e.Value);
                Session["LineTypeID"] = LineTypeID;
                RadDropDownList ddlLineType = (RadDropDownList)sender;
                GridEditableItem eitem = (GridEditableItem)ddlLineType.NamingContainer;
                calculateCosts(eitem);
            }
            catch (Exception) { }
        }

        protected void txtAddedCharges_TextChanged(object sender, EventArgs e) {
            TextBox txtAddedCharges = ((TextBox)(sender));
            GridEditableItem eitem = (GridEditableItem)txtAddedCharges.NamingContainer;
            calculateCosts(eitem);
        }

        protected void dpDate_SelectedIndexChanged(object sender, SelectedDateChangedEventArgs e) {
            RadDatePicker dpStartDate = (RadDatePicker)sender;
            GridEditableItem eitem = (GridEditableItem)dpStartDate.NamingContainer;
            calculateCosts(eitem);
        }

        protected void cbInternet_CheckedChanged(object sender, EventArgs e) {
            CheckBox cbInternet = (CheckBox)sender;
            GridEditableItem eitem = (GridEditableItem)cbInternet.NamingContainer;
            calculateCosts(eitem);
        }

        protected void cbMSN_CheckedChanged(object sender, EventArgs e) {
            CheckBox cbMSN = (CheckBox)sender;
            GridEditableItem eitem = (GridEditableItem)cbMSN.NamingContainer;
            calculateCosts(eitem);
        }

        protected void calculateCosts(GridEditableItem eitem) {
            GridDataItem parentItem = eitem.OwnerTableView.ParentItem;
            int orderID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
            OrdersController ocon = new OrdersController();
            OrderB curOrder = ocon.GetOrder(orderID);
            DateTime nullDate = new DateTime(1900, 1, 1);
            RadDatePicker dpActualStartDate = (RadDatePicker)eitem["DateTimeStartActual"].Controls[0]; ;
            DateTime actualStartDate = dpActualStartDate.SelectedDate ?? nullDate;
            RadDatePicker dpActualEndDate = (RadDatePicker)eitem["DateTimeEndActual"].Controls[0];
            DateTime actualEndDate = dpActualEndDate.SelectedDate ?? nullDate;
            RadDropDownList ddlLineType = (RadDropDownList)eitem.FindControl("ddlLineType");
            CheckBox cbInternet = (CheckBox)eitem["Internet"].Controls[0];
            CheckBox cbMSN = (CheckBox)eitem["MSN"].Controls[0];
            int LineTypeID = int.Parse(ddlLineType.SelectedItem.Value);
            CountryPricelistsController cont = new CountryPricelistsController();
            CountryPricelistB plist = cont.GetCountryPricelist(curOrder.Customer1ID, LineTypeID);
            TextBox txtActualDuration = (TextBox)eitem["DateTimeDurationActual"].Controls[0];
            TextBox txtAddedCharges = (TextBox)eitem.FindControl("txtAddedCharges");
            TextBox txtCostActual = (TextBox)eitem["CostActual"].Controls[0];
            if (actualStartDate > nullDate && actualEndDate > nullDate && actualEndDate > actualStartDate && plist != null) {
                TimeSpan actualSpan = actualEndDate.Subtract(actualStartDate);
                int duration = 0;
                txtActualDuration.Text = duration.ToString();
                if (plist.PaymentIsForWholeMonth == true) {
                    duration = Math.Abs((actualEndDate.Month - actualStartDate.Month) + 12 * (actualEndDate.Year - actualStartDate.Year));
                } else {
                    duration = (int)Math.Ceiling(actualSpan.TotalDays);
                }
                txtActualDuration.Text = duration.ToString();
                double costPerDay = (double)plist.MonthlyCharges.Value / 30;
                double calculatedCost = (double)plist.InstallationCost.Value + (costPerDay * duration);
                if (cbInternet.Checked) { calculatedCost += (double)plist.Internet.Value; }
                if (cbMSN.Checked) { calculatedCost += (double)plist.MSN.Value; }
                if (!string.IsNullOrEmpty(txtAddedCharges.Text)) { calculatedCost += double.Parse(txtAddedCharges.Text.Replace(".", ",")); }
                txtCostActual.Text = calculatedCost.ToString();
            }
        }

        protected void uplFile_FileUploaded(object sender, FileUploadedEventArgs e) {
            string fullPath = Server.MapPath(fileUploadFolder);
            bool exists = System.IO.Directory.Exists(fullPath);
            if (!exists) { System.IO.Directory.CreateDirectory(fullPath); }
            string newfilename = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + "_" + e.File.GetNameWithoutExtension().Replace(" ", "_") + e.File.GetExtension();
            uploadedFilePath = fileUploadFolder + newfilename;
            e.File.SaveAs(System.IO.Path.Combine(fullPath, newfilename));
        }

        protected void exportDOCX(RadFlowDocument doc) {
            IFormatProvider<RadFlowDocument> formatProvider = new DocxFormatProvider();
            byte[] renderedBytes = null;
            using (MemoryStream ms = new MemoryStream()) {
                formatProvider.Export(doc, ms);
                renderedBytes = ms.ToArray();
            }
            Response.ClearHeaders();
            Response.ClearContent();
            Response.AppendHeader("content-disposition", "attachment; filename=ExportedFile.docx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            Response.BinaryWrite(renderedBytes);
            Response.End();
        }

        protected RadFlowDocument LoadSampleDocument(string uniqueName) {
            RadFlowDocument document2;
            IFormatProvider<RadFlowDocument> fileFormatProvider = new DocxFormatProvider();
            string fileName = Server.MapPath(templatesFolder + uniqueName + ".docx");
            using (FileStream input = new FileStream(fileName, FileMode.Open)) {
                document2 = fileFormatProvider.Import(input);
            }
            return document2;
        }

    }

}