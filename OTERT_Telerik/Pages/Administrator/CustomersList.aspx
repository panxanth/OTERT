<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/Inside.Master" AutoEventWireup="true" CodeBehind="CustomersList.aspx.cs" Inherits="OTERT.Pages.Administrator.CustomersList" %>

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
        <telerik:RadGrid ID="gridMain" runat="server" AutoGenerateColumns="false" MasterTableView-AllowPaging="true" MasterTableView-AllowCustomPaging="true" MasterTableView-PageSize="10" EnableViewState="true" Skin="Metro" MasterTableView-AllowFilteringByColumn="True" MasterTableView-PagerStyle-AlwaysVisible="true"
            OnNeedDataSource="gridMain_NeedDataSource" 
            OnUpdateCommand="gridMain_UpdateCommand"
            OnItemCreated="gridMain_ItemCreated" 
            OnDeleteCommand="gridMain_DeleteCommand"
            OnInsertCommand="gridMain_InsertCommand"
            OnItemDataBound="gridMain_ItemDataBound"
            OnItemCommand="gridMain_ItemCommand"
            OnDetailTableDataBind="gridMain_DetailTableDataBind">
            <ExportSettings> 
                <Pdf FontType="Subset" PaperSize="Letter" /> 
                <Excel Format="Html" /> 
                <Csv ColumnDelimiter="Comma" RowDelimiter="NewLine" /> 
            </ExportSettings> 
            <GroupingSettings CaseSensitive="false" />
            <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" InsertItemPageIndexAction="ShowItemOnCurrentPage" AllowFilteringByColumn="True" NoMasterRecordsText="Δεν υπάρχουν ακόμη εγγραφές" Name="Master">
                <CommandItemSettings AddNewRecordText="Προσθήκη νέας εγγραφής" RefreshText="Ανανέωση" />
                <PagerStyle PageSizeLabelText=" Εγγραφές ανά σελίδα:" PagerTextFormat=" {4} <strong>{5}</strong> εγγραφές σε <strong>{1}</strong> σελίδες " AlwaysVisible="true" />
                <DetailTables>
                    <telerik:GridTableView DataKeyNames="ID" Width="100%" runat="server" CommandItemDisplay="Top" Name="AttachedFiles" Caption="Συνοδευτικά Αρχεία" NoDetailRecordsText="Δεν υπάρχουν Συνοδευτικά Αρχεία">
                        <CommandItemSettings AddNewRecordText="Προσθήκη νέου αρχείου" RefreshText="Ανανέωση" />
                        <Columns>
                            <telerik:GridBoundColumn SortExpression="TaskID" HeaderText="TaskID" DataField="TaskID" UniqueName="TaskID" ReadOnly="true" Visible="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Όνομα" DataField="FileName" UniqueName="FileName" Visible="false">
                                <ColumnValidationSettings EnableRequiredFieldValidation="true" RequiredFieldValidator-ForeColor="Red" RequiredFieldValidator-ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn HeaderText="Αρχείο" DataField="FilePath" UniqueName="FilePath" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:HyperLink runat="server" Text='<% #Eval("FileName") %>' NavigateUrl='<% #Eval("FilePath") %>' Target="_blank" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <telerik:RadAsyncUpload RenderMode="Lightweight" ID="uplFile" AllowedFileExtensions="doc,docx,xls,xlsx,zip,rar,pdf,msg" runat="server" OnFileUploaded="uplFile_FileUploaded">
                                    </telerik:RadAsyncUpload>
                                </EditItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn HeaderText="Ημ/νία Καταχώρησης" DataField="DateStamp" UniqueName="DateStamp" DataType="System.DateTime" ReadOnly="true" AllowFiltering="false">
                            </telerik:GridBoundColumn>
                            <telerik:GridButtonColumn UniqueName="btnDeleteFile" ConfirmText="Να διαγραφεί αυτό το Αρχείο;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>
                <Columns>
                    <telerik:GridEditCommandColumn EditText="Επεξεργασία" HeaderStyle-Width="50" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="ID" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" AllowFiltering="false" HeaderStyle-Width="50" HeaderStyle-Font-Bold="true" HeaderStyle-Wrap="false" />
                    <telerik:GridBoundColumn DataField="NameGR" HeaderText="Όνομα (GR)" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="true" HeaderStyle-Font-Bold="true">
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="NameEN" HeaderText="Όνομα (EN)" Visible="false" />
                    <telerik:GridBoundColumn DataField="NamedInvoiceGR" HeaderText="Όνομα για Τιμολόγιο (GR)" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="NamedInvoiceEN" HeaderText="Όνομα για Τιμολόγιο (EN)" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridTemplateColumn HeaderText="Χώρα" HeaderStyle-Width="180px" UniqueName="CountryID" DataField="CountryID" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="true" HeaderStyle-Font-Bold="true">
                        <FilterTemplate>
		                    <telerik:RadDropDownList runat="server" ID="ddlCountryFilter" RenderMode="Lightweight" AppendDataBoundItems="true" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlCountryFilter_SelectedIndexChanged" OnPreRender="ddlCountryFilter_PreRender" />
	                    </FilterTemplate>
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Country.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlCountries" RenderMode="Lightweight" DropDownHeight="200" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlCountries_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="ZIPCode" HeaderText="Ταχ. Κώδικας" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="CityGR" HeaderText="Πόλη (GR)" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="CityEN" HeaderText="Πόλη (EN)" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="ChargeTelephone" HeaderText="Τηλέφωνο Χρέωσης" Visible="false" />
                    <telerik:GridBoundColumn DataField="Telephone1" HeaderText="Τηλέφωνο 1" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="true" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="Telephone2" HeaderText="Τηλέφωνο 2" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="FAX1" HeaderText="FAX 1" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="FAX2" HeaderText="FAX 2" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="Address1GR" HeaderText="Διεύθυνση 1 (GR)" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="true" HeaderStyle-Font-Bold="true">
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Address1EN" HeaderText="Διεύθυνση 1 (EN)" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="Address2GR" HeaderText="Διεύθυνση 2 (GR)" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="Address2EN" HeaderText="Διεύθυνση 2 (EN)" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="ContactPersonGR" HeaderText="Πρόσωπο Επαφής (GR)" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="ContactPersonEN" HeaderText="Πρόσωπο Επαφής (EN)" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridTemplateColumn HeaderText="Τύπος Πελάτη" HeaderStyle-Width="100px" UniqueName="CustomerTypeID" DataField="CustomerTypeID" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="true" HeaderStyle-Font-Bold="true">
                        <FilterTemplate>
		                    <telerik:RadDropDownList runat="server" ID="ddlCustomerTypeFilter" RenderMode="Lightweight" AppendDataBoundItems="true" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlCustomerTypeFilter_SelectedIndexChanged" OnPreRender="ddlCustomerTypeFilter_PreRender" />
	                    </FilterTemplate>
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("CustomerType.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlCustomerTypes" RenderMode="Lightweight" DropDownHeight="200" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlCustomerTypes_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Γλώσσα" HeaderStyle-Width="100px" UniqueName="LanguageID" DataField="LanguageID" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="true" HeaderStyle-Font-Bold="true">
                        <FilterTemplate>
		                    <telerik:RadDropDownList runat="server" ID="ddlLanguageFilter" RenderMode="Lightweight" AppendDataBoundItems="true" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlLanguageFilter_SelectedIndexChanged" OnPreRender="ddlLanguageFilter_PreRender" />
	                    </FilterTemplate>
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Language.Name") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlLanguages" RenderMode="Lightweight" DropDownHeight="200" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlLanguages_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="Email" HeaderText="Email" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="URL" HeaderText="URL" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="AFM" HeaderText="ΑΦΜ" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="DOY" HeaderText="ΔΟΥ" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="SAPCode" HeaderText="Κωδικός SAP" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridTemplateColumn HeaderText="Διαχειριστής" HeaderStyle-Width="180px" UniqueName="UserID" DataField="UserID" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="true" HeaderStyle-Font-Bold="true">
                        <FilterTemplate>
		                    <telerik:RadDropDownList runat="server" ID="ddlUserFilter" RenderMode="Lightweight" AppendDataBoundItems="true" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlUserFilter_SelectedIndexChanged" OnPreRender="ddlUserFilter_PreRender" />
	                    </FilterTemplate>
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("User.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlUsers" RenderMode="Lightweight" DropDownHeight="200" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlUsers_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="Comments" HeaderText="Σχόλια" Visible="false" HeaderStyle-Font-Bold="true" />
                    <telerik:GridCheckBoxColumn DataField="IsProvider" HeaderText="Πάροχος" DataType="System.Boolean" AutoPostBackOnFilter="true" CurrentFilterFunction="NoFilter" ShowFilterIcon="true" HeaderStyle-Font-Bold="true" />
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