<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" Culture="el-GR" AutoEventWireup="true" CodeBehind="PlacesList.aspx.cs" Inherits="OTERT.Pages.Administrator.PlacesList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PHTitle" runat="server"><% =pageTitle %></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PHHead" runat="server">
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
        <telerik:RadGrid ID="gridMain" runat="server" AutoGenerateColumns="false" AllowPaging="true" AllowCustomPaging="true" PageSize="10" Skin="Metro" MasterTableView-AllowFilteringByColumn="True"
            PagerStyle-AlwaysVisible="true" AllowSorting="true" AllowCustomSorting="true"
            OnNeedDataSource="gridMain_NeedDataSource" 
            OnUpdateCommand="gridMain_UpdateCommand"
            OnItemCreated="gridMain_ItemCreated" 
            OnDeleteCommand="gridMain_DeleteCommand"
            OnInsertCommand="gridMain_InsertCommand"
            OnItemDataBound="gridMain_ItemDataBound" >
            <GroupingSettings CaseSensitive="false" />
            <ExportSettings> 
                <Pdf FontType="Subset" PaperSize="Letter" /> 
                <Excel Format="Html" /> 
                <Csv ColumnDelimiter="Comma" RowDelimiter="NewLine" /> 
            </ExportSettings> 
            <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" InsertItemPageIndexAction="ShowItemOnCurrentPage" NoMasterRecordsText="Δεν υπάρχουν ακόμη εγγραφές">
                <CommandItemSettings AddNewRecordText="Προσθήκη νέας εγγραφής" RefreshText="Ανανέωση" />
                <PagerStyle PageSizeLabelText=" Εγγραφές ανά σελίδα:" PagerTextFormat=" {4} <strong>{5}</strong> εγγραφές σε <strong>{1}</strong> σελίδες " AlwaysVisible="true" />
                <Columns>
                    <telerik:GridEditCommandColumn EditText="Επεξεργασία" HeaderStyle-Width="50" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="ID" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" AllowFiltering="false" HeaderStyle-Width="80" HeaderStyle-Font-Bold="true" />
                    <telerik:GridTemplateColumn HeaderText="Χώρα" HeaderStyle-Width="180px" UniqueName="CountryID" DataField="CountryID" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="true" HeaderStyle-Font-Bold="true">
                        <FilterTemplate>
		                    <telerik:RadDropDownList runat="server" ID="ddlCountryFilter" RenderMode="Lightweight" AppendDataBoundItems="true" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlCountryFilter_SelectedIndexChanged" OnPreRender="ddlCountryFilter_PreRender" />
	                    </FilterTemplate>
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Country.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlCountries" RenderMode="Lightweight" Width="300" DropDownWidth="300" DropDownHeight="200" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlCountries_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn UniqueName="NameGR" DataField="NameGR" HeaderText="Όνομα (GR)" HeaderStyle-Font-Bold="true">
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn UniqueName="NameEN" DataField="NameEN" HeaderText="Όνομα (EN)" Visible="true" HeaderStyle-Font-Bold="true" />
                    <telerik:GridButtonColumn ConfirmText="Να διαγραφεί αυτή η εγγραφή;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="50" HeaderStyle-Font-Bold="true" />
                </Columns>
                <EditFormSettings>
                    <EditColumn UpdateText="Ενημέρωση" InsertText="Εισαγωγή" CancelText="Ακύρωση" />                          
                </EditFormSettings>
            </MasterTableView>
        </telerik:RadGrid>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>