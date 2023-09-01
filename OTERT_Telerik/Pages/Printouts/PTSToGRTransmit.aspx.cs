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
using static System.Net.Mime.MediaTypeNames;
using Telerik.Web.UI.ImageEditor;

namespace OTERT.Pages.Printouts {

    public partial class PTSToGRTransmit : Page {

        protected RadDatePicker dpDateFrom, dpDateTo;
        protected TextBox txtNumber, txtNumberInvoice;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected RadButton btnCreate;
        protected Label lblCustomer, lblDateFrom, lblDateTo, lblDateCreated;
        protected string pageTitle;
        protected UserB loggedUser;
        const string fileUploadFolder = "~/UploadedFiles/";
        const string templatesFolder = "~/Templates/";
        const string pageUniqueName = "PTSToGRTransmit";

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Εκτυπωτικά > ΠΤΣ προς Ελλάδα - Λογαριασμοί από Ανταποκρίσεις Πελατών Εξωτερικου";
                dpDateFrom.SelectedDate = DateTime.Today.AddMonths(-1).Date;
                dpDateTo.SelectedDate = DateTime.Today.Date;
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected void btnCreate_Click(object sender, EventArgs e) {
            try{
                string strNumber = txtNumber.Text;  
                string strNumberInvoice = txtNumberInvoice.Text;
                DateTime DateFrom = (dpDateFrom.SelectedDate != null ? (DateTime)dpDateFrom.SelectedDate : DateTime.Now);
                DateTime DateTo = (dpDateTo.SelectedDate != null ? (DateTime)dpDateTo.SelectedDate : DateTime.Now);
                string dateSpan = getDatesSpan(DateFrom, DateTo);
                // Prepare Document
                DocumentReplacemetsController cont = new DocumentReplacemetsController();
                List<DocumentReplacemetB> reps = new List<DocumentReplacemetB>();
                reps = cont.GetDocumentReplacemets(pageUniqueName);
                string imgFolderPath = Server.MapPath(fileUploadFolder);
                DocumentReplacemetB curRep;
                // BookmarkRangeStart bookmarkRangeStart;
                RadFlowDocument curDoc = LoadWordTemplate(pageUniqueName);
                RadFlowDocumentEditor editor = new RadFlowDocumentEditor(curDoc);
                List<BookmarkRangeStart> docBookmarks = editor.Document.EnumerateChildrenOfType<BookmarkRangeStart>().ToList();
                Telerik.Windows.Documents.Flow.Model.TableCell currCell;
                Run currRun;
                Paragraph currPar;
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

                // OTE Logo
                curRep = reps.Find(o => o.UniqueName == "PTStoGRTransmit_OTE_Logo");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                using (Stream firstImage = File.OpenRead(imgFolderPath + curRep.Text)) {
                    var inImage = ((Paragraph)currCell.Blocks.First()).Inlines.AddImageInline();
                    inImage.Image.ImageSource = new Telerik.Windows.Documents.Media.ImageSource(firstImage, curRep.Text.Split('.').Last());
                    if (curRep.ImageHeight != null && curRep.ImageWidth != null) {
                        inImage.Image.Height = curRep.ImageHeight.Value;
                        inImage.Image.Width = curRep.ImageWidth.Value;
                    }
                }

                // Department
                curRep = reps.Find(o => o.UniqueName == "PTStoGRTransmit_Department");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                string[] arrTextDepartment = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                currPar = (Paragraph)currCell.Blocks.First();
                currPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                for (int i = 0; i < arrTextDepartment.Length; i++) {
                    currRun = editor.InsertLine(arrTextDepartment[i]);
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Paragraph.Spacing.LineSpacing = 1.25;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 15.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                }
                currCell.Blocks.Remove(currCell.Blocks.Last());

                // Date
                curRep = reps.Find(o => o.UniqueName == "PTStoGRTransmit_Date");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                currPar = (Paragraph)currCell.Blocks.First();
                currPar.Properties.TextAlignment.LocalValue = Alignment.Left;
                editor.MoveToInlineEnd(((Paragraph)currCell.Blocks.First()).Inlines.First());
                currRun = editor.InsertLine("Αθήνα, " + DateTime.Now.ToString(curRep.Text, new System.Globalization.CultureInfo("el-GR")));
                currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 15.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                currRun = editor.InsertLine("Αριθ. 2 / " + strNumber);
                currRun.Paragraph.ContextualSpacing = true;
                currRun.Paragraph.Spacing.LineSpacing = 1.25;
                currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 15.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                currCell.Blocks.Remove(currCell.Blocks.Last());

                // From
                curRep = reps.Find(o => o.UniqueName == "PTStoGRTransmit_From");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                string[] arrTextFrom = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                currPar = (Paragraph)currCell.Blocks.First();
                currPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                for (int i = 0; i < arrTextFrom.Length; i++) {
                    currRun = editor.InsertLine(arrTextFrom[i]);
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Paragraph.Spacing.LineSpacing = 1.5;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 15.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                }
                currCell.Blocks.Remove(currCell.Blocks.Last());

                // To
                curRep = reps.Find(o => o.UniqueName == "PTStoGRTransmit_To");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                string[] arrTextTo = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                currPar = (Paragraph)currCell.Blocks.First();
                currPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                for (int i = 0; i < arrTextTo.Length; i++) {
                    currRun = editor.InsertLine(arrTextTo[i]);
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Left;
                    currRun.Paragraph.Spacing.LineSpacing = 1.5;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 15.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                }
                currCell.Blocks.Remove(currCell.Blocks.Last());

                // Subject
                curRep = reps.Find(o => o.UniqueName == "PTStoGRTransmit_Subject");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                currRun.Text += curRep.Text;
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 15.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                // Main Body
                curRep = reps.Find(o => o.UniqueName == "PTStoGRTransmit_MainBody");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                curRep.Text = curRep.Text.Replace("#timespan#", DateFrom.ToString("dd/MM/yyyy") + " - " + DateTo.ToString("dd/MM/yyyy"));
                curRep.Text = curRep.Text.Replace("#invoiceno#", strNumberInvoice);
                string[] arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                currPar = (Paragraph)currCell.Blocks.First();
                currPar.Properties.TextAlignment.LocalValue = Alignment.Justified;
                currPar.Spacing.LineSpacing = 1.5;
                editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                for (int i = 0; i < arrText.Length; i++) {
                    if (!string.IsNullOrEmpty(arrText[i])) {
                        string[] arrTextBold = arrText[i].Split(new string[] { "/b/" }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrTextBold.Length > 1) {
                            for (int k = 0; k < arrTextBold.Length; k++) {
                                if (k == arrTextBold.Length - 1) {
                                    currRun = editor.InsertLine(arrTextBold[k]);
                                } else {
                                    currRun = editor.InsertText(arrTextBold[k]);
                                }
                                currRun.Paragraph.ContextualSpacing = true;
                                currRun.Paragraph.Spacing.LineSpacing = 1.5;
                                currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Justified;
                                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                                currRun.Properties.FontSize.LocalValue = 15.0;
                                if (k % 2 == 1) {
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                } else {
                                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                                }
                                //if (i == 0) { currRun.Properties.FontWeight.LocalValue = FontWeights.Bold; }
                                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                            }
                        } else {
                            currRun = editor.InsertLine(arrText[i]);
                            currRun.Paragraph.ContextualSpacing = true;
                            currRun.Paragraph.Spacing.LineSpacing = 1.5;
                            currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Justified;
                            currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                            currRun.Properties.FontSize.LocalValue = 15.0;
                            currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                            currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                        }
                    }
                }
                currCell.Blocks.Remove(currCell.Blocks.Last());

                // Chief Name
                curRep = reps.Find(o => o.UniqueName == "PTStoGRTransmit_Chief_Name");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)docBookmarks.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                string[] arrTextChiefName = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                currPar = (Paragraph)currCell.Blocks.First();
                currPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                for (int i = 0; i < arrTextChiefName.Length; i++) {
                    currRun = editor.InsertLine(arrTextChiefName[i]);
                    currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Center;
                    currRun.Paragraph.Spacing.LineSpacing = 1.5;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 15.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                }
                currCell.Blocks.Remove(currCell.Blocks.Last());

                curDoc.UpdateFields();
                string filename = "";
                if (!string.IsNullOrEmpty(strNumberInvoice)) {
                    filename = "Transmit_" + strNumberInvoice + ".docx";
                } else {
                    filename = "Transmit_" + dateSpan.Replace(" / ", " - ").Replace(" ", "_") + ".docx";
                }
                exportDOCX(curDoc, filename);
            }
            catch (Exception) { }
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

        protected void exportDOCX(RadFlowDocument doc, string filename) {
            IFormatProvider<RadFlowDocument> formatProvider = new DocxFormatProvider();
            byte[] renderedBytes = null;
            using (MemoryStream ms = new MemoryStream()) {
                formatProvider.Export(doc, ms);
                renderedBytes = ms.ToArray();
            }
            Response.ClearHeaders();
            Response.ClearContent();
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename);
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