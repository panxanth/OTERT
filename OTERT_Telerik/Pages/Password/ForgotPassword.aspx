<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/InsideNoUser.Master" Culture="el-GR" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="OTERT.Pages.Password.ForgotPassword" %>

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
            <tr>
                <td style="height: 50px;"></td>
                <td></td>
            </tr>
            <tr>
                <td style="height: 35px;">Όνομα Χρήστη: </td>
                <td><asp:TextBox ID="txtUserName" runat="server" Width="230" EnableViewState="true" /></td>
            </tr>
            <tr>
                <td colspan="2"><asp:CustomValidator ID= "valUserName" runat="server" Enabled="true" ControlToValidate="txtUserName" ValidateEmptyText="true" Display="Dynamic" ForeColor="Red" OnServerValidate="valUserName_Validate" /></td>
            </tr>
            <tr>
                <td>
                    <br /><br />
                    <telerik:RadButton RenderMode="Lightweight" ID="btnUpdate" runat="server" SingleClick="true" SingleClickText="Please wait..." OnClick="btnUpdate_Click" Text="Αποστολή" CausesValidation="true">
                        <Icon PrimaryIconCssClass="rbNext" />
                    </telerik:RadButton>
                </td>
                <td></td>
            </tr>
            <tr>
                <td colspan="2">
                    <br /><br />
                    <strong>Κανόνες δημιουργίας Κωδικού Χρήστη</strong>
                    <ul>
                        <li>Το μέγεθος του κωδικού πρέπει να είναι τουλάχιστον 12 χαρακτήρες</li>
                        <li>Επιτρέπονται μόνο λατινικοί χαρακτήρες</li>
                        <li>Πρέπει να υπάρχει τουλάχιστον ένας κεφαλαίος χαρακτήρας</li>
                        <li>Πρέπει να υπάρχει τουλάχιστον ένας πεζός χαρακτήρας</li>
                        <li>Πρέπει να υπάρχει τουλάχιστον ένας ειδικός χαρακτήρας (#?!@$%^&*-)</li>
                        <li>Δεν πρέπει να είναι ο ίδιος με κάποιον από τους πέντε παλαιότερους κωδικούς</li>
                        <li>Δεν πρέπει να είναι ή να περιέχει το Όνομα Χρήστη ή το email του</li>
                    </ul>
                </td>
            </tr>
        </table>
        </td></tr></table>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>