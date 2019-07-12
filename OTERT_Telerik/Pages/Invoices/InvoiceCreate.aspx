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
        <%--<telerik:RadDropDownList RenderMode="Lightweight" ID="ddlCustomers" runat="server"  DropDownHeight="200px" Width="200px" DefaultMessage="Select a product" DropDownWidth="200px" OnItemDataBound="RadDropDownProducts_ItemDataBound" DataValueField="ProductID" DataTextField="ProductName" />--%>
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
                <td>
                    <telerik:RadTextBox RenderMode="Lightweight" runat="server" ID="txtAccountNo" Width="200px" EmptyMessage="Enter username" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAccountNo" ErrorMessage="!" ForeColor="Red" />
                </td>
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
                    <telerik:RadButton RenderMode="Lightweight" ID="RadButton1" runat="server" OnClick="btnShow1_Click" Text="Επόμενο">
                        <Icon PrimaryIconCssClass="rbNext" />
                    </telerik:RadButton>
                </td>
                <td></td>
                <td></td>
            </tr>
        </table>
        <br /><br />
        <telerik:RadGrid ID="gridJobs" runat="server" AutoGenerateColumns="false" Skin="Metro" Width="50%"
            OnNeedDataSource="gridJobs_NeedDataSource">
            <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" CommandItemStyle-HorizontalAlign="Right" InsertItemPageIndexAction="ShowItemOnCurrentPage" NoMasterRecordsText="Δεν υπάρχουν Μεταδόσεις για τη συγκεκριμένη ημέρα">
                <CommandItemTemplate>
                    <asp:Button ID="btnSelectAll" Text="Επιλογή Όλων" runat="server" OnClick="btnSelectAll_Click" />
                    <asp:Button ID="btnDeSelectAll" Text="Καθαρισμός" runat="server" OnClick="btnDeSelectAll_Click" />
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
        <asp:Button ID="btnShow2" runat="server" Text="Προβολή" OnClick="btnShow2_Click" />
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>