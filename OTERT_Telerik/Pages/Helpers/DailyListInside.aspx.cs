﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Telerik.Web.UI.Calendar;
using System.Windows;
using Telerik.Windows.Documents.Flow.Model;
using Telerik.Windows.Documents.Flow.Model.Editing;
using Telerik.Windows.Documents.Flow.Model.Styles;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Documents.Common.FormatProviders;
using Telerik.Windows.Documents.Flow.FormatProviders.Docx;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using ExpressionParser;
using OTERT.Model;
using OTERT.Controller;
using OTERT_Entity;

namespace OTERT.Pages.Helpers {

    public partial class DailyListInside : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected Button btnExportDOCX, btnExportXLSX;
        protected string pageTitle, uploadedFilePath;
        protected UserB loggedUser;
        const string fileUploadFolder = "~/UploadedFiles/";
        const string templatesFolder = "~/Templates/";
        const string pageUniqueName = "KET";

        protected DateTime forDate = DateTime.Parse("1900-01-01");
        protected readonly ThemableColor cellBackground = ThemableColor.FromArgb(0, 255, 255, 255);
        protected readonly ThemableColor tcBlack = ThemableColor.FromArgb(255, 0, 0, 0);
        protected readonly ThemableColor tcWhite = ThemableColor.FromArgb(255, 255, 255, 255);
        protected readonly string dateFormat = "dd/MM/yyyy HH:mm";

        protected void Page_Load(object sender, EventArgs e) {
            string forDateStr = Request.QueryString["date"].ToString();
            DateTime tmpDate;
            if (DateTime.TryParse(forDateStr, out tmpDate)) { forDate = tmpDate; }
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Τμήμα Υποστήριξης (ΚΕΤ) - Λίστα Ημερ. Μεταδόσεων";
                gridMain.MasterTableView.Caption = "Λίστα Ημερ. Μεταδόσεων (" + forDate.ToString("dd/MM/yyyy") + ")";
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            try {
                gridMain.DataSource = getTasksForHelpers(forDate);
            }
            catch (Exception) {}
        }

        private List<TaskForH> getTasksForHelpers(DateTime forDay) {
            TasksController cont = new TasksController();
            List<TaskB> tasks = cont.GetTasksForHelpers(forDay);
            List<TaskForH> finalTasks = new List<TaskForH>();
            int newAA = 1;
            if (tasks != null) {
                foreach (TaskB testc in tasks) {
                    TaskForH cTask = new TaskForH();
                    cTask.Count = newAA;
                    cTask.Customer = testc.Customer.NameGR;
                    cTask.FromPlace = testc.Distance.Position1;
                    cTask.ToPlace = testc.Distance.Position2;
                    cTask.FromTime = testc.DateTimeStartOrder.HasValue ? testc.DateTimeStartOrder.Value.ToString("HH:mm") : string.Empty;
                    cTask.ToTime = testc.DateTimeEndOrder.HasValue ? testc.DateTimeEndOrder.Value.ToString("HH:mm") : string.Empty;
                    cTask.Comments = testc.Comments;
                    finalTasks.Add(cTask);
                    newAA++;
                }
            }
            return finalTasks;
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
            Response.AppendHeader("content-disposition", "attachment; filename=Daily_Transmissions_List_" + forDate.ToString("dd-MM-yyyy") + ".xlsx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.BinaryWrite(renderedBytes);
            Response.End();
        }

        protected Workbook createWorkbook() {
            Workbook workbook = new Workbook();
            workbook.Sheets.Add(SheetType.Worksheet);
            Worksheet worksheet = workbook.ActiveWorksheet;
            worksheet.Name = "KET " + forDate.ToString("dd-MM-yyyy");
            List<TaskForH> tasks = getTasksForHelpers(forDate);
            prepareDocument(worksheet);
            int currentRow = 1;
            CellBorder border = new CellBorder(CellBorderStyle.Thin, tcBlack);
            CellBorders borders = new CellBorders(border, border, border, border, null, null, null, null);
            double fontSize = 12;
            foreach (TaskForH curTask in tasks) {
                worksheet.Cells[currentRow, 0].SetValue(curTask.Count.ToString());
                worksheet.Cells[currentRow, 0].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 0].SetBorders(borders);
                worksheet.Cells[currentRow, 1].SetValue(curTask.Customer);
                worksheet.Cells[currentRow, 1].SetFormat(new CellValueFormat("@"));
                worksheet.Cells[currentRow, 1].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 1].SetBorders(borders);
                worksheet.Cells[currentRow, 2].SetValue(curTask.FromPlace);
                worksheet.Cells[currentRow, 2].SetFormat(new CellValueFormat("@"));
                worksheet.Cells[currentRow, 2].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 2].SetBorders(borders);
                worksheet.Cells[currentRow, 3].SetValue(curTask.FromTime + " - " + curTask.ToTime);
                worksheet.Cells[currentRow, 3].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 3].SetBorders(borders);
                worksheet.Cells[currentRow, 4].SetValue(curTask.Comments);
                worksheet.Cells[currentRow, 4].SetIsWrapped(true);
                worksheet.Cells[currentRow, 4].SetFontSize(fontSize);
                worksheet.Cells[currentRow, 4].SetBorders(borders);
                currentRow++;
            }
            for (int i = 0; i < worksheet.Columns.Count; i++) { worksheet.Columns[i].AutoFitWidth(); }
            for (int i = 0; i < worksheet.Columns.Count; i++) {
                if (i == 4) { worksheet.Columns[i].SetWidth(new ColumnWidth(500, true)); }
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
            worksheet.Cells[0, 1].SetValue("ΑΙΤΩΝ");
            worksheet.Cells[0, 1].SetFontSize(fontSize);
            worksheet.Cells[0, 1].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 1].SetIsBold(true);
            //worksheet.Cells[0, 1].SetIsWrapped(true);
            worksheet.Cells[0, 1].SetFill(pfGreen);
            worksheet.Cells[0, 1].SetBorders(borders);
            worksheet.Cells[0, 2].SetValue("ΑΠΟ");
            worksheet.Cells[0, 2].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 2].SetFontSize(fontSize);
            worksheet.Cells[0, 2].SetIsBold(true);
            //worksheet.Cells[0, 2].SetIsWrapped(true);
            worksheet.Cells[0, 2].SetFill(pfOrange);
            worksheet.Cells[0, 2].SetBorders(borders);
            worksheet.Cells[0, 3].SetValue("ΩΡΑ");
            worksheet.Cells[0, 3].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 3].SetFontSize(fontSize);
            worksheet.Cells[0, 3].SetIsBold(true);
            worksheet.Cells[0, 3].SetFill(pfGreen);
            worksheet.Cells[0, 3].SetBorders(borders);
            worksheet.Cells[0, 4].SetValue("ΠΑΡΑΤΗΡΗΣΕΙΣ");
            worksheet.Cells[0, 4].SetHorizontalAlignment(RadHorizontalAlignment.Center);
            worksheet.Cells[0, 4].SetFontSize(fontSize);
            worksheet.Cells[0, 4].SetIsBold(true);
            worksheet.Cells[0, 4].SetFill(pfGreen);
            worksheet.Cells[0, 4].SetBorders(borders);
        }

        protected void btnExportDOCX_Click(object sender, EventArgs e) {
            try {
                DocumentReplacemetsController cont = new DocumentReplacemetsController();
                List<DocumentReplacemetB> reps = new List<DocumentReplacemetB>();
                reps = cont.GetDocumentReplacemets(pageUniqueName);
                string imgFolderPath = Server.MapPath(fileUploadFolder);
                DocumentReplacemetB curRep;
                BookmarkRangeStart bookmarkRangeStart;
                RadFlowDocument curDoc = LoadSampleDocument(pageUniqueName);
                RadFlowDocumentEditor editor = new RadFlowDocumentEditor(curDoc);
                List<BookmarkRangeStart> test = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().ToList();
                Telerik.Windows.Documents.Flow.Model.TableCell currCell;
                Run currRun;

                Header defaultHeader = editor.Document.Sections.First().Headers.Default;
                Footer defaultFooter = editor.Document.Sections.First().Footers.Default;

                Telerik.Windows.Documents.Flow.Model.Styles.Style tableStyle = new Telerik.Windows.Documents.Flow.Model.Styles.Style("TableStyle", StyleType.Table);
                tableStyle.Name = "Table Style";
                tableStyle.TableProperties.Borders.LocalValue = new TableBorders(new Border(1, Telerik.Windows.Documents.Flow.Model.Styles.BorderStyle.Single, new ThemableColor(System.Windows.Media.Colors.Black)));
                tableStyle.TableProperties.Alignment.LocalValue = Alignment.Left;
                tableStyle.TableCellProperties.VerticalAlignment.LocalValue = VerticalAlignment.Center;
                tableStyle.TableCellProperties.PreferredWidth.LocalValue = new TableWidthUnit(TableWidthUnitType.Percent, 100);
                tableStyle.TableCellProperties.Padding.LocalValue = new Telerik.Windows.Documents.Primitives.Padding(8);
                editor.Document.StyleRepository.Add(tableStyle);

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
                editor.InsertText(forDate.ToString(curRep.Text, new System.Globalization.CultureInfo("el-GR")));

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

                List<TaskForH> lstDummy = getTasksForHelpers(forDate);
                bookmarkRangeStart = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().Where(rangeStart => rangeStart.Bookmark.Name == "Body_Main").FirstOrDefault();
                editor.MoveToInlineEnd(bookmarkRangeStart);
                Telerik.Windows.Documents.Flow.Model.Table tblContent = editor.InsertTable();
                tblContent.StyleId = "TableStyle";
                tblContent.LayoutType = TableLayoutType.AutoFit;
                ThemableColor cellBackground = new ThemableColor(System.Windows.Media.Colors.Beige);
                for (int i = 0; i < lstDummy.Count + 1; i++) {
                    Telerik.Windows.Documents.Flow.Model.TableRow row = tblContent.Rows.AddTableRow();
                    for (int j = 0; j < 5; j++) {
                        Telerik.Windows.Documents.Flow.Model.TableCell cell = row.Cells.AddTableCell();
                        if (i == 0) {
                            if (j == 0) {
                                currRun = cell.Blocks.AddParagraph().Inlines.AddRun("A/A");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 15.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 5);
                            } else if (j == 1) {
                                currRun = cell.Blocks.AddParagraph().Inlines.AddRun("ΑΙΤΩΝ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 15.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 20);
                            } else if (j == 2) {
                                currRun = cell.Blocks.AddParagraph().Inlines.AddRun("ΑΠΟ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 15.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 20);
                            } else if (j == 3) {
                                currRun = cell.Blocks.AddParagraph().Inlines.AddRun("ΩΡΑ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 15.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 20);
                            } else if (j == 4) {
                                currRun = cell.Blocks.AddParagraph().Inlines.AddRun("ΠΑΡΑΤΗΡΗΣΕΙΣ");
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 15.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                                cell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Percent, 35);
                            }
                        } else {
                            if (j == 0) {
                                currRun = cell.Blocks.AddParagraph().Inlines.AddRun(lstDummy.Where(o => o.Count == i).FirstOrDefault().Count.ToString());
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 15.0;
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            } else if (j == 1) {
                                cell.Blocks.AddParagraph().Inlines.AddRun(lstDummy.Where(o => o.Count == i).FirstOrDefault().Customer);
                            } else if (j == 2) {
                                cell.Blocks.AddParagraph().Inlines.AddRun(lstDummy.Where(o => o.Count == i).FirstOrDefault().FromPlace);
                            } else if (j == 3) {
                                cell.Blocks.AddParagraph().Inlines.AddRun(lstDummy.Where(o => o.Count == i).FirstOrDefault().FromTime + " - " + lstDummy.Where(o => o.Count == i).FirstOrDefault().ToTime);
                            } else if (j == 4) {
                                if (!string.IsNullOrEmpty(lstDummy.Where(o => o.Count == i).FirstOrDefault().Comments)) {
                                    cell.Blocks.AddParagraph().Inlines.AddRun(lstDummy.Where(o => o.Count == i).FirstOrDefault().Comments);
                                } else {
                                    cell.Blocks.AddParagraph().Inlines.AddRun("");
                                }
                            }
                        }
                    }
                }
                curDoc.UpdateFields();
                exportDOCX(curDoc);
            }
            catch (Exception) { }
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
            Response.AppendHeader("content-disposition", "attachment; filename=Daily_Transmissions_List_" + forDate.ToString("dd-MM-yyyy") + ".docx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            Response.BinaryWrite(renderedBytes);
            Response.End();
        }

        protected RadFlowDocument LoadSampleDocument(string uniqueName) {
            RadFlowDocument document2;
            IFormatProvider<RadFlowDocument> fileFormatProvider = new DocxFormatProvider();
            string fileName = Server.MapPath(templatesFolder + uniqueName +".docx");
            using (FileStream input = new FileStream(fileName, FileMode.Open)) {
                document2 = fileFormatProvider.Import(input);
            }
            return document2;
        }

    }

    public class TaskForH {
        public int Count { get; set; }
        public string Customer { get; set; }
        public string FromPlace { get; set; }
        public string ToPlace { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string Comments { get; set; }
    }

}