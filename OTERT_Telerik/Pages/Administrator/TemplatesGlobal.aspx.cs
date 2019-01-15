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

namespace OTERT.Pages.Administrator {

    public partial class TemplatesGlobal : Page {

        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected Button btnPrint;
        protected string pageTitle, uploadedFilePath;
        const string templatesFolder = "~/Templates/";

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Εκτυπώσεων - Γενικά Στοιχεία";
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e) {
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
        }

        protected void btnReplace_Click(object sender, EventArgs e) {
            RadFlowDocument document2 = LoadSampleDocument();
            RadFlowDocumentEditor editor = new RadFlowDocumentEditor(document2);
            //System.Text.RegularExpressions.Regex textRegex = new System.Text.RegularExpressions.Regex("ΣΑΟΥΣΟΠΟΥΛΟΥ ΑΝΝΑ");
            editor.ReplaceText("ΣΑΟΥΣΟΠΟΥΛΟΥ ΑΝΝΑ", "ΞΑΝΘΟΠΟΥΛΟΣ ΠΑΝΟΣ", true, true);
            exportDOCX(document2);
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

}