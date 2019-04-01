using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Windows;
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
using System.Globalization;

namespace OTERT.Pages.PrintTemplates {

    public partial class KETDailyList : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected Button btnPrint;
        protected string pageTitle, uploadedFilePath, textFromSession;
        const string templatesFolder = "~/Templates/";
        const string fileUploadFolder = "~/UploadedFiles/";
        const string pageUniqueName = "KET";

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Εκτυπώσεων > ΚΕΤ - Λίστα Ημερησίων Μεταδόσεων";
                gridMain.MasterTableView.Caption = "Διαχείριση Εκτυπώσεων > ΚΕΤ - Λίστα Ημερησίων Μεταδόσεων";
                textFromSession = "";
                Session.Remove("textFromSession");
            }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            try {
                DocumentReplacemetsController cont = new DocumentReplacemetsController();
                gridMain.DataSource = cont.GetDocumentReplacemets(pageUniqueName);
            }
            catch (Exception) { }

        }

        private void ShowErrorMessage() {
            RadWindowManager1.RadAlert("Υπήρξε κάποιο λάθος στα δεδομένα! Παρακαλώ ξαναπροσπαθήστε.", 400, 200, "Σφάλμα", "");
        }

        protected void gridMain_UpdateCommand(object source, GridCommandEventArgs e) {
            var editableItem = ((GridEditableItem)e.Item);
            var ID = (int)editableItem.GetDataKeyValue("ID");
            using (var dbContext = new OTERTConnStr()) {
                var docRep = dbContext.DocumentReplacemets.Where(n => n.ID == ID).FirstOrDefault();
                if (docRep != null) {
                    editableItem.UpdateValues(docRep);
                    if (Session["textFromSession"] != null) { textFromSession = Session["textFromSession"].ToString().Trim(); }
                    if (!string.IsNullOrEmpty(textFromSession)) {
                        docRep.Text = textFromSession;
                        textFromSession = "";
                        Session.Remove("textFromSession");
                    }
                    try { dbContext.SaveChanges(); }
                    catch (Exception) { ShowErrorMessage(); }
                }
            }
        }

        protected void gridMain_PreRender(object sender, EventArgs e) {
            foreach (GridDataItem item in gridMain.MasterTableView.Items) {
                var ID = (int)item.GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var docRep = dbContext.DocumentReplacemets.Where(n => n.ID == ID).FirstOrDefault();
                    if (docRep != null) {
                        Panel pnlText = (Panel)item.FindControl("pnlText");
                        Panel pnlImage = (Panel)item.FindControl("pnlImage");
                        Panel pnlTextWidth = (Panel)item.FindControl("pnlTextWidth");
                        Panel pnlImageWidth = (Panel)item.FindControl("pnlImageWidth");
                        Panel pnlTextHeight = (Panel)item.FindControl("pnlTextHeight");
                        Panel pnlImageHeight = (Panel)item.FindControl("pnlImageHeight");
                        Panel pnlDate = (Panel)item.FindControl("pnlDate");
                        if (docRep.Type == "Image" || docRep.Type == "Cell_Image") {
                            pnlText.Visible = false;
                            pnlTextWidth.Visible = false;
                            pnlTextHeight.Visible = false;
                            pnlImage.Visible = true;
                            pnlImageWidth.Visible = true;
                            pnlImageHeight.Visible = true;
                            pnlDate.Visible = false;
                        } else if (docRep.Type == "Date" || docRep.Type == "Cell_Date") {
                            pnlText.Visible = false;
                            pnlTextWidth.Visible = false;
                            pnlTextHeight.Visible = false;
                            pnlImage.Visible = false;
                            pnlImageWidth.Visible = false;
                            pnlImageHeight.Visible = false;
                            pnlDate.Visible = true;
                        } else {
                            pnlText.Visible = true;
                            pnlTextWidth.Visible = true;
                            pnlTextHeight.Visible = true;
                            pnlImage.Visible = false;
                            pnlImageWidth.Visible = false;
                            pnlImageHeight.Visible = false;
                            pnlDate.Visible = false;
                        }
                    }
                }
            }
        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) {
            if (e.Item is GridEditFormItem && e.Item.IsInEditMode) {
                GridEditableItem item = e.Item as GridEditableItem;
                var ID = (int)item.GetDataKeyValue("ID");
                using (var dbContext = new OTERTConnStr()) {
                    var docRep = dbContext.DocumentReplacemets.Where(n => n.ID == ID).FirstOrDefault();
                    if (docRep != null) {
                        Panel pnlText = (Panel)item.FindControl("pnlText");
                        Panel pnlImage = (Panel)item.FindControl("pnlImage");
                        Panel pnlTextWidth = (Panel)item.FindControl("pnlTextWidth");
                        Panel pnlImageWidth = (Panel)item.FindControl("pnlImageWidth");
                        Panel pnlTextHeight = (Panel)item.FindControl("pnlTextHeight");
                        Panel pnlImageHeight = (Panel)item.FindControl("pnlImageHeight");
                        Panel pnlPageNo = (Panel)item.FindControl("pnlPageNo");
                        Panel pnlDate = (Panel)item.FindControl("pnlDate");
                        if (docRep.Type == "Image" || docRep.Type == "Cell_Image") {
                            pnlText.Visible = false;
                            pnlTextWidth.Visible = false;
                            pnlTextHeight.Visible = false;
                            pnlImage.Visible = true;
                            pnlImageWidth.Visible = true;
                            pnlImageHeight.Visible = true;
                            pnlPageNo.Visible = false;
                            pnlDate.Visible = false;
                            item["ImageWidth"].Parent.Visible = true;
                            item["ImageHeight"].Parent.Visible = true;
                        } else if (docRep.Type == "PageNo" || docRep.Type == "Cell_PageNo") {
                            pnlText.Visible = false;
                            pnlTextWidth.Visible = false;
                            pnlTextHeight.Visible = false;
                            pnlImage.Visible = false;
                            pnlImageWidth.Visible = false;
                            pnlImageHeight.Visible = false;
                            pnlPageNo.Visible = true;
                            pnlDate.Visible = false;
                            item["ImageWidth"].Parent.Visible = false;
                            item["ImageHeight"].Parent.Visible = false;
                            RadDropDownList ddlText = item.FindControl("ddlText") as RadDropDownList;
                            ddlText.SelectedIndex = ddlText.FindItemByValue(docRep.Text).Index;
                        } else if (docRep.Type == "Date" || docRep.Type == "Cell_Date") {
                            pnlText.Visible = false;
                            pnlTextWidth.Visible = false;
                            pnlTextHeight.Visible = false;
                            pnlImage.Visible = false;
                            pnlImageWidth.Visible = false;
                            pnlImageHeight.Visible = false;
                            pnlPageNo.Visible = false;
                            pnlDate.Visible = true;
                            item["ImageWidth"].Parent.Visible = false;
                            item["ImageHeight"].Parent.Visible = false;
                            RadDropDownList ddlDate = item.FindControl("ddlDate") as RadDropDownList;
                            ddlDate.SelectedIndex = ddlDate.FindItemByValue(docRep.Text).Index;
                        } else {
                            pnlText.Visible = true;
                            pnlTextWidth.Visible = true;
                            pnlTextHeight.Visible = true;
                            pnlImage.Visible = false;
                            pnlImageWidth.Visible = false;
                            pnlImageHeight.Visible = false;
                            pnlPageNo.Visible = false;
                            pnlDate.Visible = false;
                            item["ImageWidth"].Parent.Visible = false;
                            item["ImageHeight"].Parent.Visible = false;
                        }
                    }
                }
            }
        }

        protected void uplFile_FileUploaded(object sender, FileUploadedEventArgs e) {
            string fullPath = Server.MapPath(fileUploadFolder);
            bool exists = System.IO.Directory.Exists(fullPath);
            if (!exists) { System.IO.Directory.CreateDirectory(fullPath); }
            string newfilename = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + "_" + e.File.GetNameWithoutExtension().Replace(" ", "_") + e.File.GetExtension();
            uploadedFilePath = fileUploadFolder + newfilename;
            e.File.SaveAs(System.IO.Path.Combine(fullPath, newfilename));
            Session["textFromSession"] = newfilename;
        }

        protected void ddlText_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            try { Session["textFromSession"] = e.Value; }
            catch (Exception) { }
        }

        protected void btnPrint_Click(object sender, EventArgs e) {
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
                reps = cont.GetDocumentReplacemets(pageUniqueName);
                string imgFolderPath = Server.MapPath(fileUploadFolder);
                DocumentReplacemetB curRep;
                BookmarkRangeStart bookmarkRangeStart;
                RadFlowDocument curDoc = LoadSampleDocument(pageUniqueName);

                //------------------- Styles Start -------------------------------------------
                Telerik.Windows.Documents.Flow.Model.Styles.Style styleTimes11 = new Telerik.Windows.Documents.Flow.Model.Styles.Style("styleTimes11", StyleType.Paragraph);
                styleTimes11.CharacterProperties.FontFamily.LocalValue = new ThemableFontFamily("Times New Roman");
                styleTimes11.CharacterProperties.FontSize.LocalValue = 14.20833333333333;
                styleTimes11.CharacterProperties.FontWeight.LocalValue = FontWeights.Normal;
                //styleTimes11.CharacterProperties.ForegroundColor.LocalValue = DefaultForeground;
                curDoc.StyleRepository.Add(styleTimes11);
                Telerik.Windows.Documents.Flow.Model.Styles.Style styleArial10 = new Telerik.Windows.Documents.Flow.Model.Styles.Style("styleArial10", StyleType.Paragraph);
                styleArial10.CharacterProperties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                styleArial10.CharacterProperties.FontSize.LocalValue = 12.91666666666667;
                styleArial10.CharacterProperties.FontWeight.LocalValue = FontWeights.Normal;
                curDoc.StyleRepository.Add(styleArial10);
                Telerik.Windows.Documents.Flow.Model.Styles.Style styleArial11 = new Telerik.Windows.Documents.Flow.Model.Styles.Style("styleArial11", StyleType.Paragraph);
                styleArial11.CharacterProperties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                styleArial11.CharacterProperties.FontSize.LocalValue = 14.20833333333333;
                styleArial11.CharacterProperties.FontWeight.LocalValue = FontWeights.Normal;
                curDoc.StyleRepository.Add(styleArial11);
                Telerik.Windows.Documents.Flow.Model.Styles.Style styleArial11Bold = new Telerik.Windows.Documents.Flow.Model.Styles.Style("styleArial11Bold", StyleType.Paragraph);
                styleArial11Bold.CharacterProperties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                styleArial11Bold.CharacterProperties.FontSize.LocalValue = 14.20833333333333;
                styleArial11Bold.CharacterProperties.FontWeight.LocalValue = FontWeights.Bold;
                curDoc.StyleRepository.Add(styleArial11Bold);
                //------------------- Styles End -------------------------------------------

                RadFlowDocumentEditor editor = new RadFlowDocumentEditor(curDoc);
                //System.Text.RegularExpressions.Regex textRegex = new System.Text.RegularExpressions.Regex("ΣΑΟΥΣΟΠΟΥΛΟΥ ΑΝΝΑ");
                //editor.ReplaceText("ΣΑΟΥΣΟΠΟΥΛΟΥ ΑΝΝΑ", txtNew.Text, true, true);
                List<BookmarkRangeStart> test = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().ToList();
                Telerik.Windows.Documents.Flow.Model.TableCell currCell;
                Run currRun;


                Header defaultHeader = editor.Document.Sections.First().Headers.Default;
                //Telerik.Windows.Documents.Flow.Model.Table headerTable = defaultHeader.Blocks.OfType<Telerik.Windows.Documents.Flow.Model.Table>().First();
                //Telerik.Windows.Documents.Flow.Model.TableCell firstCell = headerTable.Rows[0].Cells[0];

                curRep = reps.Find(o => o.UniqueName == "KET_Header_OTELogo");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test[2].Paragraph.BlockContainer;
                using (Stream firstImage = File.OpenRead(imgFolderPath + curRep.Text)) {
                    var inImage = ((Paragraph)currCell.Blocks.First()).Inlines.AddImageInline();
                    inImage.Image.ImageSource = new Telerik.Windows.Documents.Media.ImageSource(firstImage, curRep.Text.Split('.').Last());
                    if (curRep.ImageHeight != null && curRep.ImageWidth != null) {
                        inImage.Image.Height = curRep.ImageHeight.Value;
                        inImage.Image.Width = curRep.ImageWidth.Value;
                    }
                }

                curRep = reps.Find(o => o.UniqueName == "KET_Header_OTEMoto");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test[3].Paragraph.BlockContainer;
                //editor.MoveToParagraphStart((Paragraph)currCell.Blocks.First());
                //editor.ParagraphFormatting.StyleId = curDoc.StyleRepository.GetStyle("styleArial10").Id;
                //curDoc.StyleRepository.GetStyle("styleArial11");
                //editor.InsertText("ΟΡΓΑΝΙΣΜΟΣ ΤΗΛΕΠΙΚΟΙΝΩΝΙΩΝ ΤΗΣ ΕΛΛΑΔΟΣ Α.Ε.");
                currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                currRun.Text = curRep.Text;
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 13.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                curRep = reps.Find(o => o.UniqueName == "KET_Header_Title");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test[4].Paragraph.BlockContainer;
                currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                currRun.Text = curRep.Text;
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 15.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                curRep = reps.Find(o => o.UniqueName == "KET_Header_Department");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test[5].Paragraph.BlockContainer;
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
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test[6].Paragraph.BlockContainer;
                currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                currRun.Text = curRep.Text;
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 15.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                curRep = reps.Find(o => o.UniqueName == "KET_Header_PageNo");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test[7].Paragraph.BlockContainer;
                editor.MoveToParagraphStart((Paragraph)currCell.Blocks.First());
                editor.InsertText("ΣΕΛΙΔΑ ");
                editor.InsertField("PAGE", "3");
                if (curRep.Text == "Σελίδα Χ από Υ") {
                    editor.InsertText(" ΑΠΟ ");
                    editor.InsertField("NUMPAGES", "5");
                }

                curRep = reps.Find(o => o.UniqueName == "KET_Header_Date");
                bookmarkRangeStart = defaultHeader.EnumerateChildrenOfType<BookmarkRangeStart>().Where(rangeStart => rangeStart.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault();
                //BookmarkRangeStart bookmarkRangeStart2 = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().Where(rangeStart => rangeStart.Bookmark.Name == "OTELogo").FirstOrDefault();
                editor.MoveToInlineEnd(bookmarkRangeStart);
                editor.InsertText(DateTime.Now.ToString(curRep.Text, new CultureInfo("el-GR")));

                curRep = reps.Find(o => o.UniqueName == "KET_Header_To");
                bookmarkRangeStart = defaultHeader.EnumerateChildrenOfType<BookmarkRangeStart>().Where(rangeStart => rangeStart.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault();
                editor.MoveToInlineEnd(bookmarkRangeStart);
                editor.InsertText(curRep.Text);

                //firstCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test[2].Paragraph.BlockContainer;
                //editor.MoveToParagraphStart((Paragraph)firstCell.Blocks.First());
                //editor.InsertText("tralala");
                //firstCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test[2].Paragraph.BlockContainer;
                //editor.MoveToParagraphStart((Paragraph)firstCell.Blocks.First());
                //editor.InsertText("tralala");
                //Telerik.Windows.Documents.Flow.Model.Table table = (Telerik.Windows.Documents.Flow.Model.Table)defaultHeader.Blocks.First().EnumerateChildrenOfType<Telerik.Windows.Documents.Flow.Model.Table>().FirstOrDefault();
                curDoc.UpdateFields();
                exportDOCX(curDoc);
            }
            catch (Exception ex) { }
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