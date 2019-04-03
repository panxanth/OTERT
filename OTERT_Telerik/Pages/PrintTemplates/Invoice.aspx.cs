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

namespace OTERT.Pages.PrintTemplates {

    public partial class Invoice : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected Button btnPrint;
        protected string pageTitle, uploadedFilePath, textFromSession;
        const string templatesFolder = "~/Templates/";
        const string fileUploadFolder = "~/UploadedFiles/";
        const string pageUniqueName = "Invoice";

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Εκτυπώσεων > Τιμολόγιο (Συνοπτικό)";
                gridMain.MasterTableView.Caption = "Διαχείριση Εκτυπώσεων > Τιμολόγιο (Συνοπτικό)";
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
                Paragraph newPar;

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

                curRep = reps.Find(o => o.UniqueName == "Invoice_Department_Title");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                string[] arrText = curRep.Text.Replace("\r\n", "#").Replace("\n", "#").Split(new char[] { '#' });
                newPar = (Paragraph)currCell.Blocks.First();
                //newPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                for (int i = 0; i < arrText.Length; i++) {
                    currRun = editor.InsertLine(arrText[i]);
                    //currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Center;
                    currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                    currRun.Properties.FontSize.LocalValue = 13.0;
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                }
                currCell.Blocks.Remove(currCell.Blocks.Last());

                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == "Header_Date").FirstOrDefault().Paragraph.BlockContainer;
                currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                currRun.Text = DateTime.Now.ToString("d/M/yyyy", new System.Globalization.CultureInfo("el-GR"));
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 13.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                curRep = reps.Find(o => o.UniqueName == "Invoice_Title");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                currRun.Text = curRep.Text;
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 15.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == "Date_From").FirstOrDefault().Paragraph.BlockContainer;
                currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                currRun.Text = "25/12/2016";
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 15.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == "Date_To").FirstOrDefault().Paragraph.BlockContainer;
                currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                currRun.Text = "30/12/2016";
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 15.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                curRep = reps.Find(o => o.UniqueName == "Invoice_Chief_Name");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                newPar = (Paragraph)currCell.Blocks.First();
                newPar.Properties.TextAlignment.LocalValue = Alignment.Center;
                editor.MoveToInlineStart(((Paragraph)currCell.Blocks.First()).Inlines.First());
                currRun = editor.InsertLine(curRep.Text);
                currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Center;
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 13.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                currRun = editor.InsertLine(" ");
                currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Center;
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 13.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                currRun = editor.InsertLine(reps.Find(o => o.UniqueName == "Invoice_Chief_Title").Text);
                currRun.Paragraph.Properties.TextAlignment.LocalValue = Alignment.Center;
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 11.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;
                currCell.Blocks.Remove(currCell.Blocks.Last());

                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == "Footer_Date").FirstOrDefault().Paragraph.BlockContainer;
                currRun = ((Paragraph)currCell.Blocks.First()).Inlines.AddRun();
                currRun.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("el-GR"));
                currRun.Properties.FontFamily.LocalValue = new ThemableFontFamily("Arial");
                currRun.Properties.FontSize.LocalValue = 13.0;
                currRun.Properties.FontWeight.LocalValue = FontWeights.Normal;
                currRun.Properties.FontStyle.LocalValue = FontStyles.Normal;

                curRep = reps.Find(o => o.UniqueName == "Invoice_PageNo");
                currCell = (Telerik.Windows.Documents.Flow.Model.TableCell)test.Where(o => o.Bookmark.Name == curRep.BookmarkTitle).FirstOrDefault().Paragraph.BlockContainer;
                editor.MoveToParagraphStart((Paragraph)currCell.Blocks.First());
                editor.InsertText("ΣΕΛΙΔΑ ");
                editor.InsertField("PAGE", "3");
                if (curRep.Text == "Σελίδα Χ από Υ") {
                    editor.InsertText(" ΑΠΟ ");
                    editor.InsertField("NUMPAGES", "5");
                }

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

        protected List<TaskForH> createDummyList() {
            List<TaskForH> list2return = new List<TaskForH>();
            for (int i=1; i<11; i++) {
                TaskForH newTask = new TaskForH();
                newTask.Count = i;
                newTask.Customer = "Πελάτης " + i.ToString();
                newTask.FromPlace = "Τοποθεσία " + i.ToString();
                newTask.FromPlace = "Τοποθεσία " + (i+10).ToString();
                Random random = new System.Random();
                int rndValue1 = random.Next(0, 10);
                int rndValue2 = random.Next(0, 10);
                newTask.FromTime = rndValue1.ToString() + ":00";
                newTask.ToTime = (rndValue1+rndValue2).ToString() + ":00";
                newTask.Comments = "Παρατηρήσεις " + i.ToString();
                list2return.Add(newTask);
            }
            return list2return;
        }

    }

}