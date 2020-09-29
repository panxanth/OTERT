<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" Culture="el-GR" AutoEventWireup="true" CodeBehind="InvoiceEdit.aspx.cs" Inherits="OTERT.Pages.Invoices.InvoiceEdit" %>

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
        <table>
            <tr>
                <td style="height: 35px;">Πελάτης: </td>
                <td><asp:Label runat="server" ID="lblCustomer" /></td>
                <td></td>
            </tr>
            <tr>
                <td style="height: 35px;">Ημ/νία Τιμολογίου: </td>
                <td>Από: <asp:Label runat="server" ID="lblDateFrom" /></td>
                <td>Έως: <asp:Label runat="server" ID="lblDateTo" /></td>
            </tr>
            <tr>
                <td style="height: 35px;">Ημ/νία Δημιουργίας Τιμολογίου: </td>
                <td><asp:Label runat="server" ID="lblDateCreated" /></td>
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
                    <telerik:RadButton RenderMode="Lightweight" ID="btnUpdate" runat="server" SingleClick="true" SingleClickText="Please wait..." OnClick="btnUpdate_Click" Text="Ενημέρωση">
                        <Icon PrimaryIconCssClass="rbNext" />
                    </telerik:RadButton>
                </td>
                <td></td>
                <td></td>
            </tr>
        </table>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>