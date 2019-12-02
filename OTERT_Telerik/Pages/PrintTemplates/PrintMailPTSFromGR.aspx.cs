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

    public partial class PrintMailPTSFromGR : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected Button btnPrint;
        protected string pageTitle, uploadedFilePath, textFromSession;
        const string templatesFolder = "~/Templates/";
        const string fileUploadFolder = "~/UploadedFiles/";
        const string sqlUniqueName = "mailPTSFromGR";
        const string docTemplate = "mailCosmote";
        const int OrderTypeID = 2;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Εκτυπωτικά > Επιστολή ΠΤΣ προς Εξωτερικό";
                gridMain.MasterTableView.Caption = "Εκτυπωτικά > Επιστολή ΠΤΣ προς Εξωτερικό";
                textFromSession = "";
                Session.Remove("textFromSession");
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

        protected void gridMain_ItemCommand(object sender, GridCommandEventArgs e) {
            if (e.CommandName == "invPrint") {
                GridDataItem item = (GridDataItem)e.Item;
                int orderID = (int)((GridDataItem)e.Item).GetDataKeyValue("ID");
                OrdersController oCont = new OrdersController();
                OrderB curOrder = oCont.GetOrder(orderID);
                TasksController lCont = new TasksController();
                List<TaskB> tasksForOrder = lCont.GetTasksForOrder(orderID);
                CustomersController cCont = new CustomersController();
                CustomerB curCust = cCont.GetCustomer(curOrder.Customer1.ID);
                try {
                    DocumentReplacemetsController cont = new DocumentReplacemetsController();
                    List<DocumentReplacemetB> reps = new List<DocumentReplacemetB>();
                    reps = cont.GetDocumentReplacemets(sqlUniqueName);
                    DocumentReplacemetB curRep;
                    string imgFolderPath = Server.MapPath(fileUploadFolder);
                    RadFlowDocument curDoc = LoadTemplateDocument(docTemplate);
                    RadFlowDocumentEditor editor = new RadFlowDocumentEditor(curDoc);
                    Run currRun;
                    Paragraph currPar;
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    currPar.TextAlignment = Alignment.Right;
                    currRun = editor.InsertText("ΑΘΗΝΑ " + DateTime.Now.ToString("dd/MM/yyyy"));
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = insertParagraph(editor);
                    currPar.TextAlignment = Alignment.Right;
                    string regNo = curOrder.RegNo == null ? "" : curOrder.RegNo;
                    currRun = editor.InsertText("ΑΡΙΘ. ΠΡΩΤ. " + regNo);
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("ΟΡΓAΝΙΣΜΟΣ ΤΗΛΕΠΙΚΟΙΝΩΝΙΩΝ ΤΗΣ ΕΛΛΑΔΟΣ ΑΕ");
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("GENTRANS ATHENS");
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = insertParagraph(editor);
                    Telerik.Windows.Documents.Flow.Model.Table infoTable = editor.InsertTable(3, 2);
                    infoTable.Borders = new TableBorders(new Border(Telerik.Windows.Documents.Flow.Model.Styles.BorderStyle.None));
                    currPar = infoTable.Rows[0].Cells[0].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    currRun = currPar.Inlines.AddRun("Πληροφορίες:");
                    currPar = infoTable.Rows[0].Cells[1].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    curRep = reps.Where(o => o.Title == "Πληροφορίες").FirstOrDefault();
                    if (curRep != null) { currRun = currPar.Inlines.AddRun(curRep.Text); }
                    currPar = infoTable.Rows[1].Cells[0].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    currRun = currPar.Inlines.AddRun("Τηλέφωνο:");
                    currPar = infoTable.Rows[1].Cells[1].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    curRep = reps.Where(o => o.Title == "Τηλέφωνο").FirstOrDefault();
                    if (curRep != null) { currRun = currPar.Inlines.AddRun(curRep.Text); }
                    currPar = infoTable.Rows[2].Cells[0].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    currRun = currPar.Inlines.AddRun("FAX:");
                    currPar = infoTable.Rows[2].Cells[1].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    curRep = reps.Where(o => o.Title == "FAX").FirstOrDefault();
                    if (curRep != null) { currRun = currPar.Inlines.AddRun(curRep.Text); }
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("ΘΕΜΑ:	ΠΡΟΣΩΡΙΝΕΣ ΤΗΛΕΦΩΝΙΚΕΣ ΣΥΝΔΕΣΕΙΣ");
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = insertParagraph(editor);
                    Telerik.Windows.Documents.Flow.Model.Table toTable = editor.InsertTable(3, 2);
                    toTable.Borders = new TableBorders(new Border(Telerik.Windows.Documents.Flow.Model.Styles.BorderStyle.None));
                    currPar = toTable.Rows[0].Cells[0].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    currRun = currPar.Inlines.AddRun("Αιτηθείσα χώρα:");
                    currPar = toTable.Rows[0].Cells[1].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    string curC = curCust.Country.NameGR == null ? "" : toUpperGR(curCust.Country.NameGR);
                    currRun = currPar.Inlines.AddRun(curC);
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = toTable.Rows[1].Cells[0].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    currRun = currPar.Inlines.AddRun("Χώρος διεξαγωγής:");
                    currPar = toTable.Rows[1].Cells[1].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    string curP = curOrder.Event.Place.NameGR == null ? "" : toUpperGR(curOrder.Event.Place.NameGR);
                    currRun = currPar.Inlines.AddRun(curP);
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = toTable.Rows[2].Cells[0].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    currRun = currPar.Inlines.AddRun("Κάλυψη γεγονότος:");
                    currPar = toTable.Rows[2].Cells[1].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    string curE = curOrder.Event.NameGR == null ? "" : toUpperGR(curOrder.Event.NameGR);
                    currRun = currPar.Inlines.AddRun(curE);
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    currPar.TextAlignment = Alignment.Justified;
                    var distinctTasksLT = tasksForOrder.Select(m => new { m.LineTypeID, m.Internet, m.MSN }).Distinct().ToList();
                    string concatString = "";
                    int distinctItems = 0;
                    foreach (var dItem in distinctTasksLT) {
                        if (distinctItems > 0) { concatString += ", "; }
                        TaskB demoTask = tasksForOrder.Where(k => k.LineTypeID == dItem.LineTypeID && k.Internet == dItem.Internet && k.MSN == dItem.MSN).FirstOrDefault();
                        int itemCount = tasksForOrder.Where(k => k.LineTypeID == dItem.LineTypeID && k.Internet == dItem.Internet && k.MSN == dItem.MSN).Count();
                        concatString += itemCount.ToString() + " " + demoTask.LineType.Name;
                        if (demoTask.Internet == true && demoTask.MSN == true) { concatString += " με ασύρματο ρούτερ και με MSN"; }
                        else if (demoTask.Internet == true) { concatString += " με ασύρματο ρούτερ"; }
                        else if (demoTask.MSN == true) { concatString += " με MSN"; }
                        distinctItems++;
                    }
                    currRun = editor.InsertText("Παρακαλούμε για την παροχή των εξής Προσωρινών Τηλεφωνικών Συνδέσεων για λογαριασμό του πιο κάτω πελάτη στη αντίστοιχη θέση: " + concatString + ".");
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    int tRows = tasksForOrder.Count + 1;
                    Telerik.Windows.Documents.Flow.Model.Table contentTable = editor.InsertTable(tRows, 5);
                    toTable.Borders = new TableBorders(new Border(Telerik.Windows.Documents.Flow.Model.Styles.BorderStyle.None));
                    currPar = contentTable.Rows[0].Cells[0].Blocks.AddParagraph();
                    currRun = currPar.Inlines.AddRun("Πελάτης Εξωτερικού:");
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = contentTable.Rows[0].Cells[1].Blocks.AddParagraph();
                    currRun = currPar.Inlines.AddRun("Από");
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = contentTable.Rows[0].Cells[2].Blocks.AddParagraph();
                    currRun = currPar.Inlines.AddRun("Έως:");
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = contentTable.Rows[0].Cells[3].Blocks.AddParagraph();
                    currRun = currPar.Inlines.AddRun("Θέση:");
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = contentTable.Rows[0].Cells[4].Blocks.AddParagraph();
                    currRun = currPar.Inlines.AddRun("Σύνδεση:");
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    int curRow = 1;
                    foreach (TaskB curTask in tasksForOrder) {
                        currPar = contentTable.Rows[curRow].Cells[0].Blocks.AddParagraph();
                        currRun = currPar.Inlines.AddRun(curCust.NameGR);
                        currPar = contentTable.Rows[curRow].Cells[1].Blocks.AddParagraph();
                        currRun = currPar.Inlines.AddRun(curTask.DateTimeStartOrder.ToString());
                        currPar = contentTable.Rows[curRow].Cells[2].Blocks.AddParagraph();
                        currRun = currPar.Inlines.AddRun(curTask.DateTimeEndOrder.ToString());
                        currPar = contentTable.Rows[curRow].Cells[3].Blocks.AddParagraph();
                        currRun = currPar.Inlines.AddRun(curTask.RequestedPosition.NameGR);
                        currPar = contentTable.Rows[curRow].Cells[4].Blocks.AddParagraph();
                        string LineType2Print = curTask.LineType.Name;
                        if (curTask.Internet == true) { LineType2Print += " + WIFI ROUTER"; }
                        currRun = currPar.Inlines.AddRun(LineType2Print);
                        curRow++;
                    }
                    var distinctTasksC = tasksForOrder.Select(m => new { m.Comments }).Distinct().ToList();
                    if (distinctTasksC.Count > 0) {
                        currPar = insertParagraph(editor);
                        currPar = insertParagraph(editor);
                        currRun = editor.InsertText("ΣΗΜΕΙΩΣΕΙΣ ΠΕΛΑΤΗ:");
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                        currRun.Properties.ForegroundColor.LocalValue = new ThemableColor(System.Windows.Media.Color.FromRgb(255, 0, 0));
                        foreach (var dItem in distinctTasksC) {
                            if (!string.IsNullOrEmpty(dItem.Comments)) {
                                TaskB demoTask = tasksForOrder.Where(k => k.Comments == dItem.Comments).FirstOrDefault();
                                currPar = insertParagraph(editor);
                                currPar.Indentation.LeftIndent = Telerik.Windows.Documents.Media.Unit.InchToDip(1);
                                currRun = editor.InsertText(demoTask.LineType.Name + "  -  " + demoTask.Comments);
                                currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                                currRun.Properties.ForegroundColor.LocalValue = new ThemableColor(System.Windows.Media.Color.FromRgb(255, 0, 0));
                            }
                        }
                    }
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("Εγκατάσταση στο  OB VAN/TV COMPOUND");
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("Ανταποκριτής: Mr. Leo Giovanni  +393357415070");
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("Παρακαλούμε να μας γνωρίσετε τον αριθμό κλήσης που θα διαθέσετε.");
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("H χρέωση θα γίνει από το Τμήμα Ρ/Τ Μεταδόσεων.");
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    currPar.Indentation.LeftIndent = Telerik.Windows.Documents.Media.Unit.InchToDip(3);
                    //currPar.TextAlignment = Alignment.Center;
                    curRep = reps.Where(o => o.Title == "Προϊστάμενος").FirstOrDefault();
                    if (curRep != null) { currRun = editor.InsertText(curRep.Text); }
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = insertParagraph(editor);
                    currPar.Indentation.LeftIndent = Telerik.Windows.Documents.Media.Unit.InchToDip(3);
                    currPar.Indentation.HangingIndent = Telerik.Windows.Documents.Media.Unit.InchToDip(0);
                    currPar.Indentation.RightIndent = Telerik.Windows.Documents.Media.Unit.InchToDip(0);
                    currPar.Indentation.FirstLineIndent = Telerik.Windows.Documents.Media.Unit.InchToDip(0);
                    currPar.TextAlignment = Alignment.Left;
                    curRep = reps.Where(o => o.Title == "Τίτλος").FirstOrDefault();
                    if (curRep != null) { currRun = editor.InsertText(curRep.Text); }
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    curDoc.UpdateFields();
                    exportDOCX(curDoc);
                }
                catch (Exception ex) { }
            }
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