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

namespace OTERT.Pages.Helpers {

    public partial class DailyListInside : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected Button btnExportDOCX, btnExportXLSX;
        protected string pageTitle, uploadedFilePath;
        const string templatesFolder = "~/Templates/";

        protected DateTime forDate = DateTime.Parse("1900-01-01");

        protected void Page_Load(object sender, EventArgs e) {
            string forDateStr = Request.QueryString["date"].ToString();
            DateTime tmpDate;
            if (DateTime.TryParse(forDateStr, out tmpDate)) { forDate = tmpDate; }
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Τμήμα Υποστήριξης (ΚΕΤ) - Λίστα Ημερ. Μεταδόσεων";
                gridMain.MasterTableView.Caption = "Λίστα Ημερ. Μεταδόσεων (" + forDate.ToShortDateString() + ")";
            }
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
            /*
            var document = new RadFlowDocument();
            var editor = new RadFlowDocumentEditor(document);
            editor.ParagraphFormatting.TextAlignment.LocalValue = Alignment.Justified;
            editor.InsertLine("Dear Telerik User,");
            editor.InsertText("We're happy to introduce the new Telerik RadWordsProcessing component for WPF. High performance library that enables you to read, write and manipulate documents in DOCX, RTF and plain text format. The document model is independent from UI and ");
            Run run = editor.InsertText("does not require");
            run.Underline.Pattern = UnderlinePattern.Single;
            editor.InsertLine(" Microsoft Office.");
            editor.InsertText("bold, ").FontWeight = System.Windows.FontWeights.Bold;
            editor.InsertText("italic, ").FontStyle = System.Windows.FontStyles.Italic;
            editor.InsertText("underline,").Underline.Pattern = UnderlinePattern.Single;
            editor.InsertText(" font sizes and ").FontSize = 20;
            editor.InsertText("colors ").ForegroundColor = new ThemableColor(System.Windows.Media.Colors.Green);
            editor.InsertText("or ");
            editor.InsertText("colors2 ").ForegroundColor = new ThemableColor(System.Windows.Media.Color.FromRgb(255,0,0));
            editor.InsertLine("as well as text alignment and indentation. Other options include tables, hyperlinks, inline and floating images. Even more sweetness is added by the built-in styles and themes.");
            editor.InsertText("Here at Telerik we strive to provide the best services possible and fulfill all needs you as a customer may have. We would appreciate any feedback you send our way through the ");
            editor.InsertHyperlink("public forums", "http://www.telerik.com/forums", false, "Telerik Forums");
            editor.InsertLine(" or support ticketing system.");
            editor.InsertLine("We hope you'll enjoy RadWordsProcessing as much as we do. Happy coding!");
            editor.InsertParagraph();
            editor.InsertLine("Kind regards,");
            editor.InsertLine("Telerik Admin");
            exportDOCX(document);
            */
            try {
                DocumentReplacemetsController cont = new DocumentReplacemetsController();
                List<DocumentReplacemetB> reps = new List<DocumentReplacemetB>();
                reps = cont.GetDocumentReplacemets("GLOBAL");
                DocumentReplacemetB curRep = reps.Find(o => o.Title == "Τίτλος Test");
                if (curRep != null) {
                    RadFlowDocument curDoc = LoadSampleDocument();
                    RadFlowDocumentEditor editor = new RadFlowDocumentEditor(curDoc);
                    //System.Text.RegularExpressions.Regex textRegex = new System.Text.RegularExpressions.Regex("ΣΑΟΥΣΟΠΟΥΛΟΥ ΑΝΝΑ");
                    //editor.ReplaceText("ΣΑΟΥΣΟΠΟΥΛΟΥ ΑΝΝΑ", txtNew.Text, true, true);
                    BookmarkRangeStart bookmarkRangeStart = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().Where(rangeStart => rangeStart.Bookmark.Name == "Name2Change").FirstOrDefault();
                    editor.MoveToInlineEnd(bookmarkRangeStart);
                    editor.InsertText(curRep.Text);
                    exportXLSX(curDoc);
                }
            }
            catch (Exception) { }
        }

        protected void exportXLSX(RadFlowDocument doc) {
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

        protected void btnExportDOCX_Click(object sender, EventArgs e) {
            /*
            var document = new RadFlowDocument();
            var editor = new RadFlowDocumentEditor(document);
            editor.ParagraphFormatting.TextAlignment.LocalValue = Alignment.Justified;
            editor.InsertLine("Dear Telerik User,");
            editor.InsertText("We're happy to introduce the new Telerik RadWordsProcessing component for WPF. High performance library that enables you to read, write and manipulate documents in DOCX, RTF and plain text format. The document model is independent from UI and ");
            Run run = editor.InsertText("does not require");
            run.Underline.Pattern = UnderlinePattern.Single;
            editor.InsertLine(" Microsoft Office.");
            editor.InsertText("bold, ").FontWeight = System.Windows.FontWeights.Bold;
            editor.InsertText("italic, ").FontStyle = System.Windows.FontStyles.Italic;
            editor.InsertText("underline,").Underline.Pattern = UnderlinePattern.Single;
            editor.InsertText(" font sizes and ").FontSize = 20;
            editor.InsertText("colors ").ForegroundColor = new ThemableColor(System.Windows.Media.Colors.Green);
            editor.InsertText("or ");
            editor.InsertText("colors2 ").ForegroundColor = new ThemableColor(System.Windows.Media.Color.FromRgb(255,0,0));
            editor.InsertLine("as well as text alignment and indentation. Other options include tables, hyperlinks, inline and floating images. Even more sweetness is added by the built-in styles and themes.");
            editor.InsertText("Here at Telerik we strive to provide the best services possible and fulfill all needs you as a customer may have. We would appreciate any feedback you send our way through the ");
            editor.InsertHyperlink("public forums", "http://www.telerik.com/forums", false, "Telerik Forums");
            editor.InsertLine(" or support ticketing system.");
            editor.InsertLine("We hope you'll enjoy RadWordsProcessing as much as we do. Happy coding!");
            editor.InsertParagraph();
            editor.InsertLine("Kind regards,");
            editor.InsertLine("Telerik Admin");
            exportDOCX(document);
            */
            try {
                DocumentReplacemetsController cont = new DocumentReplacemetsController();
                List<DocumentReplacemetB> reps = new List<DocumentReplacemetB>();
                reps = cont.GetDocumentReplacemets("GLOBAL");
                DocumentReplacemetB curRep = reps.Find(o => o.Title == "Τίτλος Test");
                if (curRep != null) {
                    RadFlowDocument curDoc = LoadSampleDocument();
                    RadFlowDocumentEditor editor = new RadFlowDocumentEditor(curDoc);
                    //System.Text.RegularExpressions.Regex textRegex = new System.Text.RegularExpressions.Regex("ΣΑΟΥΣΟΠΟΥΛΟΥ ΑΝΝΑ");
                    //editor.ReplaceText("ΣΑΟΥΣΟΠΟΥΛΟΥ ΑΝΝΑ", txtNew.Text, true, true);
                    BookmarkRangeStart bookmarkRangeStart = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().Where(rangeStart => rangeStart.Bookmark.Name == "Name2Change").FirstOrDefault();
                    editor.MoveToInlineEnd(bookmarkRangeStart);
                    editor.InsertText(curRep.Text);
                    exportDOCX(curDoc);
                }
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
            Response.AppendHeader("content-disposition", "attachment; filename=ExportedFile.docx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            Response.BinaryWrite(renderedBytes);
            Response.End();
        }

        protected RadFlowDocument LoadSampleDocument() {
            RadFlowDocument document2;
            IFormatProvider<RadFlowDocument> fileFormatProvider = new DocxFormatProvider();
            string fileName = Server.MapPath(templatesFolder + "ΤΙΜ ΕΛΛ 1212 598.docx");
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