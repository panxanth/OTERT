<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" AutoEventWireup="true" CodeBehind="DailyListInside.aspx.cs" Inherits="OTERT.Pages.Helpers.DailyListInside" %>

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
        <telerik:RadGrid ID="gridMain" runat="server" AutoGenerateColumns="false" Skin="Metro"
            OnNeedDataSource="gridMain_NeedDataSource">
            <GroupingSettings CaseSensitive="false" />
            <MasterTableView DataKeyNames="Count" CommandItemDisplay="Top" InsertItemPageIndexAction="ShowItemOnCurrentPage" NoMasterRecordsText="Δεν υπάρχουν Μεταδόσεις για τη συγκεκριμένη ημέρα">
                <CommandItemSettings RefreshText="Ανανέωση" ShowAddNewRecordButton="false" />
                <Columns>
                    <telerik:GridBoundColumn DataField="Count" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" HeaderStyle-Width="50" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="Customer" HeaderText="Αιτών" ReadOnly="true" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="FromPlace" HeaderText="Από" ReadOnly="true" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="ToPlace" HeaderText="Έως" ReadOnly="true" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="FromTime" HeaderText="Ώρα Έναρξης" ReadOnly="true" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="ToTime" HeaderText="Ώρα Λήξης" ReadOnly="true" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="Comments" HeaderText="Παρατηρήσεις" ReadOnly="true" HeaderStyle-Font-Bold="true" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
        <br /><br />
        <asp:Button ID="btnExportDOCX" runat="server" Text="Εκτύπωση Word" OnClick="btnExportDOCX_Click" />&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnExportXLSX" runat="server" Text="Εκτύπωση Excel" OnClick="btnExportXLSX_Click" />
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>