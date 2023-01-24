<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/InsideNoUser.Master" Culture="el-GR" AutoEventWireup="true" CodeBehind="EmailChangePasswordOK.aspx.cs" Inherits="OTERT.Pages.Password.EmailChangePasswordOK" %>

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
    <div>
        <table style="width:100%;"><tr><td style="width:100%;">
        <table style="margin: 0 auto;">
            <tr><td style="height: 50px;"></td></tr>
            <tr>
                <td>
                    <br /><br />
                    <asp:Literal ID="litText" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <br /><br />
                    <telerik:RadButton RenderMode="Lightweight" ID="btnUpdate" runat="server" SingleClick="true" SingleClickText="Please wait..." OnClick="btnUpdate_Click" Text="Επιστροφή" CausesValidation="false">
                        <Icon PrimaryIconCssClass="rbNext" />
                    </telerik:RadButton>
                </td>
            </tr>
        </table>
        </td></tr></table>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>