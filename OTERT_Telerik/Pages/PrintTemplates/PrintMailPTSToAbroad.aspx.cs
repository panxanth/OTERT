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

    public partial class PrintMailPTSToAbroad : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected Button btnPrint;
        protected string pageTitle, uploadedFilePath, textFromSession;
        protected UserB loggedUser;
        const string templatesFolder = "~/Templates/";
        const string fileUploadFolder = "~/UploadedFiles/";
        const string sqlUniqueName = "mailPTSToAbroad";
        const string docTemplate = "mailCosmote";
        const int OrderTypeID = 2;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Εκτυπωτικά > Επιστολή ΠΤΣ προς Εξωτερικό";
                gridMain.MasterTableView.Caption = "Εκτυπωτικά > Επιστολή ΠΤΣ προς Εξωτερικό";
                textFromSession = "";
                Session.Remove("textFromSession");
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
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
                    currRun = editor.InsertText("ATHENS " + DateTime.Now.ToString("dd/MM/yyyy"));
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = insertParagraph(editor);
                    currPar.TextAlignment = Alignment.Right;
                    string regNo = curOrder.RegNo == null ? "" : curOrder.RegNo;
                    currRun = editor.InsertText("REF. " + regNo);
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    Telerik.Windows.Documents.Flow.Model.Table infoTable = editor.InsertTable(4, 2);
                    infoTable.Borders = new TableBorders(new Border(Telerik.Windows.Documents.Flow.Model.Styles.BorderStyle.None));
                    currPar = infoTable.Rows[0].Cells[0].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    currRun = currPar.Inlines.AddRun("Information:");
                    currPar = infoTable.Rows[0].Cells[1].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    curRep = reps.Where(o => o.Title == "Πληροφορίες").FirstOrDefault();
                    if (curRep != null) {
                        currRun = currPar.Inlines.AddRun(curRep.Text);
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    }
                    currPar = infoTable.Rows[1].Cells[0].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    currRun = currPar.Inlines.AddRun("Phone:");
                    currPar = infoTable.Rows[1].Cells[1].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    curRep = reps.Where(o => o.Title == "Τηλέφωνο").FirstOrDefault();
                    if (curRep != null) {
                        currRun = currPar.Inlines.AddRun(curRep.Text);
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    }
                    currPar = infoTable.Rows[2].Cells[0].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    currRun = currPar.Inlines.AddRun("FAX:");
                    currPar = infoTable.Rows[2].Cells[1].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    curRep = reps.Where(o => o.Title == "FAX").FirstOrDefault();
                    if (curRep != null) {
                        currRun = currPar.Inlines.AddRun(curRep.Text);
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    }
                    currPar = infoTable.Rows[3].Cells[0].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    currRun = currPar.Inlines.AddRun("Email:");
                    currPar = infoTable.Rows[3].Cells[1].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    curRep = reps.Where(o => o.Title == "Email").FirstOrDefault();
                    if (curRep != null) {
                        Run currRun2 = currPar.Inlines.AddRun();
                        editor.MoveToInlineEnd(currRun2);
                        editor.InsertHyperlink(curRep.Text, "mailto:"+ curRep.Text, false);
                        editor.MoveToTableEnd(infoTable);
                    }
                    currPar = insertParagraph(editor);
                    editor.MoveToParagraphStart(currPar);
                    Telerik.Windows.Documents.Flow.Model.Table toTable = editor.InsertTable(3, 2);
                    toTable.Borders = new TableBorders(new Border(Telerik.Windows.Documents.Flow.Model.Styles.BorderStyle.None));
                    currPar = toTable.Rows[0].Cells[0].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    currRun = currPar.Inlines.AddRun("ATT.:");
                    currPar = toTable.Rows[0].Cells[1].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    if (!string.IsNullOrEmpty(tasksForOrder[0].Customer.NameEN)) {
                        currRun = currPar.Inlines.AddRun(tasksForOrder[0].Customer.NameEN);
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    }
                    currPar = toTable.Rows[1].Cells[0].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    currRun = currPar.Inlines.AddRun("Phone:");
                    currPar = toTable.Rows[1].Cells[1].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    if (!string.IsNullOrEmpty(tasksForOrder[0].Customer.Telephone1)) {
                        currRun = currPar.Inlines.AddRun(tasksForOrder[0].Customer.Telephone1);
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    }
                    currPar = toTable.Rows[2].Cells[0].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    currRun = currPar.Inlines.AddRun("FAX:");
                    currPar = toTable.Rows[2].Cells[1].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    if (!string.IsNullOrEmpty(tasksForOrder[0].Customer.FAX1)) {
                        currRun = currPar.Inlines.AddRun(tasksForOrder[0].Customer.FAX1);
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    }
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("SUBJECT:  TEMPORARΥ TELEPHONE LINES");
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    currPar.TextAlignment = Alignment.Justified;
                    currRun = editor.InsertText("You are kindly requested to provide " + tasksForOrder.Count.ToString() + " temporary telephone lines.");
                    currPar = insertParagraph(editor);
                    Telerik.Windows.Documents.Flow.Model.Table eventTable = editor.InsertTable(2, 2);
                    eventTable.Borders = new TableBorders(new Border(Telerik.Windows.Documents.Flow.Model.Styles.BorderStyle.None));
                    currPar = eventTable.Rows[0].Cells[0].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    currRun = currPar.Inlines.AddRun("in:");
                    currPar = eventTable.Rows[0].Cells[1].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    string curP = curOrder.Event.Place.NameEN == null ? "" : curOrder.Event.Place.NameEN;
                    currRun = currPar.Inlines.AddRun(curP);
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = eventTable.Rows[1].Cells[0].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    currRun = currPar.Inlines.AddRun("event:");
                    currPar = eventTable.Rows[1].Cells[1].Blocks.AddParagraph();
                    currPar.Spacing.SpacingAfter = 0;
                    string curE = curOrder.Event.NameEN == null ? "" : curOrder.Event.NameEN;
                    currRun = currPar.Inlines.AddRun(curE);
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    int tRows = tasksForOrder.Count + 1;
                    Telerik.Windows.Documents.Flow.Model.Table contentTable = editor.InsertTable(tRows, 5);
                    toTable.Borders = new TableBorders(new Border(Telerik.Windows.Documents.Flow.Model.Styles.BorderStyle.None));
                    currPar = contentTable.Rows[0].Cells[0].Blocks.AddParagraph();
                    currRun = currPar.Inlines.AddRun("Customer Name");
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = contentTable.Rows[0].Cells[1].Blocks.AddParagraph();
                    currRun = currPar.Inlines.AddRun("From");
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = contentTable.Rows[0].Cells[2].Blocks.AddParagraph();
                    currRun = currPar.Inlines.AddRun("To");
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = contentTable.Rows[0].Cells[3].Blocks.AddParagraph();
                    currRun = currPar.Inlines.AddRun("Place of Instal.");
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = contentTable.Rows[0].Cells[4].Blocks.AddParagraph();
                    currRun = currPar.Inlines.AddRun("");
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    int curRow = 1;
                    foreach (TaskB curTask in tasksForOrder) {
                        currPar = contentTable.Rows[curRow].Cells[0].Blocks.AddParagraph();
                        currRun = currPar.Inlines.AddRun(curCust.NameEN);
                        currPar = contentTable.Rows[curRow].Cells[1].Blocks.AddParagraph();
                        currRun = currPar.Inlines.AddRun(curTask.DateTimeStartOrder.ToString());
                        currPar = contentTable.Rows[curRow].Cells[2].Blocks.AddParagraph();
                        currRun = currPar.Inlines.AddRun(curTask.DateTimeEndOrder.ToString());
                        currPar = contentTable.Rows[curRow].Cells[3].Blocks.AddParagraph();
                        currRun = currPar.Inlines.AddRun(curTask.RequestedPosition.NameEN);
                        currPar = contentTable.Rows[curRow].Cells[4].Blocks.AddParagraph();
                        string LineType2Print = curTask.LineType.Name;
                        if (curTask.Internet == true) { LineType2Print += " + WIFI ROUTER"; }
                        currRun = currPar.Inlines.AddRun(LineType2Print);
                        curRow++;
                    }
                    var distinctTasksLT = tasksForOrder.Select(m => new { m.LineTypeID, m.Internet, m.MSN }).Distinct().ToList();
                    string concatString = "";
                    int distinctItems = 0;
                    foreach (var dItem in distinctTasksLT) {
                        if (distinctItems > 0 && distinctItems < distinctTasksLT.Count-1) { concatString += ", "; } else if (distinctItems > 0 && distinctItems == distinctTasksLT.Count-1) { concatString += " & "; }
                        TaskB demoTask = tasksForOrder.Where(k => k.LineTypeID == dItem.LineTypeID && k.Internet == dItem.Internet && k.MSN == dItem.MSN).FirstOrDefault();
                        int itemCount = tasksForOrder.Where(k => k.LineTypeID == dItem.LineTypeID && k.Internet == dItem.Internet && k.MSN == dItem.MSN).Count();
                        concatString += itemCount.ToString() + " " + demoTask.LineType.Name;
                        if (demoTask.Internet == true && demoTask.MSN == true) { concatString += " with router and MSN"; } else if (demoTask.Internet == true) { concatString += " with router"; } else if (demoTask.MSN == true) { concatString += " with MSN"; }
                        distinctItems++;
                    }
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("Please provide " + concatString);
                    currRun.Properties.FontSize.LocalValue = Telerik.Windows.Documents.Media.Unit.PointToDip(9);
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("Contact Person: " + reps.Where(o => o.Title == "Contact Person").FirstOrDefault().Text + "  Mob:" + reps.Where(o => o.Title == "Κινητό C.P.").FirstOrDefault().Text);
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("Please advise on allocated numbers A.S.A.P. to the e-mail address: ");
                    editor.InsertHyperlink(curRep.Text, "mailto:" + curRep.Text, false);
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("HELLENIC TELECOMMUNICATIONS ORGANIZATION SA (OTE GROUP OF COMPANIES)");
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("VAT Number: ");
                    currRun = editor.InsertText("EL 094019245");
                    currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("Responsible for the payments is our company:");
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("HELLENIC TELECOMMUNICATIONS ORGANIZATION SA (OTE GROUP OF COMPANIES) via its Billing department bellow");
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("Delivery invoice address for the original invoice (Post mail):");
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("OTE INTERNATIONAL SOLUTION SA (OTEGLOBE)");
                    curRep = reps.Where(o => o.Title == "OTEGlobe Διεύθυνση 1").FirstOrDefault();
                    currPar = insertParagraph(editor);
                    if (curRep != null) {
                        //currPar = insertParagraph(editor);
                        //currRun = editor.InsertText(curRep.Text);
                        currRun = editor.InsertLine(curRep.Text);
                    }
                    curRep = reps.Where(o => o.Title == "OTEGlobe Διεύθυνση 2").FirstOrDefault();
                    if (curRep != null) {
                        //currPar = insertParagraph(editor);
                        //currRun = editor.InsertText(curRep.Text);
                        currRun = editor.InsertLine(curRep.Text);
                    }
                    curRep = reps.Where(o => o.Title == "OTEGlobe Site").FirstOrDefault();
                    if (curRep != null) {
                        //currPar = insertParagraph(editor);
                        editor.InsertHyperlink(curRep.Text, "http://" + curRep.Text, false);
                    }
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    string rep = "";
                    curRep = reps.Where(o => o.Title == "Person In Charge").FirstOrDefault();
                    if (curRep != null) { rep += curRep.Text; }
                    curRep = reps.Where(o => o.Title == "Person In Charge (Phone)").FirstOrDefault();
                    if (curRep != null) { rep += curRep.Text; }
                    if (!string.IsNullOrEmpty(rep)) {
                        currRun = editor.InsertText("(Person in charge " + rep + ")");
                        currPar = insertParagraph(editor);
                    }
                    curRep = reps.Where(o => o.Title == "Person In Charge (Email)").FirstOrDefault();
                    if (curRep != null) {
                        currRun = editor.InsertText("Email: ");
                        editor.InsertHyperlink(curRep.Text, "mailto:" + curRep.Text, false);
                        currPar = insertParagraph(editor);
                    }
                    currRun = editor.InsertText("Email: ");
                    editor.InsertHyperlink("CreditControl@oteglobe.gr", "mailto:CreditControl@oteglobe.gr", false);
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    curRep = reps.Where(o => o.Title == "Email").FirstOrDefault();
                    if (curRep != null) {
                        currPar = insertParagraph(editor);
                        currRun = editor.InsertText("You could notify a copy of the invoice (as a pdf) to this e-mail: ");
                        editor.InsertHyperlink(curRep.Text, "mailto:" + curRep.Text, false);
                        currRun.Properties.FontWeight.LocalValue = FontWeights.Bold;
                    }
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("The charges will be settled via Bank transfer.");
                    currPar = insertParagraph(editor);
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("Best regards,");
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("SAOUSOPOULOU ANNA");
                    currPar = insertParagraph(editor);
                    currRun = editor.InsertText("MANAGER OF GENTRANS, ATHENS");
                    curDoc.UpdateFields();
                    exportDOCX(curDoc);
                }
                catch (Exception) { }
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