<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" AutoEventWireup="true" CodeBehind="InvoiceCreate.aspx.cs" Inherits="OTERT.Pages.Invoices.InvoiceCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PHTitle" runat="server"><% =pageTitle %></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PHHead" runat="server"></asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PHContent" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <script type="text/javascript">
        //Put your JavaScript code here.
    </script>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="gridMain">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gridMain" LoadingPanelID="RadAjaxLoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="RadWindowManager1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Height="75px" Width="75px" Transparency="25" InitialDelayTime="500" />
    <div>
        <asp:PlaceHolder ID="phStep1" runat="server">    
            <table>
                <tr>
                    <td>Πελάτης: </td>
                    <td><telerik:RadDropDownList RenderMode="Lightweight" ID="ddlCustomers" runat="server" DropDownHeight="200px" Width="200px" DropDownWidth="200px" AutoPostBack="false" CausesValidation="false" /></td>
                    <td></td>
                </tr>
                <tr>
                    <td>Ημ/νία Τιμολογίου: </td>
                    <td><telerik:RadDatePicker RenderMode="Lightweight" ID="dpDateFrom" runat="server" DateInput-Label="Από: " Culture="el-GR" /></td>
                    <td><telerik:RadDatePicker RenderMode="Lightweight" ID="dpDateTo" runat="server" DateInput-Label="Έως: " Culture="el-GR" /></td>
                </tr>
                <tr>
                    <td>Ημ/νία Δημιουργίας Τιμολογίου: </td>
                    <td><telerik:RadDatePicker RenderMode="Lightweight" ID="dpDateCreated" runat="server" Culture="el-GR" /></td>
                    <td></td>
                </tr>
                <tr>
                    <td>Κωδικός Τιμολογίου από Λογιστήριο: </td>
                    <td><telerik:RadTextBox RenderMode="Lightweight" runat="server" ID="txtAccountNo" Width="200px" EmptyMessage="" /></td>
                    <td></td>
                </tr>
                <tr>
                    <td>Ημ/νία Πληρωμής Τιμολογίου: </td>
                    <td><telerik:RadDatePicker RenderMode="Lightweight" ID="dpDatePay" runat="server" Culture="el-GR" /></td>
                    <td></td>
                </tr>
                <tr>
                    <td>Κλειδωμένο Τιμολόγιο: </td>
                    <td><telerik:RadCheckBox ID="chkIsLocked" runat="server" AutoPostBack="false" CausesValidation="false" /></td>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <br /><br />
                        <telerik:RadButton RenderMode="Lightweight" ID="btnShow1" runat="server" SingleClick="true" SingleClickText="Please wait..." OnClick="btnShow1_Click" Text="Επόμενο">
                            <Icon PrimaryIconCssClass="rbNext" />
                        </telerik:RadButton>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
            </table>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phStep2" runat="server">  
            <telerik:RadGrid ID="gridJobs" runat="server" AutoGenerateColumns="false" Skin="Metro" Width="50%"
                OnNeedDataSource="gridJobs_NeedDataSource">
                <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" CommandItemStyle-HorizontalAlign="Right" InsertItemPageIndexAction="ShowItemOnCurrentPage" NoMasterRecordsText="Δεν υπάρχουν Κατηγορίες Έργων">
                    <CommandItemTemplate>
                        <asp:Button ID="btnSelectAllJobs" Text="Επιλογή Όλων" runat="server" OnClick="btnSelectAllJobs_Click" />
                        <asp:Button ID="btnDeSelectAllJobs" Text="Καθαρισμός" runat="server" OnClick="btnDeSelectAllJobs_Click" />
                    </CommandItemTemplate>   
                    <CommandItemSettings RefreshText="Ανανέωση" ShowAddNewRecordButton="false" />
                    <Columns>
                        <telerik:GridTemplateColumn>
                            <ItemTemplate>
                                <asp:CheckBox ID="chk" runat="server" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="ID" HeaderText="ID" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" />
                        <telerik:GridBoundColumn DataField="Name" HeaderText="Τίτλος" ReadOnly="true" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
            <br /><br />
            <telerik:RadButton RenderMode="Lightweight" ID="btnShowPrev2" runat="server" SingleClick="true" SingleClickText="Please wait..." OnClick="btnShowPrev2_Click" Text="Προηγούμενο">
                <Icon PrimaryIconCssClass="rbPrevious" />
            </telerik:RadButton>&nbsp;&nbsp;&nbsp;&nbsp;
            <telerik:RadButton RenderMode="Lightweight" ID="btnShow2" runat="server" SingleClick="true" SingleClickText="Please wait..." OnClick="btnShow2_Click" Text="Επόμενο">
                <Icon PrimaryIconCssClass="rbNext" />
            </telerik:RadButton>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phStep3" runat="server">
            <telerik:RadGrid ID="gridTasks" runat="server" AutoGenerateColumns="false" Skin="Metro"
                OnNeedDataSource="gridTasks_NeedDataSource">
                <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" CommandItemStyle-HorizontalAlign="Right" InsertItemPageIndexAction="ShowItemOnCurrentPage" NoMasterRecordsText="Δεν υπάρχουν Έργα που να πληρούν αυτές τις συνθήκες">
                    <CommandItemTemplate>
                        <asp:Button ID="btnSelectAllTasks" Text="Επιλογή Όλων" runat="server" OnClick="btnSelectAllTasks_Click" />
                        <asp:Button ID="btnDeSelectAllTasks" Text="Καθαρισμός" runat="server" OnClick="btnDeSelectAllTasks_Click" />
                    </CommandItemTemplate>   
                    <CommandItemSettings RefreshText="Ανανέωση" ShowAddNewRecordButton="false" />
                    <Columns>
                        <telerik:GridTemplateColumn>
                            <ItemTemplate>
                                <asp:CheckBox ID="chk" runat="server" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="ID" HeaderText="ID" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" />
                        <telerik:GridBoundColumn DataField="OrderDate" HeaderText="Ημ/νία Παραγγελίας" ReadOnly="true" />
                        <telerik:GridBoundColumn DataField="RegNo" HeaderText="Κωδ. Τιμολογίου" ReadOnly="true" />
                        <telerik:GridBoundColumn DataField="Job.Name" HeaderText="Κατ. Έργου" ReadOnly="true" />
                        <telerik:GridBoundColumn DataField="Distance.Position1" HeaderText="Από" ReadOnly="true" />
                        <telerik:GridBoundColumn DataField="DateTimeStartActual" HeaderText="Ημ/νια Αρχής" ReadOnly="true" />
                        <telerik:GridBoundColumn DataField="DateTimeEndActual" HeaderText="Ημ/νια Τέλους" ReadOnly="true" />
                        <telerik:GridBoundColumn DataField="CostActual" HeaderText="Ποσό (&euro;)" ReadOnly="true" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>  
            <br /><br />
            <telerik:RadButton RenderMode="Lightweight" ID="btnShowPrev3" runat="server" SingleClick="true" SingleClickText="Please wait..." OnClick="btnShowPrev3_Click" Text="Προηγούμενο">
                <Icon PrimaryIconCssClass="rbPrevious" />
            </telerik:RadButton>&nbsp;&nbsp;&nbsp;&nbsp;
            <telerik:RadButton RenderMode="Lightweight" ID="btnShow3" runat="server" SingleClick="true" SingleClickText="Please wait..." OnClick="btnShow3_Click" Text="Επόμενο">
                <Icon PrimaryIconCssClass="rbNext" />
            </telerik:RadButton>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phStep4" runat="server">
            <telerik:RadGrid ID="gridSales" runat="server" AutoGenerateColumns="false" Skin="Metro"
                OnNeedDataSource="gridSales_NeedDataSource">
                <MasterTableView DataKeyNames="JobID" CommandItemDisplay="Top" CommandItemStyle-HorizontalAlign="Right" InsertItemPageIndexAction="ShowItemOnCurrentPage" NoMasterRecordsText="Δεν υπάρχουν Εκπτώσεις για τα συγκεκριμένα Έργα">
                    <CommandItemTemplate>
                        <asp:Button ID="btnSelectAllSales" Text="Επιλογή Όλων" runat="server" OnClick="btnSelectAllSales_Click" />
                        <asp:Button ID="btnDeSelectAllSales" Text="Καθαρισμός" runat="server" OnClick="btnDeSelectAllSales_Click" />
                    </CommandItemTemplate>   
                    <CommandItemSettings RefreshText="Ανανέωση" ShowAddNewRecordButton="false" />
                    <Columns>
                        <telerik:GridTemplateColumn>
                            <ItemTemplate>
                                <asp:CheckBox ID="chk" runat="server" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="JobID" HeaderText="ID" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" />
                        <telerik:GridBoundColumn DataField="JobName" HeaderText="Κατηγορία Έργου" ReadOnly="true" />
                        <telerik:GridBoundColumn DataField="TasksCount" HeaderText="Πλήθος Έργων" ReadOnly="true" />
                        <telerik:GridBoundColumn DataField="TasksCost" UniqueName="TasksCost" HeaderText="Συνολικό Ποσό" ReadOnly="true" />
                        <telerik:GridBoundColumn DataField="SalesCost" UniqueName="SalesCost" HeaderText="Συνολική Έκπτωση" ReadOnly="true" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid> 
            <br /><br />
            <telerik:RadButton RenderMode="Lightweight" ID="btnShowPrev4" runat="server" SingleClick="true" SingleClickText="Please wait..." OnClick="btnShowPrev4_Click" Text="Προηγούμενο">
                <Icon PrimaryIconCssClass="rbPrevious" />
            </telerik:RadButton>&nbsp;&nbsp;&nbsp;&nbsp;
            <telerik:RadButton RenderMode="Lightweight" ID="btnShow4" runat="server" SingleClick="true" SingleClickText="Please wait..." OnClick="btnShow4_Click" Text="Επόμενο">
                <Icon PrimaryIconCssClass="rbNext" />
            </telerik:RadButton>
        </asp:PlaceHolder>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>