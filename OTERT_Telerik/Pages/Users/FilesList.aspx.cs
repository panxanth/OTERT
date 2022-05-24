using System;
using System.Collections;
using System.Configuration;
using System.Web.UI;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Telerik.Windows.Zip;
using OTERT.Model;
using OTERT.Controller;
using OTERT_Entity;
using System.Collections.Generic;
using System.IO;

namespace OTERT.Pages.UserPages {

    public partial class FilesList : System.Web.UI.Page {

        protected RadGrid gridMain;
        protected RadAjaxManager RadAjaxManager1;
        protected RadWindowManager RadWindowManager1;
        protected string pageTitle;
        protected UserB loggedUser;
        protected Literal litErrors;

        protected void Page_Load(object sender, EventArgs e) {
            if (!Page.IsPostBack) {
                pageTitle = ConfigurationManager.AppSettings["AppTitle"].ToString() + "Αρχεία";
                gridMain.MasterTableView.Caption = "Αρχεία";
                litErrors.Text = "";
            }
            if (Session["LoggedUser"] != null) { loggedUser = Session["LoggedUser"] as UserB; } else { Response.Redirect("/Default.aspx", true); }
        }

        protected void gridMain_NeedDataSource(object sender, GridNeedDataSourceEventArgs e) {
            int recSkip = gridMain.CurrentPageIndex * gridMain.PageSize;
            int recTake = gridMain.PageSize;
            string recFilter = gridMain.MasterTableView.FilterExpression;
            GridSortExpressionCollection gridSortExxpressions = gridMain.MasterTableView.SortExpressions;
            try {
                FilesController cont = new FilesController();
                gridMain.VirtualItemCount = cont.CountFiles(recFilter);
                gridMain.DataSource = cont.GetFilesForList(recSkip, recTake, recFilter, gridSortExxpressions);
            }
            catch (Exception) { }
        }

        protected void gridMain_ItemDataBound(object sender, GridItemEventArgs e) {   
            if (e.Item is GridFilteringItem) {
                GridFilteringItem filterItem = (GridFilteringItem)e.Item;
                RadDropDownList ctflist = (RadDropDownList)filterItem.FindControl("ddlCustomerFilter");
                try {
                    CustomersController ctcont = new CustomersController();
                    ctflist.DataSource = ctcont.GetAllCustomers();
                    ctflist.DataTextField = "NameGR";
                    ctflist.DataValueField = "ID";
                    ctflist.DataBind();
                    ctflist.Items.Insert(0, new DropDownListItem("Κανένα Φίλτρο", "0"));
                }
                catch (Exception) { }
            }
        }

        protected void gridMain_ItemCreated(object sender, GridItemEventArgs e) {
            if (e.Item is GridFilteringItem) {
                GridFilteringItem filterItem = (GridFilteringItem)e.Item;
                (filterItem["DateStamp"].Controls[0] as LiteralControl).Text = "Από: ";
                (filterItem["DateStamp"].Controls[3] as LiteralControl).Text = "<br />Έως: ";
                RadDateTimePicker OrderDateFrom = filterItem["DateStamp"].Controls[1] as RadDateTimePicker;
                OrderDateFrom.TimePopupButton.Visible = false;
                OrderDateFrom.DateInput.DisplayDateFormat = "d/M/yyyy";
                OrderDateFrom.DateInput.DateFormat = "d/M/yyyy";
                RadDateTimePicker OrderDateTo = filterItem["DateStamp"].Controls[4] as RadDateTimePicker;
                OrderDateTo.TimePopupButton.Visible = false;
                OrderDateTo.DateInput.DisplayDateFormat = "d/M/yyyy";
                OrderDateTo.DateInput.DateFormat = "d/M/yyyy";
                OrderDateFrom.DateInput.Attributes.Add("onchange", "javascript:UpdateTo('" + OrderDateFrom.ClientID + "', '" + OrderDateTo.ClientID + "');");
            }
        }

        protected void ddlCustomerFilter_SelectedIndexChanged(object sender, DropDownListEventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            string[] expressions = gridMain.MasterTableView.FilterExpression.Split(new string[] { "AND" }, StringSplitOptions.None);
            List<string> columnExpressions = new List<string>(expressions);
            foreach (string expression in columnExpressions) {
                if (expression.Contains("CustomerID")) {
                    columnExpressions.Remove(expression);
                    break;
                }
            }
            string finalExpression = string.Join("AND", columnExpressions.ToArray());
            if (e.Value != "0") {
                if (!string.IsNullOrEmpty(finalExpression)) { finalExpression += " AND "; }
                finalExpression += "(CustomerID = " + e.Value + ")";
                gridMain.MasterTableView.GetColumn("CustomerID").CurrentFilterFunction = GridKnownFunction.EqualTo;
                gridMain.MasterTableView.GetColumn("CustomerID").CurrentFilterValue = e.Value;
            } else {
                gridMain.MasterTableView.GetColumn("CustomerID").CurrentFilterFunction = GridKnownFunction.NoFilter;
                gridMain.MasterTableView.GetColumn("CustomerID").CurrentFilterValue = null;
            }
            gridMain.MasterTableView.FilterExpression = finalExpression;
            ViewState[list.ClientID] = e.Value;
            gridMain.MasterTableView.Rebind();
        }

        protected void ddlCustomerFilter_PreRender(object sender, EventArgs e) {
            RadDropDownList list = sender as RadDropDownList;
            if (ViewState[list.ClientID] != null) { list.SelectedValue = ViewState[list.ClientID].ToString(); }
        }

        protected void btnExportZip_Click(object sender, EventArgs e) {
            try {
                List<File4ListB> files = new List<File4ListB>();
                List<string> errors = new List<string>();
                string recFilter = gridMain.MasterTableView.FilterExpression;
                GridSortExpressionCollection gridSortExxpressions = gridMain.MasterTableView.SortExpressions;
                FilesController cont = new FilesController();
                int tasksCount = cont.CountFiles(recFilter);
                files = cont.GetFilesForList(0, tasksCount, recFilter, gridSortExxpressions);
                if (files.Count > 0) {
                    MemoryStream memStream = new MemoryStream();
                    using (ZipArchive archive = new ZipArchive(memStream, ZipArchiveMode.Create, true, null)) {
                        foreach (File4ListB file in files) {
                            string physicalPath = System.Web.HttpContext.Current.Server.MapPath(file.FilePath);
                            string fileName = Path.GetFileName(physicalPath);
                            if (File.Exists(physicalPath)) {
                                using (ZipArchiveEntry entry = archive.CreateEntry(fileName)) {
                                    BinaryWriter writer = new BinaryWriter(entry.Open());
                                    writer.Write(File.ReadAllBytes(physicalPath));
                                    writer.Flush();
                                }
                            } else {
                                errors.Add(fileName + " (A/A:" + file.ID.ToString() + ")");
                            }
                        }
                    }
                    bool sendZip = false;
                    if (files.Count > errors.Count) { sendZip = true; }
                    if (errors.Count > 0) { ShowMissingFiles(errors, memStream, sendZip); } else { SendZipToClient(memStream); }
                }
            }
            catch (Exception) { ShowErrorMessage(); }
        }

        protected void SendZipToClient(MemoryStream memStream) {
            memStream.Seek(0, SeekOrigin.Begin);
            if (memStream != null && memStream.Length > 0) {
                Response.Clear();
                Response.AddHeader("content-disposition", "attachment; filename=files.zip");
                Response.ContentType = "application/zip";
                Response.BinaryWrite(memStream.ToArray());
                Response.End();
            }
        }

        protected void ShowMissingFiles(List<string> errors, MemoryStream memStream, bool sendZip) {
            string msg = "<br /><br />&nbsp;&nbsp;&nbsp;Τα αρχεία:<br />";
            foreach (string error in errors) {
                msg += "&nbsp;&nbsp;&nbsp;" + error + "<br />";
            }
            msg += "&nbsp;&nbsp;&nbsp;δε βρέθηκαν στον server!";
            litErrors.Text = msg;
            if (sendZip == true) { SendZipToClient(memStream); }
        }

        protected void ShowErrorMessage() {
            RadWindowManager1.RadAlert("Υπήρξε κάποιο τεχνικό πρόβλημα! Παρακαλώ προσπαθήστε αργότερα.", 400, 200, "Σφάλμα", "");
        }

    }

}