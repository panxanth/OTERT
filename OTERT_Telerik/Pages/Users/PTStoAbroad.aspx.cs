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
using System.Web;

namespace OTERT.Pages.Administrator {

    public partial class PTStoAbroad : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected RadScriptManager RadScriptManager1;
        protected string pageTitle, uploadedFilePath;
        protected int Customer1ID, EventID, CountryID, PlaceID;
        protected int CustomerID, PositionID, LineTypeID;
        protected UserB loggedUser;
        const string fileUploadFolder = "~/UploadedFiles/";
        const string templatesFolder = "~/Templates/";
        const string pageUniqueName = "PTStoAbroad";
        const int OrderTypeID = 2;

        const string sqlUniqueName = "PTStoAbroadCharges_";
        const string sqlUniqueNameOrders = "PTStoAbroadOrders_";
        const string docTemplate = "PTStoAbroadCharges";
        const string docTemplateOrders = "PTStoAbroadOrders";

        protected TextBox txtOrderID;
        protected ImageButton btnPrintOrder;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Έργα > ΠΤΣ προς ΕΞωτερικό";
                gridMain.MasterTableView.Caption = "Έργα > ΠΤΣ προς ΕΞωτερικό";
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
                PlaceID = -1;
                Session.Remove("PlaceID");
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.MasterTableView.CurrentPageIndex * gridMain.MasterTableView.PageSize;
            int recTake = gridMain.MasterTableView.PageSize;
            string recFilter = gridMain.MasterTableView.FilterExpression;
            GridSortExpressionCollection gridSortExxpressions = gridMain.MasterTableView.SortExpressions;
            try {
                OrdersController cont = new OrdersController();
                gridMain.VirtualItemCount = cont.CountOrders(OrderTypeID);
                gridMain.DataSource = cont.GetOrders(OrderTypeID, recSkip, recTake, recFilter, gridSortExxpressions);
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
                if (e.Item is GridFilteringItem) {
                    GridFilteringItem filterItem = (GridFilteringItem)e.Item;
                    (filterItem["DateTimeStart"].Controls[0] as LiteralControl).Text = "Από: ";
                    (filterItem["DateTimeStart"].Controls[3] as LiteralControl).Text = "<br />Έως: ";
                    RadDateTimePicker DateTimeStartFrom = filterItem["DateTimeStart"].Controls[1] as RadDateTimePicker;
                    DateTimeStartFrom.TimePopupButton.Visible = false;
                    RadDateTimePicker DateTimeStartΤο = filterItem["DateTimeStart"].Controls[4] as RadDateTimePicker;
                    DateTimeStartΤο.TimePopupButton.Visible = false;
                    DateTimeStartFrom.DateInput.Attributes.Add("onchange", "javascript:UpdateTo('" + DateTimeStartFrom.ClientID + "', '" + DateTimeStartΤο.ClientID + "');");
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
                    PlaceID = -1;
                    Session.Remove("PlaceID");
                    GridEditableItem item = e.Item as GridEditableItem;
                    RadDropDownList ddlCountry = item.FindControl("ddlCountry") as RadDropDownList;
                    RadDropDownList ddlCustomer1 = item.FindControl("ddlCustomer1") as RadDropDownList;
                    RadDropDownList ddlEvent = item.FindControl("ddlEvent") as RadDropDownList;
                    RadDropDownList ddlPlace = item.FindControl("ddlPlace") as RadDropDownList;
                    try {
                        OrderB currOrder = e.Item.DataItem as OrderB;
                        CountriesController ccont = new CountriesController();
                        ddlCountry.DataSource = ccont.GetCountries();
                        ddlCountry.DataTextField = "NameGR";
                        ddlCountry.DataValueField = "ID";
                        ddlCountry.DataBind();
                        ddlCountry.Items.Insert(0, new DropDownListItem("Όλες οι Χώρες ...", "0"));
                        CustomersController custcont = new CustomersController();
                        //ddlCustomer1.DataSource = custcont.GetProvidersForCountry(Int32.Parse(ddlCountry.SelectedItem.Value));
                        ddlCustomer1.DataSource = custcont.GetProvidersForCountry(currOrder.Event.Place.CountryID);
                        ddlCustomer1.DataTextField = "NameGR";
                        ddlCustomer1.DataValueField = "ID";
                        ddlCustomer1.DataBind();
                        PlacesController plcont = new PlacesController();
                        //ddlPlace.DataSource = plcont.GetPlacesForCountry(Int32.Parse(ddlCountry.SelectedItem.Value));
                        ddlPlace.DataSource = plcont.GetPlacesForCountry(currOrder.Event.Place.CountryID);
                        ddlPlace.DataTextField = "NameGR";
                        ddlPlace.DataValueField = "ID";
                        ddlPlace.DataBind();
                        EventsController econt = new EventsController();
                        //ddlEvent.DataSource = econt.GetEventsForPlace(Int32.Parse(ddlPlace.SelectedItem.Value));
                        ddlEvent.DataSource = econt.GetEventsForPlace(currOrder.Event.PlaceID);
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

                            ddlPlace.SelectedIndex = ddlPlace.FindItemByValue(currOrder.Event.PlaceID.ToString()).Index;
                            Session["PlaceID"] = currOrder.Event.PlaceID;

                            ddlCustomer1.SelectedIndex = ddlCustomer1.FindItemByValue(currOrder.Customer1ID.ToString()).Index;
                            Session["Customer1ID"] = currOrder.Customer1ID;
                            ddlEvent.SelectedIndex = ddlEvent.FindItemByValue(currOrder.EventID.ToString()).Index;
                            Session["EventID"] = currOrder.EventID;
                        } else {
                            ddlCountry.SelectedIndex = 0;
                            Session["CountryID"] = ddlCountry.SelectedItem.Value;
                            ddlCustomer1.SelectedIndex = 0;
                            if (ddlCustomer1.SelectedItem != null) { Session["Customer1ID"] = ddlCustomer1.SelectedItem.Value; }
                            ddlPlace.SelectedIndex = 0;
                            if (ddlPlace.SelectedItem != null) { Session["PlaceID"] = ddlPlace.SelectedItem.Value; }
                            ddlEvent.SelectedIndex = 0;
                            if (ddlEvent.SelectedItem != null) { Session["EventID"] = ddlEvent.SelectedItem.Value; }
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
                    TextBox txtTelephoneNumber = item["TelephoneNumber"].Controls[0] as TextBox;
                    try {
                        txtRegNo.Text = curOrder.RegNo;
                        
                        TaskB currTask = e.Item.DataItem as TaskB;
                        CustomersController ccont = new CustomersController();
                        ddlCustomers.DataSource = ccont.GetGreekProviders();
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
                            if (string.IsNullOrEmpty(currTask.TelephoneNumber)) { txtTelephoneNumber.Text = "ΤΙΜΟΛΟΓΙΟ"; }
                        } else {
                            ddlCustomers.SelectedIndex = 0;
                            Session["CustomerID"] = ddlCustomers.SelectedItem.Value;
                            ddlRequestedPosition.SelectedIndex = 0;
                            Session["PositionID"] = ddlRequestedPosition.SelectedItem.Value;
                            ddlLineType.SelectedIndex = 0;
                            Session["LineTypeID"] = ddlLineType.SelectedItem.Value;
                            txtTelephoneNumber.Text = "ΤΙΜΟΛΟΓΙΟ";
                        }
                    }
                    catch (Exception) { }
                }
            }
            if (e.Item is GridFilteringItem) {
                GridFilteringItem filterItem = (GridFilteringItem)e.Item;
                RadDropDownList custlist = (RadDropDownList)filterItem.FindControl("ddlCustomer1Filter");
                RadDropDownList cunlist = (RadDropDownList)filterItem.FindControl("ddlCountryFilter");
                RadDropDownList plist = (RadDropDownList)filterItem.FindControl("ddlPlaceFilter");
                RadDropDownList elist = (RadDropDownList)filterItem.FindControl("ddlEventFilter");
                try {
                    CustomersController custcont = new CustomersController();
                    custlist.DataSource = custcont.GetForeignProviders();
                    custlist.DataTextField = "NameGR";
                    custlist.DataValueField = "ID";
                    custlist.DataBind();
                    custlist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                    CountriesController cuncont = new CountriesController();
                    cunlist.DataSource = cuncont.GetForeignCountries();
                    cunlist.DataTextField = "NameGR";
                    cunlist.DataValueField = "ID";
                    cunlist.DataBind();
                    cunlist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                    PlacesController pcont = new PlacesController();
                    plist.DataSource = pcont.GetForeignPlaces();
                    plist.DataTextField = "NameGR";
                    plist.DataValueField = "ID";
                    plist.DataBind();
                    plist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                    EventsController econt = new EventsController();
                    elist.DataSource = econt.GetForeignEvents();
                    elist.DataTextField = "NameGR";
                    elist.DataValueField = "ID";
                    elist.DataBind();
                    elist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                }
                catch (Exception) { }
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
                case 3:
                    RadWindowManager1.RadAlert("Το Α/Α που δώσατε είναι λάθος! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
                    break;
                default:
                    RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
                    break;
            }
        }

        protected void gridMain_ItemCommand(object sender, GridCommandEventArgs e) {
            if (e.CommandName == "printCharges") {
                GridDataItem item = (GridDataItem)e.Item;
                int orderID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                OrdersController oCont = new OrdersController();
                OrderB curOrder = oCont.GetOrder(orderID);
                TasksController lCont = new TasksController();
                List<TaskB> tasksForOrder = lCont.GetTasksForOrder(orderID);
                try {
                    DocumentReplacemetsController cont = new DocumentReplacemetsController();
                    List<DocumentReplacemetB> reps = new List<DocumentReplacemetB>();
                    reps = cont.GetDocumentReplacemets(sqlUniqueName);
                    string imgFolderPath = Server.MapPath(fileUploadFolder);
                    DocumentReplacemetB curRep;
                    BookmarkRangeStart bookmarkRangeStart;
                    RadFlowDocument curDoc = LoadSampleDocument(docTemplate);
                    RadFlowDocumentEditor editor = new RadFlowDocumentEditor(curDoc);
                    List<BookmarkRangeStart> docBookmarks = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().ToList();
                    Telerik.Windows.Documents.Flow.Model.TableCell currCell;
                    Run currRun;
                    Paragraph currPar;
                    string[] arrText;
                    Header defaultHeader = editor.Document.Sections.First().Headers.Default;
                    Footer defaultFooter = editor.Document.Sections.First().Footers.Default;
                    Telerik.Windows.Documents.Flow.Model.Styles.Style tableStyle = new Telerik.Windows.Documents.Flow.Model.Styles.Style("TableStyle", StyleType.Table);
                    tableStyle.Name = "Table Style";
                    tableStyle.TableProperties.Borders.LocalValue = new TableBorders(new Border(0, Telerik.Windows.Documents.Flow.Model.Styles.BorderStyle.None, new ThemableColor(System.Windows.Media.Colors.Black)));
                    tableStyle.TableProperties.Alignment.LocalValue = Alignment.Left;
                    tableStyle.TableRowProperties.Height.LocalValue = new TableRowHeight(HeightType.Auto);
                    tableStyle.TableCellProperties.VerticalAlignment.LocalValue = VerticalAlignment.Center;
                    tableStyle.TableCellProperties.PreferredWidth.LocalValue = new TableWidthUnit(TableWidthUnitType.Percent, 100);
                    tableStyle.TableCellProperties.Padding.LocalValue = new Telerik.Windows.Documents.Primitives.Padding(8);
                    editor.Document.StyleRepository.Add(tableStyle);

                    // OTE Title
                    curRep = reps.Find(o => o.UniqueName == "PTStoAbroadCharges_OTETitle");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                    currPar.Spacing.LineSpacing = 1;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertLine(curRep.Text);
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 13.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    for (int i = 0; i < 2; i++) {
                        currRun = editor.InsertLine(" ");
                        currRun.Paragraph.ContextualSpacing = true;
                        currRun.Paragraph.Spacing.LineSpacing = 1;
                        currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 12.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                        currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    }

                    // Address
                    curRep = reps.Find(o => o.UniqueName == "PTStoAbroadCharges_Address");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currPar.Spacing.LineSpacing = 1;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText.Length; i++) {
                        if (!string.IsNullOrEmpty(arrText[i])) {
                            currRun = editor.InsertLine(arrText[i]);
                            if (i == arrText.Length - 1) { currRun.Underline.Pattern = UnderlinePattern.Single; }
                            currRun.Paragraph.ContextualSpacing = true;
                            currRun.Paragraph.Spacing.LineSpacing = 1;
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 13.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    for (int i = 0; i < 3; i++) {
                        currRun = editor.InsertLine(" ");
                        currRun.Paragraph.ContextualSpacing = true;
                        currRun.Paragraph.Spacing.LineSpacing = 1;
                        currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 12.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                        currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    }
                    currCell.Blocks.Remove(currCell.Blocks.Last());

                    // Date
                    curRep = reps.Find(o => o.UniqueName == "PTStoAbroadCharges_Date");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineEnd(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertLine("ΑΘΗΝΑ, " + DateTime.Now.ToString(curRep.Text, new System.Globalization.CultureInfo("el-GR")));
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 13.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    string spaces = "                  ";
                    currRun = editor.InsertLine("Αριθμ:    76200/311/" + spaces);
                    currRun.Paragraph.ContextualSpacing = true;
                    currRun.Paragraph.Spacing.LineSpacing = 1;
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 13.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    // Info
                    curRep = reps.Find(o => o.UniqueName == "PTStoAbroadCharges_Info");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currPar.Spacing.LineSpacing = 1;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText.Length; i++) {
                        if (!string.IsNullOrEmpty(arrText[i])) {
                            currRun = editor.InsertLine(arrText[i]);
                            currRun.Paragraph.ContextualSpacing = true;
                            currRun.Paragraph.Spacing.LineSpacing = 1;
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 13.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    for (int i = 0; i < 3; i++) {
                        currRun = editor.InsertLine(" ");
                        currRun.Paragraph.ContextualSpacing = true;
                        currRun.Paragraph.Spacing.LineSpacing = 1;
                        currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 12.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                        currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    }
                    currCell.Blocks.Remove(currCell.Blocks.Last());

                    // To
                    curRep = reps.Find(o => o.UniqueName == "PTStoAbroadCharges_To");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currPar.Spacing.LineSpacing = 1;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText.Length; i++) {
                        if (!string.IsNullOrEmpty(arrText[i])) {
                            currRun = editor.InsertLine(arrText[i]);
                            if (i == 0) { currRun.Underline.Pattern = UnderlinePattern.Single; }
                            currRun.Paragraph.ContextualSpacing = true;
                            currRun.Paragraph.Spacing.LineSpacing = 1;
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 13.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    currCell.Blocks.Remove(currCell.Blocks.Last());

                    // Main Title
                    curRep = reps.Find(o => o.UniqueName == "PTStoAbroadCharges_Main_Title");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                    currRun.Text = curRep.Text;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 16.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    // Body_Table Title
                    bookmarkRangeStart = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().Where(rangeStart => rangeStart.Bookmark.Name == "Body_Table_Title").FirstOrDefault();
                    editor.MoveToInlineEnd(bookmarkRangeStart);
                    Telerik.Windows.Documents.Flow.Model.Table tblTitleContent = editor.InsertTable();
                    tblTitleContent.StyleId = "TableStyle";
                    tblTitleContent.LayoutType = TableLayoutType.AutoFit;
                    ThemableColor cellTitleBackground = new ThemableColor(System.Windows.Media.Colors.Beige);
                    for (int i = 0; i < 2; i++) {
                        Telerik.Windows.Documents.Flow.Model.TableRow row1 = tblTitleContent.Rows.AddTableRow();
                        for (int j = 0; j < 2; j++) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell = row1.Cells.AddTableCell();
                            if (i == 0) {
                                if (j == 0) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("ΔΙΟΡΓΑΝΩΣΗ: ");
                                    currRun.Paragraph.ContextualSpacing = true;
                                    currRun.Paragraph.Spacing.LineSpacing = 1.0;
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 15.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                    cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 40);
                                } else if (j == 1) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun(curOrder.Event.NameGR);
                                    currRun.Paragraph.ContextualSpacing = true;
                                    currRun.Paragraph.Spacing.LineSpacing = 1.0;
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 15.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                    cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 60);
                                }
                            } else {
                                int k = i - 1;
                                if (j == 0) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("ΧΩΡΑ: ");
                                    currRun.Paragraph.ContextualSpacing = true;
                                    currRun.Paragraph.Spacing.LineSpacing = 1.0;
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 15.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 1) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun(toUpperGR(curOrder.Event.Place.Country.NameGR));
                                    currRun.Paragraph.ContextualSpacing = true;
                                    currRun.Paragraph.Spacing.LineSpacing = 1.0;
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 15.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                }
                            }
                        }
                    }

                    // Main Table
                    bookmarkRangeStart = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().Where(rangeStart => rangeStart.Bookmark.Name == "Body_Main").FirstOrDefault();
                    editor.MoveToInlineEnd(bookmarkRangeStart);
                    Telerik.Windows.Documents.Flow.Model.Table tblContent = editor.InsertTable();
                    tblContent.StyleId = "TableStyle";
                    tblContent.LayoutType = TableLayoutType.AutoFit;
                    ThemableColor cellBackground = new ThemableColor(System.Windows.Media.Colors.Beige);
                    for (int i = 0; i < tasksForOrder.Count + 1; i++) {
                        Telerik.Windows.Documents.Flow.Model.TableRow row1 = tblContent.Rows.AddTableRow();
                        for (int j = 0; j < 6; j++) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell = row1.Cells.AddTableCell();
                            if (i == 0) {
                                if (j == 0) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("Τ.Ε.Κ.");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                    cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 17);
                                } else if (j == 1) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("ΤΛΦ ΧΡΕΩΣΗΣ");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                    cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 17);
                                } else if (j == 2) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("ΕΠΩΝΥΜΙΑ ΠΕΛΑΤΗ");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                    cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 24);
                                } else if (j == 3) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("ΗΜ/ΝΙΑ");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                    cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 18);
                                } else if (j == 4) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("Κ.Τ. 6630(€)");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                    cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 12);
                                } else if (j == 5) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("Κ.Τ. 6951(€)");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                    cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 12);
                                }
                            } else {
                                int k = i - 1;
                                if (j == 0) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 1) {
                                    string chTel = tasksForOrder[k].TelephoneNumber != null ? tasksForOrder[k].TelephoneNumber : "";
                                    if (chTel == "") { chTel = tasksForOrder[k].Customer.ChargeTelephone != null ? tasksForOrder[k].Customer.ChargeTelephone : ""; }
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun(chTel);
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 2) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun(tasksForOrder[k].Customer.NameGR != null ? tasksForOrder[k].Customer.NameGR : "");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 3) {
                                    string date2print = getDatesSpan(tasksForOrder[k].DateTimeStartActual, tasksForOrder[k].DateTimeEndActual);
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun(date2print);
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 4) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun(tasksForOrder[k].InvoiceCost != null ? tasksForOrder[k].InvoiceCost.ToString() : "");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 5) {
                                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun(tasksForOrder[k].AddedCharges != null ? tasksForOrder[k].AddedCharges.ToString() : "");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                }
                            }
                        }
                    }
                    //for (int i = 0; i < 1; i++) {
                    Telerik.Windows.Documents.Flow.Model.TableRow row2 = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 6; j++) {
                        Telerik.Windows.Documents.Flow.Model.TableCell cell = row2.Cells.AddTableCell();
                        currRun = cell.Blocks.AddParagraph().Inlines.AddRun("");
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 12.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                        currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    }
                    //}
                    Telerik.Windows.Documents.Flow.Model.TableRow row = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 6; j++) {
                        Telerik.Windows.Documents.Flow.Model.TableCell cell = row.Cells.AddTableCell();
                        if (j == 0) {
                            currRun = cell.Blocks.AddParagraph().Inlines.AddRun("");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 1) {
                            currRun = cell.Blocks.AddParagraph().Inlines.AddRun("");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 2) {
                            currRun = cell.Blocks.AddParagraph().Inlines.AddRun("");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 3) {
                            currRun = cell.Blocks.AddParagraph().Inlines.AddRun("ΣΥΝΟΛΟ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 16.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 4) {
                            decimal totInvoiceCost = 0;
                            for (int tr = 0; tr < tasksForOrder.Count(); tr++) {
                                totInvoiceCost += (tasksForOrder[tr].InvoiceCost != null ? tasksForOrder[tr].InvoiceCost.Value : 0);
                            }
                            currRun = cell.Blocks.AddParagraph().Inlines.AddRun(totInvoiceCost.ToString());
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.ForegroundColor.LocalValue = new ThemableColor(System.Windows.Media.Color.FromRgb(0, 0, 255));
                            currRun.Properties.FontSize.LocalValue = 13.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 5) {
                            decimal totAddedCharges = 0;
                            for (int tr = 0; tr < tasksForOrder.Count(); tr++){
                                totAddedCharges += (tasksForOrder[tr].AddedCharges != null ? tasksForOrder[tr].AddedCharges.Value : 0);
                            }
                            currRun = cell.Blocks.AddParagraph().Inlines.AddRun(totAddedCharges.ToString());
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 13.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }

                    // Main Text
                    curRep = reps.Find(o => o.UniqueName == "PTStoAbroadCharges_Main_Text");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    curRep.Text = curRep.Text.Replace("#provider#", "isBlue" + curOrder.Customer1.NameEN + " - " + toUpperGR(curOrder.Event.Place.Country.NameGR) + "isBlue");
                    arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Justified;
                    currPar.Spacing.LineSpacing = 1.5;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText.Length; i++) {
                        if (!string.IsNullOrEmpty(arrText[i])) {
                            string[] arrTextBold = arrText[i].Split(new string[] { "/b/" }, StringSplitOptions.RemoveEmptyEntries);
                            if (arrTextBold.Length > 1) {
                                for (int k = 0; k < arrTextBold.Length; k++) {
                                    bool isBlue = false;
                                    if (arrTextBold[k].StartsWith("isBlue") && arrTextBold[k].EndsWith("isBlue")) {
                                        arrTextBold[k] = arrTextBold[k].Replace("isBlue", "");
                                        isBlue = true; 
                                    }
                                    if (k == arrTextBold.Length - 1) {
                                        currRun = editor.InsertLine(arrTextBold[k]);
                                    } else {
                                        currRun = editor.InsertText(arrTextBold[k]);
                                    }
                                    currRun.Paragraph.ContextualSpacing = true;
                                    currRun.Paragraph.Spacing.LineSpacing = 1.5;
                                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Justified;
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    if (isBlue == true) { currRun.Properties.ForegroundColor.LocalValue = new ThemableColor(System.Windows.Media.Color.FromRgb(0, 0, 255)); }
                                    currRun.Properties.FontSize.LocalValue = 13.0;
                                    if (k % 2 == 1) {
                                        currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                    } else {
                                        currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    }
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                }
                            } else {
                                currRun = editor.InsertLine(arrText[i]);
                                currRun.Paragraph.ContextualSpacing = true;
                                currRun.Paragraph.Spacing.LineSpacing = 1.5;
                                currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Justified;
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 13.0;
                                if (i == arrText.Length - 1) {
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                } else {
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                }
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            }
                        }
                    }
                    for (int i = 0; i < 4; i++) {
                        currRun = editor.InsertLine(" ");
                        currRun.Paragraph.ContextualSpacing = true;
                        currRun.Paragraph.Spacing.LineSpacing = 1;
                        currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 12.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                        currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    }
                    currCell.Blocks.Remove(currCell.Blocks.Last());

                    // Check Name
                    curRep = reps.Find(o => o.UniqueName == "PTStoAbroadCharges_Check_Name");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                    currPar.Spacing.LineSpacing = 1;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText.Length; i++) {
                        if (!string.IsNullOrEmpty(arrText[i])) {
                            currRun = editor.InsertLine(arrText[i]);
                            currRun.Paragraph.ContextualSpacing = true;
                            currRun.Paragraph.Spacing.LineSpacing = 1;
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Center;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            if (i == 0) {
                                currRun.Properties.FontSize.LocalValue = 12.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            } else {
                                currRun.Properties.FontSize.LocalValue = 15.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                            }
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    currCell.Blocks.Remove(currCell.Blocks.Last());

                    // Chief Name
                    curRep = reps.Find(o => o.UniqueName == "PTStoAbroadCharges_Chief_Name");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                    currPar.Spacing.LineSpacing = 1;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText.Length; i++) {
                        if (!string.IsNullOrEmpty(arrText[i])) {
                            currRun = editor.InsertLine(arrText[i]);
                            currRun.Paragraph.ContextualSpacing = true;
                            currRun.Paragraph.Spacing.LineSpacing = 1;
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Center;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            if (i == arrText.Length - 2) {
                                currRun.Properties.FontSize.LocalValue = 16.0;
                            } else if (i == arrText.Length - 1) {
                                currRun.Properties.FontSize.LocalValue = 11.0;
                            } else {
                                currRun.Properties.FontSize.LocalValue = 12.0;
                            }
                            if (i == 0) {
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            } else {
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                            }
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    currCell.Blocks.Remove(currCell.Blocks.Last());


                    curDoc.UpdateFields();
                    exportDOCX(curDoc, "ExportedFile");
                }
                catch (Exception) { }
            } 
            else if (e.CommandName == "orderCopy") {
                GridDataItem item = (GridDataItem)e.Item;
                int orderID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                OrdersController oCont = new OrdersController();
                OrderB curOrder = oCont.GetOrder(orderID);
                using (var dbContext = new OTERTConnStr()) {
                    var newOrder = new Orders();
                    try {
                        newOrder.RegNo = curOrder.RegNo;
                        newOrder.InoiceProtocol = curOrder.InoiceProtocol;
                        newOrder.OrderTypeID = curOrder.OrderTypeID;
                        newOrder.Customer1ID = curOrder.Customer1ID;
                        newOrder.EventID = curOrder.EventID;
                        newOrder.IsLocked = false;
                        dbContext.Orders.Add(newOrder);
                        dbContext.SaveChanges();
                        gridMain.Rebind();
                    }
                    catch (Exception) { ShowErrorMessage(-1); }
                }
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
                        PlaceID = -1;
                        Session.Remove("PlaceID");
                        CountryID = -1;
                        Session.Remove("CountryID");
                        try { dbContext.SaveChanges(); }
                        catch (Exception) { ShowErrorMessage(-1); }
                    }
                }
            } else if (e.Item.OwnerTableView.Name == "TasksDetails") {
                var editableItem = ((GridEditableItem)e.Item);
                var ID = (int)editableItem.GetDataKeyValue("ID");
                GridDataItem parentItem = e.Item.OwnerTableView.ParentItem;
                int orderID = int.Parse(parentItem.GetDataKeyValue("ID").ToString());
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
                            int test = dbContext.SaveChanges();
                            var curOrder = dbContext.Orders.Where(n => n.ID == orderID).FirstOrDefault();
                            DateTime?[] datesForOrder = getDatesForOrder(orderID);
                            curOrder.DateTimeStart = datesForOrder[0];
                            curOrder.DateTimeEnd = datesForOrder[1];
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
                            gridMain.EditIndexes.Clear();
                            e.Item.OwnerTableView.Rebind();
                            string oldID = parentItem.GetDataKeyValue("ID").ToString();
                            gridMain.Rebind();
                            expandRowByID(oldID);
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
                            curOrder.InoiceProtocol = (string)values["InoiceProtocol"];
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
                            PlaceID = -1;
                            Session.Remove("PlaceID");
                        }
                    } else { ShowErrorMessage(-1); }
                }
            } else if (e.Item.OwnerTableView.Name == "TasksDetails") {
                JobsController jc = new JobsController();
                JobB tmpJob = jc.GetJobsForPageID(1)[0];
                DistancesController dc = new DistancesController();
                DistanceB tmpDistance = dc.GetDistancesForPageID(1)[0];
                GridDataItem parentItem = e.Item.OwnerTableView.ParentItem;
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
                            if (values["DateTimeStartActual"] != null) { curTask.DateTimeStartActual = DateTime.Parse((string)values["DateTimeStartActual"]); } else { curTask.DateTimeStartActual = null; }
                            if (values["DateTimeEndActual"] != null) { curTask.DateTimeEndActual = DateTime.Parse((string)values["DateTimeEndActual"]); } else { curTask.DateTimeEndActual = null; }
                            if (values["DateTimeDurationActual"] != null) { curTask.DateTimeDurationActual = int.Parse((string)values["DateTimeDurationActual"]); } else { curTask.DateTimeDurationActual = null; }
                            if (values["CostCalculated"] != null) { curTask.CostCalculated = decimal.Parse((string)values["CostCalculated"]); } else { curTask.CostCalculated = null; }
                            curTask.InstallationCharges = false;
                            curTask.MonthlyCharges = false;
                            curTask.CallCharges = 0;
                            curTask.InvoceComments = (string)values["InvoceComments"]; //Ονομ/νυμο Ανταποκριτή
                            curTask.CorrespondentPhone = (string)values["CorrespondentPhone"];
                            curTask.TelephoneNumber = (string)values["TelephoneNumber"];
                            curTask.GivenPhoneNumber = (string)values["GivenPhoneNumber"];
                            curTask.TechnicalSupport = 0;
                            if (values["AddedCharges"] != null) { curTask.AddedCharges = decimal.Parse((string)values["AddedCharges"]); } else { curTask.AddedCharges = null; }
                            if (values["CostActual"] != null) { curTask.CostActual = decimal.Parse((string)values["CostActual"]); } else { curTask.CostActual = null; }
                            if (values["PaymentDateOrder"] != null) { curTask.PaymentDateOrder = DateTime.Parse((string)values["PaymentDateOrder"]); } else { curTask.PaymentDateOrder = null; }
                            if (values["PaymentDateActual"] != null) { curTask.PaymentDateActual = DateTime.Parse((string)values["PaymentDateActual"]); } else { curTask.PaymentDateActual = null; }
                            curTask.IsForHelpers = null;
                            curTask.IsLocked = (bool)values["IsLocked"];
                            curTask.IsCanceled = (bool)values["IsCanceled"];
                            curTask.CancelPrice = 0;
                            curTask.Comments = (string)values["Comments"];
                            curTask.SateliteID = null;
                            curTask.Internet = false;
                            curTask.MSN = false;
                            curTask.LineTypeID = LineTypeID;
                            curTask.DateStamp = DateTime.Now;
                            dbContext.Tasks.Add(curTask);
                            dbContext.SaveChanges();
                            var curOrder = dbContext.Orders.Where(n => n.ID == orderID).FirstOrDefault();
                            DateTime?[] datesForOrder = getDatesForOrder(orderID);
                            curOrder.DateTimeStart = datesForOrder[0];
                            curOrder.DateTimeEnd = datesForOrder[1];
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
                            e.Item.OwnerTableView.Rebind();
                            string oldID = parentItem.GetDataKeyValue("ID").ToString();
                            gridMain.Rebind();
                            expandRowByID(oldID);
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
                GridDataItem parentItem = e.Item.OwnerTableView.ParentItem;
                using (var dbContext = new OTERTConnStr()) {
                    var curTask = dbContext.Tasks.Where(n => n.ID == ID).FirstOrDefault();
                    if (curTask != null) {
                        if (curTask.IsLocked != true) {
                            try {
                                int curOrderID = curTask.OrderID == null ? -1 : (int)curTask.OrderID;
                                dbContext.Tasks.Remove(curTask);
                                dbContext.SaveChanges();
                                var curOrder = dbContext.Orders.Where(n => n.ID == curOrderID).FirstOrDefault();
                                DateTime?[] datesForOrder = getDatesForOrder(curOrderID);
                                curOrder.DateTimeStart = datesForOrder[0];
                                curOrder.DateTimeEnd = datesForOrder[1];
                                dbContext.SaveChanges(); 
                            }
                            catch (Exception ex) {
                                string err = ex.InnerException.InnerException.Message;
                                int errCode = -1;
                                if (err.StartsWith("The DELETE statement conflicted with the REFERENCE constraint")) { errCode = 1; }
                                ShowErrorMessage(errCode);
                            }
                            finally {
                                e.Item.OwnerTableView.Rebind();
                                string oldID = parentItem.GetDataKeyValue("ID").ToString();
                                gridMain.Rebind();
                                expandRowByID(oldID);
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

        protected void ddlPlace_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try {
                PlaceID = int.Parse(e.Value);
                Session["PlaceID"] = PlaceID;
                RadDropDownList ddlPlaces = (RadDropDownList)sender;
                GridEditableItem item = (GridEditableItem)ddlPlaces.NamingContainer;
                RadDropDownList ddlEvent = (RadDropDownList)item.FindControl("ddlEvent");
                ddlEvent.ClearSelection();
                EventsController econt = new EventsController();
                ddlEvent.DataSource = econt.GetEventsForPlace(PlaceID);
                ddlEvent.DataTextField = "NameGR";
                ddlEvent.DataValueField = "ID";
                ddlEvent.DataBind();
                ddlEvent.SelectedIndex = 0;
                if (ddlEvent.Items.Count > 0) { Session["EventID"] = ddlEvent.SelectedItem.Value; } else { Session.Remove("EventID"); }
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
                    RadDropDownList ddlCustomer1 = (RadDropDownList)item.FindControl("ddlCustomer1");
                    ddlCustomer1.ClearSelection();
                    CustomersController custcont = new CustomersController();
                    ddlCustomer1.DataSource = custcont.GetProvidersForCountry(CountryID);
                    ddlCustomer1.DataTextField = "NameGR";
                    ddlCustomer1.DataValueField = "ID";
                    ddlCustomer1.DataBind();
                    ddlCustomer1.SelectedIndex = 0;
                    if (ddlCustomer1.Items.Count > 0) { Session["Customer1ID"] = ddlCustomer1.SelectedItem.Value; } else { Session.Remove("Customer1ID"); }
                    RadDropDownList ddlPlace = (RadDropDownList)item.FindControl("ddlPlace");
                    ddlPlace.ClearSelection();
                    PlacesController pcont = new PlacesController();
                    ddlPlace.DataSource = pcont.GetPlacesForCountry(CountryID);
                    ddlPlace.DataTextField = "NameGR";
                    ddlPlace.DataValueField = "ID";
                    ddlPlace.DataBind();
                    ddlPlace.SelectedIndex = 0;
                    if (ddlPlace.Items.Count > 0) { Session["PlaceID"] = ddlPlace.SelectedItem.Value; } else { Session.Remove("PlaceID"); }
                    RadDropDownList ddlEvent = (RadDropDownList)item.FindControl("ddlEvent");
                    ddlEvent.ClearSelection();
                    EventsController econt = new EventsController();
                    ddlEvent.DataSource = econt.GetEventsForPlace(Int32.Parse(ddlPlace.SelectedItem.Value));
                    ddlEvent.DataTextField = "NameGR";
                    ddlEvent.DataValueField = "ID";
                    ddlEvent.DataBind();
                    ddlEvent.SelectedIndex = 0;
                    if (ddlEvent.Items.Count > 0) { Session["EventID"] = ddlEvent.SelectedItem.Value; } else { Session.Remove("EventID"); }

                } else {
                    RadDropDownList ddlCountries = (RadDropDownList)sender;
                    GridEditableItem item = (GridEditableItem)ddlCountries.NamingContainer;
                    RadDropDownList ddlCustomer1 = (RadDropDownList)item.FindControl("ddlCustomer1");
                    ddlCustomer1.ClearSelection();
                    CustomersController custcont = new CustomersController();
                    ddlCustomer1.DataSource = custcont.GetProvidersForCountry(0);
                    ddlCustomer1.DataTextField = "NameGR";
                    ddlCustomer1.DataValueField = "ID";
                    ddlCustomer1.DataBind();
                    ddlCustomer1.SelectedIndex = 0;
                    Session["Customer1ID"] = ddlCustomer1.SelectedItem.Value;
                    RadDropDownList ddlPlace = (RadDropDownList)item.FindControl("ddlPlace");
                    ddlPlace.ClearSelection();
                    PlacesController pcont = new PlacesController();
                    ddlPlace.DataSource = pcont.GetPlacesForCountry(0);
                    ddlPlace.DataTextField = "NameGR";
                    ddlPlace.DataValueField = "ID";
                    ddlPlace.DataBind();
                    ddlPlace.SelectedIndex = 0;
                    Session["PlaceID"] = ddlPlace.SelectedItem.Value;
                    RadDropDownList ddlEvent = (RadDropDownList)item.FindControl("ddlEvent");
                    ddlEvent.ClearSelection();
                    EventsController econt = new EventsController();
                    ddlEvent.DataSource = econt.GetEventsForPlace(Int32.Parse(ddlPlace.SelectedItem.Value));
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
                //calculateCosts(eitem);
            }
            catch (Exception) { }
        }

        protected void txtAddedCharges_TextChanged(object sender, EventArgs e) {
            TextBox txtAddedCharges = (TextBox)sender;
            GridEditableItem eitem = (GridEditableItem)txtAddedCharges.NamingContainer;
            calculateCosts(eitem);
        }

        protected void txtInvoiceCost_TextChanged(object sender, EventArgs e) {
            TextBox txtInvoiceCost = (TextBox)sender;
            GridEditableItem eitem = (GridEditableItem)txtInvoiceCost.NamingContainer;
            calculateCosts(eitem);
        }

        protected void dpDate_SelectedIndexChanged(object sender, SelectedDateChangedEventArgs e) {
            RadDatePicker dpStartDate = (RadDatePicker)sender;
            GridEditableItem eitem = (GridEditableItem)dpStartDate.NamingContainer;
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
            TextBox txtActualDuration = (TextBox)eitem["DateTimeDurationActual"].Controls[0];
            TextBox txtAddedCharges = (TextBox)eitem.FindControl("txtAddedCharges");
            TextBox txtInvoiceCost = (TextBox)eitem.FindControl("txtInvoiceCost");
            TextBox txtCostActual = (TextBox)eitem["CostActual"].Controls[0];
            if (actualStartDate > nullDate && actualEndDate > nullDate && actualEndDate > actualStartDate) {
                TimeSpan actualSpan = actualEndDate.Subtract(actualStartDate);
                int duration = 0;
                duration = (int)Math.Ceiling(actualSpan.TotalDays);
                txtActualDuration.Text = duration.ToString();
                double calculatedCost = 0;
                if (!string.IsNullOrEmpty(txtAddedCharges.Text)) { calculatedCost += double.Parse(txtAddedCharges.Text.Replace(".", ",")); }
                if (!string.IsNullOrEmpty(txtInvoiceCost.Text)) { calculatedCost += double.Parse(txtInvoiceCost.Text.Replace(".", ",")); }
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

        protected void exportDOCX(RadFlowDocument doc, string filename) {
            IFormatProvider<RadFlowDocument> formatProvider = new DocxFormatProvider();
            byte[] renderedBytes = null;
            using (MemoryStream ms = new MemoryStream()) {
                formatProvider.Export(doc, ms);
                renderedBytes = ms.ToArray();
            }
            Response.ClearHeaders();
            Response.ClearContent();
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename + ".docx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            Response.BinaryWrite(renderedBytes);
            Response.End();
            //HttpContext.Current.Response.Flush(); // Sends all currently buffered output to the client.
            //HttpContext.Current.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
            //HttpContext.Current.ApplicationInstance.CompleteRequest();
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

        protected void ddlCustomer1Filter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("Customer1ID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(Customer1ID = " + e.Value + ")";
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

        protected void ddlCountryFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("Event.Place.CountryID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(Event.Place.CountryID = " + e.Value + ")";
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

        protected void ddlCountryFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

        protected void ddlPlaceFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("Event.PlaceID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(Event.PlaceID = " + e.Value + ")";
                gridMain.MasterTableView.GetColumn("PlaceID").CurrentFilterFunction = GridKnownFunction.EqualTo;
                gridMain.MasterTableView.GetColumn("PlaceID").CurrentFilterValue = e.Value;
            } else {
                gridMain.MasterTableView.GetColumn("PlaceID").CurrentFilterFunction = GridKnownFunction.NoFilter;
                gridMain.MasterTableView.GetColumn("PlaceID").CurrentFilterValue = null;
            }
            gridMain.MasterTableView.FilterExpression = finalExpression;
            ViewState[list.ClientID] = e.Value;
            gridMain.MasterTableView.Rebind();
        }

        protected void ddlPlaceFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

        protected void ddlEventFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("EventID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(EventID = " + e.Value + ")";
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

        protected DateTime?[] getDatesForOrder(int orderID) {
            DateTime?[] dates2Return = new DateTime?[] { null, null };
            TasksController tc = new TasksController();
            List<TaskB> tasksForOrder = tc.GetTasksForOrder(orderID);
            if (tasksForOrder.Count > 0) {
                TaskB smallerDateTask = tasksForOrder.OrderBy(x => x.DateTimeStartActual).FirstOrDefault();
                TaskB biggerDateTask = tasksForOrder.OrderBy(x => x.DateTimeStartActual).LastOrDefault();
                dates2Return[0] = smallerDateTask.DateTimeStartActual;
                dates2Return[1] = biggerDateTask.DateTimeEndActual;
            }
            return dates2Return;
        }

        protected void expandRowByID(string ID) {
            foreach (GridDataItem item in gridMain.MasterTableView.Items) {
                if (item.GetDataKeyValue("ID").ToString() == ID) {
                    item.Expanded = true;
                }
            }
        }

        protected string getDatesSpan (DateTime? dateStart, DateTime? dateEnd) {
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

        protected void btnPrintOrder_Click(object sender, EventArgs e){
            try {
                int taskID = Int32.Parse(txtOrderID.Text);
                TasksController tCont = new TasksController();
                OrdersController oCont = new OrdersController();
                CustomersController cCont = new CustomersController();
                TaskB task2print = tCont.GetTask(taskID);
                int orderID = task2print.OrderID.GetValueOrDefault();
                if (orderID > 0) {
                    OrderB curOrder = oCont.GetOrder(orderID);
                    CustomerB curProvider = cCont.GetCustomer(curOrder.Customer1ID);
                    DocumentReplacemetsController cont = new DocumentReplacemetsController();
                    List<DocumentReplacemetB> reps = new List<DocumentReplacemetB>();
                    reps = cont.GetDocumentReplacemets(sqlUniqueNameOrders);
                    string imgFolderPath = Server.MapPath(fileUploadFolder);
                    DocumentReplacemetB curRep;
                    BookmarkRangeStart bookmarkRangeStart;
                    RadFlowDocument curDoc = LoadSampleDocument(docTemplateOrders);
                    RadFlowDocumentEditor editor = new RadFlowDocumentEditor(curDoc);
                    List<BookmarkRangeStart> docBookmarks = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().ToList();
                    Telerik.Windows.Documents.Flow.Model.TableCell currCell;
                    Run currRun;
                    Paragraph currPar;
                    string[] arrText;
                    Header defaultHeader = editor.Document.Sections.First().Headers.Default;
                    Footer defaultFooter = editor.Document.Sections.First().Footers.Default;
                    Telerik.Windows.Documents.Flow.Model.Styles.Style tableStyle = new Telerik.Windows.Documents.Flow.Model.Styles.Style("TableStyle", StyleType.Table);
                    tableStyle.Name = "Table Style";
                    tableStyle.TableProperties.Borders.LocalValue = new TableBorders(new Border(0, Telerik.Windows.Documents.Flow.Model.Styles.BorderStyle.None, new ThemableColor(System.Windows.Media.Colors.Black)));
                    tableStyle.TableProperties.Alignment.LocalValue = Alignment.Left;
                    tableStyle.TableRowProperties.Height.LocalValue = new TableRowHeight(HeightType.Auto);
                    tableStyle.TableCellProperties.VerticalAlignment.LocalValue = VerticalAlignment.Center;
                    tableStyle.TableCellProperties.PreferredWidth.LocalValue = new TableWidthUnit(TableWidthUnitType.Percent, 100);
                    tableStyle.TableCellProperties.Padding.LocalValue = new Telerik.Windows.Documents.Primitives.Padding(8);
                    editor.Document.StyleRepository.Add(tableStyle);

                    // Date
                    curRep = reps.Find(o => o.UniqueName == "PTStoAbroadOrders_Date");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineEnd(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertLine("Athens, " + DateTime.Now.ToString(curRep.Text, new System.Globalization.CultureInfo("el-GR")));
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    string spaces = "                  ";
                    currRun = editor.InsertLine("Ref. 76200/311/" + spaces);
                    currRun.Paragraph.ContextualSpacing = true;
                    currRun.Paragraph.Spacing.LineSpacing = 1;
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    currCell.Blocks.Remove(currCell.Blocks.Last());

                    // Info Titles
                    curRep = reps.Find(o => o.UniqueName == "PTStoAbroadOrders_Info_Titles");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currPar.Spacing.LineSpacing = 1;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText.Length; i++) {
                        if (!string.IsNullOrEmpty(arrText[i])) {
                            currRun = editor.InsertLine(arrText[i]);
                            currRun.Paragraph.ContextualSpacing = true;
                            currRun.Paragraph.Spacing.LineSpacing = 1;
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    for (int i = 0; i < 2; i++) {
                        currRun = editor.InsertLine(" ");
                        currRun.Paragraph.ContextualSpacing = true;
                        currRun.Paragraph.Spacing.LineSpacing = 1;
                        currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 11.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                        currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    }
                    currCell.Blocks.Remove(currCell.Blocks.Last());

                    // Info
                    curRep = reps.Find(o => o.UniqueName == "PTStoAbroadOrders_Info");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currPar.Spacing.LineSpacing = 1;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText.Length; i++) {
                        if (!string.IsNullOrEmpty(arrText[i])) {
                            currRun = editor.InsertLine(arrText[i]);
                            currRun.Paragraph.ContextualSpacing = true;
                            currRun.Paragraph.Spacing.LineSpacing = 1;
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }

                    // Provider Info Titles
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Provider_Info_Title").FirstOrDefault().Paragraph.BlockContainer;
                    string curText = "\nAtt:\nFax:\nTel:";
                    arrText = curText.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currPar.Spacing.LineSpacing = 1;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText.Length; i++) {
                        if (string.IsNullOrEmpty(arrText[i])) { arrText[i] = " "; }
                        currRun = editor.InsertLine(arrText[i]);
                        currRun.Paragraph.ContextualSpacing = true;
                        currRun.Paragraph.Spacing.LineSpacing = 1;
                        currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 12.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                        currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    }
                    currRun = editor.InsertLine(" ");
                    currRun.Paragraph.ContextualSpacing = true;
                    currRun.Paragraph.Spacing.LineSpacing = 1;
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 11.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    currCell.Blocks.Remove(currCell.Blocks.Last());

                    // Provider Info
                    curRep = reps.Find(o => o.UniqueName == "PTStoAbroadOrders_Info");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Provider_Info").FirstOrDefault().Paragraph.BlockContainer;
                    curText = (string.IsNullOrEmpty(curProvider.NameEN) ? " " : curProvider.NameEN) + "\n" +
                              (string.IsNullOrEmpty(curProvider.ContactPersonEN) ? " " : curProvider.ContactPersonEN) + "\n" + 
                              (string.IsNullOrEmpty(curProvider.FAX1) ? " " : curProvider.FAX1) + "\n" + 
                              (string.IsNullOrEmpty(curProvider.Telephone1) ? " " : curProvider.Telephone1);
                    arrText = curText.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currPar.Spacing.LineSpacing = 1;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText.Length; i++) {
                        if (!string.IsNullOrEmpty(arrText[i])) {
                            currRun = editor.InsertLine(arrText[i]);
                            currRun.Paragraph.ContextualSpacing = true;
                            currRun.Paragraph.Spacing.LineSpacing = 1;
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }

                    // Subject Title
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Subject_Title").FirstOrDefault().Paragraph.BlockContainer;
                    currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                    currRun.Text = "SUBJECT:";
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    // Subject
                    curRep = reps.Find(o => o.UniqueName == "PTStoAbroadOrders_Subject");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                    currRun.Text = curRep.Text;
                    currRun.Underline.Pattern = UnderlinePattern.Single;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 13.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    // Kindly Requested
                    curRep = reps.Find(o => o.UniqueName == "PTStoAbroadOrders_Kindly_Requested");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                    currRun.Text = curRep.Text;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    // Event Place
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Event_Place").FirstOrDefault().Paragraph.BlockContainer;
                    currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                    currRun.Text = string.IsNullOrEmpty(curOrder.Event.Place.NameEN) ? curOrder.Event.Place.NameGR : curOrder.Event.Place.NameEN;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    // Event Title
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Event_Name").FirstOrDefault().Paragraph.BlockContainer;
                    currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                    currRun.Text = string.IsNullOrEmpty(curOrder.Event.NameEN) ? curOrder.Event.NameGR : curOrder.Event.NameEN;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    // Customer Name
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Customer_Name").FirstOrDefault().Paragraph.BlockContainer;
                    currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                    currRun.Text = string.IsNullOrEmpty(task2print.Customer.NameEN) ? task2print.Customer.NameGR : task2print.Customer.NameEN;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    // Date From
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Date_From").FirstOrDefault().Paragraph.BlockContainer;
                    currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                    currRun.Text = task2print.DateTimeStartActual.GetValueOrDefault().ToShortDateString();
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    // Date To
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Date_To").FirstOrDefault().Paragraph.BlockContainer;
                    currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                    currRun.Text = task2print.DateTimeEndActual.GetValueOrDefault().ToShortDateString();
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    // Requested Position
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Requested_Position").FirstOrDefault().Paragraph.BlockContainer;
                    currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                    currRun.Text = string.IsNullOrEmpty(task2print.RequestedPosition.NameEN) ? task2print.RequestedPosition.NameGR : task2print.RequestedPosition.NameEN;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    // Line Type
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Line_Type").FirstOrDefault().Paragraph.BlockContainer;
                    currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                    currRun.Text = task2print.LineType.Name;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    // Customer Type
                    CustomerB cust2 = cCont.GetCustomer(task2print.Customer.ID);
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Customer_Type").FirstOrDefault().Paragraph.BlockContainer;
                    currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                    currRun.Text = string.IsNullOrEmpty(cust2.CustomerType.NameEN) ? cust2.CustomerType.NameGR : cust2.CustomerType.NameEN;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    // Line Type 2
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Line_Type_2").FirstOrDefault().Paragraph.BlockContainer;
                    currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                    currRun.Text = task2print.LineType.Name;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    // Contact Person
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Contact_Person").FirstOrDefault().Paragraph.BlockContainer;
                    currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                    currRun.Text = (string.IsNullOrEmpty(task2print.InvoceComments) ? "" : task2print.InvoceComments) + " " + (string.IsNullOrEmpty(task2print.CorrespondentPhone) ? "" : task2print.CorrespondentPhone);
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                    // Main Text
                    bookmarkRangeStart = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().Where(rangeStart => rangeStart.Bookmark.Name == "Main_Text").FirstOrDefault();
                    editor.MoveToInlineEnd(bookmarkRangeStart);
                    curRep = reps.Find(o => o.UniqueName == "PTStoAbroadOrders_Main_Text");
                    arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' }, StringSplitOptions.None);
                    for (int i = 0; i < arrText.Length; i++) {
                        if (string.IsNullOrEmpty(arrText[i])) { arrText[i] = "   "; }
                        string[] arrTextBold = arrText[i].Split(new string[] { "/b/" }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrTextBold.Length > 1) {
                            for (int k = 0; k < arrTextBold.Length; k++) {
                                bool isBlue = false;
                                if (arrTextBold[k].StartsWith("isBlue") && arrTextBold[k].EndsWith("isBlue")) {
                                    arrTextBold[k] = arrTextBold[k].Replace("isBlue", "");
                                    isBlue = true;
                                }
                                if (k == arrTextBold.Length - 1) {
                                    currRun = editor.InsertLine(arrTextBold[k]);
                                } else {
                                    currRun = editor.InsertText(arrTextBold[k]);
                                }
                                currRun.Paragraph.ContextualSpacing = true;
                                currRun.Paragraph.Spacing.LineSpacing = 1;
                                currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Justified;
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                if (isBlue == true) { currRun.Properties.ForegroundColor.LocalValue = new ThemableColor(System.Windows.Media.Color.FromRgb(0, 0, 255)); }
                                currRun.Properties.FontSize.LocalValue = 12.0;
                                if (k % 2 == 1) {
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                } else {
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                }
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            }
                        } else {
                            currRun = editor.InsertLine(arrText[i]);
                            currRun.Paragraph.ContextualSpacing = true;
                            currRun.Paragraph.Spacing.LineSpacing = 1;
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Justified;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            if (i == 0) {
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            } else {
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                            }
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }

                    curDoc.UpdateFields();
                    string filename = "TLR " + curOrder.Event.NameEN + " @ " + curOrder.Event.Place.Country.NameEN + " " + (string.IsNullOrEmpty(task2print.Customer.NameEN) ? task2print.Customer.NameGR : task2print.Customer.NameEN);
                    exportDOCX(curDoc, filename);
                }
            }
            catch (Exception) { ShowErrorMessage(3); }
        }

    }

}