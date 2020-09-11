<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" Culture="el-GR" AutoEventWireup="true" CodeBehind="InvoiceShow.aspx.cs" Inherits="OTERT.Pages.Invoices.InvoiceShow" %>

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
        function RequestStart(sender, args) {
            var re = new RegExp("\.btnPrint$|\.lnkDownload$|\.btnDownArrow$|\.btnUpArrow$", "ig");
            if (args.get_eventTarget().match(re)) {args.set_enableAjax(false); }
        }
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
        <ClientEvents OnRequestStart="RequestStart" />
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
                    <td>Κωδικός Τιμολογίου από Λογιστήριο: </td>
                    <td>
                        <telerik:RadTextBox RenderMode="Lightweight" runat="server" ID="txtAccountNo" Width="200px" EmptyMessage="" />
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <br /><br />
                        <telerik:RadButton RenderMode="Lightweight" ID="btnShow" runat="server" SingleClick="true" SingleClickText="Please wait..." OnClick="btnShow_Click" Text="Αναζήτηση">
                            <Icon PrimaryIconCssClass="rbNext" />
                        </telerik:RadButton>
                        <br /><br />
                    </td>
                    <td></td>
                    <td></td>
                </tr>
            </table>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phStep2" runat="server">  
            <telerik:RadGrid ID="gridInvoices" runat="server" AutoGenerateColumns="false" Skin="Metro" Width="100%"
                OnNeedDataSource="gridInvoices_NeedDataSource"
                OnItemCommand="gridInvoices_ItemCommand" >
                <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" CommandItemStyle-HorizontalAlign="Right" InsertItemPageIndexAction="ShowItemOnCurrentPage" NoMasterRecordsText="Δεν υπάρχουν Τιμολόγια">
                    <CommandItemSettings RefreshText="Ανανέωση" ShowAddNewRecordButton="false" />
                    <Columns>
                        <telerik:GridBoundColumn DataField="ID" HeaderText="ID" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" />
                        <telerik:GridTemplateColumn UniqueName="CustomerID" DataField="CustomerID" HeaderText="Πελάτης" ReadOnly="true" >
                            <ItemTemplate>
                                <asp:Label Text='<% #Eval("Customer.NameGR") %>' runat="server" /> 
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridDateTimeColumn UniqueName="DateFrom" DataField="DateFrom" HeaderText="Ημ/νία Έναρξης" DataType="System.DateTime" />
                        <telerik:GridDateTimeColumn UniqueName="DateTo" DataField="DateTo" HeaderText="Ημ/νία Λήξης" DataType="System.DateTime" />
                        <telerik:GridBoundColumn DataField="RegNo" HeaderText="Κωδικός Τιμολογίου" ReadOnly="true" />
                        <telerik:GridTemplateColumn UniqueName="btnPrintColumn" HeaderText="" AllowFiltering="false">
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            <ItemTemplate>
                                <asp:Button ID="btnPrint" runat="server" Text="Εκτύπωση" CommandName="invPrint"></asp:Button>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="btnPrintDetailColumn" HeaderText="" AllowFiltering="false">
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            <ItemTemplate>
                                <asp:Button ID="btnPrintDetail" runat="server" Text="Εκτύπωση Αναλυτικού" CommandName="invPrintDetail"></asp:Button>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="btnPrintMailColumn" HeaderText="" AllowFiltering="false">
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            <ItemTemplate>
                                <asp:Button ID="btnPrintMail" runat="server" Text="Εκτύπωση Επιστολής" CommandName="invPrintMail"></asp:Button>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
            <br /><br />
            <table>
                <tr>
                    <td>ΗΜΕΡ/ΝΙΑ </td>
                    <td><asp:CheckBox ID="chkDate" Checked="true" runat="server" /></td>
                </tr>
                <tr>
                    <td>ΔΙΑΔΡΟΜΗ </td>
                    <td><asp:CheckBox ID="chkRoute" Checked="true" runat="server" EnableViewState="true" /></td>
                </tr>
                <tr>
                    <td>ΑΠΟ </td>
                    <td><asp:CheckBox ID="chkDateFrom" Checked="true" runat="server" EnableViewState="true" /></td>
                </tr>
                <tr>
                    <td>ΕΩΣ </td>
                    <td><asp:CheckBox ID="chkDateTo" Checked="true" runat="server" EnableViewState="true" /></td>
                </tr>
                <tr>
                    <td>ΔΙΑΡΚΕΙΑ </td>
                    <td><asp:CheckBox ID="chkTotalTime" Checked="true" runat="server" EnableViewState="true" /></td>
                </tr>
                <tr>
                    <td>ΑΠΟΣΤΑΣΗ </td>
                    <td><asp:CheckBox ID="chkTotalDistance" Checked="true" runat="server" EnableViewState="true" /></td>
                </tr>
                <tr>
                    <td>ΤΕΛΟΣ ΜΕΤΑΔΟΣΗΣ </td>
                    <td><asp:CheckBox ID="chkTransferCost" Checked="true" runat="server" EnableViewState="true" /></td>
                </tr>
                <tr>
                    <td>ΔΙΑΦΟΡΕΣ ΧΡΕΩΣΕΙΣ </td>
                    <td><asp:CheckBox ID="chkAddedCharges" Checked="true" runat="server" EnableViewState="true" /></td>
                </tr>
                <tr>
                    <td>ΣΥΝΟΛΟ </td>
                    <td><asp:CheckBox ID="chkTotalCost" Checked="true" runat="server" EnableViewState="true" /></td>
                </tr>
                <tr>
                    <td>ΠΑΡΑΤΗΡΗΣΕΙΣ </td>
                    <td><asp:CheckBox ID="chkComments" Checked="true" runat="server" EnableViewState="true" /></td>
                </tr>
            </table>
        </asp:PlaceHolder>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>