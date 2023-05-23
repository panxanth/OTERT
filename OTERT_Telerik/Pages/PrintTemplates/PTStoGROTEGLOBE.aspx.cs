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

    public partial class PTStoGROTEGLOBE : Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected Button btnPrint;
        protected string pageTitle, uploadedFilePath, textFromSession;
        protected UserB loggedUser;
        const string templatesFolder = "~/Templates/";
        const string fileUploadFolder = "~/UploadedFiles/";
        const string pageUniqueName = "PTStoGRPTStoGROTEGLOBE_";

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Διαχείριση Εκτυπώσεων > ΠΤΣ προς Ελλάδα - Επιστολή Οφειλών προς OTEGLOBE";
                gridMain.MasterTableView.Caption = "Διαχείριση Εκτυπώσεων > ΠΤΣ προς Ελλάδα - Επιστολή Οφειλών προς OTEGLOBE";
                textFromSession = "";
                Session.Remove("textFromSession");
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
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

    }

}