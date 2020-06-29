﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows;
using Telerik.Web.UI;
using Telerik.Web.UI.Calendar;
using Telerik.Windows.Documents.Flow.Model;
using Telerik.Windows.Documents.Flow.Model.Editing;
using Telerik.Windows.Documents.Flow.Model.Styles;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Documents.Common.FormatProviders;
using Telerik.Windows.Documents.Flow.FormatProviders.Docx;
using ExpressionParser;
using OTERT.Model;
using OTERT.Controller;
using OTERT_Entity;

namespace OTERT.Pages.Invoices {

    public partial class InvoiceShow : Page {

        protected RadDatePicker dpDateFrom, dpDateTo;
        protected RadDropDownList ddlCustomers;
        protected RadGrid gridInvoices;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected RadButton btnShow;
        protected PlaceHolder phStep1, phStep2;
        protected RadTextBox txtAccountNo;
        protected Button btnPrint, btnPrintDetail, btnPrintMail;
        protected string pageTitle, uploadedFilePath;
        protected UserB loggedUser;
        protected CheckBox chkDate, chkRoute, chkDateFrom, chkDateTo, chkTotalTime, chkTotalDistance, chkTransferCost, chkAddedCharges, chkTotalCost, chkComments;
        const string templatesFolder = "~/Templates/";
        const string fileUploadFolder = "~/UploadedFiles/";
        const string sqlUniqueName = "Invoice";
        const string sqlUniqueNameDetails = "InvoiceDet";
        const string sqlUniqueNameMail = "InvoiceMail";
        const string docTemplate = "Invoice";
        const string docTemplateDetails = "InvoiceDetails";
        const string docTemplateMail = "InvoiceMail";
        const int PTSFromGreeceID = 14;
        const int PTSToGreeceID = 13;
        const decimal fpa = 0.23M;

        protected void Page_Load(object sender, EventArgs e) {
            wizardData wData;
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Τιμολόγια > Αναζήτηση Τιμολογίου";
                dpDateFrom.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dpDateTo.SelectedDate = DateTime.Now.Date;
                wData = new wizardData();
                wData.Step = 1;
                Session["wizardStep"] = wData;
                showWizardSteps(wData);
                try {
                    CustomersController cont = new CustomersController();
                    ddlCustomers.DataSource = cont.GetCustomersForCountry(1);
                    ddlCustomers.DataTextField = "NameGR";
                    ddlCustomers.DataValueField = "ID";
                    ddlCustomers.DataBind();
                }
                catch (Exception) { }
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected void showWizardSteps(wizardData wData) {
            switch (wData.Step) {
                case 1:
                    phStep1.Visible = true;
                    phStep2.Visible = false;
                    break;
                case 2:
                    phStep1.Visible = true;
                    phStep2.Visible = true;
                    break;
                default:
                    phStep1.Visible = true;
                    phStep2.Visible = false;
                    break;
            }
        } 

        protected wizardData readWizardSteps() {
            wizardData wData = (Session["wizardStep"] != null ? (wizardData)Session["wizardStep"] : new wizardData());
            return (wData);
        }

        protected void gridInvoices_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            try {
                wizardData wData = readWizardSteps();
                InvoicesController icont = new InvoicesController();
                //gridInvoices.DataSource = icont.GetInvoices().Where(o => o.JobsMainID != PTSFromGreeceID && o.JobsMainID != PTSToGreeceID).OrderBy(o => o.Name);
                gridInvoices.DataSource = icont.GetInvoices(wData.CustomerID, wData.DateFrom, wData.DateTo, wData.Code).OrderBy(o => o.Customer.NameGR);
            }
            catch (Exception) { }
        }

        protected void btnShow_Click(object sender, EventArgs e) {
            wizardData wData = readWizardSteps();
            wData.Step = 2;
            wData.CustomerID = int.Parse(ddlCustomers.SelectedItem.Value);
            wData.DateFrom = (dpDateFrom.SelectedDate != null ? (DateTime)dpDateFrom.SelectedDate : new DateTime(1900,1,1));
            wData.DateTo = (dpDateTo.SelectedDate != null ? (DateTime)dpDateTo.SelectedDate : new DateTime(1900, 1, 1));
            wData.Code = txtAccountNo.Text.Trim();
            Session["wizardInv"] = wData;
            showWizardSteps(wData);
            gridInvoices.Rebind();
        }

        protected void gridInvoices_ItemCommand(object sender, GridCommandEventArgs e) {
            if (e.CommandName == "invPrint") {
                GridDataItem item = (GridDataItem)e.Item;
                int invoiceID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                InvoicesController iCont = new InvoicesController();
                InvoiceB curInvoice = iCont.GetInvoice(invoiceID);
                TaskLinesController tlCont = new TaskLinesController();
                List<TasksLineB> tasksForInvoice = tlCont.GetTaskLinesForInvoice(curInvoice.ID);
                List<JobB> distinctJobsForInvoice = tlCont.GetDistinctJobsForInvoice(curInvoice.ID);
                try {
                    DocumentReplacemetsController cont = new DocumentReplacemetsController();
                    List<DocumentReplacemetB> reps = new List<DocumentReplacemetB>();
                    reps = cont.GetDocumentReplacemets(sqlUniqueName);
                    DocumentReplacemetB curRep;
                    BookmarkRangeStart bookmarkRangeStart;
                    RadFlowDocument curDoc = LoadTemplateDocument(docTemplate);
                    RadFlowDocumentEditor editor = new RadFlowDocumentEditor(curDoc);
                    List<BookmarkRangeStart> docBookmarks = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().ToList();
                    Header defaultHeader = editor.Document.Sections.First().Headers.Default;
                    Footer defaultFooter = editor.Document.Sections.First().Footers.Default;
                    Telerik.Windows.Documents.Flow.Model.TableCell currCell;
                    Run currRun;
                    Paragraph currPar;
                    curRep = reps.Find(o => o.UniqueName == "Invoice_Header_Department_Title");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    string[] arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText.Length; i++) {
                        if (!string.IsNullOrEmpty(arrText[i])) {
                            currRun = editor.InsertLine(arrText[i]);
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    currCell.Blocks.Remove(currCell.Blocks.Last());
                    curRep = reps.Find(o => o.UniqueName == "Invoice_Header_Date");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Right;
                    editor.MoveToInlineEnd(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertText("Αθήνα, " + DateTime.Now.ToString(curRep.Text, new System.Globalization.CultureInfo("el-GR")));
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Right;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    curRep = reps.Find(o => o.UniqueName == "Invoice_Footer_Date");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineEnd(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertText(DateTime.Now.ToString(curRep.Text, new System.Globalization.CultureInfo("el-GR")));
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    curRep = reps.Find(o => o.UniqueName == "Invoice_Footer_PageNo");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    editor.MoveToParagraphStart((Paragraph)currCell.Blocks.First());
                    editor.InsertText("Σελίδα ");
                    editor.InsertField("PAGE", "3");
                    if (curRep.Text == "Σελίδα Χ από Υ") {
                        currRun = editor.InsertText(" από ");
                        editor.InsertField("NUMPAGES", "5");
                    }
                    curRep = reps.Find(o => o.UniqueName == "Invoice_Title");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertLine(curRep.Text);
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    currCell.Blocks.Remove(currCell.Blocks.Last());
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Date_From").FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertLine(curInvoice.DateFrom.GetValueOrDefault().ToString("dd/MM/yyyy", new System.Globalization.CultureInfo("el-GR")));
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    currCell.Blocks.Remove(currCell.Blocks.Last());
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Date_To").FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertLine(curInvoice.DateTo.GetValueOrDefault().ToString("dd/MM/yyyy", new System.Globalization.CultureInfo("el-GR")));
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    currCell.Blocks.Remove(currCell.Blocks.Last());
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Reg_No").FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertLine(curInvoice.RegNo);
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Center;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    currCell.Blocks.Remove(currCell.Blocks.Last());
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Date_Created").FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertLine(curInvoice.DateCreated.GetValueOrDefault().ToString("dd/MM/yyyy", new System.Globalization.CultureInfo("el-GR")));
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    currCell.Blocks.Remove(currCell.Blocks.Last());
                    string custTitle = curInvoice.Customer.NamedInvoiceGR + "#" + curInvoice.Customer.Address1GR + " " + curInvoice.Customer.Address2GR + "#" + curInvoice.Customer.ZIPCode + ", " + curInvoice.Customer.CityGR + "#" + "ΔΟΥ: " + curInvoice.Customer.DOY + "#" + "ΑΦΜ: " + curInvoice.Customer.AFM;
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Customer").FirstOrDefault().Paragraph.BlockContainer;
                    string[] arrText2 = custTitle.Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText2.Length; i++) {
                        if (!string.IsNullOrEmpty(arrText2[i])) {
                            currRun = editor.InsertLine(arrText2[i]);
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    currCell.Blocks.Remove(currCell.Blocks.Last());
                    bookmarkRangeStart = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().Where(rangeStart => rangeStart.Bookmark.Name == "Main_Table").FirstOrDefault();
                    editor.MoveToInlineEnd(bookmarkRangeStart);
                    Telerik.Windows.Documents.Flow.Model.Table tblContent = editor.InsertTable();
                    tblContent.StyleId = "TableStyle";
                    tblContent.LayoutType = TableLayoutType.AutoFit;
                    ThemableColor cellBackground = new ThemableColor(System.Windows.Media.Colors.Beige);
                    Telerik.Windows.Documents.Flow.Model.TableRow row = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 5; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell9 = row.Cells.AddTableCell();
                            currRun = cell9.Blocks.AddParagraph().Inlines.AddRun("ΤΕΛΟΣ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            cell9.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 10);
                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell9 = row.Cells.AddTableCell();
                            currRun = cell9.Blocks.AddParagraph().Inlines.AddRun("ΠΕΡΙΓΡΑΦΗ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            cell9.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 45);
                        } else if (j == 2) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell9 = row.Cells.AddTableCell();
                            currRun = cell9.Blocks.AddParagraph().Inlines.AddRun("ΠΟΣΟ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            cell9.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 15);
                        } else if (j == 3) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell9 = row.Cells.AddTableCell();
                            currRun = cell9.Blocks.AddParagraph().Inlines.AddRun("ΕΚΠΤΩΣΗ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            cell9.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 15);
                        } else if (j == 4) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell9 = row.Cells.AddTableCell();
                            currRun = cell9.Blocks.AddParagraph().Inlines.AddRun("ΜΕΤΑ ΤΗΝ ΕΚΠΤΩΣΗ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            cell9.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 15);
                        }
                    }
                    List<JobB> urbanJobs = distinctJobsForInvoice.Where(o => o.JobTypesID == 1).ToList();
                    List<JobB> ldJobs = distinctJobsForInvoice.Where(o => o.JobTypesID == 2).ToList();
                    List<JobB> otherJobs = distinctJobsForInvoice.Where(o => o.JobTypesID == 3).ToList();
                    decimal totalCost = 0;
                    decimal totalCostUrban = 0;
                    decimal totalCostLD = 0;
                    decimal totalCostOther = 0;
                    if (urbanJobs.Count != distinctJobsForInvoice.Count && ldJobs.Count != distinctJobsForInvoice.Count && otherJobs.Count != distinctJobsForInvoice.Count) {
                        for (int k = 0; k < 3; k++) {
                            if (k == 0 && urbanJobs.Count > 0) {
                                foreach (JobB curJob in urbanJobs) {
                                    List<TasksLineB> taskLinesForCurrentJob = tasksForInvoice.Where(o => o.JobID == curJob.ID).ToList();
                                    decimal totalCostForJob = 0;
                                    foreach (TasksLineB curTaskLine in taskLinesForCurrentJob) {
                                        totalCostForJob += curTaskLine.Task.CostActual.GetValueOrDefault();
                                    }
                                    totalCostUrban += totalCostForJob;
                                    totalCost += totalCostForJob;
                                    Telerik.Windows.Documents.Flow.Model.TableRow row2 = tblContent.Rows.AddTableRow();
                                    for (int j = 0; j < 5; j++) {
                                        Telerik.Windows.Documents.Flow.Model.TableCell cell11 = row2.Cells.AddTableCell();
                                        if (j == 0) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(curJob.InvoiceCode);
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 1) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(curJob.Name);
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 2) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(totalCostForJob.ToString("0.00"));
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 3) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun("00.00");
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 4) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(totalCostForJob.ToString("0.00"));
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        }
                                    }
                                }
                                Telerik.Windows.Documents.Flow.Model.TableRow row3331 = tblContent.Rows.AddTableRow();
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3331 = row3331.Cells.AddTableCell();
                                cell3331.ColumnSpan = 3;
                                currPar = cell3331.Blocks.AddParagraph();
                                currPar.Properties.TextAlignment.LocalValue = Alignment.Right;
                                currRun = currPar.Inlines.AddRun("ΣΥΝΟΛΟ ΑΣΤΙΚΩΝ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3332 = row3331.Cells.AddTableCell();
                                currRun = cell3332.Blocks.AddParagraph().Inlines.AddRun(" ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3333 = row3331.Cells.AddTableCell();
                                currRun = cell3333.Blocks.AddParagraph().Inlines.AddRun(totalCostUrban.ToString("0.00"));
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            } else if (k == 1 && ldJobs.Count > 0) {
                                foreach (JobB curJob in ldJobs) {
                                    List<TasksLineB> taskLinesForCurrentJob = tasksForInvoice.Where(o => o.JobID == curJob.ID).ToList();
                                    decimal totalCostForJob = 0;
                                    foreach (TasksLineB curTaskLine in taskLinesForCurrentJob) {
                                        totalCostForJob += curTaskLine.Task.CostActual.GetValueOrDefault();
                                    }
                                    totalCostLD += totalCostForJob;
                                    totalCost += totalCostForJob;
                                    Telerik.Windows.Documents.Flow.Model.TableRow row2 = tblContent.Rows.AddTableRow();
                                    for (int j = 0; j < 5; j++) {
                                        Telerik.Windows.Documents.Flow.Model.TableCell cell11 = row2.Cells.AddTableCell();
                                        if (j == 0) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(curJob.InvoiceCode);
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 1) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(curJob.Name);
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 2) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(totalCostForJob.ToString("0.00"));
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 3) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun("00.00");
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 4) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(totalCostForJob.ToString("0.00"));
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        }
                                    }
                                }
                                Telerik.Windows.Documents.Flow.Model.TableRow row3332 = tblContent.Rows.AddTableRow();
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3331 = row3332.Cells.AddTableCell();
                                cell3331.ColumnSpan = 3; currPar = cell3331.Blocks.AddParagraph();
                                currPar.Properties.TextAlignment.LocalValue = Alignment.Right;
                                currRun = currPar.Inlines.AddRun("ΣΥΝΟΛΟ ΥΠΕΡΑΣΤΙΚΩΝ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3332 = row3332.Cells.AddTableCell();
                                currRun = cell3332.Blocks.AddParagraph().Inlines.AddRun(" ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3333 = row3332.Cells.AddTableCell();
                                currRun = cell3333.Blocks.AddParagraph().Inlines.AddRun(totalCostLD.ToString("0.00"));
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            } else if (k == 2 && otherJobs.Count > 0) {
                                foreach (JobB curJob in otherJobs) {
                                    List<TasksLineB> taskLinesForCurrentJob = tasksForInvoice.Where(o => o.JobID == curJob.ID).ToList();
                                    decimal totalCostForJob = 0;
                                    foreach (TasksLineB curTaskLine in taskLinesForCurrentJob) {
                                        totalCostForJob += curTaskLine.Task.CostActual.GetValueOrDefault();
                                    }
                                    totalCostOther += totalCostForJob;
                                    totalCost += totalCostForJob;
                                    Telerik.Windows.Documents.Flow.Model.TableRow row2 = tblContent.Rows.AddTableRow();
                                    for (int j = 0; j < 5; j++) {
                                        Telerik.Windows.Documents.Flow.Model.TableCell cell11 = row2.Cells.AddTableCell();
                                        if (j == 0) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(curJob.InvoiceCode);
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 1) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(curJob.Name);
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 2) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(totalCostForJob.ToString("0.00"));
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 3) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun("00.00");
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 4) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(totalCostForJob.ToString("0.00"));
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        }
                                    }
                                }
                                Telerik.Windows.Documents.Flow.Model.TableRow row3332 = tblContent.Rows.AddTableRow();
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3331 = row3332.Cells.AddTableCell();
                                cell3331.ColumnSpan = 3; currPar = cell3331.Blocks.AddParagraph();
                                currPar.Properties.TextAlignment.LocalValue = Alignment.Right;
                                currRun = currPar.Inlines.AddRun("ΣΥΝΟΛΟ ΛΟΙΠΩΝ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3332 = row3332.Cells.AddTableCell();
                                currRun = cell3332.Blocks.AddParagraph().Inlines.AddRun(" ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3333 = row3332.Cells.AddTableCell();
                                currRun = cell3333.Blocks.AddParagraph().Inlines.AddRun(totalCostOther.ToString("0.00"));
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            }
                        }
                    } else {
                        foreach (JobB curJob in distinctJobsForInvoice) {
                            List<TasksLineB> taskLinesForCurrentJob = tasksForInvoice.Where(o => o.JobID == curJob.ID).ToList();
                            decimal totalCostForJob = 0;
                            foreach (TasksLineB curTaskLine in taskLinesForCurrentJob) {
                                totalCostForJob += curTaskLine.Task.CostActual.GetValueOrDefault();
                            }
                            totalCost += totalCostForJob;
                            Telerik.Windows.Documents.Flow.Model.TableRow row2 = tblContent.Rows.AddTableRow();
                            for (int j = 0; j < 5; j++) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell11 = row2.Cells.AddTableCell();
                                if (j == 0) {
                                    currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(curJob.InvoiceCode);
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 1) {
                                    currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(curJob.Name);
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 2) {
                                    currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(totalCostForJob.ToString("0.00"));
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 3) {
                                    currRun = cell11.Blocks.AddParagraph().Inlines.AddRun("00.00");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 4) {
                                    currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(totalCostForJob.ToString("0.00"));
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                }
                            }
                        }
                    }
                    Telerik.Windows.Documents.Flow.Model.TableRow row3 = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 4; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell12 = row3.Cells.AddTableCell();
                            cell12.ColumnSpan = 2;
                            currRun = cell12.Blocks.AddParagraph().Inlines.AddRun("ΣΥΝΟΛΟ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell12 = row3.Cells.AddTableCell();
                            currRun = cell12.Blocks.AddParagraph().Inlines.AddRun(totalCost.ToString("0.00"));
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 2) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell12 = row3.Cells.AddTableCell();
                            currRun = cell12.Blocks.AddParagraph().Inlines.AddRun("00.00");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 3) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell12 = row3.Cells.AddTableCell();
                            currRun = cell12.Blocks.AddParagraph().Inlines.AddRun(totalCost.ToString("0.00"));
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    Telerik.Windows.Documents.Flow.Model.TableRow row4 = tblContent.Rows.AddTableRow();
                    decimal fpaTotalCost = totalCost * fpa;
                    for (int j = 0; j < 2; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell13 = row4.Cells.AddTableCell();
                            cell13.ColumnSpan = 4;
                            currRun = cell13.Blocks.AddParagraph().Inlines.AddRun("ΦΠΑ:  ");
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Right;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell13 = row4.Cells.AddTableCell();
                            currRun = cell13.Blocks.AddParagraph().Inlines.AddRun(fpaTotalCost.ToString("0.00"));
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    Telerik.Windows.Documents.Flow.Model.TableRow row5 = tblContent.Rows.AddTableRow();
                    decimal totalCostwithFPA = totalCost + fpaTotalCost;
                    for (int j = 0; j < 2; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell14 = row5.Cells.AddTableCell();
                            cell14.ColumnSpan = 4;
                            currRun = cell14.Blocks.AddParagraph().Inlines.AddRun("ΟΦΕΙΛΟΜΕΝΟ:  ");
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Right;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell14 = row5.Cells.AddTableCell();
                            currRun = cell14.Blocks.AddParagraph().Inlines.AddRun(totalCostwithFPA.ToString("0.00"));
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    Telerik.Windows.Documents.Flow.Model.TableRow row6 = tblContent.Rows.AddTableRow();
                    Telerik.Windows.Documents.Flow.Model.TableCell cell = row6.Cells.AddTableCell();
                    cell.ColumnSpan = 5;
                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("  ");
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Right;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    Telerik.Windows.Documents.Flow.Model.TableRow row21 = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 3; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell21 = row21.Cells.AddTableCell();
                            cell21.ColumnSpan = 2;
                            currRun = cell21.Blocks.AddParagraph().Inlines.AddRun(" ");
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Right;
                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell21 = row21.Cells.AddTableCell();
                            cell21.ColumnSpan = 2;
                            currRun = cell21.Blocks.AddParagraph().Inlines.AddRun("ΑΞΙΑ ΠΡΟ ΕΚΠΤΩΣΗΣ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 2) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell21 = row21.Cells.AddTableCell();
                            currRun = cell21.Blocks.AddParagraph().Inlines.AddRun(totalCost.ToString("0.00"));
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    Telerik.Windows.Documents.Flow.Model.TableRow row22 = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 3; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell22 = row22.Cells.AddTableCell();
                            cell22.ColumnSpan = 2;
                            currRun = cell22.Blocks.AddParagraph().Inlines.AddRun(" ");
                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell22 = row22.Cells.AddTableCell();
                            cell22.ColumnSpan = 2;
                            currRun = cell22.Blocks.AddParagraph().Inlines.AddRun("ΕΚΠΤΩΣΗ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 2) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell22 = row22.Cells.AddTableCell();
                            currRun = cell22.Blocks.AddParagraph().Inlines.AddRun("00.00");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    Telerik.Windows.Documents.Flow.Model.TableRow row23 = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 3; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell23 = row23.Cells.AddTableCell();
                            cell23.ColumnSpan = 2;
                            currRun = cell23.Blocks.AddParagraph().Inlines.AddRun(" ");
                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell23 = row23.Cells.AddTableCell();
                            cell23.ColumnSpan = 2;
                            currRun = cell23.Blocks.AddParagraph().Inlines.AddRun("ΑΞΙΑ ΜΕΤΑ ΤΗΝ ΕΚΠTΩΣΗ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 2) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell23 = row23.Cells.AddTableCell();
                            currRun = cell23.Blocks.AddParagraph().Inlines.AddRun(totalCost.ToString("0.00"));
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    Telerik.Windows.Documents.Flow.Model.TableRow row24 = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 3; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell24 = row24.Cells.AddTableCell();
                            cell24.ColumnSpan = 2;
                            currRun = cell24.Blocks.AddParagraph().Inlines.AddRun(" ");
                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell24 = row24.Cells.AddTableCell();
                            cell24.ColumnSpan = 2;
                            currRun = cell24.Blocks.AddParagraph().Inlines.AddRun("ΦΠΑ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 2) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell24 = row24.Cells.AddTableCell();
                            currRun = cell24.Blocks.AddParagraph().Inlines.AddRun(fpaTotalCost.ToString("0.00"));
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    Telerik.Windows.Documents.Flow.Model.TableRow row25 = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 3; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell25 = row25.Cells.AddTableCell();
                            cell25.ColumnSpan = 2;
                            currRun = cell25.Blocks.AddParagraph().Inlines.AddRun(" ");
                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell25 = row25.Cells.AddTableCell();
                            cell25.ColumnSpan = 2;
                            currRun = cell25.Blocks.AddParagraph().Inlines.AddRun("ΓΕΝΙΚΟ ΣΥΝΟΛΟ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 2) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell25 = row25.Cells.AddTableCell();
                            currRun = cell25.Blocks.AddParagraph().Inlines.AddRun(totalCostwithFPA.ToString("0.00"));
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    curRep = reps.Find(o => o.UniqueName == "Invoice_Chief_Name");
                    DocumentReplacemetB curRep2 = reps.Find(o => o.UniqueName == "Invoice_Chief_Title");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    string chiefData = curRep.Text + "#" + curRep2.Text;
                    string[] arrText3 = chiefData.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText3.Length; i++) {
                        if (!string.IsNullOrEmpty(arrText3[i])) {
                            currRun = editor.InsertLine(arrText3[i]);
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Center;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    curDoc.UpdateFields();
                    string filename = "Invoice_" + curInvoice.Customer.NamedInvoiceGR.Replace(" ", "_") + "_from_" + curInvoice.DateFrom.GetValueOrDefault().ToString("dd-MM-yyyy") + "_to_" + curInvoice.DateTo.GetValueOrDefault().ToString("dd-MM-yyyy");
                    exportDOCX(curDoc, filename);
                }
                catch (Exception ex) { }
            } 
            else if (e.CommandName == "invPrintDetail") {
                GridDataItem item = (GridDataItem)e.Item;
                int invoiceID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                InvoicesController iCont = new InvoicesController();
                InvoiceB curInvoice = iCont.GetInvoice(invoiceID);
                TaskLinesController tlCont = new TaskLinesController();
                List<TasksLineB> tasksForInvoice = tlCont.GetTaskLinesForInvoice(curInvoice.ID);
                List<JobB> distinctJobsForInvoice = tlCont.GetDistinctJobsForInvoice(curInvoice.ID);
                bool[] visibleColumns = getVisibleColumns();
                try {
                    DocumentReplacemetsController cont = new DocumentReplacemetsController();
                    List<DocumentReplacemetB> reps = new List<DocumentReplacemetB>();
                    reps = cont.GetDocumentReplacemets(sqlUniqueNameDetails);
                    DocumentReplacemetB curRep;
                    BookmarkRangeStart bookmarkRangeStart;
                    RadFlowDocument curDoc = LoadTemplateDocument(docTemplateDetails);
                    RadFlowDocumentEditor editor = new RadFlowDocumentEditor(curDoc);
                    List<BookmarkRangeStart> docBookmarks = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().ToList();
                    Header defaultHeader = editor.Document.Sections.First().Headers.Default;
                    Footer defaultFooter = editor.Document.Sections.First().Footers.Default;
                    Telerik.Windows.Documents.Flow.Model.TableCell currCell;
                    Run currRun;
                    Paragraph currPar;
                    curRep = reps.Find(o => o.UniqueName == "InvoiceDet_Header_OTE");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertLine(curRep.Text);
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Center;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 13.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    curRep = reps.Find(o => o.UniqueName == "InvoiceDet_Header_Department");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    string[] arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText.Length; i++) {
                        if (!string.IsNullOrEmpty(arrText[i])) {
                            currRun = editor.InsertLine(arrText[i]);
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 13.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    currCell.Blocks.Remove(currCell.Blocks.Last());
                    curRep = reps.Find(o => o.UniqueName == "InvoiceDet_Header_Date");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineEnd(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertText("ΑΘΗΝΑ, " + DateTime.Now.ToString(curRep.Text, new System.Globalization.CultureInfo("el-GR")));
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 13.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    curRep = reps.Find(o => o.UniqueName == "InvoiceDet_Info");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    string[] arrText2 = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText2.Length; i++) {
                        if (!string.IsNullOrEmpty(arrText2[i])) {
                            currRun = editor.InsertLine(arrText2[i]);
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 13.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    currCell.Blocks.Remove(currCell.Blocks.Last());
                    curRep = reps.Find(o => o.UniqueName == "InvoiceDet_Footer_PageNo");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    editor.MoveToParagraphStart((Paragraph)currCell.Blocks.First());
                    editor.InsertText("ΣΕΛΙΔΑ ");
                    editor.InsertField("PAGE", "3");
                    if (curRep.Text == "Σελίδα Χ από Υ") {
                        currRun = editor.InsertText(" ΑΠΟ ");
                        editor.InsertField("NUMPAGES", "5");
                    }
                    bookmarkRangeStart = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().Where(rangeStart => rangeStart.Bookmark.Name == "Main_Table").FirstOrDefault();
                    editor.MoveToInlineEnd(bookmarkRangeStart);
                    JobB lastJob = distinctJobsForInvoice.Last();
                    foreach (JobB curJob in distinctJobsForInvoice) {
                        currPar = insertParagraph(editor);
                        currPar.TextAlignment = Alignment.Left;
                        currRun = editor.InsertText("T/O ΜΕΤΑΔΟΣΕΙΣ ΕΣΩΤΕΡΙΚΟΥ ΑΠΟ " + curInvoice.DateFrom.GetValueOrDefault().ToString("dd/MM/yyyy") + " ΕΩΣ " + curInvoice.DateTo.GetValueOrDefault().ToString("dd/MM/yyyy"));
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 15.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                        currPar = insertParagraph(editor);
                        if (!string.IsNullOrEmpty(curInvoice.Customer.NamedInvoiceGR)) {
                            currPar.TextAlignment = Alignment.Left;
                            currRun = editor.InsertText(curInvoice.Customer.NamedInvoiceGR);
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 15.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currPar = insertParagraph(editor);
                        }
                        currPar.TextAlignment = Alignment.Left;
                        currRun = editor.InsertText(curJob.Name);
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 15.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                        currPar = insertParagraph(editor);
                        Telerik.Windows.Documents.Flow.Model.Table tblContent = editor.InsertTable();
                        tblContent.StyleId = "TableStyle";
                        tblContent.LayoutType = TableLayoutType.AutoFit;
                        ThemableColor cellBackground = new ThemableColor(System.Windows.Media.Colors.Beige);
                        Telerik.Windows.Documents.Flow.Model.TableRow row = tblContent.Rows.AddTableRow();
                        for (int j = 0; j < 10; j++) {
                            if (j == 0 && visibleColumns[j] == true) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row.Cells.AddTableCell();
                                currRun = cell1.Blocks.AddParagraph().Inlines.AddRun("ΗΜΕΡ/ΝΙΑ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 10.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                cell1.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 10);
                            } else if (j == 1 && visibleColumns[j] == true) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row.Cells.AddTableCell();
                                currRun = cell1.Blocks.AddParagraph().Inlines.AddRun("ΔΙΑΔΡΟΜΗ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 10.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                cell1.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 10);
                            } else if (j == 2 && visibleColumns[j] == true) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row.Cells.AddTableCell();
                                currRun = cell1.Blocks.AddParagraph().Inlines.AddRun("ΑΠΟ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 10.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                cell1.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 10);
                            } else if (j == 3 && visibleColumns[j] == true) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row.Cells.AddTableCell();
                                currRun = cell1.Blocks.AddParagraph().Inlines.AddRun("ΕΩΣ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 10.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                cell1.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 10);
                            } else if (j == 4 && visibleColumns[j] == true) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row.Cells.AddTableCell();
                                currRun = cell1.Blocks.AddParagraph().Inlines.AddRun("ΔΙΑΡΚΕΙΑ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 10.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                cell1.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 10);
                            } else if (j == 5 && visibleColumns[j] == true) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row.Cells.AddTableCell();
                                currRun = cell1.Blocks.AddParagraph().Inlines.AddRun("ΑΠΟΣΤΑΣΗ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 10.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                cell1.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 10);
                            } else if (j == 6 && visibleColumns[j] == true) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row.Cells.AddTableCell();
                                currRun = cell1.Blocks.AddParagraph().Inlines.AddRun("ΤΕΛΟΣ ΜΕΤΑΔΟΣΗΣ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 10.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                cell1.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 10);
                            } else if (j == 7 && visibleColumns[j] == true) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row.Cells.AddTableCell();
                                currRun = cell1.Blocks.AddParagraph().Inlines.AddRun("ΔΙΑΦΟΡΕΣ ΧΡΕΩΣΕΙΣ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 10.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                cell1.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 10);
                            } else if (j == 8 && visibleColumns[j] == true) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row.Cells.AddTableCell();
                                currRun = cell1.Blocks.AddParagraph().Inlines.AddRun("ΣΥΝΟΛΟ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 10.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                cell1.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 10);
                            } else if (j == 9 && visibleColumns[j] == true) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row.Cells.AddTableCell();
                                currRun = cell1.Blocks.AddParagraph().Inlines.AddRun("ΠΑΡΑΤΗΡΗΣΕΙΣ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 10.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                cell1.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 10);
                            }
                        }
                        List<TasksLineB> taskLinesForCurrentJob = tasksForInvoice.Where(o => o.JobID == curJob.ID).ToList();
                        decimal totalCostForJob = 0;
                        decimal totalCalcCostForJob = 0;
                        int totalDistance = 0;
                        decimal totalAddedValues = 0;
                        foreach (TasksLineB curTaskLine in taskLinesForCurrentJob) {
                            totalCostForJob += curTaskLine.Task.CostActual.GetValueOrDefault();
                            totalCalcCostForJob += curTaskLine.Task.CostCalculated.GetValueOrDefault();
                            totalDistance += curTaskLine.Task.Distance.KM;
                            totalAddedValues += curTaskLine.Task.AddedCharges.GetValueOrDefault();
                            Telerik.Windows.Documents.Flow.Model.TableRow row2 = tblContent.Rows.AddTableRow();
                            for (int j = 0; j < 10; j++) {
                                if (j == 0 && visibleColumns[j] == true) {
                                    Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row2.Cells.AddTableCell();
                                    currRun = cell1.Blocks.AddParagraph().Inlines.AddRun(curTaskLine.Task.DateTimeStartActual.GetValueOrDefault().ToString("d/M/yyyy"));
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 10.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 1 && visibleColumns[j] == true) {
                                    Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row2.Cells.AddTableCell();
                                    currRun = cell1.Blocks.AddParagraph().Inlines.AddRun(curTaskLine.Task.Distance.Position1 + " - " + curTaskLine.Task.Distance.Position2);
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 10.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 2 && visibleColumns[j] == true) {
                                    Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row2.Cells.AddTableCell();
                                    currRun = cell1.Blocks.AddParagraph().Inlines.AddRun(curTaskLine.Task.DateTimeStartActual.GetValueOrDefault().ToString("d/M/yyyy H:mm"));
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 10.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 3 && visibleColumns[j] == true) {
                                    Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row2.Cells.AddTableCell();
                                    currRun = cell1.Blocks.AddParagraph().Inlines.AddRun(curTaskLine.Task.DateTimeEndActual.GetValueOrDefault().ToString("d/M/yyyy H:mm"));
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 10.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 4 && visibleColumns[j] == true) {
                                    Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row2.Cells.AddTableCell();
                                    currRun = cell1.Blocks.AddParagraph().Inlines.AddRun(curTaskLine.Task.DateTimeDurationActual.GetValueOrDefault().ToString());
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 10.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 5 && visibleColumns[j] == true) {
                                    Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row2.Cells.AddTableCell();
                                    currRun = cell1.Blocks.AddParagraph().Inlines.AddRun(curTaskLine.Task.Distance.KM.ToString());
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 10.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 6 && visibleColumns[j] == true) {
                                    Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row2.Cells.AddTableCell();
                                    decimal calCost = curTaskLine.Task.CostCalculated.GetValueOrDefault();
                                    currRun = cell1.Blocks.AddParagraph().Inlines.AddRun(calCost.ToString());
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 10.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 7 && visibleColumns[j] == true) {
                                    Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row2.Cells.AddTableCell();
                                    currRun = cell1.Blocks.AddParagraph().Inlines.AddRun(curTaskLine.Task.AddedCharges.GetValueOrDefault().ToString());
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 10.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 8 && visibleColumns[j] == true) {
                                    Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row2.Cells.AddTableCell();
                                    currRun = cell1.Blocks.AddParagraph().Inlines.AddRun(curTaskLine.Task.CostActual.GetValueOrDefault().ToString());
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 10.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 9 && visibleColumns[j] == true) {
                                    Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row2.Cells.AddTableCell();
                                    if (!string.IsNullOrEmpty(curTaskLine.Task.InvoceComments)) {
                                        currRun = cell1.Blocks.AddParagraph().Inlines.AddRun(curTaskLine.Task.InvoceComments);
                                    } else {
                                        currRun = cell1.Blocks.AddParagraph().Inlines.AddRun(" ");
                                    }
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 10.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                }
                            }

                        }
                        Telerik.Windows.Documents.Flow.Model.TableRow row3 = tblContent.Rows.AddTableRow();
                        for (int j = 0; j < visibleColumns.Length; j++) {
                            if (j == 0 && getSumColSpan(visibleColumns) > 0) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row3.Cells.AddTableCell();
                                cell1.ColumnSpan = getSumColSpan(visibleColumns);
                                string kykloma = "κυκλώματα";
                                if (taskLinesForCurrentJob.Count < 2) { kykloma = "κύκλωμα"; }
                                currRun = cell1.Blocks.AddParagraph().Inlines.AddRun("Σύνολο για " + curJob.Name + " (" + taskLinesForCurrentJob.Count.ToString() + " " + kykloma + ")");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 10.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            } else if (j == 1 && visibleColumns[j + 4] == true) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row3.Cells.AddTableCell();
                                currRun = cell1.Blocks.AddParagraph().Inlines.AddRun(totalDistance.ToString());
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 10.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            } else if (j == 2 && visibleColumns[j + 4] == true) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row3.Cells.AddTableCell();
                                currRun = cell1.Blocks.AddParagraph().Inlines.AddRun(totalCalcCostForJob.ToString());
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 10.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            } else if (j == 3 && visibleColumns[j + 4] == true) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row3.Cells.AddTableCell();
                                currRun = cell1.Blocks.AddParagraph().Inlines.AddRun(totalAddedValues.ToString());
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 10.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            } else if (j == 4 && visibleColumns[j + 4] == true) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row3.Cells.AddTableCell();
                                currRun = cell1.Blocks.AddParagraph().Inlines.AddRun(totalCostForJob.ToString());
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 10.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            } else if (j == 5 && visibleColumns[j + 4] == true) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell1 = row3.Cells.AddTableCell();
                                currRun = cell1.Blocks.AddParagraph().Inlines.AddRun(" ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 10.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            }
                        }
                        currPar = insertParagraph(editor);
                        currPar = insertParagraph(editor);
                        curRep = reps.Find(o => o.UniqueName == "InvoiceDet_Chief_Name");
                        DocumentReplacemetB curRep2 = reps.Find(o => o.UniqueName == "InvoiceDet_Chief_Title");
                        currPar = insertParagraph(editor);
                        currPar.TextAlignment = Alignment.Right;
                        currRun = editor.InsertLine(curRep.Text);
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 12.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                        currPar = insertParagraph(editor);
                        currPar.TextAlignment = Alignment.Right;
                        currRun = editor.InsertLine(curRep2.Text);
                        currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                        currRun.Properties.FontSize.LocalValue = 12.0;
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                        if (!curJob.Equals(lastJob)) { Break br = editor.InsertBreak(BreakType.PageBreak); }
                    }
                    curDoc.UpdateFields();
                    string filename = "Invoice_" + curInvoice.Customer.NamedInvoiceGR.Replace(" ", "_") + "_from_" + curInvoice.DateFrom.GetValueOrDefault().ToString("dd-MM-yyyy") + "_to_" + curInvoice.DateTo.GetValueOrDefault().ToString("dd-MM-yyyy") + "_(Analytical)";
                    exportDOCX(curDoc, filename);
                }
                catch (Exception ex) { }
            } 
            else if (e.CommandName == "invPrintMail") {
                string test1 = Utilities.ConvertToText(11243.15.ToString());
                string test2 = Utilities.ConvertToText(1203.78.ToString());
                string test3 = Utilities.ConvertToText(3240.5.ToString());
                GridDataItem item = (GridDataItem)e.Item;
                int invoiceID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                InvoicesController iCont = new InvoicesController();
                InvoiceB curInvoice = iCont.GetInvoice(invoiceID);
                TaskLinesController tlCont = new TaskLinesController();
                List<TasksLineB> tasksForInvoice = tlCont.GetTaskLinesForInvoice(curInvoice.ID);
                List<JobB> distinctJobsForInvoice = tlCont.GetDistinctJobsForInvoice(curInvoice.ID);
                try {
                    DocumentReplacemetsController cont = new DocumentReplacemetsController();
                    List<DocumentReplacemetB> reps = new List<DocumentReplacemetB>();
                    reps = cont.GetDocumentReplacemets(sqlUniqueNameMail);
                    DocumentReplacemetB curRep;
                    BookmarkRangeStart bookmarkRangeStart;
                    RadFlowDocument curDoc = LoadTemplateDocument(docTemplateMail);
                    RadFlowDocumentEditor editor = new RadFlowDocumentEditor(curDoc);
                    List<BookmarkRangeStart> docBookmarks = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().ToList();
                    Header defaultHeader = editor.Document.Sections.First().Headers.Default;
                    Footer defaultFooter = editor.Document.Sections.First().Footers.Default;
                    Telerik.Windows.Documents.Flow.Model.TableCell currCell;
                    Run currRun;
                    Paragraph currPar;
                    curRep = reps.Find(o => o.UniqueName == "Invoice_Header_Department_Title");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    string[] arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText.Length; i++) {
                        if (!string.IsNullOrEmpty(arrText[i])) {
                            currRun = editor.InsertLine(arrText[i]);
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    currCell.Blocks.Remove(currCell.Blocks.Last());
                    curRep = reps.Find(o => o.UniqueName == "Invoice_Header_Date");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Right;
                    editor.MoveToInlineEnd(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertText("Αθήνα, " + DateTime.Now.ToString(curRep.Text, new System.Globalization.CultureInfo("el-GR")));
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Right;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    curRep = reps.Find(o => o.UniqueName == "Invoice_Footer_Date");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineEnd(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertText(DateTime.Now.ToString(curRep.Text, new System.Globalization.CultureInfo("el-GR")));
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    curRep = reps.Find(o => o.UniqueName == "Invoice_Footer_PageNo");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    editor.MoveToParagraphStart((Paragraph)currCell.Blocks.First());
                    editor.InsertText("Σελίδα ");
                    editor.InsertField("PAGE", "3");
                    if (curRep.Text == "Σελίδα Χ από Υ") {
                        currRun = editor.InsertText(" από ");
                        editor.InsertField("NUMPAGES", "5");
                    }
                    curRep = reps.Find(o => o.UniqueName == "Invoice_Title");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertLine(curRep.Text);
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    currCell.Blocks.Remove(currCell.Blocks.Last());
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Date_From").FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertLine(curInvoice.DateFrom.GetValueOrDefault().ToString("dd/MM/yyyy", new System.Globalization.CultureInfo("el-GR")));
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    currCell.Blocks.Remove(currCell.Blocks.Last());
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Date_To").FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertLine(curInvoice.DateTo.GetValueOrDefault().ToString("dd/MM/yyyy", new System.Globalization.CultureInfo("el-GR")));
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    currCell.Blocks.Remove(currCell.Blocks.Last());
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Reg_No").FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertLine(curInvoice.RegNo);
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Center;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    currCell.Blocks.Remove(currCell.Blocks.Last());
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Date_Created").FirstOrDefault().Paragraph.BlockContainer;
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    currRun = editor.InsertLine(curInvoice.DateCreated.GetValueOrDefault().ToString("dd/MM/yyyy", new System.Globalization.CultureInfo("el-GR")));
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    currCell.Blocks.Remove(currCell.Blocks.Last());
                    string custTitle = curInvoice.Customer.NamedInvoiceGR + "#" + curInvoice.Customer.Address1GR + " " + curInvoice.Customer.Address2GR + "#" + curInvoice.Customer.ZIPCode + ", " + curInvoice.Customer.CityGR + "#" + "ΔΟΥ: " + curInvoice.Customer.DOY + "#" + "ΑΦΜ: " + curInvoice.Customer.AFM;
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == "Customer").FirstOrDefault().Paragraph.BlockContainer;
                    string[] arrText2 = custTitle.Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText2.Length; i++) {
                        if (!string.IsNullOrEmpty(arrText2[i])) {
                            currRun = editor.InsertLine(arrText2[i]);
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    currCell.Blocks.Remove(currCell.Blocks.Last());
                    bookmarkRangeStart = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().Where(rangeStart => rangeStart.Bookmark.Name == "Main_Table").FirstOrDefault();
                    editor.MoveToInlineEnd(bookmarkRangeStart);
                    Telerik.Windows.Documents.Flow.Model.Table tblContent = editor.InsertTable();
                    tblContent.StyleId = "TableStyle";
                    tblContent.LayoutType = TableLayoutType.AutoFit;
                    ThemableColor cellBackground = new ThemableColor(System.Windows.Media.Colors.Beige);
                    Telerik.Windows.Documents.Flow.Model.TableRow row = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 5; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell9 = row.Cells.AddTableCell();
                            currRun = cell9.Blocks.AddParagraph().Inlines.AddRun("ΤΕΛΟΣ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            cell9.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 10);
                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell9 = row.Cells.AddTableCell();
                            currRun = cell9.Blocks.AddParagraph().Inlines.AddRun("ΠΕΡΙΓΡΑΦΗ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            cell9.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 45);
                        } else if (j == 2) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell9 = row.Cells.AddTableCell();
                            currRun = cell9.Blocks.AddParagraph().Inlines.AddRun("ΠΟΣΟ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            cell9.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 15);
                        } else if (j == 3) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell9 = row.Cells.AddTableCell();
                            currRun = cell9.Blocks.AddParagraph().Inlines.AddRun("ΕΚΠΤΩΣΗ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            cell9.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 15);
                        } else if (j == 4) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell9 = row.Cells.AddTableCell();
                            currRun = cell9.Blocks.AddParagraph().Inlines.AddRun("ΜΕΤΑ ΤΗΝ ΕΚΠΤΩΣΗ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            cell9.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 15);
                        }
                    }
                    List<JobB> urbanJobs = distinctJobsForInvoice.Where(o => o.JobTypesID == 1).ToList();
                    List<JobB> ldJobs = distinctJobsForInvoice.Where(o => o.JobTypesID == 2).ToList();
                    List<JobB> otherJobs = distinctJobsForInvoice.Where(o => o.JobTypesID == 3).ToList();
                    decimal totalCost = 0;
                    decimal totalCostUrban = 0;
                    decimal totalCostLD = 0;
                    decimal totalCostOther = 0;
                    if (urbanJobs.Count != distinctJobsForInvoice.Count && ldJobs.Count != distinctJobsForInvoice.Count && otherJobs.Count != distinctJobsForInvoice.Count) {
                        for (int k = 0; k < 3; k++) {
                            if (k == 0 && urbanJobs.Count > 0) {
                                foreach (JobB curJob in urbanJobs) {
                                    List<TasksLineB> taskLinesForCurrentJob = tasksForInvoice.Where(o => o.JobID == curJob.ID).ToList();
                                    decimal totalCostForJob = 0;
                                    foreach (TasksLineB curTaskLine in taskLinesForCurrentJob) {
                                        totalCostForJob += curTaskLine.Task.CostActual.GetValueOrDefault();
                                    }
                                    totalCostUrban += totalCostForJob;
                                    totalCost += totalCostForJob;
                                    Telerik.Windows.Documents.Flow.Model.TableRow row2 = tblContent.Rows.AddTableRow();
                                    for (int j = 0; j < 5; j++) {
                                        Telerik.Windows.Documents.Flow.Model.TableCell cell11 = row2.Cells.AddTableCell();
                                        if (j == 0) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(curJob.InvoiceCode);
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 1) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(curJob.Name);
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 2) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(totalCostForJob.ToString("0.00"));
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 3) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun("00.00");
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 4) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(totalCostForJob.ToString("0.00"));
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        }
                                    }
                                }
                                Telerik.Windows.Documents.Flow.Model.TableRow row3331 = tblContent.Rows.AddTableRow();
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3331 = row3331.Cells.AddTableCell();
                                cell3331.ColumnSpan = 3;
                                currPar = cell3331.Blocks.AddParagraph();
                                currPar.Properties.TextAlignment.LocalValue = Alignment.Right;
                                currRun = currPar.Inlines.AddRun("ΣΥΝΟΛΟ ΑΣΤΙΚΩΝ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3332 = row3331.Cells.AddTableCell();
                                currRun = cell3332.Blocks.AddParagraph().Inlines.AddRun(" ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3333 = row3331.Cells.AddTableCell();
                                currRun = cell3333.Blocks.AddParagraph().Inlines.AddRun(totalCostUrban.ToString("0.00"));
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            } else if (k == 1 && ldJobs.Count > 0) {
                                foreach (JobB curJob in ldJobs) {
                                    List<TasksLineB> taskLinesForCurrentJob = tasksForInvoice.Where(o => o.JobID == curJob.ID).ToList();
                                    decimal totalCostForJob = 0;
                                    foreach (TasksLineB curTaskLine in taskLinesForCurrentJob) {
                                        totalCostForJob += curTaskLine.Task.CostActual.GetValueOrDefault();
                                    }
                                    totalCostLD += totalCostForJob;
                                    totalCost += totalCostForJob;
                                    Telerik.Windows.Documents.Flow.Model.TableRow row2 = tblContent.Rows.AddTableRow();
                                    for (int j = 0; j < 5; j++) {
                                        Telerik.Windows.Documents.Flow.Model.TableCell cell11 = row2.Cells.AddTableCell();
                                        if (j == 0) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(curJob.InvoiceCode);
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 1) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(curJob.Name);
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 2) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(totalCostForJob.ToString("0.00"));
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 3) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun("00.00");
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 4) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(totalCostForJob.ToString("0.00"));
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        }
                                    }
                                }
                                Telerik.Windows.Documents.Flow.Model.TableRow row3332 = tblContent.Rows.AddTableRow();
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3331 = row3332.Cells.AddTableCell();
                                cell3331.ColumnSpan = 3; currPar = cell3331.Blocks.AddParagraph();
                                currPar.Properties.TextAlignment.LocalValue = Alignment.Right;
                                currRun = currPar.Inlines.AddRun("ΣΥΝΟΛΟ ΥΠΕΡΑΣΤΙΚΩΝ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3332 = row3332.Cells.AddTableCell();
                                currRun = cell3332.Blocks.AddParagraph().Inlines.AddRun(" ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3333 = row3332.Cells.AddTableCell();
                                currRun = cell3333.Blocks.AddParagraph().Inlines.AddRun(totalCostLD.ToString("0.00"));
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            } else if (k == 2 && otherJobs.Count > 0) {
                                foreach (JobB curJob in otherJobs) {
                                    List<TasksLineB> taskLinesForCurrentJob = tasksForInvoice.Where(o => o.JobID == curJob.ID).ToList();
                                    decimal totalCostForJob = 0;
                                    foreach (TasksLineB curTaskLine in taskLinesForCurrentJob) {
                                        totalCostForJob += curTaskLine.Task.CostActual.GetValueOrDefault();
                                    }
                                    totalCostOther += totalCostForJob;
                                    totalCost += totalCostForJob;
                                    Telerik.Windows.Documents.Flow.Model.TableRow row2 = tblContent.Rows.AddTableRow();
                                    for (int j = 0; j < 5; j++) {
                                        Telerik.Windows.Documents.Flow.Model.TableCell cell11 = row2.Cells.AddTableCell();
                                        if (j == 0) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(curJob.InvoiceCode);
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 1) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(curJob.Name);
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 2) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(totalCostForJob.ToString("0.00"));
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 3) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun("00.00");
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        } else if (j == 4) {
                                            currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(totalCostForJob.ToString("0.00"));
                                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                            currRun.Properties.FontSize.LocalValue = 12.0;
                                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                        }
                                    }
                                }
                                Telerik.Windows.Documents.Flow.Model.TableRow row3332 = tblContent.Rows.AddTableRow();
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3331 = row3332.Cells.AddTableCell();
                                cell3331.ColumnSpan = 3; currPar = cell3331.Blocks.AddParagraph();
                                currPar.Properties.TextAlignment.LocalValue = Alignment.Right;
                                currRun = currPar.Inlines.AddRun("ΣΥΝΟΛΟ ΛΟΙΠΩΝ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3332 = row3332.Cells.AddTableCell();
                                currRun = cell3332.Blocks.AddParagraph().Inlines.AddRun(" ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                Telerik.Windows.Documents.Flow.Model.TableCell cell3333 = row3332.Cells.AddTableCell();
                                currRun = cell3333.Blocks.AddParagraph().Inlines.AddRun(totalCostOther.ToString("0.00"));
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 11.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            }
                        }
                    } else {
                        foreach (JobB curJob in distinctJobsForInvoice) {
                            List<TasksLineB> taskLinesForCurrentJob = tasksForInvoice.Where(o => o.JobID == curJob.ID).ToList();
                            decimal totalCostForJob = 0;
                            foreach (TasksLineB curTaskLine in taskLinesForCurrentJob) {
                                totalCostForJob += curTaskLine.Task.CostActual.GetValueOrDefault();
                            }
                            totalCost += totalCostForJob;
                            Telerik.Windows.Documents.Flow.Model.TableRow row2 = tblContent.Rows.AddTableRow();
                            for (int j = 0; j < 5; j++) {
                                Telerik.Windows.Documents.Flow.Model.TableCell cell11 = row2.Cells.AddTableCell();
                                if (j == 0) {
                                    currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(curJob.InvoiceCode);
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 1) {
                                    currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(curJob.Name);
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 2) {
                                    currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(totalCostForJob.ToString("0.00"));
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 3) {
                                    currRun = cell11.Blocks.AddParagraph().Inlines.AddRun("00.00");
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                } else if (j == 4) {
                                    currRun = cell11.Blocks.AddParagraph().Inlines.AddRun(totalCostForJob.ToString("0.00"));
                                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                    currRun.Properties.FontSize.LocalValue = 12.0;
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                }
                            }
                        }
                    }
                    Telerik.Windows.Documents.Flow.Model.TableRow row3 = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 4; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell12 = row3.Cells.AddTableCell();
                            cell12.ColumnSpan = 2;
                            currRun = cell12.Blocks.AddParagraph().Inlines.AddRun("ΣΥΝΟΛΟ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell12 = row3.Cells.AddTableCell();
                            currRun = cell12.Blocks.AddParagraph().Inlines.AddRun(totalCost.ToString("0.00"));
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 2) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell12 = row3.Cells.AddTableCell();
                            currRun = cell12.Blocks.AddParagraph().Inlines.AddRun("00.00");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 3) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell12 = row3.Cells.AddTableCell();
                            currRun = cell12.Blocks.AddParagraph().Inlines.AddRun(totalCost.ToString("0.00"));
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    Telerik.Windows.Documents.Flow.Model.TableRow row4 = tblContent.Rows.AddTableRow();
                    decimal fpaTotalCost = totalCost * fpa;
                    for (int j = 0; j < 2; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell13 = row4.Cells.AddTableCell();
                            cell13.ColumnSpan = 4;
                            currRun = cell13.Blocks.AddParagraph().Inlines.AddRun("ΦΠΑ:  ");
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Right;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell13 = row4.Cells.AddTableCell();
                            currRun = cell13.Blocks.AddParagraph().Inlines.AddRun(fpaTotalCost.ToString("0.00"));
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    Telerik.Windows.Documents.Flow.Model.TableRow row5 = tblContent.Rows.AddTableRow();
                    decimal totalCostwithFPA = totalCost + fpaTotalCost;
                    for (int j = 0; j < 2; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell14 = row5.Cells.AddTableCell();
                            cell14.ColumnSpan = 4;
                            currRun = cell14.Blocks.AddParagraph().Inlines.AddRun("ΟΦΕΙΛΟΜΕΝΟ:  ");
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Right;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell14 = row5.Cells.AddTableCell();
                            currRun = cell14.Blocks.AddParagraph().Inlines.AddRun(totalCostwithFPA.ToString("0.00"));
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    Telerik.Windows.Documents.Flow.Model.TableRow row6 = tblContent.Rows.AddTableRow();
                    Telerik.Windows.Documents.Flow.Model.TableCell cell = row6.Cells.AddTableCell();
                    cell.ColumnSpan = 5;
                    currRun = cell.Blocks.AddParagraph().Inlines.AddRun("  ");
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Right;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 12.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                    Telerik.Windows.Documents.Flow.Model.TableRow row21 = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 3; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell21 = row21.Cells.AddTableCell();
                            cell21.ColumnSpan = 2;
                            currRun = cell21.Blocks.AddParagraph().Inlines.AddRun(" ");
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Right;
                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell21 = row21.Cells.AddTableCell();
                            cell21.ColumnSpan = 2;
                            currRun = cell21.Blocks.AddParagraph().Inlines.AddRun("ΑΞΙΑ ΠΡΟ ΕΚΠΤΩΣΗΣ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 2) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell21 = row21.Cells.AddTableCell();
                            currRun = cell21.Blocks.AddParagraph().Inlines.AddRun(totalCost.ToString("0.00"));
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    Telerik.Windows.Documents.Flow.Model.TableRow row22 = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 3; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell22 = row22.Cells.AddTableCell();
                            cell22.ColumnSpan = 2;
                            currRun = cell22.Blocks.AddParagraph().Inlines.AddRun(" ");
                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell22 = row22.Cells.AddTableCell();
                            cell22.ColumnSpan = 2;
                            currRun = cell22.Blocks.AddParagraph().Inlines.AddRun("ΕΚΠΤΩΣΗ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 2) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell22 = row22.Cells.AddTableCell();
                            currRun = cell22.Blocks.AddParagraph().Inlines.AddRun("00.00");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    Telerik.Windows.Documents.Flow.Model.TableRow row23 = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 3; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell23 = row23.Cells.AddTableCell();
                            cell23.ColumnSpan = 2;
                            currRun = cell23.Blocks.AddParagraph().Inlines.AddRun(" ");
                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell23 = row23.Cells.AddTableCell();
                            cell23.ColumnSpan = 2;
                            currRun = cell23.Blocks.AddParagraph().Inlines.AddRun("ΑΞΙΑ ΜΕΤΑ ΤΗΝ ΕΚΠTΩΣΗ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 2) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell23 = row23.Cells.AddTableCell();
                            currRun = cell23.Blocks.AddParagraph().Inlines.AddRun(totalCost.ToString("0.00"));
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    Telerik.Windows.Documents.Flow.Model.TableRow row24 = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 3; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell24 = row24.Cells.AddTableCell();
                            cell24.ColumnSpan = 2;
                            currRun = cell24.Blocks.AddParagraph().Inlines.AddRun(" ");
                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell24 = row24.Cells.AddTableCell();
                            cell24.ColumnSpan = 2;
                            currRun = cell24.Blocks.AddParagraph().Inlines.AddRun("ΦΠΑ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 2) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell24 = row24.Cells.AddTableCell();
                            currRun = cell24.Blocks.AddParagraph().Inlines.AddRun(fpaTotalCost.ToString("0.00"));
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    Telerik.Windows.Documents.Flow.Model.TableRow row25 = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 3; j++) {
                        if (j == 0) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell25 = row25.Cells.AddTableCell();
                            cell25.ColumnSpan = 2;
                            currRun = cell25.Blocks.AddParagraph().Inlines.AddRun(" ");
                        } else if (j == 1) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell25 = row25.Cells.AddTableCell();
                            cell25.ColumnSpan = 2;
                            currRun = cell25.Blocks.AddParagraph().Inlines.AddRun("ΓΕΝΙΚΟ ΣΥΝΟΛΟ");
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        } else if (j == 2) {
                            Telerik.Windows.Documents.Flow.Model.TableCell cell25 = row25.Cells.AddTableCell();
                            currRun = cell25.Blocks.AddParagraph().Inlines.AddRun(totalCostwithFPA.ToString("0.00"));
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    curRep = reps.Find(o => o.UniqueName == "Invoice_Chief_Name");
                    DocumentReplacemetB curRep2 = reps.Find(o => o.UniqueName == "Invoice_Chief_Title");
                    currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                    string chiefData = curRep.Text + "#" + curRep2.Text;
                    string[] arrText3 = chiefData.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                    currPar = (Paragraph)currCell.Blocks.First();
                    currPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                    editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                    for (int i = 0; i < arrText3.Length; i++) {
                        if (!string.IsNullOrEmpty(arrText3[i])) {
                            currRun = editor.InsertLine(arrText3[i]);
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Center;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 12.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                    curDoc.UpdateFields();
                    string filename = "Invoice_" + curInvoice.Customer.NamedInvoiceGR.Replace(" ", "_") + "_from_" + curInvoice.DateFrom.GetValueOrDefault().ToString("dd-MM-yyyy") + "_to_" + curInvoice.DateTo.GetValueOrDefault().ToString("dd-MM-yyyy");
                    exportDOCX(curDoc, filename);
                }
                catch (Exception ex) { }
            }
        }

        protected bool[] getVisibleColumns() {
            bool[] array2return = new bool[10];
            array2return[0] = chkDate.Checked;
            array2return[1] = chkRoute.Checked;
            array2return[2] = chkDateFrom.Checked;
            array2return[3] = chkDateTo.Checked;
            array2return[4] = chkTotalTime.Checked;
            array2return[5] = chkTotalDistance.Checked;
            array2return[6] = chkTransferCost.Checked;
            array2return[7] = chkAddedCharges.Checked;
            array2return[8] = chkTotalCost.Checked;
            array2return[9] = chkComments.Checked;
            return array2return;
        }

        protected int getSumColSpan(bool[] visibleColumns) {
            int sum2return = 0;
            for (int k=0; k<5; k++) { 
                if (visibleColumns[k] == true) { sum2return++; } 
            }
            return sum2return;
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

        protected Paragraph insertParagraph(RadFlowDocumentEditor editor) {
            Paragraph currPar;
            currPar = editor.InsertParagraph();
            currPar.Spacing.SpacingAfter = 0;
            return currPar;
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
        }

        protected RadFlowDocument LoadTemplateDocument(string uniqueName) {
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