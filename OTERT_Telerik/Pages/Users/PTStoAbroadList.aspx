﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" Culture="el-GR" AutoEventWireup="true" CodeBehind="PTStoAbroadList.aspx.cs" Inherits="OTERT.Pages.UserPages.PTStoAbroadList" %>

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
        function UpdateTo(ctlFromID, ctlToID) {
            var ctlFrom = $find(ctlFromID);
            var ctlTo = $find(ctlToID);
            var dateFrom = ctlFrom.get_selectedDate();
            ctlTo.set_selectedDate(dateFrom);
        }
        function filterMenuShowing(sender, eventArgs) {
            if (eventArgs.get_column().get_uniqueName() == "CostActual") {
                var menu = eventArgs.get_menu();
                var items = menu._itemData;
                var i = 0;
                while (i < items.length) {
                    if (items[i].value != "NoFilter" && items[i].value != "GreaterThan" && items[i].value != "LessThan" && items[i].value != "IsNull" && items[i].value != "NotIsNull") {
                        var item = menu._findItemByValue(items[i].value);
                        if (item != null) { item._element.style.display = "none"; }
                    }
                    i++;
                }    
            }
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
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Height="75px" Width="75px" Transparency="25" InitialDelayTime="500" />
    <div>
        <telerik:RadGrid ID="gridMain" runat="server" AutoGenerateColumns="false" MasterTableView-AllowPaging="true" MasterTableView-AllowCustomPaging="true" MasterTableView-PageSize="10" EnableViewState="true" Skin="Metro"
            AllowFilteringByColumn="True" PagerStyle-AlwaysVisible="true" MasterTableView-AllowSorting="true" MasterTableView-AllowCustomSorting="true" 
            OnEditCommand="gridMain_EditCommand"
            OnNeedDataSource="gridMain_NeedDataSource" 
            OnItemCreated="gridMain_ItemCreated" 
            OnItemDataBound="gridMain_ItemDataBound" 
            OnDetailTableDataBind="gridMain_DetailTableDataBind" 
            OnPreRender="gridMain_PreRender">
            <MasterTableView DataKeyNames="ID,OrderID" CommandItemDisplay="Top" InsertItemPageIndexAction="ShowItemOnCurrentPage" AllowFilteringByColumn="True" NoMasterRecordsText="Δεν υπάρχουν ακόμη εγγραφές" Name="Master">
                <CommandItemSettings ExportToExcelText="Εξαγωγή Αρχείου Excel" ShowAddNewRecordButton="false" RefreshText="Ανανέωση" />
                <PagerStyle PageSizeLabelText=" Εγγραφές ανά σελίδα:" PagerTextFormat=" {4} <strong>{5}</strong> εγγραφές σε <strong>{1}</strong> σελίδες " AlwaysVisible="true" />
                <DetailTables>
                    <telerik:GridTableView DataKeyNames="ID" Width="100%" runat="server" CommandItemDisplay="Top" Name="AttachedFiles" Caption="Συνοδευτικά Αρχεία" NoDetailRecordsText="Δεν υπάρχουν Συνοδευτικά Αρχεία" AllowFilteringByColumn="false" >
                        <CommandItemSettings ShowAddNewRecordButton="false" RefreshText="Ανανέωση" />
                        <Columns>
                            <telerik:GridBoundColumn SortExpression="TaskID" HeaderText="TaskID" DataField="TaskID" UniqueName="TaskID" ReadOnly="true" Visible="false" />
                            <telerik:GridBoundColumn HeaderText="Όνομα" DataField="FileName" UniqueName="FileName" Visible="false" />
                            <telerik:GridTemplateColumn HeaderText="Αρχείο" DataField="FilePath" UniqueName="FilePath" HeaderStyle-Font-Bold="true" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:HyperLink runat="server" Text='<% #Eval("FileName") %>' NavigateUrl='<% #Eval("FilePath") %>' Target="_blank" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn HeaderText="Ημ/νία Καταχώρησης" DataField="DateStamp" UniqueName="DateStamp" DataType="System.DateTime" ReadOnly="true" HeaderStyle-Font-Bold="true" AllowFiltering="false" />
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>
                <Columns>
                    <telerik:GridExpandColumn UniqueName="ExapandColumn" />
                    <telerik:GridEditCommandColumn EditText="Προβολή" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" ButtonType="ImageButton" />
                    <telerik:GridBoundColumn DataField="OrderID" HeaderText="Διοργάνωση Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" AllowFiltering="false" HeaderStyle-Font-Bold="true" AllowSorting="false" />
                    <telerik:GridBoundColumn UniqueName="RegNo" DataField="Order.RegNo" HeaderText="Αριθμός Πρωτοκόλλου" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="true" HeaderStyle-Font-Bold="true" FilterControlWidth="140px" />
                    <telerik:GridTemplateColumn UniqueName="CountryID" DataField="Order.Event.Place.CountryID" SortExpression="Order.Event.Place.CountryID" HeaderText="Χώρα" HeaderStyle-Font-Bold="true" AllowSorting="true" >
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Order.Event.Place.Country.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <FilterTemplate>
		                    <telerik:RadDropDownList runat="server" ID="ddlCuntryFilter" RenderMode="Lightweight" AppendDataBoundItems="true" AutoPostBack="true" CausesValidation="false" Width="280px" DropDownHeight="400px" OnSelectedIndexChanged="ddlCuntryFilter_SelectedIndexChanged" OnPreRender="ddlCuntrysFilter_PreRender" />
	                    </FilterTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="Customer1ID" DataField="Order.Customer1ID" SortExpression="Order.Customer1ID" HeaderText="Πάροχος" HeaderStyle-Font-Bold="true" AllowSorting="true" >
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Order.Customer1.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <FilterTemplate>
		                    <telerik:RadDropDownList runat="server" ID="ddlCustomer1Filter" RenderMode="Lightweight" AppendDataBoundItems="true" AutoPostBack="true" CausesValidation="false" Width="280px" DropDownHeight="400px" OnSelectedIndexChanged="ddlCustomer1Filter_SelectedIndexChanged" OnPreRender="ddlCustomer1Filter_PreRender" />
	                    </FilterTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="EventID" DataField="Order.EventID" SortExpression="Order.EventID" HeaderText="Διοργάνωση" HeaderStyle-Font-Bold="true" AllowSorting="true" >
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Order.Event.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <FilterTemplate>
		                    <telerik:RadDropDownList runat="server" ID="ddlEventFilter" RenderMode="Lightweight" AppendDataBoundItems="true" AutoPostBack="true" CausesValidation="false" Width="280px" DropDownHeight="400px" OnSelectedIndexChanged="ddlEventFilter_SelectedIndexChanged" OnPreRender="ddlEventFilter_PreRender" />
	                    </FilterTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="ID" HeaderText="Παραγγελία Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" AllowFiltering="false" HeaderStyle-Font-Bold="true" AllowSorting="false" />
                    <telerik:GridDateTimeColumn UniqueName="OrderDate" DataField="OrderDate" HeaderText="Ημ/νία Παραγγελίας" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}" PickerType="DateTimePicker" HeaderStyle-Font-Bold="true" EnableRangeFiltering="true" EnableTimeIndependentFiltering="true" FilterControlWidth="150px" />
                    <telerik:GridTemplateColumn UniqueName="CustomerID" DataField="CustomerID" SortExpression="CustomerID" HeaderText="Πελάτης" HeaderStyle-Font-Bold="true" AllowSorting="true" >
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Customer.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <FilterTemplate>
		                    <telerik:RadDropDownList runat="server" ID="ddlCustomersFilter" RenderMode="Lightweight" AppendDataBoundItems="true" AutoPostBack="true" CausesValidation="false" Width="280px" DropDownHeight="400px" OnSelectedIndexChanged="ddlCustomersFilter_SelectedIndexChanged" OnPreRender="ddlCustomersFilter_PreRender" />
	                    </FilterTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn UniqueName="TelephoneNumber" DataField="TelephoneNumber" HeaderText="Τηλέφωνο Χρέωσης" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="true" HeaderStyle-Font-Bold="true" FilterControlWidth="140px" />
                    <telerik:GridDateTimeColumn UniqueName="DateTimeStartActual" DataField="DateTimeStartActual" HeaderText="Ημ/νία Υλοποίησης (Έναρξη)" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}" PickerType="DateTimePicker" HeaderStyle-Font-Bold="true" EnableRangeFiltering="true" EnableTimeIndependentFiltering="true" FilterControlWidth="150px" />
                    <telerik:GridBoundColumn UniqueName="CostActual" DataField="CostActual" HeaderText="Ποσό Είσπραξης (€)" AutoPostBackOnFilter="true" CurrentFilterFunction="GreaterThan" ShowFilterIcon="true" HeaderStyle-Font-Bold="true" FilterControlWidth="140px" DataType="System.Decimal" />
                    <telerik:GridDateTimeColumn UniqueName="PaymentDateOrder" DataField="PaymentDateOrder" HeaderText="Ημ/νία Λήψης Ξένου Τιμολογίου" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}" PickerType="DateTimePicker" HeaderStyle-Font-Bold="true" EnableRangeFiltering="true" EnableTimeIndependentFiltering="true" FilterControlWidth="150px" />
                    <telerik:GridDateTimeColumn UniqueName="PaymentDateCalculated" DataField="PaymentDateCalculated" HeaderText="Ημ/νία Ελέγχου Τιμολογίου" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}" PickerType="DateTimePicker" HeaderStyle-Font-Bold="true" EnableRangeFiltering="true" EnableTimeIndependentFiltering="true" FilterControlWidth="150px" />
                </Columns>
            </MasterTableView>
            <ClientSettings>
                <ClientEvents OnFilterMenuShowing="filterMenuShowing" />
            </ClientSettings>
        </telerik:RadGrid>
        <br /><br />
        &nbsp;<telerik:RadButton RenderMode="Lightweight" Text="Εξαγωγή Αρχείου Excel" ID="btnExportXLSX" CssClass="downloadButton" OnClick="btnExportXLSX_Click" runat="server" />
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>