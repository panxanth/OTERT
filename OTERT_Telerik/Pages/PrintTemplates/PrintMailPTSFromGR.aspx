<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" AutoEventWireup="true" CodeBehind="PrintMailPTSFromGR.aspx.cs" Inherits="OTERT.Pages.PrintTemplates.PrintMailPTSFromGR" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PHTitle" runat="server"><% =pageTitle %></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PHHead" runat="server">
    <script type="text/javascript">
        function addValue(txtTarget, txtVal) {
            document.getElementById(txtTarget).value += txtVal;
        }
    </script>
</asp:Content>

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
            if (args.get_eventTarget().match(re)) { args.set_enableAjax(false); }
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
        <telerik:RadGrid ID="gridMain" runat="server" AutoGenerateColumns="false" AllowPaging="true" AllowCustomPaging="true" PageSize="10" Skin="Metro"
            OnNeedDataSource="gridMain_NeedDataSource" 
            OnItemCommand="gridMain_ItemCommand" >
            <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" InsertItemPageIndexAction="ShowItemOnCurrentPage" NoMasterRecordsText="Δεν υπάρχουν ακόμη εγγραφές" Name="Master">
                <CommandItemSettings RefreshText="Ανανέωση" ShowAddNewRecordButton="false" />
                <PagerStyle PageSizeLabelText=" Εγγραφές ανά σελίδα:" PagerTextFormat=" {4} <strong>{5}</strong> εγγραφές σε <strong>{1}</strong> σελίδες " AlwaysVisible="true" />
                <Columns>
                    <telerik:GridBoundColumn DataField="ID" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" HeaderStyle-Width="80" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn UniqueName="RegNo" DataField="RegNo" HeaderText="Αριθμός Πρωτοκόλλου" HeaderStyle-Font-Bold="true" >
                        <ColumnValidationSettings EnableRequiredFieldValidation="true" RequiredFieldValidator-ForeColor="Red" RequiredFieldValidator-ErrorMessage="&nbsp;&nbsp;&nbsp;Το πεδίο είναι υποχρεωτικό!" />
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Χώρα" UniqueName="CountryID" DataField="CountryID" AllowFiltering="false" HeaderStyle-Font-Bold="true">
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Event.Place.Country.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Πελάτης (Πάροχος)" UniqueName="Customer1ID" DataField="Customer1ID" AllowFiltering="false" HeaderStyle-Font-Bold="true">
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Customer1.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="EventID" DataField="EventID" HeaderText="Γεγονός" HeaderStyle-Font-Bold="true" >
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Event.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridCheckBoxColumn DataField="IsLocked" HeaderText="Κλειδωμένο Έργο" Visible="false" DataType="System.Boolean" HeaderStyle-Font-Bold="true" />
                    <telerik:GridTemplateColumn UniqueName="btnPrintColumn" HeaderText="" AllowFiltering="false">
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        <ItemTemplate>
                            <asp:Button ID="btnPrint" runat="server" Text="Εκτύπωση" CommandName="invPrint"></asp:Button>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>