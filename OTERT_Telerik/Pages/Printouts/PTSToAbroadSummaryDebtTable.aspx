<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" Culture="el-GR" AutoEventWireup="true" CodeBehind="PTSToAbroadSummaryDebtTable.aspx.cs" Inherits="OTERT.Pages.Printouts.PTSToAbroadSummaryDebtTable" %>

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
                <td valign="middle">&nbsp;&nbsp;Ημ/νία Ελέγχου Τιμολογίου:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                <td><telerik:RadDatePicker RenderMode="Lightweight" ID="dpDateFrom" runat="server" DateInput-Label="Από: " Culture="el-GR" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                <td><telerik:RadDatePicker RenderMode="Lightweight" ID="dpDateTo" runat="server" DateInput-Label="Έως: " Culture="el-GR" /></td>
            </tr>
            <tr>
                <td>
                    <br /><br />&nbsp;&nbsp;
                    <telerik:RadButton RenderMode="Lightweight" ID="btnCreate" runat="server" SingleClick="false" SingleClickText="Please wait..." OnClick="btnCreate_Click" Text="&nbsp;&nbsp;Δημιουργία">
                        <Icon PrimaryIconCssClass="rbPrint" PrimaryIconTop="8px" />
                    </telerik:RadButton>
                </td>
                <td></td>
                <td></td>
            </tr>
        </table>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>