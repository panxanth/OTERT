<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" AutoEventWireup="true" CodeBehind="FilesList.aspx.cs" Inherits="OTERT.Pages.UserPages.FilesList" %>

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
        <telerik:RadGrid ID="gridMain" runat="server" AutoGenerateColumns="false" MasterTableView-AllowPaging="true" MasterTableView-AllowCustomPaging="true" MasterTableView-PageSize="10" EnableViewState="true" Skin="Metro"
            AllowFilteringByColumn="True" PagerStyle-AlwaysVisible="true" MasterTableView-AllowSorting="true" MasterTableView-AllowCustomSorting="true"
            OnNeedDataSource="gridMain_NeedDataSource"
            OnItemDataBound="gridMain_ItemDataBound" 
            OnItemCreated="gridMain_ItemCreated" 
            OnSortCommand="gridMain_SortCommand">
            <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" InsertItemPageIndexAction="ShowItemOnCurrentPage" AllowFilteringByColumn="True" NoMasterRecordsText="Δεν υπάρχουν ακόμη εγγραφές">
                <CommandItemSettings ShowAddNewRecordButton="false" />
                <PagerStyle PageSizeLabelText=" Εγγραφές ανά σελίδα:" PagerTextFormat=" {4} <strong>{5}</strong> εγγραφές σε <strong>{1}</strong> σελίδες " AlwaysVisible="true" />
                <Columns>
                    <telerik:GridBoundColumn SortExpression="TaskID" HeaderText="TaskID" DataField="TaskID" UniqueName="TaskID" ReadOnly="true" Visible="false" />
                    <telerik:GridBoundColumn HeaderText="Όνομα" DataField="FileName" UniqueName="FileName" Visible="false" />
                    <telerik:GridTemplateColumn HeaderText="Περιγραφή Αρχείου" DataField="FileName" UniqueName="FilePath" SortExpression="FileName" HeaderStyle-Font-Bold="true" FilterControlWidth="200px" AllowSorting="true">
                        <ItemTemplate>
                            <asp:HyperLink runat="server" Text='<% #Eval("FileName") %>' NavigateUrl='<% #Eval("FilePath") %>' Target="_blank" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Πελάτης" DataField="CustomerID" SortExpression="CustomerName" UniqueName="CustomerID" HeaderStyle-Font-Bold="true" AllowSorting="true">
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("CustomerName") %>' runat="server" /> 
                        </ItemTemplate>
                        <FilterTemplate>
		                    <telerik:RadDropDownList runat="server" ID="ddlCustomerFilter" RenderMode="Lightweight" AppendDataBoundItems="true" AutoPostBack="true" CausesValidation="false" Width="280px" DropDownHeight="400px" OnSelectedIndexChanged="ddlCustomerFilter_SelectedIndexChanged" OnPreRender="ddlCustomerFilter_PreRender" />
	                    </FilterTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridDateTimeColumn UniqueName="DateStamp" DataField="DateStamp" HeaderText="Ημ/νία Καταχώρησης" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}" PickerType="DateTimePicker" HeaderStyle-Font-Bold="true" EnableRangeFiltering="true" EnableTimeIndependentFiltering="true" FilterControlWidth="200px" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>