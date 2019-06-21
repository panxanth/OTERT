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
        <telerik:RadGrid ID="gridMain" runat="server" AutoGenerateColumns="false" AllowPaging="true" AllowCustomPaging="true" PageSize="10" EnableViewState="true" Skin="Metro"
            AllowFilteringByColumn="True" PagerStyle-AlwaysVisible="true"
            OnNeedDataSource="gridMain_NeedDataSource" 
            OnUpdateCommand="gridMain_UpdateCommand"
            OnItemCreated="gridMain_ItemCreated" 
            OnDeleteCommand="gridMain_DeleteCommand"
            OnInsertCommand="gridMain_InsertCommand"
            OnItemDataBound="gridMain_ItemDataBound"
            OnItemCommand="gridMain_ItemCommand"
            OnDataBound="gridMain_DataBound">
            <ExportSettings> 
                <Pdf FontType="Subset" PaperSize="Letter" /> 
                <Excel Format="Html" /> 
                <Csv ColumnDelimiter="Comma" RowDelimiter="NewLine" /> 
            </ExportSettings> 
            <GroupingSettings CaseSensitive="false"></GroupingSettings>
            <MasterTableView DataKeyNames="ID" CommandItemDisplay="Top" InsertItemPageIndexAction="ShowItemOnCurrentPage" AllowFilteringByColumn="True" NoMasterRecordsText="Δεν υπάρχουν ακόμη εγγραφές">
                <CommandItemSettings AddNewRecordText="Προσθήκη νέας εγγραφής" RefreshText="Ανανέωση" />
                <PagerStyle PageSizeLabelText=" Εγγραφές ανά σελίδα:" PagerTextFormat=" {4} <strong>{5}</strong> εγγραφές σε <strong>{1}</strong> σελίδες " AlwaysVisible="true" />
                <Columns>
                    <telerik:GridEditCommandColumn EditText="Επεξεργασία" />
                    <telerik:GridBoundColumn DataField="ID" HeaderText="Α/Α" ReadOnly="true" ForceExtractValue="Always" ConvertEmptyStringToNull="true" AllowFiltering="false" />
                    <telerik:GridBoundColumn DataField="NameGR" HeaderText="Όνομα (GR)" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="true">
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="NameEN" HeaderText="Όνομα (EN)" Visible="false" />
                    <telerik:GridBoundColumn DataField="NamedInvoiceGR" HeaderText="Όνομα για Τιμολόγιο (GR)" Visible="false" />
                    <telerik:GridBoundColumn DataField="NamedInvoiceEN" HeaderText="Όνομα για Τιμολόγιο (EN)" Visible="false" />
                    <telerik:GridTemplateColumn HeaderText="Χώρα" HeaderStyle-Width="180px" UniqueName="CountryID" DataField="CountryID" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Country.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlCountries" RenderMode="Lightweight" DropDownHeight="200" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlCountries_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="ZIPCode" HeaderText="Ταχ. Κώδικας" Visible="false" />
                    <telerik:GridBoundColumn DataField="CityGR" HeaderText="Πόλη (GR)" Visible="false" />
                    <telerik:GridBoundColumn DataField="CityEN" HeaderText="Πόλη (EN)" Visible="false" />
                    <telerik:GridBoundColumn DataField="ChargeTelephone" HeaderText="Τηλέφωνο Χρέωσης" Visible="false" />
                    <telerik:GridBoundColumn DataField="Telephone1" HeaderText="Τηλέφωνο 1" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="true" />
                    <telerik:GridBoundColumn DataField="Telephone2" HeaderText="Τηλέφωνο 2" Visible="false" />
                    <telerik:GridBoundColumn DataField="FAX1" HeaderText="FAX 1" Visible="false" />
                    <telerik:GridBoundColumn DataField="FAX2" HeaderText="FAX 2" Visible="false" />
                    <telerik:GridBoundColumn DataField="Address1GR" HeaderText="Διεύθυνση 1 (GR)" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" ShowFilterIcon="true">
                        <ColumnValidationSettings EnableRequiredFieldValidation="true">
                            <RequiredFieldValidator ForeColor="Red" ErrorMessage="Το πεδίο είναι υποχρεωτικό!" />
                        </ColumnValidationSettings>
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Address1EN" HeaderText="Διεύθυνση 1 (EN)" Visible="false" />
                    <telerik:GridBoundColumn DataField="Address2GR" HeaderText="Διεύθυνση 2 (GR)" Visible="false" />
                    <telerik:GridBoundColumn DataField="Address2EN" HeaderText="Διεύθυνση 2 (EN)" Visible="false" />
                    <telerik:GridBoundColumn DataField="ContactPersonGR" HeaderText="Πρόσωπο Επαφής (GR)" Visible="false" />
                    <telerik:GridBoundColumn DataField="ContactPersonEN" HeaderText="Πρόσωπο Επαφής (EN)" Visible="false" />
                    <telerik:GridTemplateColumn HeaderText="Τύπος Πελάτη" HeaderStyle-Width="180px" UniqueName="CustomerTypeID" DataField="CustomerTypeID" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("CustomerType.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlCustomerTypes" RenderMode="Lightweight" DropDownHeight="200" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlCustomerTypes_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Γλώσσα" HeaderStyle-Width="180px" UniqueName="LanguageID" DataField="LanguageID" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("Language.Name") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlLanguages" RenderMode="Lightweight" DropDownHeight="200" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlLanguages_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="Email" HeaderText="Email" Visible="false" />
                    <telerik:GridBoundColumn DataField="URL" HeaderText="URL" Visible="false" />
                    <telerik:GridBoundColumn DataField="AFM" HeaderText="ΑΦΜ" Visible="false" />
                    <telerik:GridBoundColumn DataField="DOY" HeaderText="ΔΟΥ" Visible="false" />
                    <telerik:GridBoundColumn DataField="SAPCode" HeaderText="Κωδικός SAP" Visible="false" />
                    <telerik:GridTemplateColumn HeaderText="Διαχειριστής" HeaderStyle-Width="180px" UniqueName="UserID" DataField="UserID" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:Label Text='<% #Eval("User.NameGR") %>' runat="server" /> 
                        </ItemTemplate>
                        <EditItemTemplate>
                            <telerik:RadDropDownList runat="server" ID="ddlUsers" RenderMode="Lightweight" DropDownHeight="200" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlUsers_SelectedIndexChanged" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="Comments" HeaderText="Σχόλια" Visible="false" />
                    <telerik:GridCheckBoxColumn DataField="IsProvider" HeaderText="Πάροχος" DataType="System.Boolean" AutoPostBackOnFilter="true" CurrentFilterFunction="EqualTo" ShowFilterIcon="true" />
                    <telerik:GridButtonColumn ConfirmText="Να διαγραφεί αυτή η εγγραφή;" ConfirmDialogType="RadWindow" ConfirmTitle="Διαγραφή" ButtonType="FontIconButton" HeaderTooltip="Διαγραφή" CommandName="Delete" />
                </Columns>
                <NestedViewTemplate>
	                <div class="contactWrap">
		                <fieldset style="padding: 10px;">
			                <legend style="padding: 5px;"><b>Συνολική εικόνα πελάτη: <%#Eval("NameGR") %></b></legend>
			                <table>
				                <tr>
					                <td>Διεύθυνση: </td>
					                <td><asp:Label ID="addressLabel" Text='<%#Bind("Address1GR") %>' runat="server" /></td>
				                </tr>
			                </table>
		                </fieldset>
	                </div>
                </NestedViewTemplate>
                <EditFormSettings>
                    <EditColumn UpdateText="Ενημέρωση" InsertText="Εισαγωγή" CancelText="Ακύρωση" />                          
                </EditFormSettings>
            </MasterTableView>
        </telerik:RadGrid>
        <telerik:RadWindowManager RenderMode="Lightweight" ID="RadWindowManager1" runat="server" />
    </div>
</asp:Content>