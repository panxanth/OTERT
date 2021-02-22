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
        <telerik:RadGrid ID="gridInvoices" runat="server" AutoGenerateColumns="false"  MasterTableView-AllowPaging="true" MasterTableView-AllowCustomPaging="true" MasterTableView-PageSize="10" EnableViewState="true" Skin="Metro" Width="100%"
            AllowFilteringByColumn="True" PagerStyle-AlwaysVisible="true" MasterTableView-AllowSorting="true" MasterTableView-AllowCustomSorting="true"
            OnEditCommand="gridInvoices_EditCommand"
            OnDeleteCommand="gridInvoices_DeleteCommand"
            OnNeedDataSource="gridInvoices_NeedDataSource"
            OnItemCreated="gridInvoices_ItemCreated" 
            OnItemDataBound="gridInvoices_ItemDataBound" 
            OnItemCommand="gridInvoices_ItemCommand" >
            <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" CommandItemStyle-HorizontalAlign="Right" InsertItemPageIndexAction="ShowItemOnCurrentPage" NoMasterRecordsText="Δεν υπάρχουν Τιμολόγια">
                <CommandItemSettings RefreshText="Ανανέωση" ShowAddNewRecordButton="false" />
                <PagerStyle PageSizeLabelText=" Εγγραφές ανά σελίδα:" PagerTextFormat=" {4} <strong>{5}</strong> εγγραφές σε <strong>{1}</strong> σελίδες " AlwaysVisible="true" />
                <Columns>
                    <telerik:GridEditCommandColumn UniqueName="Edit" EditText="Επεξεργασία" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" ButtonType="ImageButton" />
                    <telerik:GridBoundColumn DataField="ID" HeaderText="ID" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" HeaderStyle-Font-Bold="true" AllowFiltering="false" AllowSorting="false" />
                    <telerik:GridTemplateColumn UniqueName="CustomerID" DataField="CustomerID" SortExpression="CustomerID" HeaderText="Πελάτης" ReadOnly="true" HeaderStyle-Font-Bold="true" AllowSorting="true" >
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Customer.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <FilterTemplate>
		                    <telerik:RadDropDownList runat="server" ID="ddlCustomersFilter" RenderMode="Lightweight" AppendDataBoundItems="true" AutoPostBack="true" CausesValidation="false" Width="280px" DropDownHeight="400px" OnSelectedIndexChanged="ddlCustomersFilter_SelectedIndexChanged" OnPreRender="ddlCustomersFilter_PreRender" />
	                    </FilterTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridDateTimeColumn UniqueName="DateFrom" DataField="DateFrom" HeaderText="Ημ/νία Έναρξης" ReadOnly="true" DataType="System.DateTime" PickerType="DateTimePicker" DataFormatString="{0:dd/MM/yyyy HH:mm}" HeaderStyle-Font-Bold="true" EnableRangeFiltering="true" EnableTimeIndependentFiltering="true" FilterControlWidth="150px" />
                    <telerik:GridDateTimeColumn UniqueName="DateTo" DataField="DateTo" HeaderText="Ημ/νία Λήξης" ReadOnly="true" DataType="System.DateTime" PickerType="DateTimePicker" DataFormatString="{0:dd/MM/yyyy HH:mm}" HeaderStyle-Font-Bold="true" EnableRangeFiltering="true" EnableTimeIndependentFiltering="true" FilterControlWidth="150px" />
                    <telerik:GridBoundColumn DataField="RegNo" HeaderText="Κωδικός Τιμολογίου" ReadOnly="true" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="true" HeaderStyle-Font-Bold="true" FilterControlWidth="140px" />
                    <telerik:GridButtonColumn UniqueName="btnDelete" ConfirmText="Να διαγραφεί αυτό το Τιμολόγιο;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
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
                <td>ΑΡΙΘΜΟΣ ΚΡΑΤΗΣΗΣ </td>
                <td><asp:CheckBox ID="chkOrderNo" Checked="true" runat="server" /></td>
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
                <td>ΔΙΑΔΡΟΜΗ </td>
                <td><asp:CheckBox ID="chkRoute" Checked="true" runat="server" EnableViewState="true" /></td>
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
                <td>ΔΙΑΦΟΡΕΣ ΧΡΕΩΣΕΙΣ&nbsp;&nbsp;&nbsp;&nbsp;</td>
                <td><asp:CheckBox ID="chkAddedCharges" Checked="true" runat="server" EnableViewState="true" /></td>
            </tr>
            <tr>
                <td>ΣΥΝΟΛΟ </td>
                <td><asp:CheckBox ID="chkTotalCost" Checked="true" runat="server" EnableViewState="true" /></td>
            </tr>
            <tr>
                <td>ΑΚΥΡΩΜΕΝΟ </td>
                <td><asp:CheckBox ID="chkIsCanceled" Checked="true" runat="server" EnableViewState="true" /></td>
            </tr>
            <tr>
                <td>ΠΑΡΑΤΗΡΗΣΕΙΣ </td>
                <td><asp:CheckBox ID="chkComments" Checked="true" runat="server" EnableViewState="true" /></td>
            </tr>
        </table>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>